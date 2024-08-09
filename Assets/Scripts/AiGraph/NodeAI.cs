using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

public abstract class NodeAI : Node
{
    [HideInInspector] public AiGraph aiGraph;
    [HideInInspector] public AIController aiController;


    private bool _isActive;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive != value)
            {
                _isActive = value;
                if (NodeEditorWindow.current != null)
                {
                    NodeEditorWindow.current.Repaint();
                }
            }
        }
    }

    public void InitializeNode(AiGraph graph, AIController controller)
    {
        if (graph == null)
        {
            Debug.LogError("Graph is null in NodeAI.InitializeNode");
            return;
        }
        if (controller == null)
        {
            Debug.LogError("Controller is null in NodeAI.InitializeNode");
            return;
        }

        Debug.Log("NodeAi Initializing");
        aiGraph = graph;
        aiController = controller;
        InitializeNode();
    }

    public virtual void InitializeNode() { }

    public virtual void Execute()
    {
        Debug.Log($"Executing node: {name}");
        SignalInputComplete();
        TriggerOutputs();
    }

    public virtual void Activate()
    {
        Debug.Log($"Activating node: {name}");
        if (!IsActive)
        {
            IsActive = true;
            OnActivate();
        }
    }

    public virtual void ResetState()
    {
        Debug.Log($"Resetting state for node: {name}");
        IsActive = false;
    }

    public virtual void Deactivate()
    {
        Debug.Log($"Deactivating node: {name}");
        IsActive = false;
        OnDeactivate();
    }

    public virtual void Update()
    {
        if (IsActive)
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
        return null;
    }
}