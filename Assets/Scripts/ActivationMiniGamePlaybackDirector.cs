using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ActivationMiniGamePlaybackDirector : MonoBehaviour
{
    // public PlayableDirector introductionAnimation;
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
        // introductionAnimation.stopped += OnPlayableDirectorStopped;
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
        new("NPC", "This room is an Activation Layer of the CNN."),
        // new("action", "action1"), // Robot Walk
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
        Debug.Log("Current line: " + line.Item1 + " - " + line.Item2);
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
        Debug.Log("idx " + currentLineIndex);

        if (currentLineIndex < 1) return true;
        Debug.Log("previous " + screenplay[currentLineIndex - 1].Item1);
        Debug.Log("current " + screenplay[currentLineIndex].Item1);

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

    void End()
    {
        Debug.Log("End");
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
        // introductionAnimation.stopped -= OnPlayableDirectorStopped;
        hintBalloon.OnDone -= Player.Disable;
        dialogueBalloon.OnDone -= NextLine;
    }
}
