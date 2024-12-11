using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogitNode : MonoBehaviour
{
    // TODO: notify Mode on action
    public event Action OnHover;
    public event Action OnUnhover;

    readonly float MinLogit = -26f;
    readonly float MaxLogit = 16f;

    double logitValue;
    double softmaxValue;
    string classLabel = "";

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Debug.Log("on trigger enter activation view");
        OnHover?.Invoke();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        OnHover?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        OnUnhover?.Invoke();
    }

    public void SetLabel(string newLabel)
    {
        classLabel = newLabel;
        // TODO: Update Sprite to represent the correspoding function
        Transform label = transform.Find("Label");
        label.GetComponent<TextMeshPro>().text = classLabel;
        label.localPosition = new(0, -0.6f, 0);
    }

    public void SetLogitMode(double value)
    {
        logitValue = value;
        spriteRenderer.color = GetLogitColor(logitValue);
    }
    public void SetLogitMode()
    {
        spriteRenderer.color = GetLogitColor(logitValue);
    }

    public double GetLogit()
    {
        return logitValue;
    }

    public void SetSoftmaxMode(double value)
    {
        softmaxValue = value;
        spriteRenderer.color = GetSoftmaxColor(softmaxValue);
    }

    public void SetSoftmaxMode()
    {
        spriteRenderer.color = GetSoftmaxColor(softmaxValue);
    }

    Color GetLogitColor(double logit)
    {
        // minmax normalization
        double normalized = (logit - MinLogit) / (MaxLogit - MinLogit);
        return Color.Lerp(Color.white, Color.green, (float)normalized);
    }

    Color GetSoftmaxColor(double softmax)
    {
        return Color.Lerp(Color.black, Color.white, (float)softmax);
    }

    public Color GetSoftmaxColor()
    {
        return Color.Lerp(Color.black, Color.white, (float)softmaxValue);
    }
}
