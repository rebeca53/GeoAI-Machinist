using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    const float MaxOrthoSize = 10;
    const float MinOrthoSize = 1;

    float orthoSize;
    [SerializeField] float sensitivity = 1f;

    // Update is called once per frame
    void Update()
    {

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            Debug.Log("Mouse screoll " + scroll);
            orthoSize = Input.GetAxis("Mouse ScrollWheel") * sensitivity;
            orthoSize = virtualCamera.m_Lens.OrthographicSize - orthoSize;
            Debug.Log("new Ortho sieze " + orthoSize);
            orthoSize = Mathf.Clamp(orthoSize, MinOrthoSize, MaxOrthoSize);
            Debug.Log("clamped ortho size " + orthoSize);

            virtualCamera.m_Lens.OrthographicSize = orthoSize;
        }
    }
}
