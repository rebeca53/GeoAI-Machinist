using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance { get; private set; }
    private VisualElement sampleDisplay;
    private VisualElement barBackground;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        UIDocument uiDocument = GetComponent<UIDocument>();
        barBackground = uiDocument.rootVisualElement.Q<VisualElement>("BarBackground");
        barBackground.style.display = DisplayStyle.None;
        sampleDisplay = uiDocument.rootVisualElement.Q<VisualElement>("Sample");
        sampleDisplay.style.display = DisplayStyle.None;
    }

    public void SetSample(Sprite sampleSprite)
    {
        sampleDisplay.style.backgroundImage = new StyleBackground(sampleSprite);
    }

    public void Show()
    {
        barBackground.style.display = DisplayStyle.Flex;
        sampleDisplay.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        barBackground.style.display = DisplayStyle.None;
        sampleDisplay.style.display = DisplayStyle.None;
    }
}
