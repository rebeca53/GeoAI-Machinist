using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FlattenLayer : MonoBehaviour
{
    public Action OnFlatten;

    // Scene objects
    public SelectorSwitch selectorSwitch;
    public CameraZoom cameraZoom;
    public PlayerController Player;
    public GameObject flatScreen;

    // Matrices
    public InputMatrix inputMatrix;
    public FlatMatrix flatMatrix;

    // Flattening
    bool isFlattening = false;
    int animationStep = 1;
    int iFlat = 0;
    int jFlat = 0;
    int kFlat = 0;
    int matrixSize = 13;

    // Data
    public TextAsset dataText;
    FlattenData data;

    [System.Serializable]
    class FlattenData
    {
        public List<double> inputMatrix = new List<double>();
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
            double reescalePixel = value / 5.7f;
            // Debug.Log("reescale " + value + " to " + reescalePixel);
            unflatten[i, j] = reescalePixel;
        }

        return unflatten;
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetFlattening();

        LoadMatrix();

        InitInput(UnflatMatrix(data.inputMatrix, matrixSize));

        selectorSwitch.Disable();
        selectorSwitch.OnSwitch += StartFlattening;
    }

    void LoadMatrix()
    {
        // Debug.Log(dataText.text);
        data = JsonUtility.FromJson<FlattenData>(dataText.text);

        if (data == null)
        {
            Debug.LogError("Failed to retrieve from JSON");
        }
        else
        {
            if (data.inputMatrix == null)
            {
                Debug.LogError("Input is none");
            }
        }
    }

    /* Flattening */
    void ResetFlattening()
    {
        iFlat = 0;
        jFlat = 0;
        kFlat = 0;
        isFlattening = false;
        flatMatrix.Reset();
        selectorSwitch.OnSwitch -= ResetFlattening;
        selectorSwitch.OnSwitch += StartFlattening;
        selectorSwitch.UpdateState("inactive");
    }

    public void InitInput(double[,] input)
    {
        inputMatrix.SetColor("redblue");
        inputMatrix.SetPixelSize(0.16f);
        inputMatrix.SetMatrix(input, matrixSize);
    }

    void StartFlattening()
    {
        selectorSwitch.UpdateState("correct");
        selectorSwitch.OnSwitch -= StartFlattening;
        selectorSwitch.OnSwitch += ResetFlattening;

        flatMatrix.Reset();
        iFlat = 0;
        jFlat = 0;
        kFlat = 0;
        isFlattening = true;

        for (int k = 0; k < 169; k++)
        {
            int i = k / matrixSize;
            int j = k - i * matrixSize;
            double value = inputMatrix.GetPixelValue(i, j);

            flatMatrix.SetPixel(k, value);
            flatMatrix.HidePixel(k);
        }

        Player.Disable();
        cameraZoom.ChangeZoomTarget(flatScreen);
        cameraZoom.ChangeZoomSmooth(5f);
    }

    void Flatten()
    {
        if (!isFlattening)
        {
            return;
        }

        int jNext = jFlat;
        int iNext = iFlat;
        int stepDone = 0;
        // Debug.Log("start loop iConvNext " + iConvNext + ", jConvNext " + jConvNext);
        while ((stepDone < animationStep) && (jNext < matrixSize) && (iNext < matrixSize))
        {
            // Debug.Log("iConvNext " + iNext + " limit " + matrixSize);
            // Debug.Log("jConvNext " + jNext + " limit " + (jFlat + animationStep));
            int k = iNext + jNext * matrixSize;
            inputMatrix.HidePixel(iNext, jNext);
            flatMatrix.ShowPixel(kFlat);
            // Debug.Log(kFlat);
            kFlat++;
            jNext++;
            if (jNext >= matrixSize)
            {
                iNext++;
                jNext = 1; // stride
            }
            stepDone++;
        }
        // Debug.Log("end loop iConvNext " + iConvNext + ", jConvNext " + jConvNext);


        jFlat += animationStep;
        if (jFlat >= matrixSize)
        {
            iFlat++;
            jFlat = 0;
        }
        if (iFlat >= matrixSize)
        {
            StopFlatenning();
        }
    }

    void StopFlatenning()
    {
        isFlattening = false;
        selectorSwitch.Disable();
        cameraZoom.ChangeZoomTarget(Player.gameObject);
        Player.Enable();
        cameraZoom.ChangeZoomSmooth(1.4f);
        OnFlatten?.Invoke();
    }

    float timeoutSeconds = 3f / 169;
    float timer = 0f;
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > timeoutSeconds)
        {
            timer = 0f;
            Flatten();
        }
    }
}
