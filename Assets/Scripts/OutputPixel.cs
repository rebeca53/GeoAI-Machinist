using System;
using TMPro;
using UnityEngine;

public class OutputPixel : MonoBehaviour
{
    private double pixelValue;
    private TextMeshPro label;
    private Color offColor = Color.black;
    bool isActivationResult = false;

    public void Initialize(double pixelValue, bool isActivationResult = false)
    {
        this.pixelValue = pixelValue;
        this.isActivationResult = isActivationResult;
        transform.GetComponent<SpriteRenderer>().color = GetPixelColor();
        label = transform.Find("Label").GetComponent<TextMeshPro>();
        label.text = GetPixelValue();
    }

    // private Color GetPixelColor()
    // {
    //     float threshold = 0.87f;
    //     if (pixelValue >= threshold)
    //     {
    //         return Color.blue;
    //     }
    //     Color newColor = Color.Lerp(Color.white, Color.blue, (float)pixelValue - (1 - threshold));
    //     return newColor;
    // }

    private Color GetPixelColor()
    {
        if (isActivationResult)
        {
            return GetUnboundedPixelColor();
        }
        else
        {
            float threshold = 0.87f;
            if (pixelValue >= threshold)
            {
                return Color.blue;
            }
            Color newColor = Color.Lerp(Color.white, Color.blue, (float)pixelValue - (1 - threshold));
            return newColor;
        }
    }

    private Color GetUnboundedPixelColor()
    {
        if (pixelValue <= 0)
        {
            return Color.Lerp(Color.white, Color.red, (float)(-10 * pixelValue));
        }
        Color newColor = Color.Lerp(Color.white, Color.blue, (float)pixelValue);
        return newColor;
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
