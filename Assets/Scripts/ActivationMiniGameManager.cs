using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivationMiniGameManager : BaseBoard
{
    // Pre-fabs
    public GameObject activationViewObject;
    public GameObject loadingScreen;


    // Constants
    readonly int ActivationFunctionAmount = 3;

    // Instances
    public ActivationMiniGamePlaybackDirector playbackDirector;
    Dictionary<string, ActivationView> activationViews = new Dictionary<string, ActivationView>();
    public TimedDialogueBalloon timedDialogueBalloon;
    public DialogueBalloon dialogueBalloon;
    public CameraZoom cameraZoom;

    // Data
    public TextAsset dataText;
    ActivationData data;
    [System.Serializable]
    class ActivationData
    {
        public List<double> inputMatrix = new List<double>();
    }

    double[,] UnflatMatrix(List<double> flatten, int size)
    {
        double[,] unflatten = new double[size, size];
        for (int k = 0; k < flatten.Count; k++)
        {
            double value = flatten[k];
            int i = k / size;
            int j = k - i * size;
            // Debug.Log("flatenning: k " + k + ", value " + value + ", i " + i + ", j" + j);
            unflatten[i, j] = value;
        }

        return unflatten;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateProgress(0f);
        StartCoroutine(LayoutAll());
    }

    void UpdateProgress(float progress)
    {
        // Debug.Log("Update progress " + progress);
        Image bar = GameObject.Find("ProgressBar").GetComponent<Image>();
        bar.fillAmount = progress;
    }

    void IncrementProgress(float progress)
    {
        // Debug.Log("IncrementProgress by " + progress);
        Image bar = GameObject.Find("ProgressBar").GetComponent<Image>();
        bar.fillAmount += progress;
    }


    // Start is called before the first frame update
    IEnumerator LayoutAll()
    {
        InitializeTilemap();

        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                DrawFloor(x, y);

                if (IsBorder(x, y) && !IsExit(x, y))
                {
                    DrawWall(x, y);
                }
                if (IsExit(x, y))
                {
                    DrawExit(x, y);
                }
            }
            IncrementProgress(0.01f);
            yield return null;
        }

        Player.Spawn(this, new Vector2Int(2, 1));
        NPC.Spawn(this, new Vector2Int(1, 1));

        LoadMatrix();

        StartCoroutine(LayoutActivationViews());
    }


    IEnumerator LayoutActivationViews()
    {
        float verticalGap = 4.5f;
        float xPosition = 2f;
        float verticalOffset = 2.5f;

        for (int i = 0; i < ActivationFunctionAmount; i++)
        {
            float yPosition = verticalOffset + i * verticalGap;
            Vector3 position = new(xPosition, yPosition, 0f);
            GameObject instanceView = Instantiate(activationViewObject, position, Quaternion.identity);
            ActivationView script = instanceView.GetComponent<ActivationView>();
            script.InitActivationBox(GetActivationType(i));
            script.InitInput(UnflatMatrix(data.inputMatrix, 62));
            script.OnActivationStopped += OnActivationStopped;
            activationViews.Add(GetActivationType(i), script);
            UpdateProgress((float)(i + 1) / ActivationFunctionAmount);
            yield return null;
        }

        RegisterActivationViewsMessages();
        loadingScreen.SetActive(false);
        playbackDirector.StartAnimation();
    }

    private void UnregisterActivationViewsMessages()
    {
        foreach (KeyValuePair<string, ActivationView> entry in activationViews)
        {
            entry.Value.OnHover -= DisplayActivationFunctionMessage;
            entry.Value.OnUnhover -= HideActivationFunctionMessage;
        }
    }

    private void RegisterActivationViewsMessages()
    {
        foreach (KeyValuePair<string, ActivationView> entry in activationViews)
        {
            entry.Value.OnHover += DisplayActivationFunctionMessage;
            entry.Value.OnUnhover += HideActivationFunctionMessage;
        }
    }

    private string GetActivationType(int idx)
    {
        switch (idx)
        {
            case 0:
                return "Linear";
            case 1:
                return "ReLu";
            case 2:
                return "Sigmoid";
        }
        return "";
    }

    void LoadMatrix()
    {
        // Debug.Log(dataText.text);
        data = JsonUtility.FromJson<ActivationData>(dataText.text);

        if (data == null)
        {
            Debug.LogError("Failed to retrieve from JSON");
        }
    }

    void OnActivationStopped(string type)
    {
        // Update outputline
        if (activationViews[type].HasActivationBox())
        {
            if (type == "ReLu")
            {
                activationViews[type].UpdateOutputState("correct");
            }
            else
            {
                activationViews[type].UpdateOutputState("wrong");
            }
        }
        else
        {
            activationViews[type].UpdateOutputState("inactive");
        }

        if (IsGameOver())
        {
            DisplayGameOverMessage();
        }
        else if (NeedRemoveWrongActivation())
        {
            DisplayWrongActivationMessage();
        }
    }

    private void DisplayActivationFunctionMessage(string type)
    {
        string message = "";
        switch (type)
        {
            case "Linear":
                message = "The linear function is f(x) = x. It resembles a straight line and restricts the CNN to find results that are linear combinations of the input.";
                break;
            case "ReLu":
                message = "The ReLu function is f(x) = max(0,x). It is simple and non-linear. As a consequence, it allows the CNN to find more complex solutions.";
                break;
            case "Sigmoid":
                message = "The sigmoid is f(x) = 1 / (1 + exp(-x)). It is non-linear and allows the CNN to find more complex solutions, but it costs the complex computation of an exponent.";
                break;
        }
        timedDialogueBalloon.SetSpeaker(Player.gameObject);
        timedDialogueBalloon.SetMessage(message);
        timedDialogueBalloon.PlaceUpperLeft();
        timedDialogueBalloon.Show();
    }

    private void HideActivationFunctionMessage(string type)
    {
        timedDialogueBalloon.Hide();
    }

    private void DisplayGameOverMessage()
    {
        Player.Disable();
        cameraZoom.ChangeZoomTarget(NPC.gameObject);
        ZoomIn();
        // NPC speaks message
        string message = "Good job picking the best activation function for this scenario. Let's go back for the CNN Room.";
        dialogueBalloon.SetSpeaker(NPC.gameObject);
        dialogueBalloon.SetMessage(message);
        dialogueBalloon.PlaceUpperLeft();
        dialogueBalloon.Show();
        dialogueBalloon.OnDone += GameOver;
    }

    bool IsGameOver()
    {
        return IsReLuDone() && !AreOtherActivationsConnected();
    }

    bool AreOtherActivationsConnected()
    {
        bool otherActivationBoxConnected = false;
        foreach (KeyValuePair<string, ActivationView> entry in activationViews)
        {
            if (entry.Key.Equals("ReLu"))
            {
                continue;
            }

            if (entry.Value.HasActivationBox())
            {
                otherActivationBoxConnected = true;
                break;
            }
        }
        return otherActivationBoxConnected;
    }

    bool IsReLuDone()
    {
        bool reluDone = activationViews["ReLu"].HasActivationBox() && !activationViews["ReLu"].IsApplyingActivation();
        return reluDone;
    }

    bool NeedRemoveWrongActivation()
    {
        return IsReLuDone() && AreOtherActivationsConnected();
    }

    void ZoomIn()
    {
        cameraZoom.ChangeZoomSmooth(1.4f);
    }

    private void DisplayWrongActivationMessage()
    {
        string message = "Oops, I need to disconnect the other activation functions.";
        timedDialogueBalloon.SetSpeaker(Player.gameObject);
        timedDialogueBalloon.SetMessage(message);
        timedDialogueBalloon.PlaceUpperLeft();
        timedDialogueBalloon.Show(10f);
        ZoomIn();
    }

    protected override void GameOver()
    {
        GameManager.instance.solvedMinigames["Activation 1"] = true;
        GameManager.instance.StartOverviewScene();
    }
}
