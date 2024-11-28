using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class KernelMatrix : MonoBehaviour
{
    public float pixelSize = ConvolutionalMiniGameManager.pixelSize;

    static readonly int kernelSize = 3;

    private bool grabbed = false;
    private KernelPixel center;
    // private HashSet<KernelPixel> kernelPixels = new HashSet<KernelPixel>();
    // Data
    double[][] kernel = {
    new double[] {
        -0.16016709804534912,
        0.1730394810438156,
        0.11803445965051651
    },
    new double[] {
        -0.06034918874502182,
        0.08442366123199463,
        -0.003968948498368263
    },
    new double[] {
        -0.06272831559181213,
        0.16043928265571594,
        -0.21291641891002655
    }
    };

    // Start is called before the first frame update
    void Start()
    {
        KernelPixel[] kernelPixels = GetComponentsInChildren<KernelPixel>();
        center = kernelPixels[4];
        center.SetKernelCenter();

        for (int k = 0; k < kernelPixels.Count(); k++)
        {
            KernelPixel pixel = kernelPixels[k];

            TextMeshPro label = pixel.transform.Find("Label").GetComponent<TextMeshPro>();
            int i = k / 3;
            int j = k - i * 3;
            label.text = Math.Round(GetKernelPixel(i, j), 2).ToString("N2");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private double GetKernelPixel(int i, int j)
    {
        return kernel[i][j];
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

}
