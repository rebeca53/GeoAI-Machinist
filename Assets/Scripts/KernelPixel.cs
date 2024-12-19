using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KernelPixel : MonoBehaviour
{
    public event Action<Vector3> OnHoverPixel;
    public event Action<Vector3> OnExitPixel;
    HashSet<Collider2D> currentCollisions = new HashSet<Collider2D>();
    private InputMatrixPixel currentCollision;

    public Transform GetTransform()
    {
        return transform;
    }
    public void SetKernelCenter()
    {
        tag = "KernelCenter";
        GetComponent<BoxCollider2D>().size = new(0.1f, 0.1f);
    }

    private bool IsKernelCenter()
    {
        return CompareTag("KernelCenter");
    }

    public void SetConvoluting()
    {
        Color newColor = Color.red;
        newColor.a = 0.5f;
        GetComponent<SpriteRenderer>().color = newColor;
    }

    public void SetDefault()
    {
        Color newColor = Color.black;
        newColor.a = 0.7f;
        GetComponent<SpriteRenderer>().color = newColor;
    }

    private bool IsAligned(Vector3 positionA, Vector3 positionB, float delta = 0.002f)
    {
        // Debug.Log("distance is " + (positionA - positionB).sqrMagnitude);
        return (positionA - positionB).sqrMagnitude <= delta;
    }

    private Collider2D PickCollider()
    {
        if (currentCollisions.Count == 1)
        {
            return currentCollisions.First();
        }

        // always return most recent
        return currentCollisions.Last();
    }
}
