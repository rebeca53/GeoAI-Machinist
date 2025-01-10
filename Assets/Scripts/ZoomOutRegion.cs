using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomOutRegion : MonoBehaviour
{
    public CameraZoom cameraZoom;
    public float zoom = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cameraZoom.ChangeZoomSmooth(zoom);
        }
    }
}
