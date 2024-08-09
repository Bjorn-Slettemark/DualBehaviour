using UnityEngine;
using XNode;
using System.Collections.Generic;
using UnityEditor;

public class NodeAIEvent : NodeAI
{
    [Input(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.None)]
    public string input;

    [Output(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.None)]
    public string output;

    public GameEventChannelSO eventChannel;
    public string eventName;
    public  PriorityLevel priorityLevel = PriorityLevel.Lowest;
    public bool overridePriority = false;

    private bool isListening = false;

    public override void InitializeNode()
    {
        base.InitializeNode();
        SubscribeToEvent();
    }

    private void SubscribeToEvent()
    {
        if (eventChannel != null)
        {
            eventChannel.RegisterListener(OnEventRaised);
            isListening = true;
        }
    }
    private void OnEventRaised(string raisedEventName)
    {
        if (raisedEventName == eventName && (overridePriority || CanTriggerEvent()))
        {
            aiGraph.TriggerNode(this);
        }
    }

    public override void Execute()
    {
        Debug.Log($"NodeAIEvent {name} executed. Priority level: {(int)priorityLevel}");
        aiGraph.SetCurrentPriorityLevel((int)priorityLevel, overridePriority);
        base.Execute();
    }
    private bool CanTriggerEvent()
    {
        var inputPort = GetInputPort("input");
        if (inputPort != null && inputPort.GetConnections().Count > 0)
        {
            foreach (var connection in inputPort.GetConnections())
            {
                if (connection.node is NodeAI nodeAI && !nodeAI.IsActive)
                {
                    return false;
                }
            }
        }

        return overridePriority || (int)priorityLevel <= aiGraph.CurrentPriorityLevel;
    }

    private void TriggerEvent()
    {
        aiGraph.SetCurrentPriorityLevel((int)priorityLevel);
        Debug.Log($"NodeAIEvent {name} triggered. New priority level: {(int)priorityLevel}");
        SignalInputComplete();
        TriggerOutputs();

        // Notify Unity that the graph has changed
        EditorUtility.SetDirty(aiGraph);
    }
    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "output")
        {
            return eventName;
        }
        return null;
    }
}