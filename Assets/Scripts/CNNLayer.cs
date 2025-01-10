using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CNNLayer : MonoBehaviour
{
    public event Action<string> OnHover;
    public event Action<string> OnUnhover;

    public string type;
    private TextMeshPro label;
    public bool isEndOfRow = false;

    private LineRenderer lineRenderer;
    public Sprite statusBad;
    public Sprite statusGood;

    public GameObject wormholeAnimation;


    // Start is called before the first frame update
    void Start()
    {
        label = gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
        label.text = type + "\nLayer";

        SetGameStatus();
    }

    private void SetGameStatus()
    {
        bool solved = GameManager.instance.solvedMinigames[type];
        SpriteRenderer spriteRenderer = transform.Find("StatusIndicator").GetComponent<SpriteRenderer>();

        if (solved)
        {
            spriteRenderer.sprite = statusGood;
            wormholeAnimation.SetActive(false);
        }
        else
        {
            spriteRenderer.sprite = statusBad;
            wormholeAnimation.SetActive(true);
        }
    }

    public void DrawConnection()
    {
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
        // Debug.Log("Draw connection from " + startPoint + " to " + endPoint);
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
        // Debug.Log("Draw connection from " + startPoint + " to " + endPoint);
        Connection conn = new(startPoint, endPoint, lineRenderer);
        conn.DrawLongLine();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Debug.Log(" layer on trigger enter 2d " + other.tag);
        OnHover?.Invoke(type);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Debug.Log(" layer on trigger stay 2d " + other.tag);
        OnHover?.Invoke(type);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Debug.Log(" layer on trigger exit 2d " + other.tag);
        OnUnhover?.Invoke(type);
    }
}
