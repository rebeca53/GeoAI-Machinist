using System.Collections.Generic;
using UnityEngine;
//Tells Random to use the Unity Engine random number generator.
using Random = UnityEngine.Random;
// TODO: Have an Abstract class for all Board Manager
public class BoardManager : MonoBehaviour
{
    // size of the game board
    public int columns = 8;
    public int rows = 8;
    public const int coinCount = 4;
    static List<string> classNames = new List<string> { "AnnualCrop", "Forest", "HerbaceousVegetation", "Highway", "Industrial", "Pasture", "PermanentCrop", "Residential", "River", "SeaLake" };
    public int containerCount = classNames.Count;
    public int sampleCount = classNames.Count;
    public GameObject exit;
    public GameObject invisibleWall;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject coinTile;
    public GameObject[] sampleTiles;
    public GameObject containerTile;
    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();
    public GameObject NPC;

    //Clears our list gridPositions and prepares it to generate a new board.
    void InitialiseList()
    {
        //Clear our list gridPositions.
        gridPositions.Clear();

        //Loop through x axis (columns).
        for (int x = 1; x < columns - 1; x++)
        {
            //Within each column, loop through y axis (rows).
            for (int y = 1; y < rows - 1; y++)
            {
                //At each index add a new Vector3 to our list with the x and y coordinates of that position.
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        //Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
        for (int x = -1; x < columns + 1; x++)
        {
            //Loop along y axis, starting from -1 to place floor or outerwall tiles.
            for (int y = -1; y < rows + 1; y++)
            {
                //Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                //Check if we current position is at board edge, if so choose specific wall prefab from our array of wall tiles.
                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    SetupWall(boardHolder, x, y);
                }

                //Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                //Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    void SetupWall(Transform boardHolder, int x, int y)
    {
        // Skip Exit position
        if (x == columns - 1 && y == rows)
        {
            return;
        }

        GameObject toInstantiate = null;
        if (x == -1 && y == -1)
        {
            toInstantiate = wallTiles[1]; //bottom left
        }
        else if (x == -1 && y == rows)
        {
            toInstantiate = wallTiles[6]; //top left

        }
        else if (x == columns && y == -1)
        {
            toInstantiate = wallTiles[2]; // bottom right
        }
        else if (x == columns && y == rows)
        {
            toInstantiate = wallTiles[7]; // top right
        }
        else if (x == -1)
        {
            toInstantiate = wallTiles[3];
        }
        else if (x == columns)
        {
            toInstantiate = wallTiles[4];
        }
        else if (y == -1)
        {
            toInstantiate = wallTiles[0];
        }
        else if (y == rows)
        {
            toInstantiate = wallTiles[5];
        }

        if (toInstantiate == null)
        {
            return;
        }
        //Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
        GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

        //Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
        instance.transform.SetParent(boardHolder);
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

    //LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        //Choose a random number of objects to instantiate within the minimum and maximum limits
        int objectCount = Random.Range(minimum, maximum + 1);

        //Instantiate objects until the randomly chosen limit objectCount is reached
        for (int i = 0; i < objectCount; i++)
        {
            //Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
            Vector3 randomPosition = RandomPosition();

            //Choose a random tile from tileArray and assign it to tileChoice
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

            //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    //LayoutObjectAtRandom accepts a game object to choose from along with a minimum and maximum range for the number of objects to create.
    void LayoutObjectAtRandom(GameObject tileChoice, int minimum, int maximum)
    {
        //Choose a random number of objects to instantiate within the minimum and maximum limits
        int objectCount = Random.Range(minimum, maximum + 1);

        //Instantiate objects until the randomly chosen limit objectCount is reached
        for (int i = 0; i < objectCount; i++)
        {
            //Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
            Vector3 randomPosition = RandomPosition();

            //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
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

    void LayoutContainerFixed()
    {
        int fixedIndex = 1;

        GameObject tileChoice = containerTile;

        //Instantiate objects until the randomly chosen limit objectCount is reached
        for (int i = 0; i < containerCount; i++)
        {
            Container container = tileChoice.GetComponent<Container>();
            container.type = classNames[i];

            //Declare a variable of type Vector3 called fixedPosition, set it's value to the entry at fixedIndex from our List gridPositions.
            Vector3 fixedPosition = gridPositions[fixedIndex];

            //Remove the entry at fixedIndex from the list so that it can't be re-used.
            gridPositions.RemoveAt(fixedIndex);

            //Instantiate tileChoice at the position
            Instantiate(tileChoice, fixedPosition, Quaternion.identity);


            fixedIndex++;
        }
    }

    //SetupScene initializes our level and calls the previous functions to lay out the game board
    public void SetupScene()
    {
        //Creates the outer walls and floor.
        BoardSetup();

        //Reset our list of gridpositions.
        InitialiseList();

        // Instiate the ten Container tiles
        LayoutContainerFixed();

        //Instantiate a random number of coind tiles based on minimum and maximum, at randomized positions.
        LayoutObjectAtRandom(coinTile, coinCount);

        //Instantiate the ten Sample tiles
        LayoutSampleAtRandom();

        // //Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
        // LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);

        //Instantiate the exit tile in the upper right hand corner of our game board
        LayoutExitFixed();

        UIHandler.Instance.RegisterContainers();

        NPC.GetComponent<NonPlayerCharacter>().DisplayIntroduction();
    }

    private void LayoutExitFixed()
    {
        Instantiate(exit, new Vector3(columns - 1, rows, 0f), Quaternion.identity);
        Instantiate(invisibleWall, new Vector3(columns - 1, rows + 1, 0f), Quaternion.identity);

        Exit exitScript = exit.GetComponent<Exit>();
        GameObject[] containers = GameObject.FindGameObjectsWithTag("Container");
        exitScript.RegisterContainers(containers);
    }

}
