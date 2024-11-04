using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

// TODO: Have an Abstract class for all Board Manager
public class InputMiniGameManager : MonoBehaviour
{
    private Tilemap m_Tilemap;
    private Tilemap m_Wallsmap;
    private Grid m_Grid;

    public int Width;
    public int Height;

    public Tile[] GroundTiles;

    public Tile[] WallTiles; // [TopLeft, Top, TopRight, Left, Right, BottomLeft, Bottom, BottomRight]

    public PlayerController Player;
    public NonPlayerCharacter NPC;

    // Start is called before the first frame update
    void Start()
    {
        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_Wallsmap = transform.Find("Walls").GetComponent<Tilemap>();
        m_Grid = GetComponentInChildren<Grid>();

        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                DrawFloor(x, y);

                if (IsBorder(x, y))
                {
                    DrawWall(x, y);
                }
            }
        }

        Player.Spawn(this, new Vector2Int(2, 1));
        NPC.Spawn(this, new Vector2Int(1, 1));
    }


    // TODO: move to Abstract class
    private Tile GetFloorTile()
    {
        return GroundTiles[Random.Range(0, GroundTiles.Length)];
    }

    private void DrawFloor(int x, int y)
    {
        Tile tile = GetFloorTile();
        m_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);
    }

    // TODO: move to Abstract class
    private bool IsBorder(int x, int y)
    {
        return x == 0 || y == 0 || x == Width - 1 || y == Height - 1;
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

    // TODO: move to Abstract class
    private void DrawWall(int x, int y)
    {
        Tile tile = GetWallTile(x, y);
        m_Wallsmap.SetTile(new Vector3Int(x, y, 1), tile);
    }

    // TODO: move to Abstract class
    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    private void LayoutSamples()
    {
    }

    private void LayoutGrayscaleBands(SampleBox sampleBox)
    {

    }

    private void DrawBandRegion()
    {
        // Message
        // Color
        // Label
    }

    private void GameOver()
    {
        // TODO: Update OverviewBoardManager
        GameManager.instance.StartOverviewScene();
    }
}
