using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class GameStateSO : ScriptableObject
{
    public GameState gameState;
    [SerializeField]
    private GameEventChannelSO gameStateEventChannel;

    // Method to be called when entering the state
    public virtual void EnterState()
    {
        // Use gameState.ToString() to ensure dynamic state name is passed
        EventChannelManager.Instance.RaiseEvent(gameStateEventChannel, gameState.ToString());
    }

    // Method to be called when exiting the state
    public virtual void ExitState()
    {
        EventChannelManager.Instance.RaiseEvent(gameStateEventChannel, "ExitState");
    }

    // Abstract method for state-specific functionality
    public abstract void StateUpdate();

}
