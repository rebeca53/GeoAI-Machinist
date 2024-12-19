using System;
using System.Collections.Generic;
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

    // TODO: refactor to have a OnObjectGrabbed<tag>
    public Action OnKernelGrabbed;

    // Money system
    public int CurrentMoney { get { return currentMoney; } }
    private int currentMoney;

    // Audio
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
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
        animator.SetFloat("LastInputX", -1f);
    }

    public void TurnRight()
    {
        animator.SetFloat("LastInputX", 1f);
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

    public void Walking(bool isWalking)
    {
        animator.SetBool("playerWalk", isWalking);
    }

    public void Move(InputAction.CallbackContext context)
    {
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
        if (!isEnabled)
        {
            moveInput = new(0f, 0f);
        }

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
        if (nearObject == null || !(nearObject.CompareTag("SampleBox") || nearObject.CompareTag("SpectralBand")))
        {
            return;
        }

        animator.SetTrigger("playerGrab");
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
        // Change scale
        grabbedObject.transform.localScale = new Vector3(1f, 1f, 1f);
        if (grabbedObject.CompareTag("Kernel"))
        {
            grabbedObject.transform.localScale = new Vector3(0.2f, 0.2f, 1f);
        }
        else if (grabbedObject.CompareTag("ActivationBox"))
        {
            grabbedObject.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
        }
        // change box parent
        grabbedObject.transform.parent = null;
        grabbedObject = null;
    }

    private void AttemptToFillAContainer()
    {
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

        // Change scale
        // change box parent
        GameObject playerObj = GameObject.Find("Player");
        grabbeableObject.parent = playerObj.transform;
        // get player position and Change box position
        grabbeableObject.position = playerObj.transform.position;

        grabbedObject = grabbeableObject.gameObject;

        OnKernelGrabbed?.Invoke();
    }

    private void GrabActivationBox()
    {
        Transform grabbeableObject = nearObject.transform;
        animator.SetTrigger("playerGrab");

        nearObject.GetComponent<ActivationBox>().Grab(transform.position);

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
        InputHolder inputHolder = nearObject.GetComponent<InputHolder>();
        inputHolder.AddInputObject(grabbedObject);
        grabbedObject = null;
    }

    private void FillLocker()
    {
        Locker locker = nearObject.GetComponent<Locker>();
        switch (grabbedObject.tag)
        {
            case "Kernel":
                locker.AddKernel(grabbedObject);
                break;
            case "ActivationBox":
                locker.AddActivationBox(grabbedObject);
                break;
        }
        grabbedObject = null;
    }

    private void StartMiniGame()
    {
        Transform parent = nearObject.transform.parent;

        if (parent != null)
        {
            CNNLayer layer = parent.GetComponent<CNNLayer>();
            if (GameManager.instance.IsSolved(layer.type))
            {
                return;
            }

            GameManager.instance.StartMiniGame(layer.type);
        }
    }

    private void OnSpaceAction()
    {
        if (!isEnabled)
        {
            return;
        }

        if (nearObject && nearObject.CompareTag("Exit"))
        {
            Exit exit = nearObject.GetComponent<Exit>();
            exit.AttemptExit();
            return;
        }

        if (nearObject && nearObject.CompareTag("CNNLayer"))
        {
            StartMiniGame();
            return;
        }

        if (nearObject && nearObject.CompareTag("CommandCenter"))
        {
            CommandCenter commandCenter = nearObject.GetComponent<CommandCenter>();
            commandCenter.Activate();
            return;
        }

        // TODO: improve the context of an action
        if (grabbedObject)
        {
            if (nearObject && nearObject.CompareTag("Container"))
            {
                AttemptToFillAContainer();
            }
            else if (nearObject && nearObject.CompareTag("InputHolder"))
            {
                FillInputHolder();
            }
            else if (nearObject && nearObject.CompareTag("SpectralBandContainer"))
            {
                AttemptToFillSpectralBandContainer();
            }
            else if (nearObject && nearObject.CompareTag("Locker"))
            {
                FillLocker();
            }
            else
            {
                DropObject();
            }
        }
        else
        {
            if (nearObject == null)
            {
                // Debug.Log("No nearObject to grab");
                return;
            }

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
            else if (nearObject.CompareTag("SelectorSwitch"))
            {
                nearObject.GetComponent<SelectorSwitch>().Switch();
            }
            else if (nearObject.transform.parent && nearObject.transform.parent.CompareTag("Kernel"))
            {
                GrabKernel();
            }
            else if (nearObject.CompareTag("ActivationBox"))
            {
                GrabActivationBox();
            }

        }
    }

    private void MoveGrabbedObject()
    {
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj == null)
        {
            Debug.LogError("could not find player");
        }

        if (grabbedObject)
        {
            grabbedObject.transform.position = playerObj.transform.position;
        }
    }

    //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsValidNearObject(other))
        {
            // Debug.Log("On trigger enter 2d " + other.tag);
            nearObject = other.gameObject;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (IsValidNearObject(other))
        {
            nearObject = other.gameObject;
            // Debug.Log("On trigger stay 2d " + other.tag);
        }
    }

    private bool IsValidNearObject(Collider2D other)
    {
        if (other.CompareTag("Untagged"))
        {
            // Ignore
            return false;
        }

        if (grabbedObject && grabbedObject.CompareTag(other.tag))
        {
            // Debug.Log("Ignore grabbed object which is same tag as near object:" + other.tag);
            return false;
        }

        return true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Untagged"))
        {
            // Ignore
            return;
        }
        // Debug.Log("on trigger exit 2d " + other.tag);
        nearObject = null;
    }
}
