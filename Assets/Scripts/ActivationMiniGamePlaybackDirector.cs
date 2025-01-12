using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ActivationMiniGamePlaybackDirector : MonoBehaviour
{
    public Action OnEnd;

    // public PlayableDirector introductionAnimation;
    public PlayerController Player;
    public NonPlayerCharacter NPC;
    public DialogueBalloon dialogueBalloon;
    public CameraZoom cameraZoom;
    List<(string, string)> screenplay = new List<(string, string)>();
    int currentLineIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        // introductionAnimation.stopped += OnPlayableDirectorStopped;
        // InitializeScreenplay();
        // Init();
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
        new("NPC", "This room is an Activation Layer of the CNN. It applies an activation function to the result of a convolution."),
        // An activation function helps a CNN combine simple patterns into complex ones, making it flexible and able to understand diverse data.
        // new("NPC", "The activation function is a non-linear function that enables a CNN be more 'creative' and generate new, complex and different features."),
        new("NPC", "An activation function adds non-linearity, enabling a CNN to learn complex patterns and better understand diverse data."),
        new("NPC", "It allows the CNN to adapt and learn more meaningful patterns, beyond the basic features extracted by earlier layers."),
        new("NPC", "Place the activation function in the input holder to apply it."),
        new("NPC", "Choose the best activation function that enhances the features in the image."),
        // new("action", "action1"), // Robot Walk
        };
    }

    void Init()
    {
        Player.Disable();
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
                break;
            case "action2":
                break;
        }
    }

    void ZoomIn()
    {
        cameraZoom.ChangeZoomSmooth(1.4f);
    }

    void ZoomOut()
    {
        cameraZoom.ChangeZoomSmooth(5f);
    }

    void End()
    {
        dialogueBalloon.Hide();
        ClearCallbacks();

        cameraZoom.ChangeZoomTarget(Player.gameObject);
        ZoomOut();

        Player.Enable();
        OnEnd?.Invoke();
    }

    void ClearCallbacks()
    {
        dialogueBalloon.OnDone -= NextLine;
    }

    void OnDisable()
    {
        dialogueBalloon.OnDone -= NextLine;
    }
}
