using System.Collections.Generic;
using UnityEngine;
using XNode;
public class NodeAILocalEvent : NodeAI
{
    [Input(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.None)]
    public string input;

    [Output(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.None)]
    public string output;

    [SerializeField] private string eventName;
    [SerializeField] private LocalEventChannel eventChannel;
    public PriorityLevel priorityLevel = PriorityLevel.Lowest;
    public bool overridePriority = false;

    private LocalEventHandler localEventHandler;

    public override void InitializeNode()
    {
        base.InitializeNode();
        SetupEventListener();
    }

    private void SetupEventListener()
    {
        if (!Application.isPlaying) return;

        localEventHandler = aiController.GetComponent<LocalEventHandler>();
        if (localEventHandler == null)
        {
            Debug.LogError($"LocalEventHandler not found on AIController for event: {eventName}");
            return;
        }

        Debug.Log($"Setting up listener for LocalEventNode: {eventName} on channel {eventChannel}");
        localEventHandler.RegisterListener(eventChannel, OnLocalEventRaised);
    }

    private void OnLocalEventRaised(string receivedEventName)
    {
        Debug.Log($"LocalEventNode received event: {receivedEventName} on channel {eventChannel}. Expecting: {eventName}");

        if (receivedEventName == eventName && (overridePriority || CanTriggerEvent()))
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
        Debug.Log($"NodeAILocalEvent {name} triggered. New priority level: {(int)priorityLevel}");
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

    private void OnDisable()
    {
        if (localEventHandler != null)
        {
            localEventHandler.UnregisterListener(eventChannel, OnLocalEventRaised);
        }
    }
}