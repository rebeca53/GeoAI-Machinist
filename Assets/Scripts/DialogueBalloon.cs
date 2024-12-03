using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueBalloon : MonoBehaviour
{
    public Action OnDone;
    bool waitingKey = false;
    string relativePosition = "upperRight"; // can be upperLeft

    float xOffset = 0.85f;
    float yOffset = 0.55f;
    public float hintTimeout = 5f;
    public float hintTimer = 0f;

    public GameObject speaker;
    private Transform speech;
    private Transform hint;
    private TextMeshPro label;


    void Awake()
    {
        Debug.Log("Dialogue Ballon Awake");
        speech = transform.Find("Speech");
        label = speech.GetComponent<TextMeshPro>();
        hint = transform.Find("Hint");
        Debug.Log("Dialogue Ballon Awake Done");

        // Hide();
        HideHint();
        // Place();
    }

    private void Place()
    {
        if (speaker == null)
        {
            Debug.LogError("No Speaker to use as position reference");
            return;
        }

        if (relativePosition == "upperRight")
        {
            transform.position = new(speaker.transform.position.x + xOffset, speaker.transform.position.y + yOffset, speaker.transform.position.z);
        }
        else if (relativePosition == "upperLeft")
        {
            transform.position = new(speaker.transform.position.x - xOffset, speaker.transform.position.y + yOffset, speaker.transform.position.z);
        }
    }

    public void SetSpeaker(GameObject newSpeaker)
    {
        speaker = newSpeaker;
    }

    public void PlaceUpperLeft()
    {
        relativePosition = "upperLeft";
        Place();
    }

    public void PlaceUpperRight()
    {
        relativePosition = "upperRight";
        Place();
    }

    public void SetMessage(string message)
    {
        if (label == null)
        {
            Debug.Log("label is null");
            speech = transform.Find("Speech");
            label = speech.GetComponent<TextMeshPro>();
        }
        label.text = message;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        waitingKey = true;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        waitingKey = false;
    }

    public void ShowHint()
    {
        hint.gameObject.SetActive(true);
    }

    public void HideHint()
    {
        hintTimer = hintTimeout;
        hint.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        hintTimer += Time.deltaTime;
        if (hintTimer >= hintTimeout)
        {
            ShowHint();
        }

        if (waitingKey && Input.GetKeyDown("space"))
        {
            Debug.Log("SPACE pressed");
            waitingKey = false;
            HideHint();
            OnDone?.Invoke();
        }
    }
}
