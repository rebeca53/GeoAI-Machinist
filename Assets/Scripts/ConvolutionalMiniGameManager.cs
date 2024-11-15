using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConvolutionalMiniGameManager : BaseBoard
{
    // Pre-fabs
    public GameObject inputObject;

    public GameObject kernelObject;

    public GameObject outputObject;

    // Instances
    GameObject instanceInput;
    GameObject instanceOutput;
    KernelMatrix kernelMatrix;
    KernelPixel kernelCenter;

    // UI constants
    static public readonly float pixelSize = 0.2f;
    static public readonly float verticalOffsetImages = 5f;

    // Movement
    private readonly float step = pixelSize;

    // Start is called before the first frame update
    void Start()
    {
        InitializeTilemap();

        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                DrawFloor(x, y);

                if (IsBorder(x, y) && !IsExit(x, y))
                {
                    DrawWall(x, y);
                }
                if (IsExit(x, y))
                {
                    DrawExit(x, y);
                }
            }
        }

        Player.moveSpeed = 1f;
        Player.Spawn(this, new Vector2Int(2, 1));
        NPC.Spawn(this, new Vector2Int(1, 1));

        LayoutInputMatrix();
        LayoutKernel();
        LayoutOutputMatrix();
    }

    protected override void GameOver()
    {
        GameManager.instance.solvedMinigames["Convolutional 1"] = true;
        GameManager.instance.StartOverviewScene();
    }

    private void LayoutInputMatrix()
    {
        float verticalOffset = verticalOffsetImages;
        float horizontalOffset = 5f;

        Vector3 position = new(horizontalOffset, verticalOffset, 0f);
        instanceInput = Instantiate(inputObject, position, Quaternion.identity);
    }

    private void LayoutKernel()
    {
        float verticalOffset = 2f;
        float horizontalOffset = 3f;

        Vector3 position = new(horizontalOffset, verticalOffset, 0f);
        GameObject instanceKernel = Instantiate(kernelObject, position, Quaternion.identity);
        kernelMatrix = instanceKernel.GetComponent<KernelMatrix>();
        // RegisterToKernelPixel();
    }

    private void LayoutOutputMatrix()
    {
        float verticalOffset = verticalOffsetImages;
        float horizontalOffset = OutputMatrix.horizontalOffset;

        Vector3 position = new(horizontalOffset, verticalOffset, 0f);
        instanceOutput = Instantiate(outputObject, position, Quaternion.identity);
    }

    // TODO: Move Convolution mechanics to this BoardManager
    private void RegisterToKernelPixel()
    {
        kernelCenter = kernelMatrix.GetKernelCenter();
        kernelCenter.OnHoverPixel += MultiplyMatrices;
        // kernelPixel.OnExitPixel += MultiplyMatrices;
    }

    private void MultiplyMatrices(Vector3 position)
    {
        Debug.Log("Multiply Matrices on Hover Pixel");
        instanceOutput.GetComponent<InputMatrix>().HighlightNeighboors(position);
    }

    // Kernel Controller
    // void Update()
    // {
    //     if (kernelMatrix && kernelMatrix.IsGrabbed())
    //     {
    //         HandleInput();
    //     }
    // }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            kernelMatrix.MoveRight();
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            kernelMatrix.MoveLeft();
            return;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            kernelMatrix.MoveUp();
            return;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            kernelMatrix.MoveDown();
            return;
        }
    }

}
