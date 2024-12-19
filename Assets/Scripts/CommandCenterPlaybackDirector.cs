using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Playables;

public class CommandCenterPlaybackDirector : MonoBehaviour
{
    public PlayableDirector sendHelp;

    PlayerController Player;
    NonPlayerCharacter NPC;
    public DialogueBalloon dialogueBalloon;
    public HintBalloon hintBalloon;
    CameraZoom cameraZoom;
    public CommandCenter commandCenter;
    public GameObject screen;
    List<(string, string)> screenplay = new List<(string, string)>();
    int currentLineIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        // sendHelp.stopped += OnPlayableDirectorStopped;

        // // TODO: Refactor this part. Not a good practice to Find objects
        // NPC = GameObject.Find("NPC").GetComponent<NonPlayerCharacter>();
        // Player = GameObject.Find("Player").GetComponent<PlayerController>();

        // // dialogueBalloon = GameObject.Find("DialogueBalloon").GetComponent<DialogueBalloon>();
        // // hintBalloon = GameObject.Find("HintBalloon").GetComponent<HintBalloon>();
        // cameraZoom = GameObject.Find("VirtualCamera").GetComponent<CameraZoom>();
    }

    public void StartAnimation()
    {
        InitializeScreenplay();
        Init();
    }

    void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        NextLine();
    }

    void InitializeScreenplay()
    {
        screenplay = new List<(string, string)>() {
        new("NPC", "Thanks to you the AI core is fully functional. Time to dispatch the Help Pods."),
        new("action", "action1"), // Hint CommandCenter and wait for activation
        new("action", "action2") // Play GameOver animation
        };
    }

    void Init()
    {
        sendHelp.stopped += OnPlayableDirectorStopped;

        // TODO: Refactor this part. Not a good practice to Find objects
        NPC = GameObject.Find("NPC").GetComponent<NonPlayerCharacter>();
        Player = GameObject.Find("Player").GetComponent<PlayerController>();

        // dialogueBalloon = GameObject.Find("DialogueBalloon").GetComponent<DialogueBalloon>();
        // hintBalloon = GameObject.Find("HintBalloon").GetComponent<HintBalloon>();
        cameraZoom = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CameraZoom>();

        Player.Disable();
        cameraZoom.Block();
        ZoomIn();
        dialogueBalloon.Hide();

        NextLine();
    }

    void NextLine()
    {
        ClearCallbacks();

        if (screenplay.Count <= currentLineIndex)
        {
            End();
            return;
        }

        var line = screenplay[currentLineIndex];
        // Debug.Log("Current line: " + line.Item1 + " - " + line.Item2);
        switch (line.Item1)
        {
            case "action":
                ExecuteAction(line.Item2);
                break;
            case "NPC":
                dialogueBalloon.SetSpeaker(NPC.gameObject);
                dialogueBalloon.PlaceUpperLeft();
                if (HasSpeakerChanged())
                {
                    cameraZoom.ChangeZoomTarget(NPC.gameObject);
                }
                dialogueBalloon.SetMessage(line.Item2);
                dialogueBalloon.Show();
                dialogueBalloon.OnDone += NextLine;
                break;
        }

        currentLineIndex++;
    }

    private bool HasSpeakerChanged()
    {
        if (currentLineIndex < 1) return true;
        return !screenplay[currentLineIndex].Item1.Equals(screenplay[currentLineIndex - 1].Item1);
    }

    void ExecuteAction(string actionId)
    {
        switch (actionId)
        {
            case "action1":
                HintCommandCenter();
                break;
            case "action2":
                DisplayGameOverAnimation();
                break;
        }
    }

    void HintCommandCenter()
    {
        ZoomOut();

        hintBalloon.SetSpaceKey();
        hintBalloon.SetTarget(commandCenter.gameObject);
        hintBalloon.PlaceOver();
        hintBalloon.Show();

        Player.Enable();
        commandCenter.OnActivated += NextLine;
    }

    void DisplayGameOverAnimation()
    {
        cameraZoom.ChangeZoomSmooth(3f);
        cameraZoom.ChangeZoomTarget(screen);
        sendHelp.Play();
    }

    void ZoomIn()
    {
        cameraZoom.ChangeZoomSmooth(1.4f);
    }

    void ZoomOut()
    {
        cameraZoom.ChangeZoomSmooth(2f);
    }

    void End()
    {
        Player.Disable();
        dialogueBalloon.Hide();
        ClearCallbacks();
    }

    void ClearCallbacks()
    {
        dialogueBalloon.OnDone -= NextLine;
        hintBalloon.OnDone -= Player.Disable;
        hintBalloon.OnDone -= NextLine;
        hintBalloon.OnDone -= ZoomIn;
    }

    void OnDisable()
    {
        sendHelp.stopped -= OnPlayableDirectorStopped;
        dialogueBalloon.OnDone -= NextLine;
    }
}
