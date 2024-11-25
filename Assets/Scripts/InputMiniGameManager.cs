using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class InputMiniGameManager : BaseBoard
{
    public GameObject[] sampleTiles;
    public GameObject spectralBandTile;
    public GameObject spectralBandContainerTile;
    public GameObject selectorSwitch;
    public GameObject teleportationDeviceTile;

    public DialogueBalloon dialogueBalloon;
    public HintBalloon hintBalloon;
    public PlayableDirector firstTurnAnimation;

    private List<string> bandTypes = new List<string> { "red", "green", "blue", "swir", "redEdge" };
    private int fullSpectralBandsContainer = 0;

    Dictionary<string, SelectorSwitch> bandSelectors = new Dictionary<string, SelectorSwitch>();
    TeleportationDevice teleportationDevice;
    Dictionary<string, SpectralBandContainer> containers = new Dictionary<string, SpectralBandContainer>();
    SampleBox sampleBox;

    // Turn-related variables
    private List<Turn> turns = new List<Turn> {
        new(0, "River", new List<string>{"blue", "green", "swir"}, "Place the spectral bands in their containers, then activate the switches of the best bands to reveal characteristics of a River.\nA River is characterize by the water, moisture, and it is necessary to tell it apart from vegetation."),
        new(1, "Highway", new List<string>{"red", "green"}, "Activate the switches of the bands more likely to reveal relevant characteristics of a Highway.\nA Highway is a man-made feature, and it is necessary to tell it apart from vegetation and water."),
        new(2, "Residential", new List<string>{"red", "blue"}, "Activate the switches of the bands more likely to reveal relevant characteristics of a Residential area.\nA Residential area is a man-made feature, and it is necessary to tell it apart from vegetation."),
    };

    int currentTurn = 0;

    List<(string, string)> screenplay = new List<(string, string)>();
    int currentLineIndex = 0;
    bool waitForAnimation = false;

    // Start is called before the first frame update
    void Start()
    {
        InitializeTilemap();

        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                DrawFloor(x, y);

                if (IsBorder(x, y) && !IsExit(x, y))
                {
                    DrawWall(x, y);
                }
                if (IsExit(x, y))
                {
                    DrawExit(x, y);
                }
            }
        }

        Player.Spawn(this, new Vector2Int(2, 1));
        NPC.Spawn(this, new Vector2Int(1, 1));

        // Assign the sprite to each turn
        turns[0].SetSample(sampleTiles[0]); //River
        turns[1].SetSample(sampleTiles[1]); //Highway
        turns[2].SetSample(sampleTiles[2]); //Residential

        LayoutInputHolder();
        LayoutSample();
        LayoutBandContainers();
        LayoutBandSelector();

        firstTurnAnimation.stopped += EndOfFirstTurn;
        AnimateFirstTurn();
    }


    private void AnimateFirstTurn()
    {
        Player.Disable();
        DisableManualZoom();
        ZoomIn();

        screenplay = new List<(string, string)>() {
        new("NPC", "This room is the Input Layer of the CNN. It breaks the image into spectral bands, which are wavelenghts interval of light."),
        new("NPC", "Follow me. Interact with the input sample to see its spectral bands."),
        new("NPC", "Activate the switches of the bands more likely to reveal relevant characteristics of a River."),
        new("NPC", "A River is characterize by the water, moisture, and it is necessary to tell it apart from vegetation."),
        };

        dialogueBalloon.OnDone += NextStep;

        DisplayLine(screenplay[currentLineIndex].Item1, screenplay[currentLineIndex].Item2);

        // Timeline: NPC and Player walks to the input holder
    }

    void DisplayLine(string speaker, string message)
    {
        if (speaker.Equals("NPC"))
        {
            dialogueBalloon.SetSpeaker(NPC.gameObject);
            dialogueBalloon.PlaceUpperRight();
            dialogueBalloon.SetMessage(message);
            dialogueBalloon.Show();

            // if (HasSpeakerChanged())
            // {
            //     Debug.Log("speaker has changed");
            //     NPC.Speak();
            //     FollowSpeaker(NPC.gameObject);
            // }
        }
        else if (speaker.Equals("Player"))
        {
            dialogueBalloon.SetSpeaker(Player.gameObject);
            dialogueBalloon.PlaceUpperLeft();
            dialogueBalloon.SetMessage(message);
            dialogueBalloon.Show();

            // if (HasSpeakerChanged())
            // {
            //     Debug.Log("speaker has changed");
            //     FollowSpeaker(Player.gameObject);
            // }
        }
    }

    void NextStep()
    {
        dialogueBalloon.Hide();
        if (waitForAnimation)
        {
            Debug.Log("waiting for animation");
            return;
        }
        currentLineIndex++;

        // Animate on "Follow me."
        if (currentLineIndex == 1)
        {
            Debug.Log("Animate on 'Follow me'");
            DisplayLine(screenplay[currentLineIndex].Item1, screenplay[currentLineIndex].Item2);
            firstTurnAnimation.Play();
            waitForAnimation = true;
            return;
        }

        if (currentLineIndex < screenplay.Count)
        {
            var line = screenplay[currentLineIndex];
            DisplayLine(line.Item1, line.Item2);
        }
        else
        {
            // After all hint the teleportation device interaction
            teleportationDevice.Hint();
        }
    }

    void EndOfFirstTurn(PlayableDirector aDirector)
    {
        if (firstTurnAnimation == aDirector)
        {
            Player.Enable();
            EnableManualZoom();
            waitForAnimation = false;
            NextStep();
        }
    }

    private void DisableManualZoom()
    {
        GameObject virtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera");
        CameraZoom cameraZoom = virtualCamera.GetComponent<CameraZoom>();

        if (cameraZoom == null)
        {
            Debug.LogError("InputMiniGameManager Zoom In Unable to retrieve camera");
        }
        else
        {
            Debug.Log("InputMiniGameManager Zoom In Retrieveing object");
        }
        cameraZoom.Block();
    }

    private void EnableManualZoom()
    {
        GameObject virtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera");
        CameraZoom cameraZoom = virtualCamera.GetComponent<CameraZoom>();

        if (cameraZoom == null)
        {
            Debug.LogError("InputMiniGameManager Zoom In Unable to retrieve camera");
        }
        else
        {
            Debug.Log("InputMiniGameManager Zoom In Retrieveing object");
        }
        cameraZoom.Release();
    }

    public void ZoomIn()
    {
        Debug.Log("InputMiniGameManager Zoom In");
        GameObject virtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera");
        CameraZoom cameraZoom = virtualCamera.GetComponent<CameraZoom>();

        if (cameraZoom == null)
        {
            Debug.LogError("InputMiniGameManager Zoom In Unable to retrieve camera");
        }
        else
        {
            Debug.Log("InputMiniGameManager Zoom In Retrieveing object");
        }
        cameraZoom.ChangeZoomSmooth(1.2f);
    }

    void LayoutInputHolder()
    {
        GameObject instance = Instantiate(teleportationDeviceTile, new Vector3(1f, 6.9f, 0f), Quaternion.identity);
        instance.transform.localScale = new(2f, 2f, 1f);
        teleportationDevice = instance.GetComponent<TeleportationDevice>();
    }

    private void LayoutSample()
    {
        Debug.Log("Layout Sample");
        teleportationDevice.Blink();
        teleportationDevice.Hint();

        Turn current = turns[currentTurn];
        GameObject instance = Instantiate(current.sample, current.position, Quaternion.identity);
        sampleBox = instance.GetComponent<SampleBox>();
        sampleBox.OnBreak += LayoutGrayscaleBands;

        teleportationDevice.Load(sampleBox, current.instruction);
    }

    private void LayoutGrayscaleBands(string sampleBox, Vector3 position)
    {
        teleportationDevice.StopBlink();

        // Debug.Log("Break the box " + sampleBox);
        GameObject tileChoice = spectralBandTile;

        Vector3 upper = position;
        upper.y++;
        //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
        GameObject blue = Instantiate(tileChoice, upper, Quaternion.identity);
        SampleSpectralBand scriptBlue = blue.GetComponent<SampleSpectralBand>();
        scriptBlue.LoadSprite(sampleBox + "_Blue");

        Vector3 down = position;
        down.y--;
        GameObject green = Instantiate(tileChoice, down, Quaternion.identity);
        SampleSpectralBand script = green.GetComponent<SampleSpectralBand>();
        script.LoadSprite(sampleBox + "_Green");

        Vector3 upperRight = position;
        upperRight.y++;
        upperRight.x++;
        GameObject red = Instantiate(tileChoice, upperRight, Quaternion.identity);
        SampleSpectralBand scriptRed = red.GetComponent<SampleSpectralBand>();
        scriptRed.LoadSprite(sampleBox + "_Red");

        Vector3 right = position;
        right.x++;
        GameObject swir = Instantiate(tileChoice, right, Quaternion.identity);
        SampleSpectralBand scriptSWIR = swir.GetComponent<SampleSpectralBand>();
        scriptSWIR.LoadSprite(sampleBox + "_SWIR");

        Vector3 downRight = position;
        downRight.y--;
        downRight.x++;
        GameObject redEdge = Instantiate(tileChoice, downRight, Quaternion.identity);
        SampleSpectralBand scriptRedEdge = redEdge.GetComponent<SampleSpectralBand>();
        scriptRedEdge.LoadSprite(sampleBox + "_RedEdge");
    }

    private void LayoutBandContainers()
    {
        float verticalGap = 2f;
        float verticalOffset = 2f;
        float xPosition = 6f;

        for (int i = 0; i < 5; i++)
        {
            float yPosition = verticalOffset + i * verticalGap;
            Vector3 position = new(xPosition, yPosition, 0f);
            GameObject instance = Instantiate(spectralBandContainerTile, position, Quaternion.identity);
            SpectralBandContainer spectralBandContainer = instance.GetComponent<SpectralBandContainer>();
            spectralBandContainer.SetType(bandTypes[i]);
            spectralBandContainer.DrawConnections(inputPosition: new(-4.9f, (Height / 2) - yPosition, 0f));
            spectralBandContainer.OnFull += CheckEnableSwitches;

            containers[bandTypes[i]] = spectralBandContainer;
        }
    }

    private void LayoutBandSelector()
    {
        float verticalGap = 2f;
        float verticalOffset = 2.64f;
        float xPosition = 8.86f;

        for (int i = 0; i < 5; i++)
        {
            float yPosition = verticalOffset + i * verticalGap;
            Vector3 position = new(xPosition, yPosition, 0f);
            GameObject instance = Instantiate(selectorSwitch, position, Quaternion.identity);
            SelectorSwitch bandSelector = instance.GetComponent<SelectorSwitch>();
            bandSelector.SetType(bandTypes[i]);
            // bandSelector.DrawConnections(inputPosition: new(-9.9f, (Height / 2) - yPosition, 0f));
            bandSelector.OnSwitch += CheckTurnCondition;

            bandSelectors.Add(bandTypes[i], bandSelector);
        }
    }

    // Spectral Band selections
    private void SelectWrongBand(Turn current, string type)
    {
        current.Match(type);
        UIHandler.Instance.DisplayMessage(current.negativeMessage + "\n" + current.GetProgressMessage());
        bandSelectors[type].UpdateState("wrong");
    }

    private void UnselectWrongBand(Turn current, string type)
    {
        current.Unmatch(type);
        UIHandler.Instance.DisplayMessage(current.positiveMessage + "\n" + current.GetProgressMessage());
        bandSelectors[type].UpdateState("inactive");
    }

    private void SelectCorrectBand(Turn current, string type)
    {
        current.Match(type);
        UIHandler.Instance.DisplayMessage(current.positiveMessage + "\n" + current.GetProgressMessage());
        bandSelectors[type].UpdateState("correct");
    }

    private void UnselectCorrectBand(Turn current, string type)
    {
        current.Unmatch(type);
        UIHandler.Instance.DisplayMessage(current.negativeMessage);
        bandSelectors[type].UpdateState("inactive");
    }

    private void CheckTurnCondition(string type)
    {
        Debug.Log("check turn condition");
        Turn current = turns[currentTurn];
        if (current.IsCharacteristicBand(type))
        {
            if (current.AlreadyMatched(type))
            {
                UnselectCorrectBand(current, type);
            }
            else
            {
                SelectCorrectBand(current, type);

            }
        }
        else
        {
            if (bandSelectors[type].IsActive())
            {
                SelectWrongBand(current, type);
            }
            else
            {
                UnselectWrongBand(current, type);
            }
        }

        if (current.IsOver())
        {
            TurnOver();
        }
    }
    private void CheckEnableSwitches(string type)
    {
        Debug.Log("Check enable swtiches");
        fullSpectralBandsContainer++;
        int totalCount = bandTypes.Count;
        if (fullSpectralBandsContainer == totalCount)
        {
            foreach (KeyValuePair<string, SelectorSwitch> selector in bandSelectors)
            {
                selector.Value.SetHasInput();
            }
        }
    }

    protected override void GameOver()
    {
        GameManager.instance.solvedMinigames["Input"] = true;
        GameManager.instance.StartOverviewScene();
    }

    private void AnimateGameOver()
    {
        // Block Player
        Player.Disable();

        // Camera goes to NPC
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
        // cameraZoom.ChangeFollowTarget(NPC.transform);

        // NPC walks until Player
        NPC.WalkTo(Player.transform.position);
        Player.Enable();
        // cameraZoom.ChangeFollowTarget(Player.transform);

        // Display Clickable Message
        // UIHandler.Instance.DisplayDialogue("Game Over. Click at this dialogue balloon to proceed to finish the challenge in this layer.");
        // UIHandler.Instance.OnClicked += GameOver;
    }


    private void ResetSelectors()
    {
        foreach (KeyValuePair<string, SelectorSwitch> selector in bandSelectors)
        {
            selector.Value.Reset();
        }
    }

    private void ResetContainers()
    {
        foreach (KeyValuePair<string, SpectralBandContainer> container in containers)
        {
            container.Value.Reset();
        }
    }

    void TurnOver()
    {
        ResetContainers();
        ResetSelectors();
        if (currentTurn == 2)
        {
            GameOver();
            // AnimateGameOver();
            return;
        }
        Debug.Log("Increment turn");
        currentTurn++;
        fullSpectralBandsContainer = 0;

        sampleBox.Reset();
        LayoutSample();
        // TODO: Zoom in sample
    }
}
