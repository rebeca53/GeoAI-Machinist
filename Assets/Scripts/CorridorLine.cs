using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorLine : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float startCorridor = 4;
        float endCorridor = 8;
        float LengthCorridor = 3;

        LineRenderer lineRenderer = transform.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("Failed to retrieve LineRenderer");
        }

        float mid = 0.1f + (startCorridor + endCorridor) / 2f;
        Vector3 startPoint = new(0, mid, 0f);
        Vector3 endPoint = new(LengthCorridor, mid, 0f);
        Connection conn = new(startPoint, endPoint, lineRenderer);
        conn.DrawStraightLine();
    }

}
