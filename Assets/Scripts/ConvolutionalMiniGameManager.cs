using System;
using TMPro;
using UnityEngine;

public class ConvolutionalMiniGameManager : BaseBoard
{
    public GameObject inputObject;

    public GameObject kernelObject;

    public GameObject outputObject;

    static public float pixelSize = 0.2f;

    static public float verticalOffsetImages = 5f;

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

        Player.Spawn(this, new Vector2Int(2, 1));
        NPC.Spawn(this, new Vector2Int(1, 1));

        // LayoutInputMatrix();
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
        GameObject instanceInput = Instantiate(inputObject, position, Quaternion.identity);
    }

    private void LayoutKernel()
    {
        float verticalOffset = 2f;
        float horizontalOffset = 3f;

        Vector3 position = new(horizontalOffset, verticalOffset, 0f);
        GameObject instanceKernel = Instantiate(kernelObject, position, Quaternion.identity);
    }

    private void LayoutOutputMatrix()
    {
        float verticalOffset = verticalOffsetImages;
        float horizontalOffset = OutputMatrix.horizontalOffset;

        Vector3 position = new(horizontalOffset, verticalOffset, 0f);
        GameObject instanceOutput = Instantiate(outputObject, position, Quaternion.identity);
    }

}
