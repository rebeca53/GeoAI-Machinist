using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Container : MonoBehaviour
{
    public event Action OnMatch;
    public string type;
    private Animator animator;
    private TextMeshPro label;
    private float verticalOffset = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        label = gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
        label.text = SplitCamelCase(type);

        animator = GetComponent<Animator>();

        // Debug.Log("Container type is " + type);
    }

    private string SplitCamelCase(string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
    }

    public void MatchContainer(SampleBox sampleBox)
    {
        // Change container characteristics
        animator.SetBool("matchContainer", true);

        // Update sample box chracteactis
        // Change scale
        sampleBox.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);

        // change box parent
        sampleBox.gameObject.transform.parent = gameObject.transform;
        sampleBox.gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + verticalOffset);
        sampleBox.fitInContainer();

        OnMatch?.Invoke();
    }

    public bool IsMatch(SampleBox sampleBox)
    {
        Debug.Log("container type is " + type + ", and box type is " + sampleBox.type);
        return sampleBox.type == type;
    }

    public void OnMismatch()
    {
        animator.SetTrigger("mismatchContainer");
    }
}
