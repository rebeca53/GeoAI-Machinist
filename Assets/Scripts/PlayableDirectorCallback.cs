using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayableDirectorCallback : MonoBehaviour
{
    public PlayableDirector endAnimation;
    public PlayableDirector director;
    public PlayerController Player;
    public NonPlayerCharacter NPC;

    public DialogueBalloon dialogueBalloon;
    public HintBalloon hintBalloon;
    List<(string, string)> screenplay = new List<(string, string)>();
    int currentLineIndex = -1;

    void OnEnable()
    {
        director.stopped += OnPlayableDirectorStopped;
        endAnimation.stopped += OnEndAnimationStopped;
        InitializeScreenplay();
    }

    void InitializeScreenplay()
    {
        screenplay.Add(new("NPC", "Hello, GeoAI Machinist.\n\nFeeling well?\nAre you prepared for your mission?"));
        screenplay.Add(new("Player", "Who are you? What is going on?"));
        screenplay.Add(new("NPC", "I'm your Robot Assistant, here to guide you. I'll provide all mission details shortly."));
        screenplay.Add(new("NPC", "As the GeoAI Machinist, your mission is to maintain the Big Machine—a space station designed to survey Earth and respond to emergency situations."));
        screenplay.Add(new("NPC", "Records indicate that Earth has entered another Heat Season, and the delicate balance necessary for the last humans to survive is in danger."));
        screenplay.Add(new("NPC", "A critical malfunction was detected in the Big Machine. The Artificial Intelligence, our core operational system... has gone offline. You must repair it."));
        screenplay.Add(new("NPC", "Without the AI functioning, the Big Machine cannot deploy the Help Pods, which are essential for Earth's survival."));
        screenplay.Add(new("Player", "What are Help Pods?"));
        screenplay.Add(new("NPC", "These pods are loaded with critical resources: food, water, medical supplies, emergency cooling units—everything needed to counter the deadly heat."));
        screenplay.Add(new("Player", "How severe is the AI damage?"));
        screenplay.Add(new("NPC", "Diagnostics show extensive corruption in the main AI subsystems. You'll need to prepare data, and possibly reconstruct some modules."));
        screenplay.Add(new("NPC", "Now, let’s access the AI core. Are you ready?"));
        screenplay.Add(new("Player", "Yes, I'm ready to proceed."));
        screenplay.Add(new("NPC", "Acknowledged. Follow me, Machinist."));
    }

    void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        if (director == aDirector)
        {
            Debug.Log("PlayableDirector named " + aDirector.name + " is now stopped.");
            ZoomIn();

            Player.TurnRight();
            NextLine();
            dialogueBalloon.OnDone += NextLine;
        }
    }

    void NextLine()
    {
        dialogueBalloon.Hide();
        currentLineIndex++;

        if (screenplay.Count <= currentLineIndex)
        {
            End();
            return;
        }

        var line = screenplay[currentLineIndex];

        if (line.Item1.Equals("NPC"))
        {
            dialogueBalloon.SetSpeaker(NPC.gameObject);
            dialogueBalloon.PlaceUpperRight();
            if (HasSpeakerChanged())
            {
                Debug.Log("speaker has changed");
                NPC.Speak();
                FollowSpeaker(NPC.gameObject);
            }
        }
        else if (line.Item1.Equals("Player"))
        {
            dialogueBalloon.SetSpeaker(Player.gameObject);
            dialogueBalloon.PlaceUpperLeft();
            if (HasSpeakerChanged())
            {
                Debug.Log("speaker has changed");
                FollowSpeaker(Player.gameObject);
            }
        }

        dialogueBalloon.SetMessage(line.Item2);
        dialogueBalloon.Show();
    }

    private bool HasSpeakerChanged()
    {
        Debug.Log("idx " + currentLineIndex);

        if (currentLineIndex < 1) return true;
        Debug.Log("previous " + screenplay[currentLineIndex - 1].Item1);
        Debug.Log("current " + screenplay[currentLineIndex].Item1);

        return !screenplay[currentLineIndex].Item1.Equals(screenplay[currentLineIndex - 1].Item1);
    }

    private void OnEndAnimationStopped(PlayableDirector aDirector)
    {
        if (endAnimation == aDirector)
        {
            Debug.Log("PlayableDirector named " + aDirector.name + " is now stopped.");
            hintBalloon.SetTarget(Player.gameObject);
            hintBalloon.SetArrowRightKey();
            hintBalloon.Show();
        }
    }

    private void End()
    {
        FollowSpeaker(Player.gameObject);

        endAnimation.Play();
    }

    public void ZoomIn()
    {
        Debug.Log("Zoom In");
        GameObject virtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera");
        CameraZoom cameraZoom = virtualCamera.GetComponent<CameraZoom>();

        if (cameraZoom == null)
        {
            Debug.LogError("Unable to retrieve camera");
        }
        else
        {
            Debug.Log("Retrieveing object");
        }
        cameraZoom.ChangeZoomSmooth(1.5f);
    }

    public void ZoomOut()
    {
        Debug.Log("Zoom In");
        GameObject virtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera");
        CameraZoom cameraZoom = virtualCamera.GetComponent<CameraZoom>();

        if (cameraZoom == null)
        {
            Debug.LogError("Unable to retrieve camera");
        }
        else
        {
            Debug.Log("Retrieveing object");
        }
        cameraZoom.ChangeZoomSmooth(4f);
    }

    public void FollowSpeaker(GameObject speaker)
    {
        Debug.Log("FollowSpeaker");
        GameObject virtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera");
        CameraZoom cameraZoom = virtualCamera.GetComponent<CameraZoom>();

        if (cameraZoom == null)
        {
            Debug.LogError("Unable to retrieve camera");
        }
        else
        {
            Debug.Log("Retrieveing object");
        }
        cameraZoom.ChangeZoomTarget(speaker);
    }

    void OnDisable()
    {
        director.stopped -= OnPlayableDirectorStopped;
    }
}
