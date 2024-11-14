using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KernelMatrix : MonoBehaviour
{
    public GameObject kernelPixel;
    public float pixelSize = ConvolutionalMiniGameManager.pixelSize;

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
        int kernelSize = 3;
        float verticalOffset = transform.position.x;
        float horizontalOffset = transform.position.y;

        for (int i = 0; i < kernelSize; i++)
        {
            float xPosition = horizontalOffset + i * pixelSize;
            for (int j = 0; j < kernelSize; j++)
            {
                float yPosition = verticalOffset + j * pixelSize;
                Vector3 position = new(xPosition, yPosition, 0f);

                GameObject instance = Instantiate(kernelPixel, position, Quaternion.identity);
                instance.transform.parent = transform;

                instance.transform.localScale = new(pixelSize, pixelSize, 0f);
                if (IsKernelCenter(i, j))
                {
                    instance.GetComponent<KernelPixel>().SetKernelCenter();
                }
                // instance.tag = "Kernel";
                TextMeshPro label = instance.transform.Find("Label").GetComponent<TextMeshPro>();
                label.text = Math.Round(GetKernelPixel(i, j), 2).ToString("N2");

            }
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
}
