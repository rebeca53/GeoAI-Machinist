using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Turn
{
    int id;
    public string sampleName;
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

    public string GetMessage(string bandName)
    {
        if (IsCharacteristicBand(bandName))
        {
            switch (sampleName)
            {
                case "River":
                    return "The Red Edge spectral band is the perfect choice. It has high reflectance on vegetation and low reflectance on water bodies.";
                case "Highway":
                    return "The Red band is strongly reflected by dead foliage and is useful for identifying vegetation types, soils and urban (city and town) areas.";
                case "Residential":
                    switch (bandName)
                    {
                        case "red":
                            return "The Red band is useful for identifying vegetation types, soils and urban (city and town) areas.";
                        case "blue":
                            return "The blue band is useful for soil and vegetation discrimination, forest type mapping and identifying man-made features.";
                        case "redEdge":
                            // https://www.sciencedirect.com/science/article/pii/S1470160X24011026
                            return "The Red Edge spectral band is the perfect choice. It has high reflectance on vegetation and low reflectance on buildings.";
                        default:
                            return "";
                    }
            }
        }
        return "Huum. I classified the spectral band correctly, but I feel there is a better option.";
    }

    public string GetTurnOverMessage()
    {
        Debug.Log("sampleName " + sampleName);
        switch (sampleName)
        {
            case "River":
            case "Highway":
                return "In this scenario, only one spectral band is enough to feed information for the neural network";
            case "Residential":
                return "For this sample, multiple bands combined will provide better features.";
            default:
                return "";
        }
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

    public bool AllCharacteristicSelected()
    {
        bool allCharacteristicSelected = true;
        foreach (string band in characteristicBands)
        {
            if (!correct.Contains(band))
            {
                allCharacteristicSelected = false;
            }
        }
        return allCharacteristicSelected;
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