using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using UnityEngine;


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
        aiGraph.UpdateNodes();
    }

    public void OnStateCompleted(NodeAIState state)
    {
        aiGraph.OnStateCompleted(state);
    }

    // New method to get active states
    public List<NodeAIState> GetActiveStates()
    {
        return aiGraph.GetActiveStates();
    }
}