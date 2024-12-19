using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatMatrix : MonoBehaviour
{
    public GameObject outputPixel;
    float pixelSize = 0.05f;
    int length = 169;

    OutputPixel[] outputPixels = new OutputPixel[169];

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

        float maxYPosition = verticalOffset + length * pixelSize;

        for (int i = 0; i < length; i++)
        {
            float xPosition = horizontalOffset;
            float yPosition = maxYPosition - i * pixelSize;
            Vector3 position = new(xPosition, yPosition, 0f);

            GameObject instance = Instantiate(outputPixel, position, Quaternion.identity);
            instance.transform.parent = transform;
            instance.transform.localScale = new(pixelSize * 5, pixelSize, 0f);

            OutputPixel pixelScript = instance.GetComponent<OutputPixel>();
            outputPixels[i] = pixelScript;
        }
    }

    public void Reset()
    {
        foreach (OutputPixel pixel in outputPixels)
        {
            pixel.Reset();
            pixel.gameObject.SetActive(false);
        }
    }

    public void SetPixel(int i, double value)
    {
        // Debug.Log("SetPixel i=" + i + " to " + value * 5.7);
        outputPixels[i].Initialize(value);
    }

    public void HidePixel(int i)
    {
        outputPixels[i].gameObject.SetActive(false);
    }

    public void ShowPixel(int i)
    {
        // Debug.Log("show pixel (" + i + ", " + j + ")");
        if (i >= length)
        {
            // Debug.Log("Invalid pixel. Out of bounds.");
            return;
        }
        outputPixels[i].gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
