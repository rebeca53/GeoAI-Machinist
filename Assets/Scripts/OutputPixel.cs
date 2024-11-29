using System;
using TMPro;
using UnityEngine;

public class OutputPixel : MonoBehaviour
{
    private double pixelValue;
    private TextMeshPro label;

    private Color offColor = Color.black;
    public void Initialize(double pixelValue)
    {
        this.pixelValue = pixelValue;
        transform.GetComponent<SpriteRenderer>().color = GetPixelColor();
        label = transform.Find("Label").GetComponent<TextMeshPro>();
        label.text = GetPixelValue();
    }

    private Color GetPixelColor()
    {
        float normalizedValue = MinMaxNormalization((float)pixelValue, OutputMatrix.MinimumPixelValue, OutputMatrix.MaximumPixelValue);
        Color newColor = Color.Lerp(Color.red, Color.white, normalizedValue);
        return newColor;
    }

    private float MinMaxNormalization(float value, float min, float max)
    {
        return (value - min) / (max - min);
    }

    private string GetPixelValue()
    {
        double rounded = Math.Round(pixelValue, 2);
        return rounded.ToString("N2");
    }

    public void Reset()
    {
        if (transform.GetComponent<SpriteRenderer>())
        {
            transform.GetComponent<SpriteRenderer>().color = offColor;
        }
        if (label)
        {
            label.text = "";
        }
    }
}
