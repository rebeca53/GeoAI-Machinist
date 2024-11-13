using System.Collections;
using Cinemachine;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    const float MaxOrthoSize = 5f;
    const float MinOrthoSize = 0.5f;

    float orthoSize;
    [SerializeField] float sensitivity = 0.5f;
    bool blocked = false;

    // Update is called once per frame
    void Update()
    {
        if (blocked == false)
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
}
