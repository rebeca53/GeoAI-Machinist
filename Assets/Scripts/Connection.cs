using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection
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

    public void DrawLine(float offset = 0.5f)
    {
        List<Vector3> line = new List<Vector3>();
        Vector3 center = new((Begin.x + End.x) / 2, (Begin.y + End.y) / 2, 0f);

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

    public void DrawLongLine()
    {
        List<Vector3> line = new List<Vector3>();
        Vector3 firstStop = new(Begin.x - 1f, (Begin.y + End.y) / 2, 0f);
        Vector3 secondStop = new(End.x + 1f, (Begin.y + End.y) / 2, 0f);

        float offset = 0.5f;

        for (float ratio = 0; ratio <= 1; ratio += 0.1f)
        {
            Vector3 curve = EvaluateSlerpPoint(Begin, firstStop, offset, ratio);
            line.Add(curve);
        }

        for (float ratio = 0; ratio <= 1; ratio += 0.1f)
        {
            Vector3 curve = EvaluateSlerpPoint(secondStop, End, -1 * offset, ratio);
            line.Add(curve);
        }

        lineRenderer.positionCount = line.Count;
        lineRenderer.SetPositions(line.ToArray());
    }

    public void DrawStraightLine()
    {
        List<Vector3> line = new List<Vector3>
        {
            Begin,
            End
        };
        lineRenderer.positionCount = line.Count;
        lineRenderer.SetPositions(line.ToArray());
    }
}
