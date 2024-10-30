using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHolder : MonoBehaviour
{
    private float verticalOffset = 0.7f;
    private float horizontalOffset = 0.013f;

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
