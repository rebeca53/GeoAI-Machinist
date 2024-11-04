using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
//Tells Random to use the Unity Engine random number generator.
using Random = UnityEngine.Random;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


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

    public GameObject[] sampleTiles;
    public GameObject spectralBandTile;
    private List<Vector3> gridPositions = new List<Vector3>();


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

        InitialiseList();
        LayoutSample();
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


    //Clears our list gridPositions and prepares it to generate a new board.
    void InitialiseList()
    {
        //Clear our list gridPositions.
        gridPositions.Clear();

        //Loop through x axis (columns).
        for (int x = 1; x < Width - 1; x++)
        {
            //Within each column, loop through y axis (rows).
            for (int y = 1; y < Height - 1; y++)
            {
                //At each index add a new Vector3 to our list with the x and y coordinates of that position.
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    // TODO: Move to Abstract class
    //RandomPosition returns a random position from our list gridPositions.
    Vector3 RandomPosition()
    {
        //Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
        int randomIndex = Random.Range(0, gridPositions.Count);
        //Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
        Vector3 randomPosition = gridPositions[randomIndex];

        //Remove the entry at randomIndex from the list so that it can't be re-used.
        gridPositions.RemoveAt(randomIndex);

        //Return the randomly selected Vector3 position.
        return randomPosition;
    }

    private void LayoutSample()
    {
        GameObject tileChoice = sampleTiles[0];

        //Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
        Vector3 randomPosition = RandomPosition();

        //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
        GameObject instance = Instantiate(tileChoice, randomPosition, Quaternion.identity);
        SampleBox sampleBox = instance.GetComponent<SampleBox>();
        sampleBox.OnBreak += LayoutGrayscaleBands;
    }

    private void LayoutGrayscaleBands(string sampleBox, Vector3 position)
    {
        Debug.Log("Break the box");
        GameObject tileChoice = spectralBandTile;

        Vector3 upper = position;
        upper.y++;
        //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
        GameObject blue = Instantiate(tileChoice, upper, Quaternion.identity);

        Vector3 down = position;
        down.y--;
        GameObject green = Instantiate(tileChoice, down, Quaternion.identity);
        SampleSpectralBand script = green.GetComponent<SampleSpectralBand>();
        script.LoadSprite("AnnualCrop_1_Green");

        Vector3 upperRight = position;
        upperRight.y++;
        upperRight.x++;
        GameObject red = Instantiate(tileChoice, upperRight, Quaternion.identity);
        SampleSpectralBand scriptRed = red.GetComponent<SampleSpectralBand>();
        scriptRed.LoadSprite("AnnualCrop_1_Red");

        Vector3 right = position;
        right.x++;
        GameObject swir = Instantiate(tileChoice, right, Quaternion.identity);
        SampleSpectralBand scriptSWIR = swir.GetComponent<SampleSpectralBand>();
        scriptSWIR.LoadSprite("AnnualCrop_1_SWIR");

        Vector3 downRight = position;
        downRight.y--;
        downRight.x++;
        GameObject redEdge = Instantiate(tileChoice, downRight, Quaternion.identity);
        SampleSpectralBand scriptRedEdge = redEdge.GetComponent<SampleSpectralBand>();
        scriptRedEdge.LoadSprite("AnnualCrop_1_RedEdge");
    }

    private void DrawBandRegion()
    {
        // Game Object - Prefab
        // positions pre-defined - a tiny label for each
        // rectangle with matching color
        // Connections towards the end of the room foreach rectangle
        // Label
        // Message - display on Stay
        // void MatchSpectralBand() -> place the different bands of the same Sample aligned (same position)
    }

    private void GameOver()
    {
        // TODO: Update OverviewBoardManager
        GameManager.instance.StartOverviewScene();
    }
}
