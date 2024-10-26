using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance { get; private set; }
    private Label moneyText;

    public float displayTime = 15.0f;
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

        // DisplayDialogue("Hello, GeoAI Machinist! I'm your Robot Assistant, I'm here to help you, you slept for a while and may not remember many things.\n" +
        // // "As the GeoAI Machinist, your mission is to fix the Big Machine, an space station that surveys Earth and intervenes on emergency situations.\n" +
        // // "The ancient knowledge needed to fix the malfunction is almost lost, only the GeoAI Machinist has been trained to hold this knowledge and can save humanity.\n" +
        // "Your first step to fix it is to classify all these images by placing them in the correct container. This way the Big Machine can learn from your knowledge. Press space to interact with objects in the scene, and approach me to chat.");

        DisplayDialogue("Hi, GeoAI Machinist! Your first mission is labeling all these images by placing them in the correct container." +
        "This way the Big Machine can learn from them. Press SPACE to interact with objects, and approach the Yellow Robot to chat.");

    }

    public void DisplayDialogue(string content)
    {
        message.text = content;
        m_NonPlayerDialogue.style.display = DisplayStyle.Flex;
        m_TimerDisplay = displayTime;
    }

    // Land Use and Land Cover Classes Information
    private void DisplayClassInfo(string type)
    {
        var myDict = new Dictionary<string, string>
        {
            { "AnnualCrop", "Annual crops are those that do not last more than two growing seasons and typically only one. These include crops like wheat, corn, and rice, which are planted and harvested within a single year." },
            { "Forest", "Forests are large areas covered chiefly with trees and undergrowth, providing habitat for wildlife and playing a role in carbon sequestration and oxygen production." },
            { "HerbaceousVegetation", "Herbaceous vegetation consists of non-woody plants, such as grasses." },
            { "Highway", "Highways are major public roads, often connecting cities and towns, designed for high-speed traffic and usually characterized by multiple lanes." },
            { "Industrial", "Industrial areas are zones designated for manufacturing, warehousing, and other commercial enterprises involved in the production of goods and services." },
            { "Pasture", "Pastures are grasslands or other vegetative areas managed primarily for the grazing of livestock, such as cattle, sheep, and goats." },
            { "PermanentCrop", "Permanent crops (e.g., fruit trees and vines) last for more than two growing seasons, either dying back after each season or growing continuously, and include plants like apple trees, vineyards, and coffee bushes." },
            { "Residential", "Residential areas are zones primarily designated for housing, where people live and where infrastructure like schools, parks, and other community facilities are often located." },
            { "River", "Rivers are natural flowing watercourses, usually freshwater, that flow towards a larger body of water such as an ocean, sea, lake, or another river." },
            { "SeaLake", "Sea or lakes are large bodies of water, with seas being saline water connected to oceans and lakes generally freshwater, often enclosed by land." }
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
