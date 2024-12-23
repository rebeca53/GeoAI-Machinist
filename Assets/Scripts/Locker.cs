using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : MonoBehaviour
{
    private float yOffset = 0f;
    private float xOffset = 0.3f;

    public void AddKernel(GameObject kernel)
    {
        Vector3 position = gameObject.transform.position;
        kernel.transform.parent = gameObject.transform;
        kernel.transform.localScale = new(0.1f, 0.2f, 0f);
        kernel.transform.position = new Vector3(position.x + xOffset, position.y + yOffset, position.z);
    }

    public void AddActivationBox(GameObject activationBox)
    {
        Vector3 position = gameObject.transform.position;
        activationBox.transform.parent = gameObject.transform;
        activationBox.transform.localScale = new(0.3f, 0.6f, 0f);
        activationBox.transform.position = new Vector3(position.x + xOffset, position.y + yOffset, position.z);
    }
}
