using System;
using UnityEngine;

public enum LocalState
{
    Idle,
    Walking,
    Running,
    Jumping,
    Falling,
    Landing,
    Attacking,
    TakingDamage,
    Dying,
    Dead,
    Interacting
}

public class LocalStateHandler : MonoBehaviour
{
    [SerializeField]
    private LocalState currentState = LocalState.Idle; // Default state

    private LocalEventHandler eventHandler;

    private void Awake()
    {
        // Ensure that the LocalEventHandler component is attached to the same GameObject
        eventHandler = GetComponent<LocalEventHandler>();

        if (eventHandler == null)
        {
            Debug.LogError("LocalEventHandler component is required on the GameObject.");
        }
    }

    // Method to change the state
    public void ChangeState(LocalState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            NotifyStateChange(newState);
        }
    }

    // Accessor for current state
    public LocalState GetCurrentState()
    {
        return currentState;
    }

    // Notify subscribers of state changes and also publish to the event channel
    private void NotifyStateChange(LocalState newState)
    {
        eventHandler?.Publish(LocalEventChannel.State, newState.ToString(), "Switched");
        Debug.Log($"State changed to {newState}");
    }
}
