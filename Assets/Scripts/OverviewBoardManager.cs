using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.Tilemaps;

// TODO: Have an Abstract class for all Board Manager
public class OverviewBoardManager : MonoBehaviour
{
    private Tilemap m_Tilemap;
    private Tilemap m_Wallsmap;
    private Grid m_Grid;

    public int Width;
    public int Height;
    float startCorridor;
    float endCorridor;
    public Tile[] GroundTiles;

    public Tile[] WallTiles; // [TopLeft, Top, TopRight, Left, Right, BottomLeft, Bottom, BottomRight]

    public PlayerController Player;
    public NonPlayerCharacter NPC;


    public GameObject cnnLayerRoom;

    public GameObject inputHolder;
    public GameObject sampleTile;

    // Start is called before the first frame update
    void Start()
    {
        startCorridor = Height * (1f / 3f);
        endCorridor = Height * (2f / 3f);

        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_Wallsmap = GameObject.Find("Walls").GetComponent<Tilemap>();
        m_Grid = GetComponentInChildren<Grid>();

        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                Tile tile;

                if (IsBorder(x, y) && !IsCorridor(x, y))
                {
                    tile = GetWallTile(x, y);
                    m_Wallsmap.SetTile(new Vector3Int(x, y, 1), tile);
                }

                // Also have floor behind the walls
                tile = GroundTiles[Random.Range(0, GroundTiles.Length)];
                m_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
        Player.Spawn(this, GameManager.instance.playerPositionOverview);
        NPC.Spawn(this, new Vector2Int(1, 2));

        LayoutCNNLayers();
        LayoutInputHolder();
    }

    // TODO: move to Abstract class 
    private bool IsBorder(int x, int y)
    {
        return x == 0 || y == 0 || x == Width - 1 || y == Height - 1;
    }

    private bool IsCorridor(int x, int y)
    {
        bool isRight = x == Width - 1;

        float startCorridor = Height * (1f / 3f);
        float endCorridor = Height * (2f / 3f);
        bool isWithinRange = y > startCorridor && y < endCorridor;
        // Debug.Log("isRight " + isRight + ", startCorridor: " + startCorridor + ", endCorridor: " + endCorridor + ", x: " + x + ", y:" + y);
        return isRight && isWithinRange;
    }

    private Tile GetWallTile(int x, int y)
    {
        // [TopLeft, Top, TopRight, Left, Right, BottomLeft, Bottom, BottomRight]
        if (x == 0 && y == Height - 1) // TopLeft
        {
            return WallTiles[0];
        }

        if (x == 0 && y == 0) // BottomLeft
        {
            return WallTiles[5];
        }

        if (x == Width - 1 && y == 0) // BottomRight
        {
            return WallTiles[7];
        }

        if (x == Width - 1 && y == Height - 1) // TopRight
        {
            return WallTiles[2];
        }

        if (x == Width - 1 && y == startCorridor) // Corner Bottom Right
        {
            return WallTiles[9];
        }

        if (x == Width - 1 && y == endCorridor) // Corner Top Right
        {
            return WallTiles[8];
        }

        if (x == 0) // Left
        {
            return WallTiles[3];
        }

        if (x == Width - 1) // Right
        {
            return WallTiles[4];
        }

        if (y == 0) // Bottom
        {
            return WallTiles[6];
        }

        if (y == Height - 1) // Top
        {
            return WallTiles[1];
        }

        // Default
        return WallTiles[1];
    }

    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    void LayoutCNNLayers()
    {
        Debug.Log("Layout CNN Layer");
        // // TODO: Read layers from actual CNN model

        List<string> layerNames = new List<string> {
            "Input", "Convolutional 1", "Activation 1", "Convolutional 2",
            "Activation 2", "Pooling 1", "Convolutional 3", "Activation 3",
            "Convolutional 4", "Activation 4", "Pooling 2", "Output" };
        List<Vector3> positions = new List<Vector3> {
            new Vector3(3f, 9f, 0f), new Vector3(6f, 9f, 0f), new Vector3(9f, 9f, 0f), new Vector3(12f, 9f, 0f),
            new Vector3(3f, 6f, 0f), new Vector3(6f, 6f, 0f), new Vector3(9f, 6f, 0f), new Vector3(12f, 6f, 0f),
            new Vector3(3f, 3f, 0f), new Vector3(6f, 3f, 0f), new Vector3(9f, 3f, 0f), new Vector3(12f, 3f, 0f)
        };

        GameObject tileChoice = cnnLayerRoom;

        for (int i = 0; i < 12; i++)
        {
            CNNLayer cnnLayer = tileChoice.GetComponent<CNNLayer>();
            cnnLayer.type = layerNames[i];
            cnnLayer.isEndOfRow = cnnLayer.type.Equals("Convolutional 2") || cnnLayer.type.Equals("Activation 3") || cnnLayer.type.Equals("Output");

            Vector3 fixedPosition = positions[i];

            GameObject instance = Instantiate(tileChoice, fixedPosition, Quaternion.identity);
            instance.GetComponent<CNNLayer>().DrawConnection();
        }
    }

    void LayoutInputHolder()
    {
        Debug.Log("Layout Input Holder");
        GameObject instance = Instantiate(inputHolder, new Vector3(1f, 9f, 0f), Quaternion.identity);
        InputHolder script = instance.GetComponent<InputHolder>();
        // script.Spawn(this, new Vector2Int(1, 9));
        script.DrawConnection();

        LayoutSample(script);
    }

    void LayoutSample(InputHolder inputHolder)
    {
        GameObject instance = Instantiate(sampleTile, new Vector3(1f, 10f, 0f), Quaternion.identity);
        SampleBox sampleBox = instance.GetComponent<SampleBox>();
        inputHolder.FeedInputSample(sampleBox);
    }
}
