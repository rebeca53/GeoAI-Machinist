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
            orthoSize = Input.GetAxis("Mouse ScrollWheel") * sensitivity;
            orthoSize = virtualCamera.m_Lens.OrthographicSize - orthoSize;
            orthoSize = Mathf.Clamp(orthoSize, MinOrthoSize, MaxOrthoSize);

            virtualCamera.m_Lens.OrthographicSize = orthoSize;
        }
    }
}
