using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueBalloon : MonoBehaviour
{
    public Action OnDone;
    string relativePosition = "upperRight"; // can be upperLeft

    float xOffset = 0.85f;
    float yOffset = 0.55f;
    public float hintTimeout = 5f;
    public float hintTimer = 0f;

    public GameObject speaker;
    private Transform background;
    private Transform speech;
    private Transform hint;
    private TextMeshPro label;


    // Start is called before the first frame update
    void Start()
    {
        background = transform.Find("BackgroundBalloon");
        speech = transform.Find("Speech");
        label = speech.GetComponent<TextMeshPro>();
        hint = transform.Find("Hint");

        Hide();
        HideHint();
        Place();
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
        label.text = message;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ShowHint()
    {
        hint.gameObject.SetActive(true);
    }

    public void HideHint()
    {
        hint.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        hintTimer += Time.deltaTime;
        if (hintTimer > hintTimeout)
        {
            ShowHint();
        }

        if (Input.GetKeyDown("space"))
        {
            Debug.Log("SPACE pressed");
            // Hide();
            OnDone?.Invoke();
        }
    }
}
