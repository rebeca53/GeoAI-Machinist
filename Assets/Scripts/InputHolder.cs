using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHolder : MonoBehaviour
{
    public Action OnAddedObject;
    private float verticalOffset = 0.7f;
    private float horizontalOffset = 0.013f;
    int id = -1;

    public void Init(int newId)
    {
        id = newId;
        gameObject.name = "KernelHolder" + id;
    }

    public void Spawn(OverviewBoardManager boardManager, Vector2Int cell)
    {
        transform.position = boardManager.CellToWorld(cell);
    }

    // TODO: Add audio effect
    public void FeedInputSample(SampleBox sampleBox)
    {
        // Update sample box chracteactis
        // Change scale
        sampleBox.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        // change box parent
        sampleBox.gameObject.transform.parent = gameObject.transform;
        sampleBox.gameObject.transform.position = new Vector3(gameObject.transform.position.x + horizontalOffset, gameObject.transform.position.y + verticalOffset);
    }

    public bool CanAdd(GameObject gameObject)
    {
        if (gameObject.CompareTag("Kernel"))
        {
            KernelMatrix kernel = gameObject.GetComponent<KernelMatrix>();
            return CanAddKernel(kernel);
        }
        else if (gameObject.CompareTag("ActivationBox"))
        {
            return CanAddActivationBox();
        }

        return false;
    }

    public bool CanAddKernel(KernelMatrix kernel)
    {
        return kernel.GetId() == id;
    }

    public bool CanAddActivationBox()
    {
        // TODO
        return true;
    }

    public void AddInputObject(GameObject inputObject)
    {
        // Change scale
        inputObject.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        // change box parent
        inputObject.transform.parent = gameObject.transform;
        inputObject.transform.position = new Vector3(gameObject.transform.position.x + horizontalOffset, gameObject.transform.position.y + verticalOffset);

        OnAddedObject?.Invoke();
    }

    public void RemoveInputSample(SampleBox sampleBox)
    {
        // TODO
    }

    public void DrawConnection()
    {
        DrawConnection(new(0, -1f, 0), new(1.5f, -0.5f, 0));
    }

    public void DrawConnection(Vector3 startPoint, Vector3 endPoint)
    {
        Transform line = transform.Find("Line");
        if (line == null)
        {
            Debug.LogError("Failed to retrieve Line");
            return;
        }
        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("Failed to retrieve LineRenderer");
            return;
        }
        // Debug.Log("Draw connection from " + startPoint + " to " + endPoint);
        Connection conn = new(startPoint, endPoint, lineRenderer);
        conn.DrawLine();
    }
}
