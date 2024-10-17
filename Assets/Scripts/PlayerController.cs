using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    //Delay time in seconds to restart level.
    public float changeLevelDelay = 1f;

    // Movement
    [SerializeField] private float moveSpeed = 1.5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;

    // Action
    bool spaceAction = false;
    private GameObject nearObject = null;
    public GameObject grabbedObject = null;

    // Money system
    public int coins { get { return currentMoney; } }
    private int currentMoney;

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("Hello");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentMoney = 0;
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

    // Update is called once per frame
    void Update()
    {
        rb.velocity = moveInput * moveSpeed;

        spaceAction = Input.GetKeyDown(KeyCode.Space);
        animator.SetBool("playerAction", spaceAction);
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
    private bool isOnlyVertical(Vector2 input)
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
            if (!isOnlyVertical(moveInput))
            {
                animator.SetFloat("LastInputX", moveInput.x);
            }
        }

        moveInput = context.ReadValue<Vector2>();

        // only-vertical movement keep previous direction
        if (isOnlyVertical(moveInput))
        {
            animator.SetFloat("InputX", animator.GetFloat("LastInputX"));
        }
        else
        {
            animator.SetFloat("InputX", moveInput.x);
        }

    }

    private void grabSampleBox()
    {
        if (nearObject == null || nearObject.tag != "SampleBox")
        {
            return;
        }
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

    private void dropObject()
    {
        // Debug.Log("drop object");
        // Change scale
        grabbedObject.transform.localScale = new Vector3(1f, 1f, 1f);

        // change box parent
        grabbedObject.transform.parent = null;
        grabbedObject = null;
    }

    private void attemptToFillAContainer()
    {
        // Debug.Log("drop object onto container");

        Container container = nearObject.GetComponent<Container>();
        SampleBox sampleBox = grabbedObject.GetComponent<SampleBox>();

        if (!container.isMatch(sampleBox))
        {
            container.OnMismatch();
            return;
        }

        container.fillContainer(sampleBox);
        grabbedObject = null;
    }

    private void OnSpaceAction()
    {
        // TODO: improve the context of an action
        if (grabbedObject)
        {
            if (nearObject.tag == "Container")
            {
                attemptToFillAContainer();
            }
            else
            {
                dropObject();
            }
        }
        else
        {
            grabSampleBox();
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
        // Debug.Log("On trigger enter 2d " + other.tag);
        if (other.tag == "Exit")
        {
            Invoke("AttemptExit", changeLevelDelay);
        }
        else if (other.tag == "Container")
        {
            Container container = other.gameObject.GetComponent<Container>();
            // Debug.Log(container.type);
        }
        else if (other.tag == "SampleBox")
        {

        }
        nearObject = other.gameObject;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Debug.Log("On trigger stay 2d " + other.tag);

        if (other.tag == "Container")
        {
            Container container = other.gameObject.GetComponent<Container>();
            // Debug.Log(container.type);
        }
        else if (other.tag == "SampleBox")
        {

        }
        nearObject = other.gameObject;
    }

    private void AttemptExit()
    {
        Debug.Log("TODO: check if game is unclock");
        //Load the last scene loaded, in this case Main, the only scene in the game.
        // SceneManager.LoadScene(0);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Debug.Log("on trigger exit 2d " + other.tag);
        nearObject = null;
    }

}
