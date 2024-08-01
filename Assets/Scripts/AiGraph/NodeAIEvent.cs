using UnityEngine;
using XNode;
using System.Collections.Generic;
public class NodeAIEvent : NodeAI
{
    [Input(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.None)]
    public string input;

    [Output(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.None)]
    public string output;

    public GameEventChannelSO eventChannel;
    public string eventName;
    public PriorityLevel priorityLevel = PriorityLevel.Lowest;
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
            TriggerEvent();
        }
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
        // Always set the global priority to the node's priority, even when overriding
        aiGraph.SetCurrentPriorityLevel((int)priorityLevel);
        Debug.Log($"NodeAIEvent {name} triggered. New priority level: {(int)priorityLevel}");
        SignalInputComplete();
        TriggerOutputs();
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