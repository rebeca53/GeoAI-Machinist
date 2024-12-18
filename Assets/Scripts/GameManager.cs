using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // GameManager is a Singleton
    public static GameManager instance = null;

    public Vector2Int playerPositionOverview = new(1, 1);

    public int playerCoinPoints = 0;
    public Dictionary<string, bool> solvedMinigames = new()
    {
        {"Input", false},
        {"Convolutional 1", false},
        {"Activation 1", false},
        {"Convolutional 2", true},
        {"Activation 2", true},
        {"Pooling 1", true},
        {"Convolutional 3", true},
        {"Activation 3", true},
        {"Convolutional 4", true},
        {"Activation 4", true},
        {"Pooling 2", true},
        {"Output", false}
    };

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

        Debug.Log("Wake Game Manger");
    }

    public void StartDataLabeling()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void StartOverviewScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(3, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(4, UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    public void StartMiniGame(string type)
    {
        switch (type)
        {
            case "Input":
                UnityEngine.SceneManagement.SceneManager.LoadScene(5);
                playerPositionOverview = new Vector2Int(3, 8);
                break;
            case "Convolutional 1":
                UnityEngine.SceneManagement.SceneManager.LoadScene(6);
                playerPositionOverview = new Vector2Int(6, 8);
                break;
            case "Activation 1":
                UnityEngine.SceneManagement.SceneManager.LoadScene(8);
                playerPositionOverview = new Vector2Int(9, 8);
                break;
            case "Output":
                UnityEngine.SceneManagement.SceneManager.LoadScene(9);
                playerPositionOverview = new Vector2Int(12, 2);
                break;
            default:
                break;
        }
    }

    public bool IsSolved(string type)
    {
        return solvedMinigames[type];
    }

    public bool IsGameOver()
    {
        bool allSolved = true;
        foreach (KeyValuePair<string, bool> entry in solvedMinigames)
        {
            if (entry.Value == false)
            {
                allSolved = false;
                break;
            }
        }

        return allSolved;
    }

    public void GameOver()
    {
        enabled = false;
    }
}
