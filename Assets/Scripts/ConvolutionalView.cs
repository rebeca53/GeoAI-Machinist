using System;
using System.Collections.Generic;
using UnityEngine;

public class ConvolutionalView : MonoBehaviour
{
    // Scene objects
    public InputHolder kernelHolder;
    public Locker locker;

    public GameObject inputScreen;
    public GameObject outputScreen;

    private GameObject kernelCopy;

    // Matrices
    public KernelMatrix kernelMatrix;
    public InputMatrix inputMatrix;
    public OutputMatrix outputMatrix;
    public int id;
    static int viewCounter = 0;

    // Convolution
    int stride = 1;
    int iConv = 1; // stride
    int jConv = 1; //stride

    bool isConvoluting = false;
    bool hasKernel = false;

    public Action<int> OnConvolutionStopped;

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
    }

    private void LayoutInputMatrix(GameObject parent)
    {
        // inputMatrix.transform.parent = parent.transform;
        // inputMatrix.transform.localPosition = new(-18.9f, -9f, 0f);
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
        Vector3 startPoint = new(0f, -0.5f, 0f);
        Vector3 endPoint = new(2.5f, -0.5f, 0f);
        Connection conn = new(startPoint, endPoint, lineRenderer);
        conn.DrawStraightLine();
        outputLineRenderer = conn.lineRenderer;
        UpdateOutputState("inactive");

        LayoutOutputMatrix(outputScreen);
    }

    private void LayoutOutputMatrix(GameObject parent)
    {
        // outputMatrix.transform.parent = parent.transform;
        // outputMatrix.transform.localPosition = new(-22.15f, 9.48f, 0f);
    }

    /* Convolution methods */
    void ResetConvolution()
    {
        iConv = stride;
        jConv = stride;
        isConvoluting = false;
    }

    public void UpdateOutputState(string newLineState)
    {
        Debug.Log("Update state: " + newLineState);
        outputState = newLineState;
        switch (outputState)
        {
            case "correct":
                outputLineRenderer.startColor = workingStartColor;
                outputLineRenderer.endColor = workingEndColor;
                break;
            case "wrong":
                outputLineRenderer.material.color = Color.white;
                outputLineRenderer.startColor = Color.white;
                outputLineRenderer.endColor = Color.white;
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

    public void AnimateOutputState(string newLineState)
    {
        Debug.Log("Update state: " + newLineState);
        outputState = newLineState;

        if (!outputLineRenderer)
        {
            return;
        }

        switch (outputState)
        {
            case "correct":
                outputLineRenderer.startColor = workingStartColor;
                outputLineRenderer.endColor = workingEndColor;
                break;
            case "wrong":
                outputLineRenderer.material.color = Color.white;
                outputLineRenderer.startColor = Color.white;
                outputLineRenderer.endColor = Color.white;
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

    public bool HasKernel()
    {
        return hasKernel;
    }

    public void InitKernel(List<double> flatKernel, double[,] kernel)
    {
        kernelMatrix.SetMatrix(flatKernel, kernel);
        locker.AddKernel(kernelMatrix.gameObject);

        kernelCopy = Instantiate(kernelMatrix.gameObject, kernelMatrix.transform.position, Quaternion.identity);
        kernelCopy.SetActive(false);
        kernelCopy.GetComponent<KernelMatrix>().OnGrabbed += StopConvolution;

        hasKernel = true;
    }

    public void RemoveKernel()
    {
        hasKernel = false;
        StopConvolution();
        ResetConvolution();
        outputMatrix.Reset();
    }

    public void InitInput(double[,] input)
    {
        inputMatrix.SetMatrix(id, input);
        LayoutInputMatrix(inputScreen);
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
        // Debug.Log("Convolute");
        // Debug.Log("Kernel [0][1]: " + kernelMatrix.GetKernelPixel(0, 1));
        // Initial position = Move kernel to align with input matrix

        // Copy Object
        kernelCopy.transform.position = kernelMatrix.transform.position;
        kernelCopy.transform.localScale = new(0.1f, 0.1f, 0f);
        kernelCopy.SetActive(true);

        // kernelMatrix.PlaceAt(inputMatrix.transform.position);
        kernelMatrix.transform.localScale = new(0.04f, 0.04f, 1f);
        kernelMatrix.UpdatePixelsConvoluting();

        outputMatrix.Reset();
        iConv = 0;
        jConv = 0;
        isConvoluting = true;
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
            kernelMatrix.PlaceAt(inputPixel.transform.position);
            // inputMatrix.HighlightNeighboors(inputPixel.transform.position);

            // retrieve the pixels from input matrix
            // retrieve the pixels of the kernel
            // convolute
            double convResult = MultiplyMatrices(kernelMatrix.flatKernel, inputMatrix.GetNeighboors(iConv, jConv));

            // retrieve the pixel from the output matrix
            // change its value and color
            outputMatrix.SetPixel(iConv - 1, jConv - 1, convResult);
        }

        jConv++;
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
        // Debug.Log("Stop Convolution");

        // kernelMatrix.transform.localScale = new(1f, 1f, 1f);
        // kernelMatrix.transform.position = kernelCopy.transform.position;
        // kernelMatrix.UpdatePixelsDefault();
        // kernelMatrix.transform.localScale = new(0.1f, 0.1f, 1f);

        kernelMatrix.gameObject.SetActive(false);

        isConvoluting = false;

        OnConvolutionStopped?.Invoke(id);
    }


    // Update is called once per frame
    void Update()
    {
        Convolute();
        AnimateOutputState(outputState);
    }
}
