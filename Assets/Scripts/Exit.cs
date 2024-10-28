using System.Collections;
using System.Linq;
using UnityEngine;

public class Exit : MonoBehaviour
{
    //Delay time in seconds to restart level.
    public float changeLevelDelay = 2f;
    public int TotalContainers = 10;
    private static int amountMatchedContainers;

    public Sprite unlockedDoor;
    private static bool isUnlocked = false;

    private static SpriteRenderer spriteRenderer;

    // Audio
    private static AudioSource audioSource;
    public AudioClip unlockClip;
    public AudioClip exitClip;


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("Sprite Renderer not assigned");
        }
        amountMatchedContainers = 0;

        audioSource = GetComponent<AudioSource>();
    }

    public void RegisterContainers(GameObject[] containersToObserve)
    {
        foreach (GameObject container in containersToObserve)
        {
            Container containerScript = container.GetComponent<Container>();
            containerScript.OnMatch += HandleMatchedContainer;
        }
        TotalContainers = containersToObserve.Length;
    }

    private void HandleMatchedContainer()
    {
        amountMatchedContainers++;
        if (amountMatchedContainers == TotalContainers)
        {
            UnlockExit();
        }
    }

    private void UnlockExit()
    {
        Debug.Log("UnlockExit");

        UIHandler.Instance.DisplayDialogue("Wow! You really know everything about Land Cover and Land Use as expected from the GeoAI Machinist ;)");

        isUnlocked = true;
        PlaySound(unlockClip);
        spriteRenderer.sprite = unlockedDoor;
    }

    public void AttemptExit()
    {
        StartCoroutine(DoAttemptExit());
    }

    IEnumerator DoAttemptExit()
    {
        if (!HasCollectedAllCoins())
        {
            UIHandler.Instance.DisplayDialogue("Oops! I see you didn't collect all Coins. They will be useful in the future!", 10);
        }
        else if (IsPhaseOver())
        {
            PlaySound(exitClip);

            yield return new WaitForSeconds(changeLevelDelay); // wait time

            //Load the last scene loaded, in this case Main, the only scene in the game.
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
        else
        {
            UIHandler.Instance.DisplayDialogue("Don't forget your mission, GeoAI Machinist: labeling the data is essential so the Big Machine can learn.");
        }

    }

    private bool IsPhaseOver()
    {
        Debug.Log("Exit is unlocked: " + isUnlocked);
        Debug.Log("has collected all coins: " + HasCollectedAllCoins());
        return HasCollectedAllCoins() && isUnlocked;
    }

    private bool HasCollectedAllCoins()
    {
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        return coins.Count() == 0;
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

}
