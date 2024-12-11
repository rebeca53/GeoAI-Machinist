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
    List<DenseView> denseViews = new List<DenseView>();
    List<LogitNode> logitNodes = new List<LogitNode>();
    OutputLayer outputLayer;

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

    IEnumerator AnimateDenseView()
    {
        Player.Disable();
        foreach (DenseView denseView in denseViews)
        {
            cameraZoom.ChangeZoomTarget(denseView.gameObject);
            denseView.gameObject.SetActive(true);
            denseView.ShowWeights();
            yield return new WaitForSeconds(1);
            denseView.HideWeights();
        }
        cameraZoom.ChangeZoomTarget(Player.gameObject);
        Player.Enable();
    }

    void LayoutSoftmaxView()
    {
        // outputLayer.SetLogitNodes(logitNodes);
    }

    protected override void GameOver()
    {
        GameManager.instance.solvedMinigames["Output"] = true;
        GameManager.instance.StartOverviewScene();
    }
}
