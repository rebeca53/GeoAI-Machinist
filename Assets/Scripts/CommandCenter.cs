using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandCenter : MonoBehaviour
{
    public Action OnActivated;

    public void Activate()
    {
        OnActivated?.Invoke();
    }
}
