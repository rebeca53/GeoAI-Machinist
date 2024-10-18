using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int pointsPerCoin = 1;

    public AudioClip collectedClip;

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.PlaySound(collectedClip);
            controller.ChangeMoney(pointsPerCoin);
            Destroy(gameObject);
        }
    }
}
