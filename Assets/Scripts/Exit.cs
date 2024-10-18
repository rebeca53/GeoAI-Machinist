using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
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
        isUnlocked = true;
        PlaySound(unlockClip);
        spriteRenderer.sprite = unlockedDoor;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();
        if (controller != null)
        {
            StartCoroutine(AttemptExit());
        }
    }

    IEnumerator AttemptExit()
    {
        Debug.Log("Exit is unlocked: " + isUnlocked);
        if (isUnlocked)
        {
            PlaySound(exitClip);

            yield return new WaitForSeconds(changeLevelDelay); // wait time

            //Load the last scene loaded, in this case Main, the only scene in the game.
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

}
