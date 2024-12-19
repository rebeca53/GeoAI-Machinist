using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.Tilemaps;

// TODO: Have an Abstract class for all Board Manager
public class CommandCenterBoardManager : MonoBehaviour
{
    // Scene
    private Tilemap m_Tilemap;
    private Tilemap m_Wallsmap;
    private Grid m_Grid;

    public int Width;
    public int Height;

    public float startCorridor;
    public float endCorridor;

    public Tile[] GroundTiles;

    public Tile[] WallTiles; // [TopLeft, Top, TopRight, Left, Right, BottomLeft, Bottom, BottomRight]

    // Instances
    public GameObject screen;
    public GameObject dryPlanet;
    public GameObject healthyPlanet;
    public GameObject helpPods;

    // public CommandCenter commandCenter;
    public CommandCenterPlaybackDirector playbackDirector;

    // Start is called before the first frame update
    void Start()
    {
        startCorridor = 0;
        endCorridor = 4;

        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_Wallsmap = transform.Find("Walls").GetComponent<Tilemap>();
        m_Grid = GetComponentInChildren<Grid>();

        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                Tile tile;

                // Draw Walls
                if (IsBorder(x, y) && !IsCorridor(x, y))
                {
                    tile = GetWallTile(x, y);
                    m_Wallsmap.SetTile(new Vector3Int(x, y, 1), tile);
                }

                // // Draw Floor - Also have floor behind the walls
                tile = GroundTiles[Random.Range(0, GroundTiles.Length)];
                m_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }

        if (GameManager.instance.IsGameOver())
        {
            OnGameOver();
        }
    }

    void OnGameOver()
    {
        // Fade Out
        // Place Player and NPC
        NonPlayerCharacter NPC = GameObject.Find("NPC").GetComponent<NonPlayerCharacter>();
        PlayerController Player = GameObject.Find("Player").GetComponent<PlayerController>();

        NPC.Spawn(this, new Vector2Int(7, 5));
        Player.Spawn(this, new Vector2Int(8, 5));

        // Playback Director play dialogue
        playbackDirector.StartAnimation();
        // Playback Director play game over animation
        // Go back to Home
    }

    private bool IsCorridor(int x, int y)
    {
        bool isLeft = x == 0;
        bool isWithinRange = y > startCorridor && y < endCorridor;

        // Debug.Log("isWithinLength " + isWithinLength + ", startCorridor: " + startCorridor + ", endCorridor: " + endCorridor + ", x: " + x + ", y:" + y);
        return isLeft & isWithinRange;
    }

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
            return WallTiles[6];
        }

        if (x == 0 && y == endCorridor) // Corner
        {
            return WallTiles[10];
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
