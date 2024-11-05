using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CNNLayer : MonoBehaviour
{
    public string type;
    private TextMeshPro label;
    public bool isEndOfRow = false;

    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        label = gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
        label.text = type;
    }

    public void DrawConnection()
    {
        // Debug.Log("cnn layer position is " + transform.position);
        // Debug.Log("cnn layer local position is " + transform.localPosition);
        if (type.Equals("Output"))
        {
            DrawConnection(new(0, -1f, 0), new(3.5f, 2f, 0));
            return;
        }

        if (isEndOfRow)
        {
            DrawLongConnection(new(0, -1f, 0), new(-10f, -3f, 0));
        }
        else
        {
            DrawConnection(new(0, -1f, 0), new(1.5f, -0.5f, 0));
        }
    }

    public void DrawConnection(Vector3 startPoint, Vector3 endPoint)
    {
        Transform line = transform.Find("Line");
        if (line == null)
        {
            Debug.LogError("Failed to retrieve Line");
        }
        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("Failed to retrieve LineRenderer");
        }
        Debug.Log("Draw connection from " + startPoint + " to " + endPoint);
        Connection conn = new(startPoint, endPoint, lineRenderer);
        conn.DrawLine();
    }

    public void DrawLongConnection(Vector3 startPoint, Vector3 endPoint)
    {
        Transform line = transform.Find("Line");
        if (line == null)
        {
            Debug.LogError("Failed to retrieve Line");
        }
        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("Failed to retrieve LineRenderer");
        }
        Debug.Log("Draw connection from " + startPoint + " to " + endPoint);
        Connection conn = new(startPoint, endPoint, lineRenderer);
        conn.DrawLongLine();
    }
}
