using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionTrigger : MonoBehaviour
{
    // public int SceneIndex;
    public string nextScene;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (nextScene == "Overview")
        {
            GameManager.instance.StartOverviewScene();
        }
        else if (nextScene == "SampleScene")
        {
            SceneManager.LoadScene(1);
        }
    }
}
