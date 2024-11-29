using UnityEngine;

public class OutputMatrix : MonoBehaviour
{
    public GameObject outputPixel;
    float pixelSize = 0.04f;
    public static float MinimumPixelValue = -1f;
    public static float MaximumPixelValue = +1f;

    // public static float horizontalOffset = 16f;
    int matrixSize = 62;

    OutputPixel[,] outputPixels = new OutputPixel[62, 62];

    // Start is called before the first frame update
    void Start()
    {
        Draw();
    }

    void Draw()
    {
        float verticalOffset = transform.position.y;
        float horizontalOffset = transform.position.x;

        verticalOffset = verticalOffset + 0.8f;
        horizontalOffset = horizontalOffset - 0.75f;

        for (int i = 0; i < matrixSize; i++)
        {
            float xPosition = horizontalOffset + i * pixelSize;
            for (int j = 0; j < matrixSize; j++)
            {
                float yPosition = verticalOffset + j * pixelSize;
                Vector3 position = new(xPosition, yPosition, 0f);

                GameObject instance = Instantiate(outputPixel, position, Quaternion.identity);
                instance.transform.parent = transform;
                instance.transform.localScale = new(pixelSize, pixelSize, 0f);

                OutputPixel pixelScript = instance.GetComponent<OutputPixel>();
                outputPixels[i, j] = pixelScript;
            }
        }
    }

    public void Reset()
    {
        foreach (OutputPixel pixel in outputPixels)
        {
            pixel.Reset();
        }
    }

    public void SetPixel(int i, int j, double value)
    {
        outputPixels[i, j].Initialize(value);
    }
}
