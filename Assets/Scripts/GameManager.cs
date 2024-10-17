using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // GameManager is a Singleton
    public static GameManager instance = null;
    public BoardManager boardScript;

    public int playerCoinPoints = 0;

    // TODO: have all the levels possibilities and have a state machine that defines the transition from one to another
    private int level = 1;

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
        boardScript.SetupScene(level);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GameOver()
    {
        enabled = false;
    }
}
