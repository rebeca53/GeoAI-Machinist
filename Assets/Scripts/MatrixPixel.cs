using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MatrixPixel : MonoBehaviour
{
    private double pixelValue;
    private TextMeshPro label;

    public void Initialize(double pixelValue)
    {
        this.pixelValue = pixelValue;
        transform.GetComponent<SpriteRenderer>().color = GetPixelColor();
        label = transform.Find("Label").GetComponent<TextMeshPro>();
        label.text = GetPixelValue();
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

    private void Start()
    {
        Transform outline = transform.Find("Outline");
        outline.GetComponent<LineRenderer>().enabled = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("stay over pixel. tag: " + other.tag);
        // Debug.Log("stay over pixel. tag parent: " + other.transform.parent.tag);
        if (other.CompareTag("KernelCenter"))
        {
            Transform outline = transform.Find("Outline");
            outline.GetComponent<LineRenderer>().enabled = true;
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("out of pixel");
        if (other.CompareTag("KernelCenter"))
        {
            Transform outline = transform.Find("Outline");
            outline.GetComponent<LineRenderer>().enabled = false;
        }
    }
}
