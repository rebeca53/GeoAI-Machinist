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
            Debug.Log("Player entered Zoom In Region");
            cameraZoom.ChangeZoomSmooth(zoom);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
