using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleBox : MonoBehaviour
{
    public string type;
    private BoxCollider2D boxCollider;

    public void fitInContainer()
    {
        boxCollider.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        type = gameObject.name.Substring(0, gameObject.name.LastIndexOf("_"));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("BOX On trigger stay 2d " + other.tag);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("BOX on trigger exit 2d " + other.tag);
    }
}
