using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class InputMiniGamePlaybackDirector : MonoBehaviour
{
    // public PlayableDirector director;
    public Action OnEnd;
    public PlayableDirector firstTurnAnimation;
    public PlayerController Player;
    public NonPlayerCharacter NPC;
    public DialogueBalloon dialogueBalloon;
    public HintBalloon hintBalloon;
    public CameraZoom cameraZoom;
    List<(string, string)> screenplay = new List<(string, string)>();
    int currentLineIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        firstTurnAnimation.stopped += OnPlayableDirectorStopped;
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
        new("NPC", "This room is the Input Layer of the CNN. It breaks the image into spectral bands, which are wavelength intervals of light."),
        new("NPC", "Follow me to interact with the input sample to see its spectral bands."),
        new("action", "action1"), // Robot Walk
        new("action", "action2"), // Hint Teleportation Device
        new("NPC", "Interact with the input sample to see its spectral bands."),
        new("NPC", "Choose ONE spectral band to reveal characteristics of a River and place it in the correct container."),
        // new("NPC", "Choose ONE spectral bands to reveal relevant characteristics of a River and place it in the correct container."),
        // new("NPC", "A River is characterize by the water, moisture, and it is necessary to tell it apart from vegetation."),
        };
    }

    void Init()
    {
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
                dialogueBalloon.PlaceUpperRight();
                if (HasSpeakerChanged())
                {
                    cameraZoom.ChangeZoomTarget(NPC.gameObject);
                }
                dialogueBalloon.SetMessage(line.Item2);
                dialogueBalloon.Show();
                dialogueBalloon.OnDone += NextLine;
                break;
            case "Player":
                dialogueBalloon.SetSpeaker(Player.gameObject);
                dialogueBalloon.PlaceUpperLeft();
                if (HasSpeakerChanged())
                {
                    cameraZoom.ChangeZoomTarget(Player.gameObject);
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
                NPCWalkToSample();
                break;
            case "action2":
                HintInputSample();
                break;
        }
    }

    void NPCWalkToSample()
    {
        dialogueBalloon.Hide();
        firstTurnAnimation.Play(); // on stopped, it calls NextLine
    }

    void HintInputSample()
    {
        ZoomOut();

        GameObject inputSample = GameObject.FindGameObjectWithTag("SampleBox");
        hintBalloon.SetSpaceKey();
        hintBalloon.SetTarget(inputSample);
        hintBalloon.PlaceOver();
        hintBalloon.SetWaitKey(false);
        hintBalloon.Show();

        SampleBox sampleBox = inputSample.GetComponent<SampleBox>();
        sampleBox.OnBreak += OnHintedSampleBroken;

        Player.Enable();
    }

    void OnHintedSampleBroken(string sampleBox, Vector3 position)
    {
        // hintBalloon.Hide() is called by InputMiniGameManager.LayoutGrayscaleBands
        Player.Disable();
        NextLine();
        ZoomIn();
    }

    void ZoomOut()
    {
        cameraZoom.ChangeZoomSmooth(5f);
    }

    void ZoomIn()
    {
        cameraZoom.ChangeZoomSmooth(1.2f);
    }

    void End()
    {
        dialogueBalloon.Hide();
        ClearCallbacks();

        cameraZoom.ChangeZoomTarget(Player.gameObject);
        cameraZoom.Release();
        ZoomIn();

        Player.Enable();
        OnEnd?.Invoke();
    }

    void OnDisable()
    {
        firstTurnAnimation.stopped -= OnPlayableDirectorStopped;
        dialogueBalloon.OnDone -= NextLine;
    }

    void ClearCallbacks()
    {
        dialogueBalloon.OnDone -= NextLine;
    }
}
