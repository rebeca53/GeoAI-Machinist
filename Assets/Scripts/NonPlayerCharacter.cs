using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NonPlayerCharacter : MonoBehaviour
{

    // Audio
    AudioSource audioSource;
    public AudioClip speakingClip;


    Dictionary<string, string> sceneToInstruction = new Dictionary<string, string> {
        {"SampleScene", "Hello, GeoAI Machinist! Your first mission is labeling all these images by placing them in the correct container. This way the Big Machine can learn from them. Press SPACE to interact with objects, and approach the Yellow Robot to see the instructions again."},
        {"OverviewScene", "The Big Machine is a Convolutional Neural Network (CNN) to identify land use and land cover. Fancy name, right? A CNN is a sequence of mathematical operations over 'matrices'. Each step in this sequence of operation is called a 'layer'. For us, matrices are just another name for images!\nYour mission is to enter the each 'layer' to fix them."},
        {"InputMiniGame", "This is the first layer of the CNN. It breaks the image into spectral bands. Spectral bands are wavelenghts interval of light. Some bands are visible such as Red, Green, and Blue. But there are more things in heaven and Earth that humans can see. And there are invisible spectral bands to reveal them. Your mission here is to manipulate the input samples to feed each spectral band separately."},
        {"ConvolutionalMiniGame", "This is a convolutional layer of a CNN. It applies a convolution (matrix multiplication) between an input image and a kernel matrix to transform the image and enhance features. The kernel matrix has pre-defined optimal values.\nYour mission is to activate the kernel and perform the convolution: move the kernel over each row of the input image to transform it."}
    };

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("On trigger stay 2d " + other.tag);
        if (other.CompareTag("Player"))
        {
            DisplayIntroduction();
        }
    }

    public void DisplayIntroduction(float time = 30f)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        // Retrieve the name of this scene.
        string sceneName = currentScene.name;
        UIHandler.Instance.DisplayDialogue(sceneToInstruction[sceneName], time);
    }

    // Actually not working. It is just play on Awake
    public void PlaySound(AudioClip clip)
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource.isPlaying)
        {
            return;
        }
        audioSource.PlayOneShot(clip);
    }

    public void Spawn(OverviewBoardManager boardManager, Vector2Int cell)
    {
        transform.position = boardManager.CellToWorld(cell);
    }

    public void Spawn(CommandCenterBoardManager boardManager, Vector2Int cell)
    {
        transform.position = boardManager.CellToWorld(cell);
        DisplayIntroduction();
    }

    public void Spawn(BaseBoard boardManager, Vector2Int cell)
    {
        transform.position = boardManager.CellToWorld(cell);
        DisplayIntroduction();
    }
}
