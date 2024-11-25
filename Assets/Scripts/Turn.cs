using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Turn
{
    int id;
    string sampleName;
    List<string> characteristicBands;
    List<string> correct = new List<string>();
    List<string> wrong = new List<string>();

    int amountActivated = 0;
    public GameObject sample;
    public Vector3 position = new(0.5f, 7f, 0f);
    public string instruction;

    public readonly string positiveMessage = "Good choice!";
    public readonly string negativeMessage = "Try a different band";

    public Turn(int id, string sampleName, List<string> bands, string instruction)
    {
        this.id = id;
        this.sampleName = sampleName;
        characteristicBands = bands;
        this.instruction = instruction;
    }

    public void SetSample(GameObject gameObject)
    {
        sample = gameObject;
    }

    public string GetProgressMessage()
    {
        int remaining = characteristicBands.Count - correct.Count;
        return "Still " + remaining + " to activate. And there are " + wrong.Count + " bands to deactivate.";
    }

    public bool IsCharacteristicBand(string bandName)
    {
        return characteristicBands.Contains(bandName);
    }

    public void Match(string bandName)
    {
        if (IsCharacteristicBand(bandName))
        {
            correct.Add(bandName);
        }
        else
        {
            Debug.Log("is wrong " + bandName);
            wrong.Add(bandName);
        }
    }

    public void Unmatch(string bandName)
    {
        if (IsCharacteristicBand(bandName))
        {
            correct.Remove(bandName);
        }
        else
        {
            wrong.Remove(bandName);
        }
    }

    public bool AlreadyMatched(string bandName)
    {
        return correct.Contains(bandName) || wrong.Contains(bandName);
    }

    public bool IsOver()
    {
        bool allCharacteristicSelected = true;
        foreach (string band in characteristicBands)
        {
            if (!correct.Contains(band))
            {
                allCharacteristicSelected = false;
            }
        }

        Debug.Log("Is turn over? correct " + printList(correct));
        Debug.Log("Is turn over? wrong " + printList(wrong));
        Debug.Log("Is turn over? characteristicBands " + printList(characteristicBands));

        Debug.Log("Is turn over? correct containes characteristic abnds? " + allCharacteristicSelected);

        Debug.Log("Is turn over? wrong Count " + wrong.Count);

        bool over = allCharacteristicSelected && (wrong.Count == 0);
        Debug.Log("Is turn over? " + over);
        return over;
    }

    private string printList(List<string> list)
    {
        string message = "";
        foreach (string value in list)
        {
            message += value + " ";
        }
        return message;
    }

}