using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleBox : MonoBehaviour
{
    public event Action<string, Vector3> OnBreak;
    public event Action OnTryGrabBlocked;
    public event Action OnGrab;

    public event Action OnWrongDrop;

    bool broken = false;
    bool blocked = false;
    bool canDrop = true;
    public string type;
    private BoxCollider2D boxCollider;

    public void FitInContainer()
    {
        boxCollider.enabled = false;
        UIHandler.Instance.Hide();
    }

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        type = gameObject.name.Substring(0, gameObject.name.LastIndexOf("_"));
    }

    public void Block()
    {
        blocked = true;
    }

    public void Release()
    {
        blocked = false;
    }

    public bool CanDropOnFloor()
    {
        if (!canDrop)
        {
            OnWrongDrop?.Invoke();
        }
        else
        {
            UIHandler.Instance.Hide();
        }
        return canDrop;
    }

    public void SetCanDrop(bool drop)
    {
        canDrop = drop;
    }

    public bool IsBlocked()
    {
        if (blocked)
        {
            OnTryGrabBlocked?.Invoke();
        }
        else
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            UIHandler.Instance.SetSample(spriteRenderer.sprite);
            UIHandler.Instance.Show();
            OnGrab?.Invoke();
        }
        return blocked;
    }

    public void BreakMultiband()
    {
        if (!broken)
        {
            broken = true;
            OnBreak?.Invoke(type, transform.position);
        }
    }

    public void Reset()
    {
        Destroy(gameObject);
    }

}
