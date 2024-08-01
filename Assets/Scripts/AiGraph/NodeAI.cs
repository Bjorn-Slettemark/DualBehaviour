using System.Collections.Generic;
using UnityEngine;
using XNode;

public enum PriorityLevel
{
    Highest = 1,
    High = 2,
    Medium = 3,
    Low = 4,
    Lowest = 5
}

public abstract class NodeAI : Node
{
    [HideInInspector] public AiGraph aiGraph;
    [HideInInspector] public AIController aiController;

    private bool isActive = false;

    public bool IsActive { get => isActive;  }

    public void InitializeNode(AiGraph graph, AIController controller)
    {
        if (graph == null)
        {
            UnityEngine.Debug.LogError("Graph is null in NodeAI.InitializeNode");
            return;
        }
        if (controller == null)
        {
            UnityEngine.Debug.LogError("Controller is null in NodeAI.InitializeNode");
            return;
        }

        Debug.Log("NodeAi Initializing");
        aiGraph = graph;
        aiController = controller;
        InitializeNode();
    }

    public virtual void InitializeNode() { }

    public virtual void Activate()
    {
        if (!isActive)
        {
            isActive = true;
            Debug.Log($"Activating node: {name}");
            OnActivate();
        }
    }
    public virtual void ResetState()
    {
        isActive = false;
        // Add any other state reset logic here
    }

    public virtual void Deactivate()
    {
        isActive = false;
        OnDeactivate();
    }

    public virtual void Update()
    {
        if (isActive)
        {

            OnUpdate();
        }
    }

    protected virtual void OnActivate() { }
    protected virtual void OnDeactivate() { }
    protected virtual void OnUpdate() { }

    protected void SignalInputComplete()
    {
        var inputPort = GetInputPort("input");
        if (inputPort != null)
        {
            foreach (var connection in inputPort.GetConnections())
            {
                if (connection.node is NodeAI nodeAI)
                {
                    aiGraph.DeactivateNode(nodeAI);
                }
            }
        }
    }

    public void TriggerOutputs()
    {
        Debug.Log($"Triggering outputs for {name}");
        var outputPort = GetOutputPort("output");
        if (outputPort != null)
        {
            foreach (var connection in outputPort.GetConnections())
            {
                if (connection.node is NodeAI nodeAI)
                {
                    Debug.Log($"Activating connected node: {nodeAI.name}");
                    aiGraph.ActivateNode(nodeAI);
                }
            }
        }
    }


    public override object GetValue(NodePort port)
    {
        return null; // You might want to return something meaningful here depending on your use case
    }
}