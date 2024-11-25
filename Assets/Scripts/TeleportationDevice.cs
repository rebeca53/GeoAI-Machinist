using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportationDevice : MonoBehaviour
{
    private float verticalOffset = 0.35f;
    private float horizontalOffset = 0f;
    private Animator animator;
    private string instruction;

    public TimedDialogueBalloon timedDialogueBalloon;
    public HintBalloon hintBalloon;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        hintBalloon.Hide();
    }

    public void Load(SampleBox sampleBox, string instruction)
    {
        // Update sample box chracteactis
        // Change scale
        sampleBox.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

        // change box parent
        sampleBox.gameObject.transform.parent = gameObject.transform;
        sampleBox.gameObject.transform.position = new Vector3(gameObject.transform.position.x + horizontalOffset, gameObject.transform.position.y + verticalOffset);

        this.instruction = instruction;
        timedDialogueBalloon.SetMessage(instruction);
    }

    public void Hint()
    {
        hintBalloon.SetTarget(gameObject);
        hintBalloon.Place();
        hintBalloon.SetSpaceKey();
        hintBalloon.Show();
    }

    public void Blink()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("blinking", true);
    }

    public void StopBlink()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("blinking", false);
    }

    public float GetX()
    {
        return transform.position.x;
    }

    public float GetY()
    {
        return transform.position.y;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // UIHandler.Instance.DisplayMessage(instruction);
            // timedDialogueBalloon.Show();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // timedDialogueBalloon.Hide();
            // UIHandler.Instance.HideMessage();
        }
    }
}

