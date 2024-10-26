using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleBox : MonoBehaviour
{
    public string type;
    private BoxCollider2D boxCollider;

    public void FitInContainer()
    {
        boxCollider.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        type = gameObject.name.Substring(0, gameObject.name.LastIndexOf("_"));
    }

}
