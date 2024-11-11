using System.Collections.Generic;
using UnityEngine;
//Tells Random to use the Unity Engine random number generator.
using Random = UnityEngine.Random;

// TODO: Have an Abstract class for all Board Manager
public class InputMiniGameManager : BaseBoard
{
    public GameObject[] sampleTiles;
    public GameObject spectralBandTile;
    public GameObject spectralBandContainerTile;
    private List<Vector3> samplePositions = new List<Vector3>();
    private int regionsDelimiter = 10;
    private int padding = 2;

    private List<string> bandTypes = new List<string> { "red", "green", "blue", "swir", "redEdge" };
    private int countFull = 0;
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

        InitialiseList();
        LayoutSample();
        DrawBandContainers();
    }

    //Clears our list gridPositions and prepares it to generate a new board.
    void InitialiseList()
    {
        //Clear our list gridPositions.
        samplePositions.Clear();

        //Loop through x axis (columns).
        for (int x = 1; x < regionsDelimiter; x++)
        {
            //Within each column, loop through y axis (rows).
            for (int y = padding; y < Height - padding; y++)
            {
                //At each index add a new Vector3 to our list with the x and y coordinates of that position.
                samplePositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    // TODO: Move to Abstract class
    //RandomPosition returns a random position from our list gridPositions.
    Vector3 RandomPosition()
    {
        //Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
        int randomIndex = Random.Range(0, samplePositions.Count);
        //Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
        Vector3 randomPosition = samplePositions[randomIndex];

        //Remove the entry at randomIndex from the list so that it can't be re-used.
        samplePositions.RemoveAt(randomIndex);

        //Return the randomly selected Vector3 position.
        return randomPosition;
    }

    private void LayoutSample()
    {
        foreach (GameObject tileChoice in sampleTiles)
        {
            //Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
            Vector3 randomPosition = RandomPosition();

            //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
            GameObject instance = Instantiate(tileChoice, randomPosition, Quaternion.identity);
            SampleBox sampleBox = instance.GetComponent<SampleBox>();
            sampleBox.OnBreak += LayoutGrayscaleBands;
        }
        // GameObject tileChoice = sampleTiles[0];
    }

    private void LayoutGrayscaleBands(string sampleBox, Vector3 position)
    {
        Debug.Log("Break the box " + sampleBox);
        GameObject tileChoice = spectralBandTile;

        Vector3 upper = position;
        upper.y++;
        //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
        GameObject blue = Instantiate(tileChoice, upper, Quaternion.identity);
        SampleSpectralBand scriptBlue = blue.GetComponent<SampleSpectralBand>();
        scriptBlue.LoadSprite(sampleBox + "_Blue");

        Vector3 down = position;
        down.y--;
        GameObject green = Instantiate(tileChoice, down, Quaternion.identity);
        SampleSpectralBand script = green.GetComponent<SampleSpectralBand>();
        script.LoadSprite(sampleBox + "_Green");

        Vector3 upperRight = position;
        upperRight.y++;
        upperRight.x++;
        GameObject red = Instantiate(tileChoice, upperRight, Quaternion.identity);
        SampleSpectralBand scriptRed = red.GetComponent<SampleSpectralBand>();
        scriptRed.LoadSprite(sampleBox + "_Red");

        Vector3 right = position;
        right.x++;
        GameObject swir = Instantiate(tileChoice, right, Quaternion.identity);
        SampleSpectralBand scriptSWIR = swir.GetComponent<SampleSpectralBand>();
        scriptSWIR.LoadSprite(sampleBox + "_SWIR");

        Vector3 downRight = position;
        downRight.y--;
        downRight.x++;
        GameObject redEdge = Instantiate(tileChoice, downRight, Quaternion.identity);
        SampleSpectralBand scriptRedEdge = redEdge.GetComponent<SampleSpectralBand>();
        scriptRedEdge.LoadSprite(sampleBox + "_RedEdge");
    }

    private void DrawBandContainers()
    {
        float verticalGap = 2f;
        float verticalOffset = 2f;
        float xPosition = 10f;

        for (int i = 0; i < 5; i++)
        {
            float yPosition = verticalOffset + i * verticalGap;
            Vector3 position = new(xPosition, yPosition, 0f);
            GameObject instance = Instantiate(spectralBandContainerTile, position, Quaternion.identity);
            SpectralBandContainer spectralBandContainer = instance.GetComponent<SpectralBandContainer>();
            spectralBandContainer.SetType(bandTypes[i]);
            spectralBandContainer.DrawConnections(inputPosition: new(-9.9f, (Height / 2) - yPosition, 0f));
            spectralBandContainer.OnFull += CheckWinCondition;
        }
    }

    private void CheckWinCondition(string type)
    {
        countFull++;
        if (countFull == bandTypes.Count)
        {
            GameOver();
        }
    }

    private void ExitWithoutSolving()
    {
        GameManager.instance.StartOverviewScene();
    }

    protected override void GameOver()
    {
        GameManager.instance.solvedMinigames["Input"] = true;
        GameManager.instance.StartOverviewScene();
    }
}
