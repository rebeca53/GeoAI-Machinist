using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionTrigger : MonoBehaviour
{
    private bool sceneLoaded = false;
    // public int SceneIndex;
    public string nextScene;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (nextScene == "Overview")
        {
            GameManager.instance.StartOverviewScene();
        }
        // // Debug.Log("Something entered the Transition trigger: " + other.tag);
        // if (sceneLoaded)
        // {
        //     sceneLoaded = false;
        //     SceneManager.UnloadSceneAsync(SceneIndex);
        // }
        // else
        // {
        //     sceneLoaded = true;
        //     SceneManager.LoadSceneAsync(SceneIndex, LoadSceneMode.Additive);
        // }
    }
}
