using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class InputMiniGameManager : BaseBoard
{
    public Action OnTurnOver;
    public GameObject[] sampleTiles;
    public GameObject spectralBandTile;
    public GameObject spectralBandContainerTile;
    public GameObject selectorSwitch;
    public GameObject teleportationDeviceTile;

    // public DialogueBalloon dialogueBalloon;
    // public HintBalloon hintBalloon;
    // public PlayableDirector firstTurnAnimation;
    public TimedDialogueBalloon timedDialogueBalloon;
    public DialogueBalloon dialogueBalloon;

    private List<string> bandTypes = new List<string> { "red", "green", "blue", "redEdge" };
    private int fullSpectralBandsContainer = 0;

    // Dictionary<string, SelectorSwitch> bandSelectors = new Dictionary<string, SelectorSwitch>();
    TeleportationDevice teleportationDevice;
    Dictionary<string, SpectralBandContainer> containers = new Dictionary<string, SpectralBandContainer>();
    SampleBox sampleBox;

    // Turn-related variables
    private List<Turn> turns = new List<Turn> {
        new(0, "River", new List<string>{"redEdge"}, "Choose ONE spectral band to reveal characteristics of a River and place it in the correct container."),
        new(1, "Highway", new List<string>{"red"}, "Choose ONE spectral band to analyze a Highway. A Highway is a man-made feature, that can be surrounded by vegetation and water."),
        new(2, "Residential", new List<string>{"red", "blue", "redEdge"}, "We need a band combination with THREE spectral bands to analyze a Residential area. It includes man-made features, that can be surrounded by vegetation."),
    };

    int currentTurn = 0;

    // List<(string, string)> screenplay = new List<(string, string)>();
    // int currentLineIndex = 0;
    // bool waitForAnimation = false;

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

        Player.Spawn(this, new Vector2Int(2, 2));
        NPC.Spawn(this, new Vector2Int(1, 2));

        // Assign the sprite to each turn
        turns[0].SetSample(sampleTiles[0]); //River
        turns[1].SetSample(sampleTiles[1]); //Highway
        turns[2].SetSample(sampleTiles[2]); //Residential

        LayoutInputHolder();
        LayoutSample();
        LayoutBandContainers();
        // LayoutBandSelector();

        // firstTurnAnimation.stopped += EndOfFirstTurn;
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

    public void ZoomOut(float zoom = 5f)
    {
        Debug.Log("InputMiniGameManager Zoom Out");
        GameObject virtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera");
        CameraZoom cameraZoom = virtualCamera.GetComponent<CameraZoom>();

        if (cameraZoom == null)
        {
            Debug.LogError("InputMiniGameManager Zoom Out Unable to retrieve camera");
        }
        else
        {
            Debug.Log("InputMiniGameManager Zoom Out Retrieveing object");
        }
        cameraZoom.ChangeZoomSmooth(zoom);
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

        // Vector3 down = position;
        // down.y--;
        // GameObject green = Instantiate(tileChoice, down, Quaternion.identity);
        // SampleSpectralBand script = green.GetComponent<SampleSpectralBand>();
        // script.LoadSprite(sampleBox + "_Green");

        Vector3 upperRight = position;
        upperRight.y++;
        upperRight.x++;
        GameObject red = Instantiate(tileChoice, upperRight, Quaternion.identity);
        SampleSpectralBand scriptRed = red.GetComponent<SampleSpectralBand>();
        scriptRed.LoadSprite(sampleBox + "_Red");

        Vector3 right = position;
        right.x++;
        GameObject green = Instantiate(tileChoice, right, Quaternion.identity);
        SampleSpectralBand script = green.GetComponent<SampleSpectralBand>();
        script.LoadSprite(sampleBox + "_Green");

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

        for (int i = 0; i < bandTypes.Count; i++)
        {
            float yPosition = verticalOffset + i * verticalGap;
            Vector3 position = new(xPosition, yPosition, 0f);
            GameObject instance = Instantiate(spectralBandContainerTile, position, Quaternion.identity);
            SpectralBandContainer spectralBandContainer = instance.GetComponent<SpectralBandContainer>();
            spectralBandContainer.SetType(bandTypes[i]);
            spectralBandContainer.DrawConnections(inputPosition: new(-4.9f, (Height / 2) - yPosition, 0f));
            spectralBandContainer.OnFilled += CheckWinTurn;
            spectralBandContainer.OnHover += DisplayMessage;
            spectralBandContainer.OnUnhover += HideMessage;

            containers[bandTypes[i]] = spectralBandContainer;
        }
    }

    // private void LayoutBandSelector()
    // {
    //     float verticalGap = 2f;
    //     float verticalOffset = 2.64f;
    //     float xPosition = 8.86f;

    //     for (int i = 0; i < bandTypes.Count; i++)
    //     {
    //         float yPosition = verticalOffset + i * verticalGap;
    //         Vector3 position = new(xPosition, yPosition, 0f);
    //         GameObject instance = Instantiate(selectorSwitch, position, Quaternion.identity);
    //         SelectorSwitch bandSelector = instance.GetComponent<SelectorSwitch>();
    //         bandSelector.SetType(bandTypes[i]);
    //         bandSelector.OnSwitch += CheckTurnCondition;

    //         // bandSelectors.Add(bandTypes[i], bandSelector);
    //     }
    // }

    // // Spectral Band selections
    // private void SelectWrongBand(Turn current, string type)
    // {
    //     current.Match(type);
    //     UIHandler.Instance.DisplayMessage(current.negativeMessage + "\n" + current.GetProgressMessage());
    //     bandSelectors[type].UpdateState("wrong");
    // }

    // private void UnselectWrongBand(Turn current, string type)
    // {
    //     current.Unmatch(type);
    //     UIHandler.Instance.DisplayMessage(current.positiveMessage + "\n" + current.GetProgressMessage());
    //     bandSelectors[type].UpdateState("inactive");
    // }

    // private void SelectCorrectBand(Turn current, string type)
    // {
    //     current.Match(type);
    //     UIHandler.Instance.DisplayMessage(current.positiveMessage + "\n" + current.GetProgressMessage());
    //     bandSelectors[type].UpdateState("correct");
    // }

    // private void UnselectCorrectBand(Turn current, string type)
    // {
    //     current.Unmatch(type);
    //     UIHandler.Instance.DisplayMessage(current.negativeMessage);
    //     bandSelectors[type].UpdateState("inactive");
    // }

    // private void CheckTurnCondition(string type)
    // {
    //     Debug.Log("check turn condition");
    //     return;

    //     Turn current = turns[currentTurn];
    //     if (current.IsCharacteristicBand(type))
    //     {
    //         if (current.AlreadyMatched(type))
    //         {
    //             UnselectCorrectBand(current, type);
    //         }
    //         else
    //         {
    //             SelectCorrectBand(current, type);
    //         }
    //     }
    //     else
    //     {
    //         if (bandSelectors[type].IsActive())
    //         {
    //             SelectWrongBand(current, type);
    //         }
    //         else
    //         {
    //             UnselectWrongBand(current, type);
    //         }
    //     }

    //     if (current.IsOver())
    //     {
    //         TurnOver();
    //     }
    // }

    private void CheckWinTurn(string type)
    {
        Debug.Log("Check Win Turn");

        Turn current = turns[currentTurn];
        current.Match(type);

        if (current.IsCharacteristicBand(type))
        {
            // TODO: Color connection
            containers[type].UpdateState("correct");
            // bandSelectors[type].UpdateState("correct");
        }
        else
        {
            // TODO: Gray connection
            containers[type].UpdateState("wrong");
            // bandSelectors[type].UpdateState("wrong");
        }

        if (current.AllCharacteristicSelected())
        {
            Debug.Log("TODO: animate turn over");
            TurnOver();
        }
    }

    private void DisplayMessage(string bandName)
    {
        Turn current = turns[currentTurn];
        // Player thinks message
        string message = current.GetMessage(bandName);
        timedDialogueBalloon.SetSpeaker(Player.gameObject);
        timedDialogueBalloon.SetMessage(message);
        timedDialogueBalloon.PlaceUpperLeft();
        timedDialogueBalloon.Show();
    }

    private void DisplayTurnOverMessage()
    {
        Turn current = turns[currentTurn];
        // Player thinks message
        string message = current.GetTurnOverMessage();
        Debug.Log("turnover message " + message);
        dialogueBalloon.SetSpeaker(NPC.gameObject);
        dialogueBalloon.SetMessage(message);
        dialogueBalloon.PlaceUpperLeft();
        dialogueBalloon.Show();
    }

    private void DisplayInitTurnMessage()
    {
        Turn current = turns[currentTurn];
        // Player thinks message
        string message = current.instruction;
        Debug.Log("init turn message " + message);
        dialogueBalloon.SetSpeaker(NPC.gameObject);
        dialogueBalloon.SetMessage(message);
        dialogueBalloon.PlaceUpperLeft();
        dialogueBalloon.Show();
        dialogueBalloon.OnDone += dialogueBalloon.Hide;
    }

    private void HideMessage(string bandName)
    {
        timedDialogueBalloon.Hide();
    }

    protected override void GameOver()
    {
        GameManager.instance.solvedMinigames["Input"] = true;
        GameManager.instance.StartOverviewScene();
    }

    private void ResetContainers()
    {
        foreach (KeyValuePair<string, SpectralBandContainer> container in containers)
        {
            container.Value.Reset();
        }
    }

    private void CleanUnusedBands()
    {
        GameObject[] spectralBands = GameObject.FindGameObjectsWithTag("SpectralBand");
        Turn current = turns[currentTurn];

        foreach (GameObject gameObject in spectralBands)
        {
            SampleSpectralBand sampleSpectralBand = gameObject.GetComponent<SampleSpectralBand>();
            if (!current.IsCharacteristicBand(sampleSpectralBand.GetBandType()))
            {
                gameObject.SetActive(false);
                gameObject.transform.parent = null;
            }
        }
    }

    void InitNewTurn()
    {
        Debug.Log("Change turn");
        dialogueBalloon.OnDone -= InitNewTurn;

        ResetContainers();
        // ResetSelectors();

        if (currentTurn == 2)
        {
            GameOver();
            // AnimateGameOver();
            return;
        }

        Debug.Log("Increment turn");
        fullSpectralBandsContainer = 0;

        currentTurn++;
        sampleBox.Reset();
        LayoutSample();
        DisplayInitTurnMessage();
    }

    void TurnOver()
    {
        Debug.Log("Turn Over");
        CleanUnusedBands();
        DisplayTurnOverMessage();
        dialogueBalloon.OnDone += InitNewTurn;
        ZoomOut(3f);
    }
}
