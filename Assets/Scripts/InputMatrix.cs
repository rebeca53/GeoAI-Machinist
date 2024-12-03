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
    [SerializeField] public Dictionary<(int, int), GameObject> pixelPositions = new Dictionary<(int, int), GameObject>();

    private List<Vector3> samplePositions = new List<Vector3>();

    string address = "convData";
    // Data
    int id;
    double[,] inputMatrix;

    public void SetMatrix(int id, double[,] matrix)
    {
        this.id = id;
        inputMatrix = matrix;
        // Debug.Log("Before: Position of input matrix [" + id + "]:" + transform.position);
        // transform.position = transform.parent.position; //new(1.5f, -2f, 0f);
        // Debug.Log("Position of input matrix [" + id + "]:" + transform.position);

        Draw();
    }

    void Draw()
    {
        // Debug.Log("pixel size " + pixelSize);
        float verticalOffset = transform.position.y;
        float horizontalOffset = transform.position.x;

        verticalOffset = verticalOffset + 0.8f;
        horizontalOffset = horizontalOffset - 0.75f;

        int k = 0;
        float maxYPosition = verticalOffset + matrixSize * pixelSize;
        for (int j = 0; j < matrixSize; j++)
        {
            float xPosition = horizontalOffset + j * pixelSize;
            for (int i = 0; i < matrixSize; i++)
            {
                float yPosition = maxYPosition - i * pixelSize;
                Vector3 position = new(xPosition, yPosition, 0f);

                GameObject instance = Instantiate(inputPixel, position, Quaternion.identity);
                instance.transform.parent = transform;
                instance.transform.localScale = new(pixelSize, pixelSize, 0f);

                InputMatrixPixel pixelScript = instance.GetComponent<InputMatrixPixel>();
                pixelScript.Initialize(GetPixelValue(i, j), position);

                positions.Add(instance.transform.position, instance);
                pixelPositions.Add((i, j), instance);

                // Debug.Log("input pixel [" + k + "](" + i + "," + j + ") position: " + position + " value: " + GetPixelValue(i, j));

                k++;
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

        // Debug.Log("For the pixel: " + pixelPosition);

        foreach (Vector3 position in nbPositions)
        {
            Debug.Log(position);
            if (!positions.ContainsKey(position))
            {
                // Debug.Log("NOT valid neighboor " + position);
                continue;
            }
            GameObject nbObject = positions[position];
            nbObject.GetComponent<InputMatrixPixel>().Highlight();
        }
    }

    public List<double> GetNeighboors(int i, int j)
    {
        List<double> result = new List<double>();
        result.Add(GetPixelValue(i - 1, j - 1));
        result.Add(GetPixelValue(i - 1, j));
        result.Add(GetPixelValue(i - 1, j + 1));
        result.Add(GetPixelValue(i, j - 1));
        result.Add(GetPixelValue(i, j));
        result.Add(GetPixelValue(i, j + 1));
        result.Add(GetPixelValue(i + 1, j - 1));
        result.Add(GetPixelValue(i + 1, j));
        result.Add(GetPixelValue(i + 1, j + 1));

        return result;
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
                // Debug.Log("NOT valid neighboor " + position);
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

    public GameObject GetPixelObject(int i, int j)
    {
        return pixelPositions[(i, j)];
    }

}
