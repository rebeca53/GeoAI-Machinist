using System.Net.Security;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class BaseBoard : MonoBehaviour
{
    // Board scene
    protected Tilemap m_Tilemap;
    protected Tilemap m_Wallsmap;
    protected Grid m_Grid;

    public int Width;
    public int Height;

    public Tile[] GroundTiles;
    public Tile[] WallTiles; // [TopLeft, Top, TopRight, Left, Right, BottomLeft, Bottom, BottomRight]

    // Exit
    public float exitXPosition;
    public GameObject exitObject;

    // Characters
    public PlayerController Player;
    public NonPlayerCharacter NPC;

    protected void InitializeTilemap()
    {
        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_Wallsmap = transform.Find("Walls").GetComponent<Tilemap>();
        m_Grid = GetComponentInChildren<Grid>();
    }

    // Floor
    protected Tile GetFloorTile()
    {
        return GroundTiles[Random.Range(0, GroundTiles.Length)];
    }
    protected void DrawFloor(int x, int y)
    {
        Tile tile = GetFloorTile();
        m_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);
    }

    // Walls and Exit
    protected bool IsBorder(int x, int y)
    {
        return x == 0 || y == 0 || x == Width - 1 || y == Height - 1;
    }

    protected bool IsExit(int x, int y)
    {
        bool isBottom = y == 0;
        bool isExitRegion = x == exitXPosition || x == exitXPosition - 1 || x == exitXPosition + 1;
        return isBottom && isExitRegion;
    }

    protected void DrawExit(int x, int y)
    {
        if (x == exitXPosition - 1)
        {
            Tile tile = WallTiles[9];
            m_Wallsmap.SetTile(new Vector3Int(x, y, 1), tile);
            return;
        }
        if (x == exitXPosition + 1)
        {
            Tile tile = WallTiles[8];
            m_Wallsmap.SetTile(new Vector3Int(x, y, 1), tile);
            return;
        }
        Instantiate(exitObject, new Vector3(exitXPosition + 0.5f, 0.5f, 0f), Quaternion.identity);
    }

    protected Tile GetWallTile(int x, int y)
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

    protected void DrawWall(int x, int y)
    {
        Tile tile = GetWallTile(x, y);
        m_Wallsmap.SetTile(new Vector3Int(x, y, 1), tile);
    }

    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    protected abstract void GameOver();
}
