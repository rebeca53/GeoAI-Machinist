using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Movement
    [SerializeField] public float moveSpeed = 1.5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 moveDirection = new Vector2(1, 0);

    private Animator animator;

    // Action
    bool isEnabled = true;
    bool spaceAction = false;
    private GameObject nearObject = null;
    public GameObject grabbedObject = null;

    // Money system
    public int CurrentMoney { get { return currentMoney; } }
    private int currentMoney;

    // Audio
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        TurnLeft();
        currentMoney = 0;

        audioSource = GetComponent<AudioSource>();
    }

    public void SetEnable(bool enable)
    {
        isEnabled = enable;
    }

    public void Disable()
    {
        isEnabled = false;
    }

    public void Enable()
    {
        isEnabled = true;
    }

    public void Spawn(OverviewBoardManager boardManager, Vector2Int cell)
    {
        transform.position = boardManager.CellToWorld(cell);
    }

    public void Spawn(CommandCenterBoardManager boardManager, Vector2Int cell)
    {
        transform.position = boardManager.CellToWorld(cell);
    }

    public void Spawn(BaseBoard boardManager, Vector2Int cell)
    {
        transform.position = boardManager.CellToWorld(cell);
    }

    private void TurnLeft()
    {
        Debug.Log("Turn left");
        animator.SetFloat("LastInputX", -1f);
    }

    public void ChangeMoney(int amount)
    {
        currentMoney += amount;
        UIHandler.Instance.SetMoneyValue(currentMoney);
        GameManager.instance.playerCoinPoints = currentMoney;
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isEnabled)
        {
            return;
        }

        rb.velocity = moveInput * moveSpeed;

        spaceAction = Input.GetKeyDown(KeyCode.Space);
        if (spaceAction)
        {
            OnSpaceAction();
        }

        if (grabbedObject)
        {
            MoveGrabbedObject();
        }
    }

    // Check if input movement is only vertical
    private bool IsOnlyVertical(Vector2 input)
    {
        return input.x.Equals(0) && !input.y.Equals(0);
    }

    public void MoveManual(Vector3 newPosition)
    {
        rb.position = newPosition;
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!isEnabled)
        {
            return;
        }

        animator.SetBool("playerWalk", true);

        if (context.canceled)
        {
            animator.SetBool("playerWalk", false);

            // only-vertical movement won't change direction
            if (!IsOnlyVertical(moveInput))
            {
                animator.SetFloat("LastInputX", moveInput.x);
            }
        }

        moveInput = context.ReadValue<Vector2>();

        if (!Mathf.Approximately(moveInput.x, 0.0f))
        {
            moveDirection.Set(moveInput.x, moveInput.y);
            moveDirection.Normalize();
        }

        // only-vertical movement keep previous direction
        if (IsOnlyVertical(moveInput))
        {
            animator.SetFloat("InputX", animator.GetFloat("LastInputX"));
        }
        else
        {
            animator.SetFloat("InputX", moveInput.x);
        }

    }

    private void GrabSampleBox()
    {
        Debug.Log("grab " + nearObject.tag);

        if (nearObject == null || !(nearObject.CompareTag("SampleBox") || nearObject.CompareTag("SpectralBand")))
        {
            return;
        }

        animator.SetTrigger("playerGrab");
        // Debug.Log("grab object");
        // Change scale
        nearObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        // change box parent
        GameObject playerObj = GameObject.Find("Player");
        nearObject.transform.parent = playerObj.transform;
        // get player position and Change box position
        nearObject.transform.position = playerObj.transform.position;

        grabbedObject = nearObject;
    }

    private void BreakSampleBox()
    {
        Debug.Log("break " + nearObject.tag);
        if (nearObject == null || !nearObject.CompareTag("SampleBox"))
        {
            return;
        }

        SampleBox sampleBox = nearObject.GetComponent<SampleBox>();
        sampleBox.BreakMultiband();
    }

    private void DropObject()
    {
        animator.SetTrigger("playerGrab");
        // Debug.Log("drop object");
        // Change scale
        grabbedObject.transform.localScale = new Vector3(1f, 1f, 1f);

        // change box parent
        grabbedObject.transform.parent = null;
        grabbedObject = null;
    }

    private void AttemptToFillAContainer()
    {
        Debug.Log("drop object onto container");

        Container container = nearObject.GetComponent<Container>();
        SampleBox sampleBox = grabbedObject.GetComponent<SampleBox>();

        if (!container.IsMatch(sampleBox))
        {
            container.OnMismatch();
            return;
        }

        container.MatchContainer(sampleBox);
        grabbedObject = null;
    }

    private void AttemptToFillSpectralBandContainer()
    {
        Debug.Log("drop object onto SPECTRAL BAND container");

        SpectralBandContainer container = nearObject.GetComponent<SpectralBandContainer>();
        SampleSpectralBand sampleBand = grabbedObject.GetComponent<SampleSpectralBand>();

        if (container.IsMatch(sampleBand))
        {
            container.MatchSpectralBand(sampleBand);
            grabbedObject = null;
        }
    }

    private void GrabKernel()
    {
        Transform grabbeableObject = nearObject.transform.parent;
        animator.SetTrigger("playerGrab");

        grabbeableObject.GetComponent<KernelMatrix>().Grab(transform.position);

        // Debug.Log("grab object");
        // Change scale
        // change box parent
        GameObject playerObj = GameObject.Find("Player");
        grabbeableObject.parent = playerObj.transform;
        // get player position and Change box position
        grabbeableObject.position = playerObj.transform.position;

        grabbedObject = grabbeableObject.gameObject;
    }

    private void FillInputHolder()
    {
        // Debug.Log("drop object onto container");

        InputHolder inputHolder = nearObject.GetComponent<InputHolder>();
        SampleBox sampleBox = grabbedObject.GetComponent<SampleBox>();

        inputHolder.FeedInputSample(sampleBox);

        grabbedObject = null;
    }

    private void StartMiniGame()
    {
        Transform parent = nearObject.transform.parent;

        if (parent != null)
        {
            CNNLayer layer = parent.GetComponent<CNNLayer>();
            Debug.Log("layer " + layer.type);
            GameManager.instance.StartMiniGame(layer.type);
        }
        else
        {
            Debug.Log("This GameObject has no parent!");
        }
    }

    private void OnSpaceAction()
    {
        if (nearObject.CompareTag("Exit"))
        {
            Exit exit = nearObject.GetComponent<Exit>();
            exit.AttemptExit();
            return;
        }

        if (nearObject.CompareTag("CNNLayer"))
        {
            StartMiniGame();
            return;
        }

        // TODO: improve the context of an action
        if (grabbedObject)
        {
            Debug.Log("space action and BUT grabbed object");
            if (nearObject.CompareTag("Container"))
            {
                AttemptToFillAContainer();
            }
            else if (nearObject.CompareTag("InputHolder"))
            {
                FillInputHolder();
            }
            else if (nearObject.CompareTag("SpectralBandContainer"))
            {
                AttemptToFillSpectralBandContainer();
            }
            else
            {
                DropObject();
            }
        }
        else
        {
            Debug.Log("space action and no grabbed object");
            if (nearObject.CompareTag("SampleBox"))
            {
                Scene currentScene = SceneManager.GetActiveScene();
                // Retrieve the name of this scene.
                string sceneName = currentScene.name;
                if (sceneName == "InputMiniGame")
                {
                    BreakSampleBox();
                }
                else
                {
                    GrabSampleBox();
                }
            }
            else if (nearObject.CompareTag("SpectralBand"))
            {
                GrabSampleBox();
            }
            else if (nearObject.transform.parent.CompareTag("Kernel"))
            {
                GrabKernel();
            }

        }
    }

    private void MoveGrabbedObject()
    {
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj == null)
        {
            Debug.LogError("Cloud not find player");
        }

        if (grabbedObject)
        {
            grabbedObject.transform.position = playerObj.transform.position;
        }
    }

    //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
    private void OnTriggerEnter2D(Collider2D other)
    {
        nearObject = other.gameObject;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Debug.Log("On trigger stay 2d " + other.tag);
        nearObject = other.gameObject;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("on trigger exit 2d " + other.tag);
        nearObject = null;
    }
}
