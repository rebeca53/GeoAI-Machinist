using UnityEngine;

public class ConvolutionalMiniGameManager : BaseBoard
{
    public GameObject pixelTile;

    // public GameObject kernel;

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

        LayoutInputMatrix();
    }

    protected override void GameOver()
    {
        GameManager.instance.solvedMinigames["Convolutional 1"] = true;
        GameManager.instance.StartOverviewScene();
    }

    private void LayoutInputMatrix()
    {
        int matrixSize = 28;
        float pixelSize = 0.3f;
        float verticalOffset = 2f;
        float horizontalOffset = 2f;

        for (float i = 0; i < matrixSize; i++)
        {
            float xPosition = horizontalOffset + i * pixelSize;
            for (float j = 0; j < matrixSize; j++)
            {
                float yPosition = verticalOffset + j * pixelSize;
                Vector3 position = new(xPosition, yPosition, 0f);

                GameObject instance = Instantiate(pixelTile, position, Quaternion.identity);
                instance.transform.localScale = new(pixelSize, pixelSize, 0f);
                Color color = xPosition == yPosition ? Color.white : Color.gray;
                instance.GetComponent<SpriteRenderer>().color = color;
                // Pixel pixel = instance.GetComponent<Pixel>();
                // pixel.SetValue(cnnNode.GetValue());
            }
        }

    }

}
