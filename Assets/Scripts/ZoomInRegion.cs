using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomInRegion : MonoBehaviour
{
    public CameraZoom cameraZoom;

    public float zoom = 2f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cameraZoom.ChangeZoomSmooth(zoom);
        }
    }
}
