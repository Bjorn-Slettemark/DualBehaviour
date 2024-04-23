using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public struct Transition
{
    public string eventName;          // Event that triggers the transition
    public AIState targetState;       // State to transition to when the event is fired
}

[CreateAssetMenu(fileName = "AIState", menuName = "AI/State")]
public class AIState : ScriptableObject
{
    public LocalEventChannel listenChannel; // Channel to listen for events
    public string triggerEventName;         // Specific event name to trigger transitions

    public List<Transition> transitions = new List<Transition>();

    public Action OnEnter;
    public Action OnExit;
    public Action OnUpdate;

    public void Enter()
    {
        OnEnter?.Invoke();
        Debug.Log($"Entering state: {name}");
    }

    public void Exit()
    {
        OnExit?.Invoke();
        Debug.Log($"Exiting state: {name}");
    }

    public void Update()
    {
        OnUpdate?.Invoke();
    }
}