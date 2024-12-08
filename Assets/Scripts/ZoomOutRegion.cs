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
            Debug.Log("Player entered Zoom In Region");
            cameraZoom.ChangeZoomSmooth(5f);
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
