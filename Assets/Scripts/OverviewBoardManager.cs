using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
    public Tile[] GroundTiles;

    public Tile[] WallTiles; // [TopLeft, Top, TopRight, Left, Right, BottomLeft, Bottom, BottomRight]

    public PlayerController Player;

    // Start is called before the first frame update
    void Start()
    {
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
        Player.Spawn(this, new Vector2Int(1, 1));

    }

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
        Debug.Log("isRight " + isRight + ", startCorridor: " + startCorridor + ", endCorridor: " + endCorridor + ", x: " + x + ", y:" + y);
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
}
