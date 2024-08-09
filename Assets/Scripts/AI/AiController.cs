using UnityEngine;
using System.Collections.Generic;

public class AIController : MonoBehaviour
{
    public AiGraph aiGraph;

    private void Awake()
    {
        if (aiGraph != null)
        {
            aiGraph.ResetAllNodes();
        }
    }

    private void Start()
    {
        if (aiGraph != null)
        {
            InitializeAIGraph();
        }
        else
        {
            Debug.LogError("AIGraph not assigned to AIController!");
        }
    }

    private void InitializeAIGraph()
    {
        aiGraph.Initialize(this);
    }

    private void Update()
    {
        if (aiGraph != null)
        {
            aiGraph.UpdateNodes();
        }
    }

    public void OnStateCompleted(NodeAIState state)
    {
        aiGraph.OnStateCompleted(state);
    }

    public List<NodeAIState> GetActiveStates()
    {
        return aiGraph.GetActiveStates();
    }

    // Test method to activate a node
    public void TestNodeActivation()
    {
        Debug.Log("Testing node activation");
        if (aiGraph != null && aiGraph.nodes.Count > 0)
        {
            NodeAI firstNode = aiGraph.nodes[0] as NodeAI;
            if (firstNode != null)
            {
                aiGraph.ActivateNode(firstNode);
            }
        }
    }
}