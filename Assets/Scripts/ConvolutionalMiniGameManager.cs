using System;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ConvolutionalMiniGameManager : BaseBoard
{
    // Pre-fabs
    public GameObject inputObject;
    public GameObject kernelObject;
    public GameObject outputObject;
    public GameObject lockerObject;
    public GameObject kernelHolderObject;
    public GameObject screenObject;

    // Instances
    GameObject instanceInput;
    GameObject instanceOutput;
    KernelPixel kernelCenter;
    List<KernelMatrix> kernelMatrix = new List<KernelMatrix>();
    InputMatrix inputMatrix;

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
    readonly int KernelAmount = 1;

    // Movement
    private readonly float step = pixelSize;

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

        // UIHandler.Instance.HideMessage();
        // NPC.DisplayIntroduction();

        Player.Spawn(this, new Vector2Int(2, 1));
        NPC.Spawn(this, new Vector2Int(1, 1));
        LoadMatrix();

        LayoutKernel();
        LayoutLockers();
        LayoutKernelHolder();

        LayoutInputScreen();
        LayoutOutputScreen();
        // LayoutInputMatrix();

        // LayoutOutputMatrix();
        Convolute();
    }

    private void LayoutKernel()
    {
        float horizontalGap = 1.5f;
        float yPosition = 3f;
        float horizontalOffset = 3f;

        for (int i = 0; i < KernelAmount; i++)
        {
            float xPosition = horizontalOffset + i * horizontalGap;
            Vector3 position = new(xPosition, yPosition, 0f);
            GameObject instanceKernel = Instantiate(kernelObject, position, Quaternion.identity);
            KernelMatrix script = instanceKernel.GetComponent<KernelMatrix>();
            kernelMatrix.Add(script);

            //https://www.researchgate.net/figure/Vertical-and-horizontal-edge-detector-kernel_fig3_343947492
            switch (i)
            {
                case 0:
                    double[,] verticalEdgeDetection = {
                        {1, 0, -1},
                        {1, 0, -1},
                        {1, 0, -1},
                    };
                    script.SetMatrix(verticalEdgeDetection);
                    break;
                case 1:
                    double[,] horizontalEdgeDetection = {
                        {1, 1, 1},
                        {0,0,0},
                        {-1,-1,-1},
                    };
                    script.SetMatrix(horizontalEdgeDetection);
                    break;
                case 2:
                    script.SetMatrix(UnflatMatrix(data.kernelMatrix, 3));
                    break;
                default:
                    double[,] zeroes = new double[3, 3];
                    script.SetMatrix(zeroes);
                    break;
            }
        }
    }

    private void LayoutLockers()
    {
        float horizontalGap = 1.5f;
        float yPosition = 2.5f;
        float horizontalOffset = 5.5f;

        for (int i = 0; i < KernelAmount; i++)
        {
            float xPosition = horizontalOffset + i * horizontalGap;
            Vector3 position = new(xPosition, yPosition, 0f);
            GameObject instance = Instantiate(lockerObject, position, Quaternion.identity);
            Locker script = instance.GetComponent<Locker>();
            script.AddKernel(kernelMatrix[i].gameObject);
        }
    }

    private void LayoutKernelHolder()
    {
        GameObject instance = Instantiate(kernelHolderObject, new Vector3(1.5f, 5f, 0f), Quaternion.identity);
        InputHolder script = instance.GetComponent<InputHolder>();
        script.DrawConnection();
    }

    private void LayoutInputScreen()
    {
        GameObject instance = Instantiate(screenObject, new Vector3(4f, 4f, 0f), Quaternion.identity);

        Transform line = instance.transform.Find("OutputLine");
        if (line == null)
        {
            Debug.LogError("Failed to retrieve Line");
        }
        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("Failed to retrieve LineRenderer");
        }

        Vector3 startPoint = new(0f, -1f, 0f);
        Vector3 endPoint = new(2f, -0.5f, 0f);
        Connection conn = new(startPoint, endPoint, lineRenderer);
        conn.DrawLine(1f);

        LayoutInputMatrix(instance);
    }

    private void LayoutInputMatrix(GameObject parent)
    {
        Vector3 position = new Vector3(0f, 0f, 0f);
        instanceInput = Instantiate(inputObject, position, Quaternion.identity);

        instanceInput.transform.parent = parent.transform;
        instanceInput.transform.localPosition = new(3f, 0f, 0f);

        inputMatrix = instanceInput.GetComponent<InputMatrix>();
        inputMatrix.SetMatrix(UnflatMatrix(data.inputMatrix, 64));
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

    private void LayoutOutputScreen()
    {
        GameObject instance = Instantiate(screenObject, new Vector3(9f, 4f, 0f), Quaternion.identity);

        Transform line = instance.transform.Find("OutputLine");
        if (line == null)
        {
            Debug.LogError("Failed to retrieve Line");
        }
        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("Failed to retrieve LineRenderer");
        }

        Vector3 startPoint = new(0f, -0.5f, 0f);
        Vector3 endPoint = new(4f, -0.5f, 0f);
        Connection conn = new(startPoint, endPoint, lineRenderer);
        conn.DrawStraightLine();
    }

    private void LayoutOutputMatrix(GameObject parent)
    {
        float verticalOffset = verticalOffsetImages;
        float horizontalOffset = OutputMatrix.horizontalOffset;
        Vector3 position = new(horizontalOffset, verticalOffset, 0f);
        instanceOutput = Instantiate(outputObject, position, Quaternion.identity);

        float yOffset = 0f;
        float xOffset = 0.3f;
        instanceInput.transform.parent = parent.transform;
        Vector3 parentPosition = parent.transform.position;
        // instanceInput.transform.localScale = new(0.1f, 0.2f, 0f);
        instanceInput.transform.position = new Vector3(parentPosition.x + xOffset, parentPosition.y + yOffset, parentPosition.z);
    }

    bool IsStride(int i, int j, int stride = 1, int matrixSize = 64)
    {
        if (i + 1 <= stride)
        {
            return true;
        }
        if (j + 1 <= stride)
        {
            return true;
        }

        if (matrixSize - i <= stride)
        {
            return true;
        }

        if (matrixSize - j <= stride)
        {
            return true;
        }

        return false;
    }

    float interval = 1f;
    float timer = 0f;
    int iConv = 1;
    int jConv = 1;

    bool convolutionDone = false;
    void Update()
    {
        timer += Time.deltaTime;
        // Debug.Log("Convolution step 0.");
        if (timer >= interval)
        {
            timer = 0f;
            Debug.Log("Convolution step 1 . (" + iConv + ", " + jConv + ")");
            if (convolutionDone)
            {
                Debug.Log("Convolution done.");
            }
            else if (IsStride(iConv, jConv))
            {
                // skip the stride
                Debug.Log("Convolution step. stride");
            }
            else
            {
                // Debug.Log("Convolution step.");

                // move the kernel center over it
                GameObject inputPixel = inputMatrix.GetPixelObject(iConv, jConv);
                kernelMatrix[0].PlaceAt(inputPixel.transform.position);
                // inputMatrix.HighlightNeighboors(inputPixel.transform.position);

                // retrieve the pixels from input matrix
                // retriver the pixels of the kernel
                // convolute
                double convResult = MultiplyMatrices(data.kernelMatrix, inputMatrix.GetNeighboors(iConv, jConv));

                // retrieve the pixel from the output matrix
                // change its value and color
            }

            jConv++;
            if (jConv >= 64)
            {
                iConv++;
            }
            if (iConv >= 64)
            {
                convolutionDone = true;
            }
        }
    }

    void Convolute()
    {
        Debug.Log("Convolute");
        // Initial position = Move kernel to align with input matrix
        kernelMatrix[0].PlaceAt(inputObject.transform.position);
        kernelMatrix[0].transform.localScale = new(0.04f, 0.04f, 1f);
        kernelMatrix[0].UpdatePixelsConvoluting();

        // for each pixel in the input matrix 
        // for (int i = 0; i < 64; i++)
        // {
        //     for (int j = 0; j < 64; j++)
        //     {
        //         if (IsStride(i, j))
        //         {
        //             // skip the stride
        //             continue;
        //         }

        //         // move the kernel center over it
        //         GameObject inputPixel = inputMatrix.GetPixelObject(i, j);
        //         kernelMatrix[0].PlaceAt(inputPixel.)

        //     }
        // }

        // retrieve the pixels from input matrix
        // retriver the pixels of the kernel
        // convolute

        // retrieve the pixel from the output matrix
        // change its value and color

        // Start moving
        // kernelMatrix[0].MoveTo(new(2f, 8f, 0f));
    }

    // TODO: Move Convolution mechanics to this BoardManager
    private void RegisterToKernelPixel(int idx)
    {
        kernelCenter = kernelMatrix[idx].GetKernelCenter();
        // kernelCenter.OnHoverPixel += MultiplyMatrices;
        // kernelPixel.OnExitPixel += MultiplyMatrices;
    }

    private double MultiplyMatrices(List<double> matrixA, List<double> matrixB)
    {
        double result = 0f;
        for (int i = 0; i < matrixA.Count; i++)
        {
            double temp = matrixA[i] * matrixB[i];
            // Debug.Log(matrixA[i] + " x " + matrixB[i] + " = " + temp);
            result += temp;
        }
        // Debug.Log("multiplication result: " + result);
        return result;
    }

    protected override void GameOver()
    {
        GameManager.instance.solvedMinigames["Convolutional 1"] = true;
        GameManager.instance.StartOverviewScene();
    }

}
