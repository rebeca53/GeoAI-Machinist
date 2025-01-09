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
        screenplay.Add(new("NPC", "Did the container turn green? Perfect, do the same for the other samples. I will wait at the Exit."));
        screenplay.Add(new("action", "action4"));
        // Robot walks to the Exit
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
                if (currentLineIndex == 6) // Did the Container turn...
                {
                    dialogueBalloon.PlaceUpperLeft();
                }
                else
                {
                    dialogueBalloon.PlaceUpperRight();
                }
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
                break;
        }
    }

    // show the hint in the sample
    // release player
    // on grab, block player again
    void HintGrabSample()
    {
        dialogueBalloon.HideHint();

        GameObject residentialBox = GameObject.Find("Residential_Box(Clone)");
        if (residentialBox == null)
        {
            Debug.LogError("Not able to find residential Box");
            return;
        }
        hintBalloon.SetSpaceKey();
        hintBalloon.SetTarget(residentialBox);
        hintBalloon.PlaceOver();
        hintBalloon.SetWaitKey(false);
        hintBalloon.Show();

        BlockOtherSamples();

        cameraZoom.ChangeZoomTarget(Player.gameObject);
        Player.Enable();
    }

    void DisplayWrongSampleMessage()
    {
        dialogueBalloon.SetSpeaker(NPC.gameObject);
        dialogueBalloon.SetMessage("Grab the sample that is highlighted.");
        dialogueBalloon.PlaceUpperRight();
        dialogueBalloon.Show();
        dialogueBalloon.DisableKey();
    }

    void ResidentialSampleGrabbed()
    {
        hintBalloon.Hide();
        Player.Disable();
        NextLine();
    }

    void BlockOtherSamples()
    {
        GameObject[] samples = GameObject.FindGameObjectsWithTag("SampleBox");
        foreach (GameObject sampleObject in samples)
        {
            SampleBox sampleBox = sampleObject.GetComponent<SampleBox>();
            if (sampleBox.type.Equals("Residential"))
            {
                sampleBox.OnGrab += ResidentialSampleGrabbed;
                sampleBox.OnWrongDrop += DisplayWrongContainerMessage;
            }
            else
            {
                sampleBox.Block();
                sampleBox.OnTryGrabBlocked += DisplayWrongSampleMessage;
            }
            sampleBox.SetCanDrop(false);
        }
    }

    void ResetSamples()
    {
        GameObject[] samples = GameObject.FindGameObjectsWithTag("SampleBox");
        foreach (GameObject sampleObject in samples)
        {
            SampleBox sampleBox = sampleObject.GetComponent<SampleBox>();
            if (sampleBox.type.Equals("Residential"))
            {
                sampleBox.OnGrab -= NextLine;
                sampleBox.OnWrongDrop -= DisplayWrongContainerMessage;
            }
            else
            {
                sampleBox.Release();
                sampleBox.OnTryGrabBlocked -= DisplayWrongSampleMessage;
            }
            sampleBox.SetCanDrop(true);
        }
    }

    void NPCWalkToContainer()
    {
        dialogueBalloon.Hide();
        directorWalkContainer.Play(); // on stopped, it calls NextLine
    }

    // zoom out to show the highlighted container
    // hint the container
    // on drop in the container, display message
    void HintDropOnContainer()
    {
        ZoomOut();
        dialogueBalloon.HideHint();

        GameObject residentialContainer = FindResidentialContainer();
        hintBalloon.SetSpaceKey();
        hintBalloon.SetTarget(residentialContainer);
        hintBalloon.PlaceOver();
        hintBalloon.SetWaitKey(false);
        hintBalloon.Show();
        SetupContainers();

        Player.Enable();
    }

    void ResidentialContainerFilled()
    {
        hintBalloon.Hide();
        Player.Disable();
        NextLine();
        ZoomIn();
    }

    void DisplayWrongContainerMessage()
    {
        dialogueBalloon.SetSpeaker(NPC.gameObject);
        dialogueBalloon.SetMessage("Place the sample on the Residential container.");
        dialogueBalloon.PlaceUpperLeft();
        dialogueBalloon.Show();
        dialogueBalloon.DisableKey();
    }

    void SetupContainers()
    {
        GameObject[] containers = GameObject.FindGameObjectsWithTag("Container");
        foreach (GameObject containerObject in containers)
        {
            Container container = containerObject.GetComponent<Container>();
            if (container.type.Equals("Residential"))
            {
                container.OnMatch += ResidentialContainerFilled;
            }
            else
            {
                container.OnWrongMatch += DisplayWrongContainerMessage;
            }
        }
    }

    void ResetContainers()
    {
        GameObject[] containers = GameObject.FindGameObjectsWithTag("Container");
        foreach (GameObject containerObject in containers)
        {
            Container container = containerObject.GetComponent<Container>();
            if (container.type.Equals("Residential"))
            {
                container.OnMatch -= NextLine;
            }
            else
            {
                container.OnWrongMatch -= DisplayWrongContainerMessage;
            }
        }
    }

    void NPCWalkToExit()
    {
        dialogueBalloon.Hide();
        ZoomOut();
        director.Play(); // on stopped, it calls NextLine
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
        dialogueBalloon.Hide();
        ClearCallbacks();

        cameraZoom.ChangeZoomTarget(Player.gameObject);
        ZoomOut();

        ResetSamples();
        ResetContainers();

        Player.Enable();
        NPC.OnHover += DisplayInstruction;
    }

    void DisplayInstruction()
    {
        dialogueBalloon.SetSpeaker(NPC.gameObject);
        dialogueBalloon.SetMessage("Place each sample under the correct label, so the Big Machine can learn from you.");
        dialogueBalloon.PlaceUpperLeft();
        dialogueBalloon.Show();
        dialogueBalloon.OnDone += dialogueBalloon.Hide;
    }

    void OnDisable()
    {
        director.stopped -= OnPlayableDirectorStopped;
        hintBalloon.OnDone -= Player.Disable;
        dialogueBalloon.OnDone -= NextLine;
        NPC.OnHover -= DisplayInstruction;
    }

    void ClearCallbacks()
    {
        dialogueBalloon.OnDone -= NextLine;
        hintBalloon.OnDone -= Player.Disable;
        hintBalloon.OnDone -= NextLine;
        hintBalloon.OnDone -= ZoomIn;
    }
}
