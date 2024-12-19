using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Exit : MonoBehaviour
{
    public Action OnExitWithoutCoins;
    public Action OnExitWithoutLabels;
    public Action OnUnlockExit;

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
        spriteRenderer.color = Color.green;
        OnUnlockExit?.Invoke();
    }

    public void AttemptExit()
    {
        StartCoroutine(DoExit());
    }

    IEnumerator DoExit()
    {
        if (!HasCollectedAllCoins())
        {
            OnExitWithoutCoins?.Invoke();
        }
        else if (IsPhaseOver())
        {
            PlaySound(exitClip);

            yield return new WaitForSeconds(changeLevelDelay); // wait time

            GameManager.instance.StartOverviewScene();
        }
        else
        {
            OnExitWithoutLabels?.Invoke();
        }

    }

    private bool IsPhaseOver()
    {
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
