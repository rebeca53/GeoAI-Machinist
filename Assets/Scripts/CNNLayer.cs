using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CNNLayer : MonoBehaviour
{
    public string type;
    private TextMeshPro label;

    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        label = gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
        label.text = type;

        lineRenderer = GameObject.Find("Line").GetComponent<LineRenderer>();
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

    class Connection
    {
        public Connection(Vector3 startPoint, Vector3 endPoint, LineRenderer renderer)
        {
            Begin = startPoint;
            End = endPoint;
            lineRenderer = renderer;
        }

        public Vector3 Begin;
        public Vector3 End;
        public LineRenderer lineRenderer;

        // source code: https://gist.github.com/Matthew-J-Spencer/9044a711cddc4340c6d2aa0656a15d2a
        private Vector3 EvaluateSlerpPoint(Vector3 start, Vector3 end, float offset, float ratio)
        {
            Vector3 pivot = (start + end) * 0.5f;
            pivot -= new Vector3(0, -offset);

            Vector3 relativeBegin = start - pivot;
            Vector3 relativeCenter = end - pivot;

            return Vector3.Slerp(relativeBegin, relativeCenter, ratio) + pivot;
        }

        public void DrawLine()
        {
            List<Vector3> line = new List<Vector3>();
            Vector3 center = new((Begin.x + End.x) / 2, (Begin.y + End.y) / 2, 0f);

            Debug.Log("Set positions: ");

            float offset = 0.5f;

            for (float ratio = 0; ratio <= 1; ratio += 0.1f)
            {
                Vector3 curve = EvaluateSlerpPoint(Begin, center, offset, ratio);
                line.Add(curve);
            }

            for (float ratio = 0; ratio <= 1; ratio += 0.1f)
            {
                Vector3 curve = EvaluateSlerpPoint(center, End, -1 * offset, ratio);
                line.Add(curve);
            }

            lineRenderer.positionCount = line.Count;
            lineRenderer.SetPositions(line.ToArray());
        }
    }
}
