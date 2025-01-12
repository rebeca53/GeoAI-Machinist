using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ConvolutionalMiniGamePlaybackDirector : MonoBehaviour
{
    public Action OnEnd;
    public PlayableDirector introductionAnimation;

    public PlayerController Player;
    public NonPlayerCharacter NPC;
    public DialogueBalloon dialogueBalloon;
    public HintBalloon hintBalloon;
    public CameraZoom cameraZoom;
    List<(string, string)> screenplay = new List<(string, string)>();
    int currentLineIndex = 0;

    public void StartAnimation()
    {
        introductionAnimation.stopped += OnPlayableDirectorStopped;
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
        new("NPC", "This room is a Convolutional Layer of the CNN. It applies a filter, known as ‘kernel’, to an input image, through matrix multiplication on their matrix representations."),
        new("NPC", "A kernel is a matrix with pre-determined values to enhance features in an image. You can see a kernel blinking over there."),
        // new("action", "action1"), // Robot Walk and Hint Kernel
        // new("action", "action2"), // Hint Kernel
        new("NPC", "Place the kernel in the input holder to start a convolution."),
        new("NPC", "Choose the best kernel that enhances the streets' footprint in the image."),
        // new("action", "action2"), // Hint Kernel and the Input Holder
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
                // NPCWalkToKernel();
                break;
            case "action2":
                // HintKernel();
                // HintInputHolder();
                // PlayerWalk();
                break;
        }
    }

    void HintInputHolder()
    {
        GameObject inputHolderObject = GameObject.Find("KernelHolder1");
        hintBalloon.SetSpaceKey();
        hintBalloon.SetTarget(inputHolderObject);
        hintBalloon.PlaceOver();
        hintBalloon.SetWaitKey(false);
        hintBalloon.Show();

        InputHolder inputHolder = inputHolderObject.GetComponent<InputHolder>();
        inputHolder.OnAddedObject += hintBalloon.Hide;
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
        HintInputHolder();
        OnEnd?.Invoke();
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
        introductionAnimation.stopped -= OnPlayableDirectorStopped;
        hintBalloon.OnDone -= Player.Disable;
        dialogueBalloon.OnDone -= NextLine;
    }
}
