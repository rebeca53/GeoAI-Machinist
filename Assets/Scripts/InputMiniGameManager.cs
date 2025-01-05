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

    public InputMiniGamePlaybackDirector playbackDirector;
    public TimedDialogueBalloon timedDialogueBalloon;
    public DialogueBalloon dialogueBalloon;
    public CameraZoom cameraZoom;

    private List<string> bandTypes = new List<string> { "red", "green", "blue", "redEdge" };

    TeleportationDevice teleportationDevice;
    Dictionary<string, SpectralBandContainer> containers = new Dictionary<string, SpectralBandContainer>();
    SampleBox sampleBox;

    // Turn-related variables
    private List<Turn> turns = new List<Turn> {
        new(0, "River", new List<string>{"redEdge"}, "Choose ONE spectral band to reveal characteristics of a River and place it in the correct container."),
        new(1, "Highway", new List<string>{"blue"}, "Choose ONE spectral band to analyze a Highway, which is a man-made feature surrounded by vegetation or water."),
        new(2, "Residential", new List<string>{"red", "blue", "redEdge"}, "We need a band combination with THREE spectral bands to analyze a Residential area."),
    };

    int currentTurn = 0;

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

        playbackDirector.OnEnd += DisplayMessageOnHoverNPC;
    }

    private void DisplayMessageOnHoverNPC()
    {
        NPC.OnHover += DisplayInitTurnMessage;
    }

    private void DisableManualZoom()
    {
        GameObject virtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera");
        CameraZoom cameraZoom = virtualCamera.GetComponent<CameraZoom>();

        if (cameraZoom == null)
        {
            Debug.LogError("InputMiniGameManager Zoom In Unable to retrieve camera");
            return;
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
            return;
        }
        cameraZoom.Release();
    }

    public void ZoomIn()
    {
        GameObject virtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera");
        CameraZoom cameraZoom = virtualCamera.GetComponent<CameraZoom>();

        if (cameraZoom == null)
        {
            Debug.LogError("InputMiniGameManager Zoom In Unable to retrieve camera");
            return;
        }
        cameraZoom.ChangeZoomSmooth(1.2f);
    }

    public void ZoomOut(float zoom = 5f)
    {
        GameObject virtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera");
        CameraZoom cameraZoom = virtualCamera.GetComponent<CameraZoom>();

        if (cameraZoom == null)
        {
            Debug.LogError("InputMiniGameManager Zoom Out Unable to retrieve camera");
            return;
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

        GameObject tileChoice = spectralBandTile;

        Vector3 upper = position;
        upper.y++;
        //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
        GameObject blue = Instantiate(tileChoice, upper, Quaternion.identity);
        SampleSpectralBand scriptBlue = blue.GetComponent<SampleSpectralBand>();
        scriptBlue.LoadSprite(sampleBox + "_Blue");

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

    private void CheckWinTurn(string type)
    {
        Turn current = turns[currentTurn];
        current.Match(type);

        if (current.IsCharacteristicBand(type))
        {
            containers[type].UpdateState("correct");
        }
        else
        {
            containers[type].UpdateState("wrong");
        }

        if (current.AllCharacteristicSelected())
        {
            TurnOver();
        }
    }

    private void DisplayMessage(string bandName)
    {
        Turn current = turns[currentTurn];
        // Player thinks message
        string message = current.GetMessage(bandName) + " " + current.GetProgressMessage();
        timedDialogueBalloon.SetSpeaker(Player.gameObject);
        timedDialogueBalloon.SetMessage(message);
        timedDialogueBalloon.PlaceUpperLeft();
        timedDialogueBalloon.Show();
        ZoomIn();
    }

    private void DisplayTurnOverMessage()
    {
        Turn current = turns[currentTurn];
        // NPC displays message
        string message = current.GetTurnOverMessage();
        dialogueBalloon.SetSpeaker(NPC.gameObject);
        dialogueBalloon.SetMessage(message);
        dialogueBalloon.PlaceUpperLeft();
        dialogueBalloon.Show();
        dialogueBalloon.DisableKey();
        NPC.OnHover += dialogueBalloon.WaitForKey;
    }

    private void DisplayInitTurnMessage()
    {
        Turn current = turns[currentTurn];
        string message = current.instruction;
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

    private void DisplayGameOverMessage()
    {
        string message = "Good job, now the Input Layer Room is fixed!";
        dialogueBalloon.SetSpeaker(NPC.gameObject);
        dialogueBalloon.SetMessage(message);
        dialogueBalloon.PlaceUpperLeft();
        dialogueBalloon.Show();
        dialogueBalloon.OnDone += dialogueBalloon.Hide;
        dialogueBalloon.OnDone += GameOver;
        cameraZoom.ChangeZoomTarget(NPC.gameObject);
    }
    protected override void GameOver()
    {
        cameraZoom.ChangeZoomTarget(Player.gameObject);
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
        dialogueBalloon.OnDone -= InitNewTurn;

        if (currentTurn == 2)
        {
            DisplayGameOverMessage();
            return;
        }

        ResetContainers();

        currentTurn++;
        sampleBox.Reset();
        LayoutSample();
        NPC.OnHover += DisplayInitTurnMessage;
        DisplayInitTurnMessage();
    }

    void TurnOver()
    {
        NPC.OnHover -= DisplayInitTurnMessage;
        CleanUnusedBands();
        DisplayTurnOverMessage();
        dialogueBalloon.OnDone += InitNewTurn;
        ZoomOut(3f);
    }
}
