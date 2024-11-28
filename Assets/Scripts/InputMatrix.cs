using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class InputMatrix : MonoBehaviour
{
    public GameObject inputPixel;
    float pixelSize = 0.04f;
    int matrixSize = 64;

    [SerializeField] public Dictionary<Vector3, GameObject> positions = new Dictionary<Vector3, GameObject>();
    private List<Vector3> samplePositions = new List<Vector3>();

    string address = "convData";
    // Data
    double[,] inputMatrix;

    public void SetMatrix(double[,] matrix)
    {
        inputMatrix = matrix;
        Draw();
    }

    void Draw()
    {
        Debug.Log("pixel size " + pixelSize);
        float verticalOffset = ConvolutionalMiniGameManager.verticalOffsetImages - 0.2f;
        float horizontalOffset = 3.27f;

        for (int i = 0; i < matrixSize; i++)
        {
            float xPosition = horizontalOffset + i * pixelSize;
            for (int j = 0; j < matrixSize; j++)
            {
                float yPosition = verticalOffset + j * pixelSize;
                Vector3 position = new(xPosition, yPosition, 0f);

                GameObject instance = Instantiate(inputPixel, position, Quaternion.identity);
                instance.transform.parent = transform;

                instance.transform.localScale = new(pixelSize, pixelSize, 0f);
                InputMatrixPixel pixelScript = instance.GetComponent<InputMatrixPixel>();
                pixelScript.Initialize(GetPixelValue(i, j), position);

                positions.Add(instance.transform.position, instance);
            }
        }
    }

    public void HighlightNeighboors(Vector3 pixelPosition)
    {
        float x = pixelPosition.x;
        float y = pixelPosition.y;

        Vector3[] nbPositions = {
            new(x - pixelSize, y + pixelSize, 0f),
            new(x, y+pixelSize, 0f),
            new(x+pixelSize, y+pixelSize, 0f),
            new(x-pixelSize, y, 0f),
            new(x+pixelSize, y, 0f),
            new(x-pixelSize, y-pixelSize, 0f),
            new(x, y-pixelSize, 0f),
            new(x+pixelSize, y-pixelSize, 0f)
        };

        Debug.Log("For the pixel: " + pixelPosition);

        foreach (Vector3 position in nbPositions)
        {
            Debug.Log(position);
            if (!positions.ContainsKey(position))
            {
                Debug.Log("NOT valid neighboor " + position);
                continue;
            }
            GameObject nbObject = positions[position];
            nbObject.GetComponent<InputMatrixPixel>().Highlight();
        }
    }

    public void UnhighlightNeighboors(Vector3 pixelPosition)
    {
        float x = pixelPosition.x;
        float y = pixelPosition.y;

        Vector3[] nbPositions = {
            new(x - pixelSize, y + pixelSize, 0f),
            new(x, y+pixelSize, 0f),
            new(x+pixelSize, y+pixelSize, 0f),
            new(x-pixelSize, y, 0f),
            new(x+pixelSize, y, 0f),
            new(x-pixelSize, y-pixelSize, 0f),
            new(x, y-pixelSize, 0f),
            new(x+pixelSize, y-pixelSize, 0f)
        };


        foreach (Vector3 position in nbPositions)
        {
            if (!positions.ContainsKey(position))
            {
                Debug.Log("NOT valid neighboor " + position);
                continue;
            }
            GameObject nbObject = positions[position];
            nbObject.GetComponent<InputMatrixPixel>().Unhighlight();
        }
    }

    private void RegisterToKernelCenter()
    {
        GameObject instance = GameObject.FindWithTag("KernelCenter");
        KernelPixel kernelCenter = instance.GetComponent<KernelPixel>();
        kernelCenter.OnHoverPixel += HighlightNeighboors;
        kernelCenter.OnExitPixel += UnhighlightNeighboors;
    }

    private double GetPixelValue(int i, int j)
    {
        return inputMatrix[i, j];
    }
}
