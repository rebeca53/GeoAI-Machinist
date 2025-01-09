using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLabelingMiniGameManager : BaseBoard
{
    public GameObject invisibleWall;
    public GameObject coinTile;
    public GameObject[] sampleTiles;
    public GameObject containerTile;

    public TimedDialogueBalloon timedDialogueBalloon; // Thought balloon
    public DialogueBalloon NPCDialogueBalloon;

    public HintBalloon hintBalloon;
    public CameraZoom cameraZoom;

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

        //Reset our list of gridpositions.
        InitialiseList();

        Player.Spawn(this, new Vector2Int(2, 1));
        NPC.Spawn(this, new Vector2Int(1, 1));

        // Instiate the ten Container tiles
        LayoutContainerFixed();

        //Instantiate a random number of coind tiles based on minimum and maximum, at randomized positions.
        // LayoutObjectAtRandom(coinTile, coinCount);

        // //Instantiate the ten Sample tiles
        LayoutSampleAtRandom();

        // UIHandler.Instance.RegisterContainers();
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
        // exitScript.OnExitWithoutCoins += DisplayNoCoinsMessage;
        exitScript.OnExitWithoutLabels += DisplayNoLabelsMessage;
        exitScript.OnUnlockExit += HintExit;
    }

    //Clears our list gridPositions and prepares it to generate a new board.
    void InitialiseList()
    {
        //Clear our list gridPositions.
        gridPositions.Clear();

        //Loop through x axis (columns).
        for (int x = 1; x < Width; x++)
        {
            //Within each column, loop through y axis (rows).
            for (int y = 2; y < Height - 1; y++)
            {
                // Clear area behind the Player and the NPC
                if (x < 4 && y < 3)
                {
                    continue;
                }
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
            Vector3 position = new Vector3();

            if (tileChoice.name.Contains("Residential"))
            {
                position = this.CellToWorld(new Vector2Int(3, 1));
                gridPositions.Remove(position);
            }
            else
            {
                //Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
                position = RandomPosition();
            }

            //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
            Instantiate(tileChoice, position, Quaternion.identity);
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

        //Instantiate objects until the randomly chosen limit objectCount is reached
        for (int i = 0; i < containerCount; i++)
        {
            Container container = tileChoice.GetComponent<Container>();
            container.type = classNames[i];

            // Two rows
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
            // Workaround to avoid placing SampleBox over Container
            gridPositions.Remove(new(xPosition + 0.5f, yPosition, 0f));
            gridPositions.Remove(new(xPosition - 0.5f, yPosition, 0f));

            //Instantiate tileChoice at the position
            GameObject instance = Instantiate(tileChoice, position, Quaternion.identity);
            Container containerScript = instance.GetComponent<Container>();
            containerScript.OnMatchDisplay += DisplayClassInfo;
            containerScript.OnHover += DisplayClassInfo;
            containerScript.OnUnhover += HideClassInfo;
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

    protected override void GameOver()
    {
        GameManager.instance.StartOverviewScene();
    }

    void DisplayClassInfo(string type)
    {
        var myDict = new Dictionary<string, string>
        {
            { "AnnualCrop", "Annual crops are those that do not last more than two growing seasons. These include crops like wheat, rice, and potatoes." },
            { "Forest", "Forests are lands with tree crown cover. The trees should be able to reach a minimum height of 5 meters at maturity in situ." },
            { "HerbaceousVegetation", "Herbaceous vegetation consists of non-woody plants, such as natural grasslands, bushes, and herbaceous plants." },
            { "Highway", "Highways are major roads for fast transit." },
            { "Industrial", "Industrial areas are occupied by buildings for industrial, commercial, or transport-related uses." },
            { "Pasture", "Pastures are grasslands or other vegetative areas for the grazing of livestock, such as cattle, sheep, and goats." },
            { "PermanentCrop", "Permanent crops (e.g., fruit trees and vines) last for more than two growing seasons." },
            { "Residential", "Residential areas are built-up areas for housing and associated lands like gardens, and parks." },
            { "River", "Rivers are natural inland water courses that empty into another body of water or the sea." },
            { "SeaLake", "Sea and lakes are large waterbodies, with seas being saline water connected to oceans and lakes being often enclosed by land." }
        };

        timedDialogueBalloon.SetSpeaker(Player.gameObject);
        timedDialogueBalloon.SetMessage(myDict[type]);
        timedDialogueBalloon.PlaceUpperLeft();
        timedDialogueBalloon.Show();
        ZoomIn();
    }

    void ZoomIn()
    {
        cameraZoom.ChangeZoomSmooth(1.2f);
    }

    void HideClassInfo(string type)
    {
        timedDialogueBalloon.Hide();
    }

    void DisplayNoCoinsMessage()
    {
        NPCDialogueBalloon.SetSpeaker(NPC.gameObject);
        NPCDialogueBalloon.SetMessage("Oops! I see you didn't collect all Coins. They will be useful in the future!");
        NPCDialogueBalloon.PlaceUpperRight();
        NPCDialogueBalloon.Show();
    }

    void DisplayNoLabelsMessage()
    {
        NPCDialogueBalloon.SetSpeaker(NPC.gameObject);
        NPCDialogueBalloon.SetMessage("Don't forget your mission, GeoAI Machinist: labeling the data is essential so the Big Machine can learn");
        NPCDialogueBalloon.PlaceUpperLeft();
        NPCDialogueBalloon.Show();
        NPCDialogueBalloon.OnDone += NPCDialogueBalloon.Hide;
    }

    void DisplayPhaseOverMessage()
    {
        NPCDialogueBalloon.SetSpeaker(NPC.gameObject);
        NPCDialogueBalloon.SetMessage("Perfect, you labeled samples with which the Big Machine will learn.");
        NPCDialogueBalloon.PlaceUpperLeft();
        NPCDialogueBalloon.Show();
        NPCDialogueBalloon.DisableKey();
    }

    void HintExit()
    {
        DisplayPhaseOverMessage();
        NPC.OnHover += DisplayPhaseOverMessage;
        StartCoroutine(HintExitCoroutine());
    }

    IEnumerator HintExitCoroutine()
    {
        cameraZoom.ChangeZoomTarget(exitScript.gameObject);

        hintBalloon.SetSpaceKey();
        hintBalloon.SetTarget(exitScript.gameObject);
        hintBalloon.PlaceOver();
        hintBalloon.SetWaitKey(false);
        hintBalloon.Show();

        yield return new WaitForSeconds(2);

        cameraZoom.ChangeZoomTarget(Player.gameObject);
    }
}
