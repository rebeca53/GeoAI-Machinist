using System;
using TMPro;
using UnityEngine;

public class Container : MonoBehaviour
{
    public event Action OnMatch;
    public event Action<string> OnMatchDisplay;
    public string type;
    private Animator animator;
    private TextMeshPro label;
    private float verticalOffset = 0.3f;

    // Audio
    AudioSource audioSource;
    public AudioClip mismatchClip;
    public AudioClip matchClip;

    // Start is called before the first frame update
    void Start()
    {
        label = gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
        label.text = SplitCamelCase(type);

        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();

        // Debug.Log("Container type is " + type);
    }

    private string SplitCamelCase(string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
    }

    public void MatchContainer(SampleBox sampleBox)
    {
        PlaySound(matchClip);
        // Change container characteristics
        animator.SetBool("matchContainer", true);

        // Update sample box chracteactis
        // Change scale
        sampleBox.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);

        // change box parent
        sampleBox.gameObject.transform.parent = gameObject.transform;
        sampleBox.gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + verticalOffset);
        sampleBox.FitInContainer();

        OnMatch?.Invoke();
        OnMatchDisplay?.Invoke(type);
    }

    public bool IsMatch(SampleBox sampleBox)
    {
        Debug.Log("container type is " + type + ", and box type is " + sampleBox.type);
        return sampleBox.type == type;
    }

    public void OnMismatch()
    {
        PlaySound(mismatchClip);
        animator.SetTrigger("mismatchContainer");
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
