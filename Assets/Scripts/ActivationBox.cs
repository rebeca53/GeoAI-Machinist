using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class ActivationBox : MonoBehaviour
{
    public Action OnGrabbed;
    private bool grabbed = false;

    // Data
    string type;

    public void SetFunction(string newType)
    {
        type = newType;
        Draw();
    }

    void Draw()
    {
        // TODO: Update Sprite to represent the correspoding function
        Transform label = transform.Find("Label");
        label.GetComponent<TextMeshPro>().text = type;
    }

    public void Grab(Vector3 grabberPosition)
    {
        Debug.Log("xgrab previous transform position " + transform.position);
        Debug.Log("xgrab previous transform local position " + transform.localPosition);

        Debug.Log("xgrab " + grabberPosition);
        grabbed = true;
        Vector3 relativeToParentPosition = new(0f, 0f, 0f);
        transform.localPosition = relativeToParentPosition;
        // transform.position = new(grabberPosition.x, grabberPosition.y, grabberPosition.z);
        Debug.Log("xgrab transform position " + transform.position);
        Debug.Log("xgrab transform local position " + transform.localPosition);

        OnGrabbed?.Invoke();
    }

    public bool IsGrabbed()
    {
        return grabbed;
    }

    public void PlaceAt(Vector3 newPosition)
    {
        transform.parent = null;
        transform.position = newPosition;
    }

    public void Block()
    {
        transform.tag = "Untagged"; // Player cannot grab Untagged objects
    }

    public double ApplyFunction(double pixelValue)
    {
        switch (type)
        {
            case "ReLu":
                return ReLu(pixelValue);
            case "Sigmoid":
                return Sigmoid(pixelValue);
            case "tanh":
                return HyperbolicTangent(pixelValue);
            case "Linear":
                return Linear(pixelValue);
        }
        return pixelValue;
    }

    double ReLu(double value)
    {
        if (value > 0)
        {
            return value;
        }

        return 0;
    }

    double Sigmoid(double value)
    {
        return 1.0f / (1.0f + (float)Math.Exp(-value));
    }

    double HyperbolicTangent(double x)
    {
        if (x < -45.0) return -1.0;
        else if (x > 45.0) return 1.0;
        else return Math.Tanh(x);
    }

    double Linear(double x)
    {
        return x;
    }

}
