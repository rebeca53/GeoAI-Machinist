using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// Similar to PlayableDirectorCallback
public class DataLabelingPlaybackDirector : MonoBehaviour
{
    public PlayableDirector director;
    public PlayableDirector directorWalkContainer;
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
        director.stopped += OnPlayableDirectorStopped;
        directorWalkContainer.stopped += OnPlayableDirectorStopped;
        InitializeScreenplay();
        Init();
    }

    void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        NextLine();
    }

    void InitializeScreenplay()
    {
        screenplay.Add(new("NPC", "This is the Data Labeling Room. Looks like all the samples are shuffled. You need to place them in the container with the correct label."));
        screenplay.Add(new("NPC", "First step is to grab a sample. There is one sample just behind you."));
        screenplay.Add(new("action", "action1"));
        screenplay.Add(new("NPC", "Now, lets find the correct container and drop the sample."));
        screenplay.Add(new("action", "action2"));
        screenplay.Add(new("action", "action3"));
        screenplay.Add(new("NPC", "Did the container turn green? Perfect, do the same for the other samples. I will wait at the Exit"));
        screenplay.Add(new("action", "action4"));
        // Robot walks to the Exit
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
            case "Player":
                dialogueBalloon.SetSpeaker(Player.gameObject);
                dialogueBalloon.PlaceUpperLeft();
                if (HasSpeakerChanged())
                {
                    Debug.Log("speaker has changed");
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
                HintGrabSample();
                break;
            case "action2":
                NPCWalkToContainer();
                break;
            case "action3":
                HintDropOnContainer();
                break;
            case "action4":
                NPCWalkToExit();
                HintExit();
                break;
        }
    }

    // show the hint in the sample
    // release player
    // on grab, block player again
    void HintGrabSample()
    {
        Debug.Log("Action 1");
        dialogueBalloon.HideHint();

        GameObject residentialBox = GameObject.Find("Residential_Box(Clone)");
        if (residentialBox == null)
        {
            Debug.Log("Not able to find residential Box");
        }
        hintBalloon.SetSpaceKey();
        hintBalloon.SetTarget(residentialBox);
        hintBalloon.PlaceOver();
        hintBalloon.Show();
        hintBalloon.OnDone += Player.Disable;
        hintBalloon.OnDone += NextLine;

        cameraZoom.ChangeZoomTarget(Player.gameObject);
        Player.Enable();
    }

    void NPCWalkToContainer()
    {
        Debug.Log("Action 2");
        dialogueBalloon.Hide();
        directorWalkContainer.Play(); // on stopped, it calls NextLine
    }

    // zoom out to show the highlighted container
    // hint the container
    // on drop in the container, display message
    void HintDropOnContainer()
    {
        Debug.Log("Action 3");
        ZoomOut();
        dialogueBalloon.HideHint();

        GameObject residentialContainer = FindResidentialContainer();
        hintBalloon.SetSpaceKey();
        hintBalloon.SetTarget(residentialContainer);
        hintBalloon.PlaceOver();
        hintBalloon.Show();

        Player.Enable();
        hintBalloon.OnDone += Player.Disable;
        hintBalloon.OnDone += NextLine;
        hintBalloon.OnDone += ZoomIn;
    }

    void NPCWalkToExit()
    {
        dialogueBalloon.Hide();
        ZoomOut();
        director.Play(); // on stopped, it calls NextLine
    }

    void HintExit()
    {
        GameObject exit = GameObject.FindGameObjectWithTag("Exit");
        if (exit == null)
        {
            Debug.Log("Not able to find exit");
        }
        hintBalloon.SetSpaceKey();
        hintBalloon.SetTarget(exit);
        hintBalloon.PlaceOver();
        hintBalloon.Show();
    }

    private GameObject FindResidentialContainer()
    {
        GameObject[] allContainers = GameObject.FindGameObjectsWithTag("Container");
        foreach (GameObject container in allContainers)
        {
            Container script = container.GetComponent<Container>();
            if (script.type == "Residential")
            {
                return container;
            }
        }

        Debug.LogError("Unable to find the Residential Container");
        return null;
    }

    void ZoomOut()
    {
        cameraZoom.ChangeZoomSmooth(4f);
    }

    void ZoomIn()
    {
        cameraZoom.ChangeZoomSmooth(1.2f);
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

    void OnDisable()
    {
        director.stopped -= OnPlayableDirectorStopped;
        hintBalloon.OnDone -= Player.Disable;
        dialogueBalloon.OnDone -= NextLine;
    }

    void ClearCallbacks()
    {
        dialogueBalloon.OnDone -= NextLine;
        hintBalloon.OnDone -= Player.Disable;
        hintBalloon.OnDone -= NextLine;
        hintBalloon.OnDone -= ZoomIn;
    }
}
