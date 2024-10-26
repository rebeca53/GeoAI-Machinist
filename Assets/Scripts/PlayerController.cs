using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Movement
    [SerializeField] private float moveSpeed = 1.5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 moveDirection = new Vector2(1, 0);

    private Animator animator;

    // Action
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
        // Debug.Log("Hello");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentMoney = 0;

        audioSource = GetComponent<AudioSource>();
    }

    public void ChangeMoney(int amount)
    {
        currentMoney += amount;
        UIHandler.instance.SetMoneyValue(currentMoney);
    }

    private void OnDisable()
    {
        GameManager.instance.playerCoinPoints = currentMoney;
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    // Update is called once per frame
    void Update()
    {
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
        if (nearObject == null || !nearObject.CompareTag("SampleBox"))
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
        // Debug.Log("drop object onto container");

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

    private void OnSpaceAction()
    {
        // TODO: improve the context of an action
        if (grabbedObject)
        {
            if (nearObject.CompareTag("Container"))
            {
                AttemptToFillAContainer();
            }
            else
            {
                DropObject();
            }
        }
        else
        {
            GrabSampleBox();
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
        Debug.Log("On trigger stay 2d " + other.tag);
        nearObject = other.gameObject;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("on trigger exit 2d " + other.tag);
        nearObject = null;
    }
}
