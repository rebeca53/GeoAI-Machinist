using TMPro;
using UnityEngine;

public class CNNLayer : MonoBehaviour
{
    public string type;
    private TextMeshPro label;

    // Start is called before the first frame update
    void Start()
    {
        label = gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
        label.text = type;
    }
}
