using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputMiniGameManager : BaseBoard
{
    // Pre-fabs
    public GameObject denseViewObject;
    public GameObject outputLayerObject;

    // UI-related Instances
    public OutputMiniGamePlaybackDirector playbackDirector;
    public TimedDialogueBalloon timedDialogueBalloon;
    public DialogueBalloon dialogueBalloon;
    public CameraZoom cameraZoom;

    // Instances
    public FlattenLayer flattenLayer;
    List<DenseView> denseViews = new List<DenseView>();
    List<LogitNode> logitNodes = new List<LogitNode>();
    public OutputLayer outputLayer;


    // Properties
    int DenseViewAmount = 10;

    List<string> labels = new List<string>{
        "highway",
        "forest",
        "river",
        "permanentcrop",
        "industrial",
        "annualcrop",
        "sealake",
        "herbaceous",
        "residential",
        "pasture",
    };

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

        LayoutDenseView();
        // LayoutSoftmaxView();
        // LayoutInputHolder();
        // LayoutFlatenningGear();
        // LayoutFlatHolder();
        // LayoutDenseView(); // weights, bias, class, OnHoverClassLabel
        playbackDirector.StartAnimation();
        flattenLayer.OnFlatten += StartAnimationDenseView;
        outputLayer.OnDone += GameOver;
    }

    void LayoutDenseView()
    {
        float verticalGap = 0.9f;
        float xPosition = 6f;
        float verticalOffset = 0f;

        for (int i = 0; i < DenseViewAmount; i++)
        {
            float yPosition = verticalOffset + i * verticalGap;
            Vector3 position = new(xPosition, yPosition, 0f);
            GameObject instanceView = Instantiate(denseViewObject, position, Quaternion.identity);
            DenseView script = instanceView.GetComponent<DenseView>();
            script.SetType(labels[i]);
            script.gameObject.SetActive(false);
            denseViews.Add(script);
        }
    }

    void StartAnimationDenseView()
    {
        flattenLayer.OnFlatten -= StartAnimationDenseView;
        StartCoroutine(AnimateDenseView());
    }

    IEnumerator AnimateDenseView()
    {
        Player.Disable();
        foreach (DenseView denseView in denseViews)
        {
            cameraZoom.ChangeZoomTarget(denseView.GetLogitNode().gameObject);
            denseView.gameObject.SetActive(true);
            denseView.ShowWeights();
            yield return new WaitForSeconds(1);
            denseView.HideWeights();
        }
        cameraZoom.ChangeZoomTarget(Player.gameObject);
        Player.Enable();
        playbackDirector.NextLine();
    }

    private void DisplayGameOverMessage()
    {
        Player.Disable();
        cameraZoom.ChangeZoomTarget(NPC.gameObject);
        ZoomIn();
        // NPC speaks message
        string message = "Good job flatenning the image and applying softmax to calculate probabilities. Explore this room a bit more if you will, then go back to the CNN room.";
        // Debug.Log("turnover message " + message);
        dialogueBalloon.SetSpeaker(NPC.gameObject);
        dialogueBalloon.SetMessage(message);
        dialogueBalloon.PlaceUpperLeft();
        dialogueBalloon.Show();
        dialogueBalloon.OnDone += GameOver;
    }

    void ZoomIn()
    {
        cameraZoom.ChangeZoomSmooth(1.4f);
    }

    protected override void GameOver()
    {
        DisplayGameOverMessage();

        // GameManager.instance.solvedMinigames["Output"] = true;
        // GameManager.instance.StartOverviewScene();
    }
}
