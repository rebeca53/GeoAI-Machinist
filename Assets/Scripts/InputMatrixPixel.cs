using System;
using TMPro;
using UnityEngine;

public class InputMatrixPixel : MonoBehaviour
{
    Vector3 position;
    private double pixelValue;
    private TextMeshPro label;

    public void Initialize(double pixelValue, Vector3 pixelPosition)
    {
        this.pixelValue = pixelValue;
        position = pixelPosition;
        transform.GetComponent<SpriteRenderer>().color = GetPixelColor();
        label = transform.Find("Label").GetComponent<TextMeshPro>();
        label.text = GetPixelValue();

        // For now, do not show the pixel value
        transform.Find("Label").gameObject.SetActive(false);
    }

    private Color GetPixelColor()
    {
        float colorValue = (float)pixelValue;
        Color newColor = new(colorValue, colorValue, colorValue);
        return newColor;
    }

    private string GetPixelValue()
    {
        double rounded = Math.Round(pixelValue, 2);
        return rounded.ToString("N2");
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    public void Highlight()
    {
        Transform outline = transform.Find("Outline");
        outline.GetComponent<LineRenderer>().enabled = true;
    }

    public void Unhighlight()
    {
        Transform outline = transform.Find("Outline");
        outline.GetComponent<LineRenderer>().enabled = false;
    }

    private void Start()
    {
        Unhighlight();
    }

    // private void OnTriggerStay2D(Collider2D other)
    // {
    //     Debug.Log("stay over pixel. tag: " + other.tag);
    //     // Debug.Log("stay over pixel. tag parent: " + other.transform.parent.tag);
    //     if (other.CompareTag("KernelCenter"))
    //     {
    //         Highlight();
    //     }

    // }

    // private void OnTriggerExit2D(Collider2D other)
    // {
    //     Debug.Log("out of pixel");
    //     if (other.CompareTag("KernelCenter"))
    //     {
    //         Unhighlight();
    //     }
    // }
}
