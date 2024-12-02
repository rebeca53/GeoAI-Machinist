using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class KernelMatrix : MonoBehaviour
{
    public float pixelSize = ConvolutionalMiniGameManager.pixelSize;

    private bool grabbed = false;
    private KernelPixel center;
    KernelPixel[] kernelPixels;
    Vector3 target;
    Vector3 currentPosition;

    // private HashSet<KernelPixel> kernelPixels = new HashSet<KernelPixel>();
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
            Debug.Log("kernel pixel [" + k + "](" + i + "," + j + ") position: " + pixel.transform.position + " value: " + GetKernelPixel(i, j));

            pixel.SetDefault();
        }
    }

    public double GetKernelPixel(int i, int j)
    {
        return kernel[i, j];
    }

    private bool IsKernelCenter(int i, int j)
    {
        return (i == 1) && (j == 1);
    }

    public void MoveRight()
    {
        transform.position = new(transform.position.x + pixelSize, transform.position.y, transform.position.z);
    }

    public void MoveLeft()
    {
        transform.position = new(transform.position.x - pixelSize, transform.position.y, transform.position.z);
    }

    public void MoveUp()
    {
        transform.position = new(transform.position.x, transform.position.y + pixelSize, transform.position.z);
    }

    public void MoveDown()
    {
        transform.position = new(transform.position.x, transform.position.y - pixelSize, transform.position.z);
    }

    public void Grab(Vector3 grabberPosition)
    {
        Debug.Log("xgrab previous transform position " + transform.position);
        Debug.Log("xgrab previous transform local position " + transform.localPosition);

        Debug.Log("xgrab " + grabberPosition);
        grabbed = true;
        Vector3 relativeToParentPosition = new(0f, 0f, 0f);
        transform.localPosition = relativeToParentPosition;
        // transform.position = new(grabberPosition.x, grabberPosition.y, grabberPosition.z);
        Debug.Log("xgrab transform position " + transform.position);
        Debug.Log("xgrab transform local position " + transform.localPosition);
    }

    public bool IsGrabbed()
    {
        return grabbed;
    }

    public KernelPixel GetKernelCenter()
    {
        return center;
    }

    void Start()
    {
        target = transform.position;
        // startPosition = target = transform.position;
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

}
