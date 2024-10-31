using UnityEngine;

public class GameManager : MonoBehaviour
{
    // GameManager is a Singleton
    public static GameManager instance = null;
    public BoardManager boardScript;

    public int playerCoinPoints = 0;

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
        boardScript.SetupScene();
    }

    public void GoNextLevel()
    {
        switch (currentLevel)
        {
            case 1:
                UnityEngine.SceneManagement.SceneManager.LoadScene(1);
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(2, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(3, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                break;
            default:
                break;
        }
        currentLevel++;
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
