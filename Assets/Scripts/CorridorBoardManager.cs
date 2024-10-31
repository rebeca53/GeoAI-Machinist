using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CorridorBoardManager : MonoBehaviour
{
    private Tilemap m_Tilemap;
    private Tilemap m_Wallsmap;
    private Grid m_Grid;

    public int Width;
    public int Height;

    public float startCorridor;
    public float endCorridor;
    public int LengthCorridor;

    public Tile[] GroundTiles;

    public Tile[] WallTiles; // [TopLeft, Top, TopRight, Left, Right, BottomLeft, Bottom, BottomRight]


    // Start is called before the first frame update
    void Start()
    {
        startCorridor = 4;
        endCorridor = 8;
        LengthCorridor = 3;

        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_Wallsmap = transform.Find("Walls").GetComponent<Tilemap>();
        m_Grid = GetComponentInChildren<Grid>();

        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                Tile tile;
                // Draw Corridor
                if (IsCorridor(x, y))
                {
                    Debug.Log("IsCorridor");
                    DrawCorridor(x, y);
                    // tile = GetCorridorWallTile(x, y);
                    // m_Wallsmap.SetTile(new Vector3Int(x, y, 1), tile);
                }
            }
        }
    }



    private bool IsCorridor(int x, int y)
    {
        bool isWithinLength = x < LengthCorridor;
        bool isWithinRange = y >= startCorridor && y <= endCorridor;

        // Debug.Log("isWithinLength " + isWithinLength + ", startCorridor: " + startCorridor + ", endCorridor: " + endCorridor + ", x: " + x + ", y:" + y);
        return isWithinLength && isWithinRange;
    }

    private Tile GetCorridorWallTile(int x, int y)
    {
        Debug.Log("Get corridor tile x " + x + ", y " + y);

        // Bottom
        if (y == startCorridor)
        {
            return WallTiles[6];
        }

        // Top
        if (y == endCorridor)
        {
            return WallTiles[1];
        }

        return null;
    }

    private void DrawCorridor(int x, int y)
    {
        Tile tile = GetCorridorWallTile(x, y);
        if (tile != null)
        {
            m_Wallsmap.SetTile(new Vector3Int(x, y, 1), tile);
        }
        else
        {
            tile = GroundTiles[Random.Range(0, GroundTiles.Length)]; ;
            m_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);

        }
    }

}
