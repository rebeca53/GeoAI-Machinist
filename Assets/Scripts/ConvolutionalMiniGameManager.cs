using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ConvolutionalMiniGameManager : BaseBoard
{
    // Pre-fabs
    public GameObject convolutionalViewObject;
    public GameObject loadingScreen;

    // UI constants
    public static float pixelSize = 0.01f;
    static public float verticalOffsetImages = 5f;
    readonly int KernelAmount = 3;

    // Instances
    List<ConvolutionalView> convolutionalViews = new List<ConvolutionalView>();
    public ConvolutionalMiniGamePlaybackDirector playbackDirector;
    public TimedDialogueBalloon timedDialogueBalloon;

    public DialogueBalloon dialogueBalloon;

    public CameraZoom cameraZoom;

    public TextAsset convDataText;
    ConvData data;
    [System.Serializable]
    class ConvData
    {
        public List<double> inputMatrix = new List<double>();
        public List<double> kernelMatrix = new List<double>();
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
        UpdateProgress(0f);
        StartCoroutine(LayoutAll());
    }

    void UpdateProgress(float progress)
    {
        Image bar = GameObject.Find("ProgressBar").GetComponent<Image>();
        bar.fillAmount = progress;
    }

    void IncrementProgress(float progress)
    {
        Image bar = GameObject.Find("ProgressBar").GetComponent<Image>();
        bar.fillAmount += progress;
    }

    IEnumerator LayoutAll()
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
            IncrementProgress(0.01f);
            yield return null;
        }

        Player.Spawn(this, new Vector2Int(2, 1));
        NPC.Spawn(this, new Vector2Int(1, 1));
        LoadMatrix();

        StartCoroutine(LayoutConvolutionalViews());
    }

    IEnumerator LayoutConvolutionalViews()
    {
        float verticalGap = 4.5f;
        float xPosition = 2f;
        float verticalOffset = 2.5f;

        for (int i = 0; i < KernelAmount; i++)
        {
            float yPosition = verticalOffset + i * verticalGap;
            Vector3 position = new(xPosition, yPosition, 0f);
            GameObject instanceView = Instantiate(convolutionalViewObject, position, Quaternion.identity);
            ConvolutionalView script = instanceView.GetComponent<ConvolutionalView>();
            script.InitKernel(GetFlatKernelMatrix(i), UnflatMatrix(GetFlatKernelMatrix(i), 3));
            yield return null;
            script.InitInput(UnflatMatrix(data.inputMatrix, 64));
            script.OnConvolutionStopped += OnConvolutionStopped;
            convolutionalViews.Add(script);
            UpdateProgress((float)(i + 1) / KernelAmount);
            yield return null;
        }

        RegisterConvolutionalViewsMessages();
        loadingScreen.SetActive(false);
        playbackDirector.StartAnimation();
    }

    private void UnregisterConvolutionalViewsMessages()
    {
        for (int i = 0; i < KernelAmount; i++)
        {
            ConvolutionalView script = convolutionalViews[i];
            script.OnHover -= DisplayKernelMessage;
            script.OnUnhover -= HideKernelMessage;
        }
    }

    private void RegisterConvolutionalViewsMessages()
    {
        for (int i = 0; i < KernelAmount; i++)
        {
            ConvolutionalView script = convolutionalViews[i];
            script.OnHover += DisplayKernelMessage;
            script.OnUnhover += HideKernelMessage;
        }
    }

    List<double> GetFlatKernelMatrix(int i)
    {
        List<double> flatKernel = new List<double>();
        //https://www.researchgate.net/figure/Vertical-and-horizontal-edge-detector-kernel_fig3_343947492
        switch (i)
        {
            case 0:
                // double[,] verticalEdgeDetection = {
                //         {1, 0, -1},
                //         {1, 0, -1},
                //         {1, 0, -1},
                //     };
                List<double> flatVerticalEdgeDetection = new List<double> { 1, 0, -1, 1, 0, -1, 1, 0, -1 };
                flatKernel = flatVerticalEdgeDetection;
                break;
            case 1:
                // double[,] horizontalEdgeDetection = {
                //         {1, 1, 1},
                //         {0,0,0},
                //         {-1,-1,-1},
                //     };
                List<double> flatHorizontalEdgeDetection = new List<double> { 1, 1, 1, 0, 0, 0, -1, -1, -1 };
                flatKernel = flatHorizontalEdgeDetection;
                break;
            case 2:
                flatKernel = data.kernelMatrix;
                break;
            default:
                double[,] zeroes = new double[3, 3];
                List<double> flatZeroes = new List<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                flatKernel = flatZeroes;
                break;
        }

        return flatKernel;
    }

    void LoadMatrix()
    {
        // Debug.Log(convDataText.text);
        data = JsonUtility.FromJson<ConvData>(convDataText.text);

        if (data == null)
        {
            Debug.LogError("Failed to retrieve from JSON");
        }
    }

    void OnConvolutionStopped(int id)
    {
        // Update outputline
        if (convolutionalViews[id - 1].HasKernel())
        {
            switch (id)
            {
                case 1:
                    convolutionalViews[0].UpdateOutputState("wrong");
                    break;
                case 2:
                    convolutionalViews[1].UpdateOutputState("wrong");
                    break;
                case 3:
                    convolutionalViews[2].UpdateOutputState("correct");
                    break;
            }
        }
        else
        {
            convolutionalViews[id - 1].UpdateOutputState("inactive");
        }

        if (IsGameOver())
        {
            DisplayGameOverMessage();
        }
        else if (NeedRemoveWrongKernel())
        {
            DisplayWrongKernelMessage();
        }
    }

    private void DisplayKernelMessage(int id)
    {
        string message = "";
        switch (id)
        {
            case 1:
                message = "This is a vertical edge detection kernel.";
                break;
            case 2:
                message = "This is a horizontal edge detection kernel.";
                break;
            case 3:
                message = "This kernel highlights the features I'm aiming for.";
                break;
        }
        timedDialogueBalloon.SetSpeaker(Player.gameObject);
        timedDialogueBalloon.SetMessage(message);
        timedDialogueBalloon.PlaceUpperLeft();
        timedDialogueBalloon.Show();
    }

    private void HideKernelMessage(int id)
    {
        timedDialogueBalloon.Hide();
    }

    private void DisplayGameOverMessage()
    {
        Player.Disable();
        cameraZoom.ChangeZoomTarget(NPC.gameObject);
        ZoomIn();
        // NPC speaks message
        string message = "Good job picking the kernel that enhances human-made features to analyze a residential area. Let's go back for the CNN Room.";
        dialogueBalloon.SetSpeaker(NPC.gameObject);
        dialogueBalloon.SetMessage(message);
        dialogueBalloon.PlaceUpperLeft();
        dialogueBalloon.Show();
        dialogueBalloon.OnDone += GameOver;
    }

    bool IsGameOver()
    {
        bool verticalEdgeDetection = convolutionalViews[0].HasKernel();
        bool horizontalEdgeDetection = convolutionalViews[1].HasKernel();
        bool trainedKernel = convolutionalViews[2].HasKernel() && !convolutionalViews[2].IsConvoluting();

        return trainedKernel && !verticalEdgeDetection && !horizontalEdgeDetection;
    }

    bool NeedRemoveWrongKernel()
    {
        bool verticalEdgeDetection = convolutionalViews[0].HasKernel();
        bool horizontalEdgeDetection = convolutionalViews[1].HasKernel();
        bool trainedKernel = convolutionalViews[2].HasKernel() && !convolutionalViews[2].IsConvoluting();

        return trainedKernel && (verticalEdgeDetection || horizontalEdgeDetection);
    }

    private void DisplayWrongKernelMessage()
    {
        UnregisterConvolutionalViewsMessages();
        string message = "Oops, I need to disconnect the non-optimal kernels.";
        timedDialogueBalloon.SetSpeaker(Player.gameObject);
        timedDialogueBalloon.SetMessage(message);
        timedDialogueBalloon.PlaceUpperLeft();
        timedDialogueBalloon.Show(5f);
        timedDialogueBalloon.OnDone += RegisterConvolutionalViewsMessages;
        ZoomIn();

        // NPC speaks message
        string robotMessage = "Oops, you need to disconnect the non-optimal kernels.";
        dialogueBalloon.SetSpeaker(NPC.gameObject);
        dialogueBalloon.SetMessage(robotMessage);
        dialogueBalloon.PlaceUpperLeft();
        dialogueBalloon.Show();
        dialogueBalloon.DisableKey();
        NPC.OnHover += dialogueBalloon.WaitForKey;
    }

    void ZoomIn()
    {
        cameraZoom.ChangeZoomSmooth(1.4f);
    }

    protected override void GameOver()
    {
        GameManager.instance.solvedMinigames["Convolutional 1"] = true;
        GameManager.instance.StartOverviewScene();
    }

}
