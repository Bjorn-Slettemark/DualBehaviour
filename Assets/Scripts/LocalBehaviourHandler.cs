using UnityEngine;
using static EventChannelManager;

public class LocalBehaviourHandler : MonoBehaviour
{
    public AIState currentState;
    private LocalEventHandler eventHandler;

    private void Start()
    {
        eventHandler = GetComponent<LocalEventHandler>();
        if (eventHandler == null)
        {
            Debug.LogError("LocalEventHandler component is required on the GameObject.");
            return;
        }

        SubscribeToStateEvents();
    }

    private void SubscribeToStateEvents()
    {
        // Unsubscribe from previous state's events
        if (currentState != null)
        {
            eventHandler.UnsubscribeEvents(currentState.listenChannel, currentState.triggerEventName);
        }

        // Subscribe to new state's defined events
        if (currentState != null)
        {
            eventHandler.SubscribeEvent(currentState.listenChannel, currentState.triggerEventName, HandleStateTransition);
        }
    }


    private void Update()
    {
        currentState?.Update();
    }

    private void HandleStateTransition(string arg)
    {
        foreach (var transition in currentState.transitions)
        {
            if (transition.eventName == arg)
            {
                TransitionToState(transition.targetState);
                return;
            }
        }
    }

    private void TransitionToState(AIState nextState)
    {
        if (nextState != null && currentState != nextState)
        {
            currentState?.Exit();
            currentState = nextState;
            currentState?.Enter();
            SubscribeToStateEvents(); // Re-subscribe to new state's events
        }
    }


}
