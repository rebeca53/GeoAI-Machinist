

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Game Object - Prefab
// positions pre-defined - a tiny label for each
// rectangle with matching color
// Connections towards the end of the room foreach rectangle
// Label
// Message - display on Stay
// void MatchSpectralBand() -> place the different bands of the same Sample aligned (same position)
public class SpectralBandContainer : MonoBehaviour
{
    string type;

    // src: https://custom-scripts.sentinel-hub.com/sentinel-2/bands/
    Dictionary<string, string> typeToMessage = new Dictionary<string, string> {
        {"red", "The red band is strongly reflected by dead foliage and is useful for identifying vegetation types, soils and urban (city and town) areas. It has limited water penetration and doesn’t reflect well from live foliage with chlorophyll."},
        {"green", "The green band gives excellent contrast between clear and turbid (muddy) water, and penetrates clear water fairly well. It helps in highlighting oil on water surfaces, and vegetation. It reflects green light stronger than any other visible color. Man-made features are still visible."},
        {"blue", "The blue band is useful for soil and vegetation discrimination, forest type mapping and identifying man-made features. It is scattered by the atmosphere, it illuminates material in shadows better than longer wavelengths, and it penetrates clear water better than other colors. It is absorbed by chlorophyll, which results in darker plants."},
        {"swir", "The SWIR band is useful for measuring the moisture content of soil and vegetation, and it provides good contrast between different types of vegetation. It helps differentiate between snow and clouds. On the other hand, it has limited cloud penetration."},
        {"redEdge", "The Red Edge band is an invisible frequency useful for classifying vegetation."}
    };

    Dictionary<string, string> typeToLabel = new Dictionary<string, string> {
        {"red", "Red (B4)"},
        {"green", "Green (B3)"},
        {"blue", "Blue (B2)"},
        {"swir", "SWIR (B12)"},
        {"redEdge", "Red Edge (B5)"}
    };

    public event Action<string> OnFull;
    private int countMatched = 0;
    private const int totalMatched = 5;

    public void SetType(string bandType)
    {
        type = bandType;

        TextMeshPro label = transform.Find("SpectralBandLabel").GetComponent<TextMeshPro>();
        label.text = typeToLabel[type];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        UIHandler.Instance.DisplayMessage(typeToMessage[type]);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Debug.Log("On trigger exit 2d: " + other.tag);
        UIHandler.Instance.HideMessage();
    }

    public bool IsMatch(SampleSpectralBand sampleSpectralBand)
    {
        Debug.Log("Spectral band" + sampleSpectralBand.GetBandType() + " is MATCH with ?" + type);
        return sampleSpectralBand.GetBandType().Equals(type);
    }

    public void MatchSpectralBand(SampleSpectralBand sampleSpectralBand)
    {
        Debug.Log("MatchSpectralBand");

        Transform parentSquare = transform;
        if (parentSquare == null)
        {
            Debug.LogError("Failed to get parent based on sample spectral band class");
        }

        float verticalOffset = 0.2f;
        // Change scale
        sampleSpectralBand.gameObject.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

        // change box parent
        sampleSpectralBand.gameObject.transform.parent = parentSquare;
        sampleSpectralBand.gameObject.transform.position = new Vector3(parentSquare.position.x, parentSquare.position.y + verticalOffset);
        sampleSpectralBand.FitInContainer();

        countMatched++;
        if (countMatched == totalMatched)
        {
            OnFull?.Invoke(type);
        }
    }

    public void DrawConnections(Vector3 inputPosition)
    {
        DrawInputConnection(inputPosition);
        DrawOutputConnection();
    }

    void DrawInputConnection(Vector3 inputPosition)
    {
        Transform line = transform.Find("InputLine");
        if (line == null)
        {
            Debug.LogError("Failed to retrieve Line");
        }
        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("Failed to retrieve LineRenderer");
        }

        Vector3 startPoint = inputPosition;
        Vector3 endPoint = new(0.1f, -1f, 0f);
        // Debug.Log("Draw connection from " + startPoint + " to " + endPoint);
        Connection conn = new(startPoint, endPoint, lineRenderer);
        conn.DrawLine(5f);
    }
    void DrawOutputConnection()
    {
        Transform line = transform.Find("OutputLine");
        if (line == null)
        {
            Debug.LogError("Failed to retrieve Line");
        }
        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("Failed to retrieve LineRenderer");
        }

        Vector3 startPoint = new(0f, -1f, 0f);
        Vector3 endPoint = new(2f, -0.5f, 0f);
        // Debug.Log("Draw connection from " + startPoint + " to " + endPoint);
        Connection conn = new(startPoint, endPoint, lineRenderer);
        conn.DrawLine(1f);
    }

}
