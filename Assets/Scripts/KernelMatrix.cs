using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;

public class KernelMatrix : MonoBehaviour
{
    public Action OnGrabbed;
    public float pixelSize = ConvolutionalMiniGameManager.pixelSize;

    private bool grabbed = false;
    private KernelPixel center;
    KernelPixel[] kernelPixels;
    Vector3 target;
    Vector3 currentPosition;

    // Blink
    public GameObject kernelOutline;
    bool outlineBlinking = false;
    float blinkPeriodSeconds = 1;
    float timer = 0f;

    // Data
    double[,] kernel;
    public List<double> flatKernel;

    public void SetMatrix(List<double> newFlatKernel, double[,] newKernel)
    {
        kernel = newKernel;
        flatKernel = newFlatKernel;
        Draw();
    }

    void Draw()
    {
        kernelPixels = GetComponentsInChildren<KernelPixel>();
        center = kernelPixels[4];
        center.SetKernelCenter();

        for (int k = 0; k < kernelPixels.Count(); k++)
        {
            KernelPixel pixel = kernelPixels[k];
            TextMeshPro label = pixel.transform.Find("Label").GetComponent<TextMeshPro>();
            int i = k / 3;
            int j = k - i * 3;
            label.text = Math.Round(GetKernelPixel(i, j), 2).ToString("N2");
            // Debug.Log("kernel pixel [" + k + "](" + i + "," + j + ") position: " + pixel.transform.position + " value: " + GetKernelPixel(i, j));

            pixel.SetDefault();
        }
    }

    public double GetKernelPixel(int i, int j)
    {
        return kernel[i, j];
    }

    public void Grab(Vector3 grabberPosition)
    {
        // Debug.Log("xgrab previous transform position " + transform.position);
        // Debug.Log("xgrab previous transform local position " + transform.localPosition);

        // Debug.Log("xgrab " + grabberPosition);
        grabbed = true;
        Vector3 relativeToParentPosition = new(0f, 0f, 0f);
        transform.localPosition = relativeToParentPosition;
        // transform.position = new(grabberPosition.x, grabberPosition.y, grabberPosition.z);
        // Debug.Log("xgrab transform position " + transform.position);
        // Debug.Log("xgrab transform local position " + transform.localPosition);

        OnGrabbed?.Invoke();
        StopBlink();
    }

    public bool IsGrabbed()
    {
        return grabbed;
    }

    void Start()
    {
        target = transform.position;
    }

    public void UpdatePixelsConvoluting()
    {
        foreach (KernelPixel pixel in kernelPixels)
        {
            Debug.Log("change the color of a kernel pixel");
            pixel.SetConvoluting();
        }
    }

    public void UpdatePixelsDefault()
    {
        foreach (KernelPixel pixel in kernelPixels)
        {
            Debug.Log("change the color of a kernel pixel");
            pixel.SetDefault();
        }
    }

    public void MoveTo(Vector3 targetPosition)
    {
        transform.parent = null;
        target = targetPosition;
        Debug.Log("currentPosition " + currentPosition);
        Debug.Log("target " + target);
    }

    public void MoveRight(float x)
    {
        transform.parent = null;
        target = new(transform.position.x + x, transform.position.y, transform.position.z);
    }

    public void PlaceAt(Vector3 newPosition)
    {
        transform.parent = null;
        transform.position = newPosition;
        target = transform.position;
    }

    public void Blink()
    {
        outlineBlinking = true;
    }

    public void StopBlink()
    {
        outlineBlinking = false;
        kernelOutline.SetActive(false);
    }

    private void ToggleKernelOutline()
    {
        kernelOutline.SetActive(!kernelOutline.activeSelf);
    }

    void Update()
    {
        if (outlineBlinking)
        {
            timer += Time.deltaTime;
            if (timer >= blinkPeriodSeconds)
            {
                ToggleKernelOutline();
                timer = 0f;
            }
        }
    }
}
