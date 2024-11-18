using System;
using System.Collections;
using Cinemachine;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] float MaxOrthoSize = 8f;
    [SerializeField] float MinOrthoSize = 0.5f;

    // float orthoSize;
    float targetSize;
    readonly float zoomSpeed = 3.0f; // Speed of zoom
    readonly float deltaOrthoSize = 0.05f;
    bool IsZooming = false;

    [SerializeField] float sensitivity = 0.5f;
    bool blocked = false;

    void Start()
    {
        targetSize = virtualCamera.m_Lens.OrthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (blocked == false)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                targetSize = Input.GetAxis("Mouse ScrollWheel") * sensitivity;
                targetSize = virtualCamera.m_Lens.OrthographicSize - targetSize;
                targetSize = Mathf.Clamp(targetSize, MinOrthoSize, MaxOrthoSize);

                virtualCamera.m_Lens.OrthographicSize = targetSize;
            }
        }

        if (IsZooming)
        {
            Debug.Log($"IsZooming: OrthographicSize = {virtualCamera.m_Lens.OrthographicSize}, Target = {targetSize}");

            if (IsApproximate(targetSize, virtualCamera.m_Lens.OrthographicSize))
            {
                IsZooming = false;
            }
            else
            {
                virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, targetSize, Time.deltaTime * zoomSpeed);
                Debug.Log($"After: OrthographicSize = {virtualCamera.m_Lens.OrthographicSize}, Target = {targetSize}");
            }
        }
    }

    public void Block()
    {
        blocked = true;
    }

    public void Release()
    {
        blocked = false;
    }

    public void ChangeZoom(float orthoSize)
    {
        virtualCamera.m_Lens.OrthographicSize = orthoSize;
    }

    private bool IsApproximate(float valueA, float valueB)
    {
        return Math.Abs(valueA - valueB) < deltaOrthoSize;
    }

    public void ChangeZoomSmooth(float orthoSize)
    {
        if (virtualCamera == null)
        {
            Debug.LogError("virtualCamera is not assigned!");
            return;
        }

        Debug.Log($"Before: OrthographicSize = {virtualCamera.m_Lens.OrthographicSize}, Target = {orthoSize}");
        orthoSize = Mathf.Clamp(orthoSize, MinOrthoSize, MaxOrthoSize);
        targetSize = orthoSize;
        IsZooming = true;
    }
}
