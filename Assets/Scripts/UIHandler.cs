using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance { get; private set; }

    private Label moneyText;

    public float displayTime = 4.0f;
    private VisualElement m_NonPlayerDialogue;
    private Label message;
    private float m_TimerDisplay;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        moneyText = uiDocument.rootVisualElement.Q<Label>("MoneyBar");
        SetMoneyValue(0);

        m_NonPlayerDialogue = uiDocument.rootVisualElement.Q<VisualElement>("NPCDialogue");
        m_NonPlayerDialogue.style.display = DisplayStyle.None;
        m_TimerDisplay = -1.0f;
        message = uiDocument.rootVisualElement.Q<Label>("DialogueMessage");

        DisplayDialogue();
    }

    public void SetMoneyValue(int amount)
    {
        moneyText.text = amount.ToString();
    }

    private void Update()
    {
        if (m_TimerDisplay > 0)
        {
            m_TimerDisplay -= Time.deltaTime;
            if (m_TimerDisplay < 0)
            {
                m_NonPlayerDialogue.style.display = DisplayStyle.None;
            }
        }
    }

    public void DisplayDialogue()
    {
        message.text = "Hey! Help me to classify all this images. Press \"space\" to grab and drop them. Press \"t\" while near me to chat.";
        m_NonPlayerDialogue.style.display = DisplayStyle.Flex;
        m_TimerDisplay = displayTime;
    }

    // Land Use and Land Cover Classes Information
    private void DisplayClassInfo(string type)
    {
        var myDict = new Dictionary<string, string>
        {
            { "AnnualCrop", "Annual crops are those that do not last more than two growing seasons and typically only one." },
            { "Forest", "Forests are" },
            { "HerbaceousVegetation", "Herbaceous Vegetation are" },
            { "Highway", "Highways are ..."},
            { "Industrial", "Industrial are" },
            { "Pasture", "Pasture are" },
            { "PermanentCrop", "PermanentCrop are" },
            { "Residential", "Residential are" },
            { "River", "River are" },
            { "SeaLake", "SeaLake are" }
        };

        Debug.Log("Message type: " + type + ", content: " + myDict[type]);
        message.text = myDict[type];
        m_NonPlayerDialogue.style.display = DisplayStyle.Flex;
        m_TimerDisplay = displayTime;
    }

    public void RegisterContainers()
    {
        GameObject[] containers = GameObject.FindGameObjectsWithTag("Container");
        foreach (GameObject container in containers)
        {
            Container containerScript = container.GetComponent<Container>();
            string type = containerScript.type;
            containerScript.OnMatchDisplay += DisplayClassInfo;
        }
    }
}
