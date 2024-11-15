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

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("stay over pixel. tag: " + other.tag);
        // Debug.Log("stay over pixel. tag parent: " + other.transform.parent.tag);
        if (IsKernelCenter() && other.CompareTag("InputPixel"))
        {
            // Debug.Log("kernel center position: " + transform.position + "\ncurrent pixel position: " + other.transform.position);
            // ;
            if (currentCollision)
            {
                currentCollision.Unhighlight();
            }
            currentCollision = other.transform.GetComponent<InputMatrixPixel>();
            currentCollision.Highlight();
            OnHoverPixel?.Invoke(currentCollision.GetPosition());
        }
    }

    private bool IsAligned(Vector3 positionA, Vector3 positionB, float delta = 0.002f)
    {
        Debug.Log("distance is " + (positionA - positionB).sqrMagnitude);
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

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("out of pixel");
        if (IsKernelCenter() && other.CompareTag("InputPixel"))
        {
            // currentCollisions.Remove(other);
            currentCollision = null;
            InputMatrixPixel inputPixel = other.transform.GetComponent<InputMatrixPixel>();
            inputPixel.Unhighlight();
            OnExitPixel?.Invoke(inputPixel.GetPosition());
        }
    }
}
