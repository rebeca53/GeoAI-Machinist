using UnityEngine;

public class HomeScene : MonoBehaviour
{
    public void LoadGame()
    {
        // 1 - Sample Scene
        // 7 - Introduction CutsCene
        UnityEngine.SceneManagement.SceneManager.LoadScene(7);
    }
}
