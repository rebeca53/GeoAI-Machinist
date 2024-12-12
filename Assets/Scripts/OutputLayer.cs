using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputLayer : MonoBehaviour
{
    public Action OnDone;
    // Pre-fabs
    public GameObject lineObject;
    Transform linesRoot;
    public InputHolder softmaxHolder;
    public Locker locker;

    public ActivationBox softmaxBox;
    // List<LogitNode> logitNodes;
    List<OutputLine> outputLines = new List<OutputLine>();

    double totalLogit;

    class OutputLine
    {
        string outputState = "inactive"; // inactice, wrong, correct
        LineRenderer outputLineRenderer;
        readonly float inactiveWidth = 0.05f;
        private Color workingStartColor;
        private Color workingEndColor;
        private Color wrongColor = Color.red;
        private Color inactiveColor = Color.gray;

        public OutputLine(LineRenderer newLine)
        {
            outputLineRenderer = newLine;
        }

        public void Draw(float xPosition, float yPosition)
        {
            workingStartColor = outputLineRenderer.startColor;
            workingEndColor = outputLineRenderer.endColor;

            Vector3 startPoint = new(xPosition + 0.6f, yPosition, 0f);
            Vector3 endPoint = new(xPosition + 3.75f, yPosition, 0f);
            Connection conn = new(startPoint, endPoint, outputLineRenderer);
            conn.DrawStraightLine();

            outputLineRenderer = conn.lineRenderer;
            UpdateOutputState("inactive");
        }

        public void UpdateOutputState(string newLineState)
        {
            Debug.Log("Update state: " + newLineState);
            outputState = newLineState;
            switch (outputState)
            {
                case "correct":
                    outputLineRenderer.startColor = workingStartColor;
                    outputLineRenderer.endColor = workingEndColor;
                    break;
                case "wrong":
                    outputLineRenderer.material.color = wrongColor;
                    outputLineRenderer.startColor = wrongColor;
                    outputLineRenderer.endColor = wrongColor;
                    outputLineRenderer.startWidth = inactiveWidth;
                    outputLineRenderer.endWidth = inactiveWidth;
                    break;
                case "inactive":
                default:
                    outputLineRenderer.material.color = inactiveColor;
                    outputLineRenderer.startColor = inactiveColor;
                    outputLineRenderer.endColor = inactiveColor;
                    outputLineRenderer.startWidth = inactiveWidth;
                    outputLineRenderer.endWidth = inactiveWidth;
                    break;
            }
        }

        public void UpdateLine(Color newColor, float width)
        {
            outputLineRenderer.material.color = newColor;
            outputLineRenderer.startColor = newColor;
            outputLineRenderer.endColor = newColor;
            // outputLineRenderer.startWidth = width;
            // outputLineRenderer.endWidth = width;
        }

        public void AnimateOutputState()
        {
            if (!outputLineRenderer)
            {
                // Debug.Log("Could not find output line render");
                return;
            }

            // Debug.Log("id[" + id + "] animate output state: " + outputState);
            if (outputState.Equals("correct"))
            {
                // Debug.Log("using Lerp because it is correct.");
                outputLineRenderer.material.color = Color.Lerp(Color.white, Color.cyan, Mathf.PingPong(Time.time, 0.5f));
                outputLineRenderer.startWidth = inactiveWidth * 2;
                outputLineRenderer.endWidth = inactiveWidth * 2;
            }
        }
    }
    // public void SetLogitNodes(List<LogitNode> nodes)
    // {
    //     logitNodes = nodes;

    //     // draw gray lines
    //     DrawLines();
    // }

    // void DrawLines()
    // {
    //     foreach (LogitNode node in logitNodes)
    //     {
    //         GameObject instance = Instantiate(lineObject, new(0, 0, 0), Quaternion.identity);

    //         OutputLine outputLine = new(instance.GetComponent<LineRenderer>());
    //         outputLine.Draw(node.transform.position.y);
    //         outputLines.Add(outputLine);

    //         // compute total sum for softmax
    //         totalLogit += Math.Exp(node.GetLogit());
    //     }
    // }


    void Awake()
    {
        linesRoot = transform.Find("OutputLinesRoot");
    }
    // Start is called before the first frame update
    void Start()
    {
        softmaxBox.SetFunction("Softmax");
        softmaxHolder.OnAddedObject += OnSoftmaxAdded;
        locker.AddActivationBox(softmaxBox.gameObject);

        softmaxHolder.DrawConnection(new(0, -1f, 0), new(1.5f, 1.5f, 0));
    }

    void OnSoftmaxAdded()
    {
        // softmaxBox.Block();

        GameObject[] nodes = GameObject.FindGameObjectsWithTag("LogitNode");
        Debug.Log("We found numerb od nodes: " + nodes.Length);

        for (int i = 0; i < nodes.Length; i++)
        {
            LogitNode node = nodes[i].GetComponent<LogitNode>();
            if (node == null)
            {
                Debug.Log("Could not find LogitNode script");
                return;
            }
            // draw outputlines
            DrawLine(node);

            // compute total sum for softmax
            totalLogit += Math.Exp(node.GetLogit());
        }

        for (int i = 0; i < nodes.Length; i++)
        {
            // change nodes color
            LogitNode node = nodes[i].GetComponent<LogitNode>();
            double softmax = ApplySoftmax(node);
            node.SetSoftmaxMode(ApplySoftmax(node));

            // Add percentage value
            Debug.Log("percentage softmax = " + Math.Round(100 * softmax, 2));

            // update outputlines
            outputLines[i].UpdateLine(node.GetSoftmaxColor(), (float)softmax);

            // TODO: on hover line display softmax calculation
        }

        OnDone?.Invoke();

    }

    void DrawLine(LogitNode node)
    {
        GameObject instance = Instantiate(lineObject, new(0, 0, 0), Quaternion.identity);
        instance.transform.parent = linesRoot;

        OutputLine outputLine = new(instance.GetComponent<LineRenderer>());
        outputLine.Draw(node.transform.position.x, node.transform.position.y);
        outputLines.Add(outputLine);
    }

    // Update is called once per frame
    void Update()
    {

    }

    double ApplySoftmax(LogitNode node)
    {
        return Math.Exp(node.GetLogit()) / totalLogit;
    }
}
