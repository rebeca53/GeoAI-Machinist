using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputMiniGameManager : BaseBoard
{
    // Pre-fabs
    // public GameObject denseViewObject;

    // Instances
    public OutputMiniGamePlaybackDirector playbackDirector;
    public TimedDialogueBalloon timedDialogueBalloon;
    public DialogueBalloon dialogueBalloon;
    public CameraZoom cameraZoom;

    // Data
    public TextAsset dataText;
    OutputData data;

    [System.Serializable]
    class OutputData
    {
        public List<double> inputMatrix = new List<double>();
        public List<double> weights = new List<double>();
        public double bias;
    }

    double[,] UnflatMatrix(List<double> flatten, int size)
    {
        double[,] unflatten = new double[size, size];
        for (int k = 0; k < flatten.Count; k++)
        {
            double value = flatten[k];
            int i = k / size;
            int j = k - i * size;
            // Debug.Log("flatenning: k " + k + ", value " + value + ", i " + i + ", j" + j);
            unflatten[i, j] = value;
        }

        return unflatten;
    }

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

        // LoadMatrix();
        // LayoutInputHolder();
        // LayoutFlatenningGear();
        // LayoutFlatHolder();
        // LayoutDenseView(); // weights, bias, class, OnHoverClassLabel
    }

    void LoadMatrix()
    {
        Debug.Log(dataText.text);
        data = JsonUtility.FromJson<OutputData>(dataText.text);

        if (data == null)
        {
            Debug.Log("Failed to retrieve from JSON");
        }
        else
        {
            if (data.inputMatrix == null)
            {
                Debug.Log("Input is none");
            }
        }
    }

    void OnHoverClassLabel(string type)
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void GameOver()
    {
        GameManager.instance.solvedMinigames["Output"] = true;
        GameManager.instance.StartOverviewScene();
    }
}
