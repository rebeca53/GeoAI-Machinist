using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NonPlayerCharacter : MonoBehaviour
{
    Dictionary<string, string> sceneToInstruction = new Dictionary<string, string> {
        {"SampleScene", "Your first mission is labeling all these images by placing them in the correct container. This way the Big Machine can learn from them. Press SPACE to interact with objects, and approach the Yellow Robot to see the instructions again."},
        {"OverviewScene", "The Big Machine is a Convolutional Neural Network. Fancy name, right? It is a sequence of 'layers'. Each 'layer' executes a type of operation of matrices. Oops, fancy word again... For us, matrices are the same as images!\n Your mission is to enter the 'layer' rooms and fix them."},
        {"InputMiniGame", "This is the first layer. It breaks the image into spectral bands. Spectral bands are wavelenghts interval of light. Some bands are visible such as Red, Green, and Blue. But there are more things in heaven and Earth that humans can see. And there are invisible spectral bands to reveal them. Your mission here is to manipulate the input sample to feed each spectral band separately."}
    };

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("On trigger stay 2d " + other.tag);
        if (other.CompareTag("Player"))
        {
            DisplayIntroduction();
        }
    }

    public void DisplayIntroduction()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        // Retrieve the name of this scene.
        string sceneName = currentScene.name;
        UIHandler.Instance.DisplayMessage(sceneToInstruction[sceneName], 30);
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

    public void Spawn(InputMiniGameManager boardManager, Vector2Int cell)
    {
        transform.position = boardManager.CellToWorld(cell);
        DisplayIntroduction();
    }

}
