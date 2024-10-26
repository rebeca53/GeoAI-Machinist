using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayerCharacter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("On trigger stay 2d " + other.tag);
        if (other.CompareTag("Player"))
        {
            UIHandler.Instance.DisplayIntroduction();
        }
    }

}
