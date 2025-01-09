using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : MonoBehaviour
{
    private float yOffset = 0f;
    private float xOffset = 0.3f;

    private int id = -1;

    public void Init(int newId)
    {
        id = newId;
        gameObject.name = "Locker" + id;
    }

    public bool CanAdd(GameObject gameObject)
    {
        if (gameObject.CompareTag("Kernel"))
        {
            KernelMatrix kernel = gameObject.GetComponent<KernelMatrix>();
            return CanAddKernel(kernel);
        }
        else if (gameObject.CompareTag("ActivationBox"))
        {
            ActivationBox activationBox = gameObject.GetComponent<ActivationBox>();
            return CanAddActivationBox(activationBox);
        }

        return false;
    }

    public bool CanAddKernel(KernelMatrix kernel)
    {
        return kernel.GetId() == id;
    }

    public bool CanAddActivationBox(ActivationBox activationBox)
    {
        return activationBox.GetId() == id;
    }

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
