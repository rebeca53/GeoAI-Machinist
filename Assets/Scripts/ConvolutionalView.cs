using System;
using System.Collections.Generic;
using UnityEngine;

public class ConvolutionalView : MonoBehaviour
{
    public event Action<int> OnHover;
    public event Action<int> OnUnhover;
    public Action<int> OnConvolutionStopped;

    // Scene objects
    public InputHolder kernelHolder;
    public Locker locker;
    public GameObject inputScreen;
    public GameObject outputScreen;

    // Matrices
    public KernelMatrix kernelMatrix;
    KernelMatrix movingKernelMatrix;
    public InputMatrix inputMatrix;
    public OutputMatrix outputMatrix;
    public int id;
    static int viewCounter = 0;

    // Convolution
    int stride = 1;
    int iConv = 1; // stride
    int jConv = 1; //stride

    bool isConvoluting = false;
    bool kernelAtInputHolder = false;
    int convolutionAnimationStep = 8; // step = 4 ==> 10 seconds, step = 8 ==> 3 seconds

    // TODO: abstract OutputLine
    string outputState = "inactive"; // inactice, wrong, correct
    LineRenderer outputLineRenderer;
    readonly float inactiveWidth = 0.05f;
    private Color workingStartColor;
    private Color workingEndColor;
    private Color wrongColor = Color.red;
    private Color inactiveColor = Color.gray;


    // Start is called before the first frame update
    void Start()
    {
        viewCounter++;
        id = viewCounter;

        ResetConvolution();

        LayoutKernelHolder();
        LayoutInputScreen();
        LayoutOutputScreen();
    }

    /* UI-related methods */

    private void LayoutKernelHolder()
    {
        kernelHolder.name = "KernelHolder" + id;
        kernelHolder.DrawConnection();
        kernelHolder.OnAddedObject += StartConvolution;
    }

    private void LayoutInputScreen()
    {
        // Draw line
        Transform line = inputScreen.transform.Find("OutputLine");
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

        // Enable Label
        Transform label = inputScreen.transform.Find("Label");
        label.gameObject.SetActive(true);
    }

    private void LayoutOutputScreen()
    {
        Transform line = outputScreen.transform.Find("OutputLine");
        if (line == null)
        {
            Debug.LogError("Failed to retrieve Line");
        }
        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("Failed to retrieve LineRenderer");
        }

        workingStartColor = lineRenderer.startColor;
        workingEndColor = lineRenderer.endColor;

        Vector3 startPoint = new(0f, -0.5f, 0f);
        Vector3 endPoint = new(2.5f, -0.5f, 0f);
        Connection conn = new(startPoint, endPoint, lineRenderer);
        conn.DrawStraightLine();

        outputLineRenderer = conn.lineRenderer;
        UpdateOutputState("inactive");
    }

    /* Convolution methods */
    void ResetConvolution()
    {
        iConv = stride;
        jConv = stride;
        isConvoluting = false;
    }

    public bool IsConvoluting()
    {
        return isConvoluting;
    }

    public bool HasKernel()
    {
        return kernelAtInputHolder;
    }

    public void InitKernel(List<double> flatKernel, double[,] kernel)
    {
        kernelMatrix.name = "Kernel" + id;
        kernelMatrix.SetMatrix(flatKernel, kernel);
        locker.AddKernel(kernelMatrix.gameObject);

        GameObject instance = Instantiate(kernelMatrix.gameObject, kernelMatrix.transform.position, Quaternion.identity);
        movingKernelMatrix = instance.GetComponent<KernelMatrix>();
        movingKernelMatrix.gameObject.SetActive(false);
        movingKernelMatrix.SetMatrix(flatKernel, kernel);
    }

    public void RemoveKernel()
    {
        kernelAtInputHolder = false;
        OnUnhover?.Invoke(id);
        StopConvolution();
        ResetConvolution();
        outputMatrix.Reset();
    }

    public void InitInput(double[,] input)
    {
        inputMatrix.SetMatrix(input);
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

    void StartConvolution()
    {
        kernelAtInputHolder = true;
        kernelMatrix.transform.localScale = new(0.1f, 0.1f, 1f);
        kernelMatrix.OnGrabbed += RemoveKernel;

        GameObject inputPixel = inputMatrix.GetPixelObject(iConv, jConv);
        movingKernelMatrix.PlaceAt(inputPixel.transform.position);
        movingKernelMatrix.transform.localScale = new(0.04f, 0.04f, 1f);
        movingKernelMatrix.UpdatePixelsConvoluting();
        movingKernelMatrix.gameObject.SetActive(true);

        outputMatrix.Reset();
        iConv = 0;
        jConv = 0;
        isConvoluting = true;

        for (int i = 1; i < 64; i++)
        {
            for (int j = 1; j < 64; j++)
            {
                if (IsStride(i, j))
                {
                    continue;
                }
                // retrieve the pixels from input matrix
                // retrieve the pixels of the kernel
                // convolute
                double convResult = MultiplyMatrices(movingKernelMatrix.flatKernel, inputMatrix.GetNeighboors(i, j));

                // retrieve the pixel from the output matrix
                // change its value and color
                outputMatrix.SetPixel(i - 1, j - 1, convResult);
                outputMatrix.HidePixel(i - 1, j - 1);
            }
        }
    }

    void Convolute()
    {
        // around 4000 thousands iterations
        if (!isConvoluting)
        {
            // Debug.Log("Convolution done.");
            return;
        }
        else if (IsStride(iConv, jConv))
        {
            // skip the stride
            // Debug.Log("Convolution step. stride");
        }
        else
        {
            // move the kernel center over it
            GameObject inputPixel = inputMatrix.GetPixelObject(iConv, jConv);
            movingKernelMatrix.PlaceAt(inputPixel.transform.position);

            int jConvNext = jConv;
            int iConvNext = iConv;
            int stepDone = 0;
            // Debug.Log("start loop iConvNext " + iConvNext + ", jConvNext " + jConvNext);
            while ((stepDone < convolutionAnimationStep) && (jConvNext < 64) && (iConvNext < 64))
            {
                // Debug.Log("iConvNext " + iConvNext + " limit " + 64);
                // Debug.Log("jConvNext " + jConvNext + " limit " + (jConv + convolutionAnimationStep));
                outputMatrix.ShowPixel(iConvNext - 1, jConvNext - 1);
                jConvNext++;
                if (jConvNext >= 64)
                {
                    iConvNext++;
                    jConvNext = 1; // stride
                }
                stepDone++;
            }
            // Debug.Log("end loop iConvNext " + iConvNext + ", jConvNext " + jConvNext);
        }

        jConv += convolutionAnimationStep;
        if (jConv >= 64)
        {
            iConv++;
            jConv = 1; // stride
        }
        if (iConv >= 64)
        {
            StopConvolution();
        }
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

    void StopConvolution()
    {
        kernelMatrix.OnGrabbed -= StopConvolution;
        movingKernelMatrix.gameObject.SetActive(false);

        isConvoluting = false;
        OnConvolutionStopped?.Invoke(id);
    }


    // Update is called once per frame
    void Update()
    {
        Convolute();
        AnimateOutputState();
    }

    // TODO: abstract OutputLine
    public void UpdateOutputState(string newLineState)
    {
        // Debug.Log("Update state: " + newLineState);
        outputState = newLineState;
        switch (outputState)
        {
            case "correct":
                outputLineRenderer.startColor = workingStartColor;
                outputLineRenderer.endColor = workingEndColor;
                break;
            case "wrong":
                outputLineRenderer.material.color = wrongColor;
                outputLineRenderer.startColor = wrongColor;
                outputLineRenderer.endColor = wrongColor;
                outputLineRenderer.startWidth = inactiveWidth;
                outputLineRenderer.endWidth = inactiveWidth;
                break;
            case "inactive":
            default:
                outputLineRenderer.material.color = inactiveColor;
                outputLineRenderer.startColor = inactiveColor;
                outputLineRenderer.endColor = inactiveColor;
                outputLineRenderer.startWidth = inactiveWidth;
                outputLineRenderer.endWidth = inactiveWidth;
                break;
        }
    }

    public void AnimateOutputState()
    {
        if (!outputLineRenderer)
        {
            // Debug.Log("Could not find output line render");
            return;
        }

        if (outputState.Equals("correct"))
        {
            outputLineRenderer.material.color = Color.Lerp(Color.white, Color.cyan, Mathf.PingPong(Time.time, 0.5f));
            outputLineRenderer.startWidth = inactiveWidth * 2;
            outputLineRenderer.endWidth = inactiveWidth * 2;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (HasKernel())
        {
            OnHover?.Invoke(id);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (HasKernel())
        {
            OnHover?.Invoke(id);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (HasKernel())
        {
            OnUnhover?.Invoke(id);
        }
    }

}
