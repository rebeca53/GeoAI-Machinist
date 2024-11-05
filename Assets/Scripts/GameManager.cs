using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // GameManager is a Singleton
    public static GameManager instance = null;
    public BoardManager boardScript;

    public int playerCoinPoints = 0;
    public Dictionary<string, bool> solvedMinigames = new()
    {
        {"Input", false},
        {"Convolutional 1", false},
        {"Activation 1", false},
        {"Convolutional 2", false},
        {"Activation 2", false},
        {"Pooling 1", false},
        {"Convolutional 3", false},
        {"Activation 3", false},
        {"Convolutional 4", false},
        {"Activation 4", false},
        {"Pooling 2", false},
        {"Output", false}
    };

    // TODO: have all the levels possibilities and have a state machine that defines the transition from one to another
    private int currentLevel = 1;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        boardScript = GetComponent<BoardManager>();
        InitGame();
        Debug.Log("Wake Game Manger");
    }

    void InitGame()
    {
        // boardScript.SetupScene();
        // StartOverviewScene();
        StartMiniGame("Input");
    }

    public void StartOverviewScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(2, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(3, UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    public void StartMiniGame(string type)
    {
        switch (type)
        {
            case "Input":
                UnityEngine.SceneManagement.SceneManager.LoadScene(4);
                break;
            default:
                break;
        }
    }

    public void GameOver()
    {
        enabled = false;
    }
}
