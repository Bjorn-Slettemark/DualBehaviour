using Debug = UnityEngine.Debug;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

using XNode;
using UnityEditor;

public class AiGraph : NodeGraph
{
    private List<NodeAI> activeNodes = new List<NodeAI>();
    private List<NodeAIState> activeStates = new List<NodeAIState>();

    private AIController aiController; // Add this line

    private int currentPriorityLevel = 5; // Default to lowest priority
    public int CurrentPriorityLevel => currentPriorityLevel;

    public void Initialize(AIController controller)
    {
        if (controller == null)
        {
            Debug.LogError("AIController passed to AiGraph.Initialize is null!");
            return;
        }

        aiController = controller;
        Debug.Log($"Initializing AiGraph with {nodes.Count} nodes.");

        foreach (NodeAI node in nodes)
        {
            if (node == null)
            {
                Debug.LogWarning("Encountered a null node in AiGraph.");
                continue;
            }

            try
            {
                Debug.Log($"Initializing node: {node.name} of type {node.GetType().Name}");
                node.InitializeNode(this, aiController);
                if (node is NodeAIEvent || node is NodeAILocalEvent)
                {
                    ActivateNode(node);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error initializing node {node.name}: {e.Message}\n{e.StackTrace}");
            }
        }
    }
    public void SetCurrentPriorityLevel(int level)
    {
        currentPriorityLevel = Mathf.Clamp(level, 1, 5);
        EditorUtility.SetDirty(this);

    }
    public void UpdateNodes()
    {
        foreach (NodeAI node in activeNodes)
        {
            node.Update();
        }
    }
    public void ResetAllNodes()
    {
        foreach (NodeAI node in nodes)
        {
            if (node != null)
            {
                node.ResetState();
            }
        }
        activeNodes.Clear();
        activeStates.Clear();
    }
    public void ActivateNode(NodeAI node)
    {
        if (!activeNodes.Contains(node))
        {
            UnityEngine.Debug.Log($"AiGraph activating node: {node.name}");
            activeNodes.Add(node);
            node.Activate();

            if (node is NodeAIState stateNode)
            {
                ActivateState(stateNode);
            }
        }
    }

    public void DeactivateNode(NodeAI node)
    {
        if (activeNodes.Contains(node))
        {
            activeNodes.Remove(node);
            node.Deactivate();

            if (node is NodeAIState stateNode)
            {
                DeactivateState(stateNode);
            }
        }
    }

    private void ActivateState(NodeAIState state)
    {
        if (!activeStates.Contains(state))
        {
            activeStates.Add(state);
        }
    }

    private void DeactivateState(NodeAIState state)
    {
        activeStates.Remove(state);
    }

    public void OnStateCompleted(NodeAIState state)
    {
        state.TriggerOutputs();
        DeactivateState(state);
    }

    public List<NodeAIState> GetActiveStates()
    {
        return new List<NodeAIState>(activeStates);
    }
}