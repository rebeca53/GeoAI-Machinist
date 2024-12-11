using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogitNode : MonoBehaviour
{
    public event Action OnHover;
    public event Action OnUnhover;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Debug.Log("on trigger enter activation view");
        OnHover?.Invoke();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        OnHover?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        OnUnhover?.Invoke();
    }
}
