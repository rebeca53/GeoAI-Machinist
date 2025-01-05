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
    Transform softmaxLabel;


    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        softmaxLabel = transform.Find("SoftmaxLabel");
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
        label.localPosition = new(0, -0.9f, 0);
    }

    public void SetLogitMode(double value)
    {
        logitValue = value;
        spriteRenderer.color = GetLogitColor(logitValue);
        softmaxLabel.gameObject.SetActive(false);
    }
    public void SetLogitMode()
    {
        spriteRenderer.color = GetLogitColor(logitValue);
        softmaxLabel.gameObject.SetActive(false);
    }

    public double GetLogit()
    {
        return logitValue;
    }

    public void SetSoftmaxMode(double value)
    {
        softmaxValue = value;
        spriteRenderer.color = GetSoftmaxColor();
        softmaxLabel.GetComponent<TextMeshPro>().text = Math.Round(value * 100, 2) + "%";
        softmaxLabel.gameObject.SetActive(true);
    }

    public void SetSoftmaxMode()
    {
        spriteRenderer.color = GetSoftmaxColor();
        softmaxLabel.gameObject.SetActive(true);
    }

    Color GetLogitColor(double logit)
    {
        // minmax normalization
        double normalized = (logit - MinLogit) / (MaxLogit - MinLogit);
        return Color.Lerp(Color.black, Color.green, (float)normalized);
    }

    public Color GetSoftmaxColor()
    {
        return Color.Lerp(Color.grey, Color.white, (float)softmaxValue);
    }
}
