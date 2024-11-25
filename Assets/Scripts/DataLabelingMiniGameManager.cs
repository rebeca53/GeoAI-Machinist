using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLabelingMiniGameManager : BaseBoard
{
    public GameObject invisibleWall;
    public GameObject coinTile;
    public GameObject[] sampleTiles;
    public GameObject containerTile;


    public DialogueBalloon dialogueBalloon;
    public HintBalloon hintBalloon;
    List<(string, string)> screenplay = new List<(string, string)>();
    int currentLineIndex = 0;


    public const int coinCount = 4;
    static List<string> classNames = new List<string> { "AnnualCrop", "Forest", "HerbaceousVegetation", "Highway", "Industrial", "Pasture", "PermanentCrop", "Residential", "River", "SeaLake" };
    public int containerCount = classNames.Count;
    public int sampleCount = classNames.Count;
    private List<Vector3> gridPositions = new List<Vector3>();
    Exit exitScript;

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

        //Reset our list of gridpositions.
        InitialiseList();

        // Instiate the ten Container tiles
        LayoutContainerFixed();

        //Instantiate a random number of coind tiles based on minimum and maximum, at randomized positions.
        LayoutObjectAtRandom(coinTile, coinCount);

        // //Instantiate the ten Sample tiles
        LayoutSampleAtRandom();

        UIHandler.Instance.RegisterContainers();
    }

    protected new bool IsExit(int x, int y)
    {
        bool isTop = y == Height - 1;
        bool isExitRegion = x == exitXPosition;
        return isTop && isExitRegion;
    }

    protected new void DrawExit(int x, int y)
    {
        GameObject instance = Instantiate(exitObject, new Vector3(exitXPosition + 0.5f, Height - 0.5f, 0f), Quaternion.identity);
        Instantiate(invisibleWall, new Vector3(exitXPosition + 0.5f, Height + 0.5f, 0f), Quaternion.identity);

        exitScript = instance.GetComponent<Exit>();
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

    void LayoutSampleAtRandom()
    {
        //Instantiate objects until the randomly chosen limit objectCount is reached
        for (int i = 0; i < sampleTiles.Length; i++)
        {
            GameObject tileChoice = sampleTiles[i];

            //Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
            Vector3 randomPosition = RandomPosition();

            //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

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

    void LayoutContainerFixed()
    {

        GameObject tileChoice = containerTile;

        float horizontalOffset = 1.5f;
        float horizontalGap = 1.5f;
        float xPosition = 1f;
        float yPosition = 1f;
        // Two rows

        //Instantiate objects until the randomly chosen limit objectCount is reached
        for (int i = 0; i < containerCount; i++)
        {
            Container container = tileChoice.GetComponent<Container>();
            container.type = classNames[i];

            if (i < containerCount / 2)
            {
                yPosition = 3f;
                xPosition = horizontalOffset + i * horizontalGap;
            }
            else
            {
                yPosition = 5f;
                xPosition = horizontalOffset + (i - containerCount / 2) * horizontalGap;
            }

            Vector3 position = new(xPosition, yPosition, 0f);
            gridPositions.Remove(position);

            //Instantiate tileChoice at the position
            Instantiate(tileChoice, position, Quaternion.identity);
        }

        // Registers to containers to check when to unlock Exit
        GameObject[] containers = GameObject.FindGameObjectsWithTag("Container");
        exitScript.RegisterContainers(containers);
    }

    void LayoutObjectAtRandom(GameObject tileChoice, int objectCount)
    {
        //Choose a random number of objects to instantiate within the minimum and maximum limits
        //Instantiate objects until the randomly chosen limit objectCount is reached
        for (int i = 0; i < objectCount; i++)
        {
            //Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
            Vector3 randomPosition = RandomPosition();

            //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void GameOver()
    {
        GameManager.instance.StartOverviewScene();
    }
}