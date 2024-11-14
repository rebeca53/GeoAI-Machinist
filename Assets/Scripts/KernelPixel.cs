using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class KernelPixel : MonoBehaviour
{
    HashSet<Collider2D> currentCollisions = new HashSet<Collider2D>();
    private MatrixPixel currentCollision;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
            if (currentCollision)
            {
                currentCollision.Unhighlight();
            }
            currentCollision = other.transform.GetComponent<MatrixPixel>();
            currentCollision.Highlight();
        }
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
            MatrixPixel inputPixel = other.transform.GetComponent<MatrixPixel>();
            inputPixel.Unhighlight();
        }
    }
}
