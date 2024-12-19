using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class OutputMiniGamePlaybackDirector : MonoBehaviour
{
    public PlayableDirector playbackRobotWalk;
    public PlayerController Player;
    public NonPlayerCharacter NPC;

    public GameObject inputScreen;
    public DialogueBalloon dialogueBalloon;
    public HintBalloon hintBalloon;
    public CameraZoom cameraZoom;
    List<(string, string)> screenplay = new List<(string, string)>();
    int currentLineIndex = 0;

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
        new("NPC", "This room is the Output Layer of the CNN. At this point, each layer has extracted and highlighted features in the image."),
        new("NPC", "Follow me to see how the image looks now."), // instruction
        new("action", "action1"), // Robot Walk
        new("action", "action2"), // Wait Player
        new("NPC", "The CNN extracted features, but we can't tell which class this image belongs to. We need more steps."),
        new("NPC", "Find and activate the Flatenning Pull Lever to flat the matrix."),
        new("action", "action3"), // Wait player flatten
        // new("action", "action4"), // Layer flattened, class nodes animation done
        new("NPC", "Each pixel have a weight in the classification result."),
        new("NPC", "The connection between a pixel and a class node displays the weight of the pixel."), // instruction
        // new("NPC", "Approach a class node to highlight the weights."),
        new("NPC", "Ok, some class nodes are brighter than others. But we can not tell yet which class to choose."),
        new("NPC", "We need a softmax activation function to calculate the probability of the image belonging to each class"),
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

    public void NextLine()
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
                WalkToSample();
                break;
            case "action2":
                WaitPlayer();
                break;
            case "action3":
                WaitPlayerFlatten();
                break;
            default:
                // Do nothing
                break;
        }
    }

    void WalkToSample()
    {
        playbackRobotWalk.Play();
        playbackRobotWalk.stopped += OnPlayableDirectorStopped;
        dialogueBalloon.Hide();
    }

    void WaitPlayer()
    {
        Player.Enable();
        cameraZoom.ChangeZoomTarget(Player.gameObject);
        NPC.OnHover += NextLine;
    }

    void WaitPlayerFlatten()
    {
        cameraZoom.Release();
        cameraZoom.ChangeZoomTarget(Player.gameObject);
    }

    void HintSoftmax()
    {
        GameObject softmaxObject = GameObject.Find("ActivationBox");
        ActivationBox activationBox = softmaxObject.GetComponent<ActivationBox>();
        activationBox.Blink();
    }

    void ZoomIn()
    {
        cameraZoom.ChangeZoomSmooth(1.4f);
    }

    void ZoomOut()
    {
        cameraZoom.ChangeZoomSmooth(4f);
    }

    void End()
    {
        dialogueBalloon.Hide();
        ClearCallbacks();

        cameraZoom.ChangeZoomTarget(Player.gameObject);
        cameraZoom.Release();
        ZoomIn();

        Player.Enable();
        HintSoftmax();
    }

    void ClearCallbacks()
    {
        dialogueBalloon.OnDone -= NextLine;
        hintBalloon.OnDone -= Player.Disable;
        hintBalloon.OnDone -= NextLine;
        hintBalloon.OnDone -= ZoomIn;
        NPC.OnHover -= NextLine;
    }

    void OnDisable()
    {
        hintBalloon.OnDone -= Player.Disable;
        dialogueBalloon.OnDone -= NextLine;
    }
}
