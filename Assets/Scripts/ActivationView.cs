using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActivationView : MonoBehaviour
{
    public event Action<string> OnHover;
    public event Action<string> OnUnhover;
    public Action<string> OnActivationStopped;

    // Scene objects
    public InputHolder activationBoxHolder;
    public Locker locker;
    public GameObject inputScreen;
    public GameObject outputScreen;

    // Matrices and functions
    public ActivationBox activationBox;
    ActivationBox movingActivationBox;
    public InputMatrix inputMatrix;
    public OutputMatrix outputMatrix;
    public string type;

    // Activation
    bool isApplying = false;
    bool activationBoxAtInputHolder = false;
    int animationStep = 8;
    int iActivation = 0;
    int jActivation = 0;
    int matrixSize = 62;
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
        ResetActivation();

        LayoutKernelHolder();
        LayoutInputScreen();
        LayoutOutputScreen();
    }

    /* UI-related methods */

    private void LayoutKernelHolder()
    {
        activationBoxHolder.DrawConnection();
        activationBoxHolder.OnAddedObject += StartActivation;
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
        label.GetComponent<TextMeshPro>().text = "Node 10th";
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

    /* Activation method */

    void ResetActivation()
    {
        iActivation = 0;
        jActivation = 0;
        isApplying = false;
    }

    public void InitInput(double[,] input)
    {
        inputMatrix.SetColor("redblue");
        inputMatrix.SetMatrix(input, matrixSize);
    }

    public bool IsApplyingActivation()
    {
        return isApplying;
    }

    public bool HasActivationBox()
    {
        return activationBoxAtInputHolder;
    }

    public void InitActivationBox(string type)
    {
        Debug.Log("Init Activation Box");
        this.type = type;
        activationBox.SetFunction(type);
        locker.AddActivationBox(activationBox.gameObject);

        GameObject instance = Instantiate(activationBox.gameObject, activationBox.transform.position, Quaternion.identity);
        movingActivationBox = instance.GetComponent<ActivationBox>();
        movingActivationBox.gameObject.SetActive(false);
        movingActivationBox.SetFunction(type);
    }

    public void RemoveActivationBox()
    {
        Debug.Log("Remove Kernel");
        activationBoxAtInputHolder = false;
        OnUnhover?.Invoke(type);
        StopActivation();
        ResetActivation();
        outputMatrix.Reset();
    }

    void StartActivation()
    {
        Debug.Log("Start Convolution");
        activationBoxAtInputHolder = true;
        activationBox.transform.localScale = new(0.3f, 0.3f, 1f);
        activationBox.OnGrabbed += RemoveActivationBox;

        GameObject inputPixel = inputMatrix.GetPixelObject(iActivation, jActivation);
        movingActivationBox.PlaceAt(inputPixel.transform.position);
        movingActivationBox.transform.localScale = new(0.1f, 0.1f, 1f);
        movingActivationBox.gameObject.SetActive(true);

        outputMatrix.Reset();
        iActivation = 0;
        jActivation = 0;
        isApplying = true;

        for (int i = 1; i < matrixSize; i++)
        {
            for (int j = 1; j < matrixSize; j++)
            {
                double activationResult = activationBox.ApplyFunction(inputMatrix.GetPixelValue(i, j));

                // retrieve the pixel from the output matrix
                // change its value and color
                outputMatrix.SetPixel(i, j, activationResult, isActivationResult: true);
                outputMatrix.HidePixel(i, j);
            }
        }
    }

    void ApplyActivation()
    {
        if (!isApplying)
        {
            return;
        }

        // move the kernel center over it
        GameObject inputPixel = inputMatrix.GetPixelObject(iActivation, jActivation);
        movingActivationBox.PlaceAt(inputPixel.transform.position);

        int jNext = jActivation;
        int iNext = iActivation;
        int stepDone = 0;
        // Debug.Log("start loop iConvNext " + iConvNext + ", jConvNext " + jConvNext);
        while ((stepDone < animationStep) && (jNext < matrixSize) && (iNext < matrixSize))
        {
            Debug.Log("iConvNext " + iNext + " limit " + matrixSize);
            Debug.Log("jConvNext " + jNext + " limit " + (jActivation + animationStep));
            outputMatrix.ShowPixel(iNext, jNext);
            jNext++;
            if (jNext >= matrixSize)
            {
                iNext++;
                jNext = 1; // stride
            }
            stepDone++;
        }
        // Debug.Log("end loop iConvNext " + iConvNext + ", jConvNext " + jConvNext);


        jActivation += animationStep;
        if (jActivation >= matrixSize)
        {
            iActivation++;
            jActivation = 0;
        }
        if (iActivation >= matrixSize)
        {
            StopActivation();
        }
    }

    void StopActivation()
    {
        Debug.Log("Stop Activation");
        activationBox.OnGrabbed -= StopActivation;
        movingActivationBox.gameObject.SetActive(false);

        isApplying = false;
        OnActivationStopped?.Invoke(type);
    }

    // Update is called once per frame
    void Update()
    {
        ApplyActivation();
        AnimateOutputState();
    }


    // TODO: abstract OutputLine
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

        // Debug.Log("id[" + id + "] animate output state: " + outputState);
        if (outputState.Equals("correct"))
        {
            // Debug.Log("using Lerp because it is correct.");
            outputLineRenderer.material.color = Color.Lerp(Color.white, Color.cyan, Mathf.PingPong(Time.time, 0.5f));
            outputLineRenderer.startWidth = inactiveWidth * 2;
            outputLineRenderer.endWidth = inactiveWidth * 2;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Debug.Log("on trigger enter activation view");
        if (HasActivationBox())
        {
            OnHover?.Invoke(type);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (HasActivationBox())
        {
            OnHover?.Invoke(type);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Debug.Log("on trigger exit activation view");
        if (HasActivationBox())
        {
            OnUnhover?.Invoke(type);
        }
    }

}
