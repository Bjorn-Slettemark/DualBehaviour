using System;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using System.Linq;
using UnityEditor;
using XNodeEditor;

public enum PriorityLevel
{
    Highest = 1,
    High = 2,
    Medium = 3,
    Low = 4,
    Lowest = 5
}
[CreateAssetMenu(fileName = "New AI Graph", menuName = "AI/Graph")]
public class AiGraph : NodeGraph
{
    private List<NodeAI> activeNodes = new List<NodeAI>();
    private List<NodeAIState> activeStates = new List<NodeAIState>();
    private int currentPriorityLevel = 5; // Default to lowest priority
    public int CurrentPriorityLevel => currentPriorityLevel;

    public event Action OnPriorityChanged;
    public event System.Action OnGraphChanged;

    private AIController aiController;
    private List<NodeAI> triggeredNodes = new List<NodeAI>();

    public void Initialize(AIController controller)
    {
        aiController = controller;
        SetCurrentPriorityLevel(5, true); // Reset to lowest priority
        Debug.Log($"Initializing AiGraph with {nodes.Count} nodes. Starting priority: {CurrentPriorityLevel}");

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
            catch (System.Exception e)
            {
                Debug.LogError($"Error initializing node {node.name}: {e.Message}\n{e.StackTrace}");
            }
        }
    }

    public void OnNodeActiveStateChanged()
    {
        Debug.Log("Node active state changed");
        if (XNodeEditor.NodeEditorWindow.current != null)
        {
            EditorApplication.delayCall += XNodeEditor.NodeEditorWindow.current.Repaint;
        }
        OnGraphChanged?.Invoke();
    }
    public void SetCurrentPriorityLevel(int level, bool forceSet = false)
    {
        if (forceSet || level < currentPriorityLevel)
        {
            currentPriorityLevel = Mathf.Clamp(level, 1, 5);
            Debug.Log($"Priority level set to: {currentPriorityLevel}");
            OnPriorityChanged?.Invoke();
        }
    }

    public void ActivateNode(NodeAI node)
    {
        Debug.Log($"AiGraph activating node: {node.name}");
        if (!activeNodes.Contains(node))
        {
            activeNodes.Add(node);
            node.Activate();
            OnNodeActiveStateChanged();

            if (node is NodeAIState stateNode)
            {
                ActivateState(stateNode);
            }
        }
    }

    public void TriggerNode(NodeAI node)
    {
        if (!triggeredNodes.Contains(node))
        {
            triggeredNodes.Add(node);
        }
    }

    public void UpdateNodes()
    {
        triggeredNodes = triggeredNodes.OrderBy(n => GetNodePriority(n)).ToList();

        foreach (var node in triggeredNodes)
        {
            node.Execute();
        }

        triggeredNodes.Clear();

        foreach (NodeAI node in activeNodes)
        {
            node.Update();
        }
    }

    private int GetNodePriority(NodeAI node)
    {
        if (node is NodeAIEvent eventNode)
        {
            return eventNode.overridePriority ? 0 : (int)eventNode.priorityLevel;
        }
        else if (node is NodeAILocalEvent localEventNode)
        {
            return localEventNode.overridePriority ? 0 : (int)localEventNode.priorityLevel;
        }
        return 5;
    }

    public void ResetAllNodes()
    {
        Debug.Log("Resetting all nodes");
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

    public void DeactivateNode(NodeAI node)
    {
        Debug.Log($"AiGraph deactivating node: {node.name}");
        if (activeNodes.Contains(node))
        {
            activeNodes.Remove(node);
            node.Deactivate();
            OnNodeActiveStateChanged();

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
    public void SetNodeActive(NodeAI node, bool active)
    {
        node.IsActive = active;
        NodeEditorWindow.current.Repaint();
    }

}