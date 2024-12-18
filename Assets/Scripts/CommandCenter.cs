using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandCenter : MonoBehaviour
{
    public Action OnActivated;

    public void Activate()
    {
        Debug.Log("Activate Command Center");
        OnActivated?.Invoke();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
