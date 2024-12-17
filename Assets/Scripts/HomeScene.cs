using UnityEngine;

public class HomeScene : MonoBehaviour
{
    public void LoadGame()
    {
        // 1 - Sample Scene
        // 2 - Overview Scene
        // 7 - Introduction CutsCene
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadGame();
        }
    }
}
