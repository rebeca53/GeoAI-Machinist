using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomOutRegion : MonoBehaviour
{
    public CameraZoom cameraZoom;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cameraZoom.ChangeZoomSmooth(5f);
        }
    }
}
