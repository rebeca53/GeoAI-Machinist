using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ConvolutionalMiniGamePlaybackDirector : MonoBehaviour
{
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
        new("NPC", "This room is a Convolutional Layer of the CNN. It multiplies a kernel matrix by an image."),
        new("NPC", "A kernel is a matrix with pre-determined values to enhance features in an image. Follow me to see how a kernel looks like."),
        new("action", "action1"), // Robot Walk
        // new("action", "action2"), // Hint Kernel
        new("NPC", "Place the kernel in the input holder to start a convolution."),
        new("NPC", "Choose the best kernel that enhances the streets' footprint in the image."),
        // new("action", "action2"), // Hint Kernel and the Input Holder
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
                NPCWalkToKernel();
                break;
            case "action2":
                // HintKernel();
                // HintInputHolder();
                // PlayerWalk();
                break;
        }
    }

    void NPCWalkToKernel()
    {
        dialogueBalloon.Hide();
        HintKernel();
        introductionAnimation.Play(); // on stopped, it calls NextLine
    }

    void HintKernel()
    {
        GameObject kernelObject = GameObject.Find("Kernel0");
        KernelMatrix kernelMatrix = kernelObject.GetComponent<KernelMatrix>();
        kernelMatrix.Blink();
    }

    void HintInputHolder()
    {
        GameObject inputHolder = GameObject.Find("KernelHolder1");
        hintBalloon.SetSpaceKey();
        hintBalloon.SetTarget(inputHolder);
        hintBalloon.PlaceOver();
        hintBalloon.Show();

        Player.Enable();
        hintBalloon.OnDone += NextLine;
    }

    void ZoomIn()
    {
        cameraZoom.ChangeZoomSmooth(1.4f);
    }

    void End()
    {
        dialogueBalloon.Hide();
        ClearCallbacks();

        cameraZoom.ChangeZoomTarget(Player.gameObject);
        cameraZoom.Release();
        ZoomIn();

        Player.Enable();
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
