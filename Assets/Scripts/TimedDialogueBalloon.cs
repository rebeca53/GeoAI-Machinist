using System;
using TMPro;
using UnityEngine;

public class TimedDialogueBalloon : MonoBehaviour
{
    public Action OnDone;
    string relativePosition = "upperRight"; // can be upperLeft

    float xOffset = 0.85f;
    float yOffset = 0.55f;
    float timeout = 5f;
    float timer = 0f;

    public GameObject speaker;
    private Transform speech;
    private TextMeshPro label;

    void Awake()
    {
        Debug.Log("Dialogue Ballon Awake");
        speech = transform.Find("Speech");
        label = speech.GetComponent<TextMeshPro>();

        // Hide();
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
        else if (relativePosition == "upperCenter")
        {
            transform.position = new(speaker.transform.position.x, speaker.transform.position.y + yOffset, speaker.transform.position.z);
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

    public void PlaceUpperCenter()
    {
        relativePosition = "upperCenter";
        Place();
    }

    public void SetMessage(string message)
    {
        if (label == null)
        {
            // Debug.Log("label is null");
            speech = transform.Find("Speech");
            label = speech.GetComponent<TextMeshPro>();
        }
        label.text = message;
    }

    public void Show(float durationSeconds = 30f)
    {
        // Debug.Log("Set game object as active");
        gameObject.SetActive(true);
        timeout = durationSeconds;
        timer = 0;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Place();

        timer += Time.deltaTime;
        if (timer > timeout)
        {
            Hide();
            OnDone?.Invoke();
        }
    }
}
