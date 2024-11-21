using System;
using UnityEngine;

public class SelectorSwitch : MonoBehaviour
{
    public event Action<string> OnSwitch;

    private bool active = false;
    bool emptyInput = true;
    private string type = "";
    private Animator animator;

    private Color workingStartColor;
    private Color workingEndColor;
    private Color wrongColor = Color.red;
    private Color inactiveColor = Color.gray;
    LineRenderer lineRenderer;

    string lineState = "inactive"; // inactive, wrong, correct

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        Transform line = transform.Find("OutputLine");
        if (line == null)
        {
            Debug.LogError("Failed to retrieve Line");
        }
        lineRenderer = line.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("Failed to retrieve LineRenderer");
        }

        DrawOutputConnection();
        UpdateState("inactive");
    }

    public bool IsActive()
    {
        return active;
    }

    private void DrawOutputConnection()
    {
        Transform line = transform.Find("OutputLine");
        if (line == null)
        {
            Debug.LogError("Failed to retrieve Line");
        }
        lineRenderer = line.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("Failed to retrieve LineRenderer");
        }

        workingStartColor = lineRenderer.startColor;
        workingEndColor = lineRenderer.endColor;

        Vector3 startPoint = new(0f, -0.1f, 0f);
        Vector3 endPoint = new(5f, -0.1f, 0f);
        // Debug.Log("Draw connection from " + startPoint + " to " + endPoint);
        Connection conn = new(startPoint, endPoint, lineRenderer);
        conn.DrawStraightLine();
    }

    public void SetType(string newType)
    {
        type = newType;
    }

    public void Switch()
    {
        if (emptyInput)
        {
            UIHandler.Instance.DisplayMessage("This switch only works after you feed the container with the corresponding spectral band");
            return;
        }
        active = !active;
        animator.SetTrigger("switch");
        OnSwitch?.Invoke(type);
    }

    public void SetHasInput()
    {
        emptyInput = false;
    }

    public void TurnOff()
    {
        if (active)
        {
            animator.SetTrigger("reset");
        }
        active = false;
    }

    public void UpdateState(string newLineState)
    {
        Debug.Log("Update state: " + newLineState);
        lineState = newLineState;
        switch (lineState)
        {
            case "correct":
                lineRenderer.startColor = workingStartColor;
                lineRenderer.endColor = workingEndColor;
                break;
            case "wrong":
                lineRenderer.startColor = wrongColor;
                lineRenderer.endColor = wrongColor;
                break;
            case "inactive":
            default:
                lineRenderer.startColor = inactiveColor;
                lineRenderer.endColor = inactiveColor;
                break;
        }
    }

    public void Reset()
    {
        TurnOff();
        emptyInput = true;
        UpdateState("inactive");
    }
}
