using System.Collections.Generic;
using UnityEngine;

public class ConvolutionalMiniGameManager : BaseBoard
{
    // Pre-fabs
    public GameObject convolutionalViewObject;

    // Instances
    List<ConvolutionalView> convolutionalViews = new List<ConvolutionalView>();

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

    // UI constants
    public static float pixelSize = 0.01f;
    static public float verticalOffsetImages = 5f;
    readonly int KernelAmount = 3;

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
        LoadMatrix();

        LayoutConvolutionalViews();
    }

    private void LayoutConvolutionalViews()
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
            script.InitInput(UnflatMatrix(data.inputMatrix, 64));
            script.OnConvolutionStopped += OnConvolutionStopped;
            convolutionalViews.Add(script);
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
        Debug.Log(convDataText.text);
        data = JsonUtility.FromJson<ConvData>(convDataText.text);

        if (data == null)
        {
            Debug.Log("Failed to retrieve from JSON");
        }
        else
        {
            if (data.kernelMatrix == null)
            {
                Debug.Log("Kernel is none");
            }
            Debug.Log("kernel lenght " + data.kernelMatrix.Count);
            Debug.Log("kernel [0] " + data.kernelMatrix[0]);
            Debug.Log("kernel [1] " + data.kernelMatrix[1]);
            Debug.Log("kernel [2] " + data.kernelMatrix[2]);

            Debug.Log("kernel [0][0] " + UnflatMatrix(data.kernelMatrix, 3)[0, 0]);

            Debug.Log("input Matrix [0]" + data.inputMatrix[0]);
            Debug.Log("input Matrix [1]" + data.inputMatrix[1]);
            Debug.Log("input Matrix [2]" + data.inputMatrix[2]);

            Debug.Log("input Matrix [0,0]" + UnflatMatrix(data.inputMatrix, 64)[0, 0]);
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
            Debug.Log("Game is over");
            GameOver();
        }

        // TODO: Animate Explanation?
        /**
         * Horizontal Edge Detection highlights the 
         */
        // TODO: Zoom In NPC, "
        // NPC tells user to leave the room

        // Loading page
    }

    bool IsGameOver()
    {
        bool horizontalEdgeDetection = convolutionalViews[0].HasKernel();
        bool verticalEdgeDetection = convolutionalViews[1].HasKernel();
        bool trainedKernel = convolutionalViews[2].HasKernel() && !convolutionalViews[2].IsConvoluting();

        Debug.Log("horizontal edge detection view has no kernel?" + horizontalEdgeDetection);
        Debug.Log("vertical edge detection view has no kernel?" + verticalEdgeDetection);
        Debug.Log("trained kernel view has kernel?" + convolutionalViews[2].HasKernel());
        Debug.Log("trained kernel view is done convolution?" + !convolutionalViews[2].IsConvoluting());

        return trainedKernel && !verticalEdgeDetection && !horizontalEdgeDetection;
    }

    protected override void GameOver()
    {
        GameManager.instance.solvedMinigames["Convolutional 1"] = true;
        // GameManager.instance.StartOverviewScene();
    }

}
