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

    public GameObject leftConnection;
    public GameObject rightConnection;
    public GameObject speaker;
    private Transform speech;
    private Transform hint;
    private TextMeshPro label;


    void Awake()
    {
        speech = transform.Find("Speech");
        label = speech.GetComponent<TextMeshPro>();
        hint = transform.Find("Hint");

        HideHint();
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
            leftConnection.SetActive(true);
            rightConnection.SetActive(false);
        }
        else if (relativePosition == "upperLeft")
        {
            transform.position = new(speaker.transform.position.x - xOffset, speaker.transform.position.y + yOffset, speaker.transform.position.z);
            leftConnection.SetActive(false);
            rightConnection.SetActive(true);
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
            speech = transform.Find("Speech");
            label = speech.GetComponent<TextMeshPro>();
        }
        label.text = message;
    }

    public void Show(float minDuration = 5f)
    {
        gameObject.SetActive(true);
        waitingKey = true;
        hintTimeout = minDuration;
        HideHint();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        waitingKey = false;
    }

    public void DisableKey()
    {
        waitingKey = false;
    }

    public void WaitForKey()
    {
        waitingKey = true;
    }

    public void ShowHint()
    {
        hint.gameObject.SetActive(true);
    }

    public void HideHint()
    {
        hintTimer = 0f;
        hint.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (waitingKey == false)
        {
            return;
        }

        hintTimer += Time.deltaTime;
        if (hintTimer >= hintTimeout)
        {
            ShowHint();
            if (waitingKey && Input.GetKeyDown("space"))
            {
                waitingKey = false;
                HideHint();
                OnDone?.Invoke();
            }
        }


    }
}
