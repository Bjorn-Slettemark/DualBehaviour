using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class GameStateSO : ScriptableObject
{
    public GameState gameState;

    [SerializeField]
    private string stateName;

    // Method to be called when entering the state
    public virtual void EnterState()
    {
        EventChannelManager.Instance.RaiseEvent("GameStateEventChannel", stateName);

    }

    // Method to be called when exiting the state
    public virtual void ExitState()
    {
        EventChannelManager.Instance.RaiseEvent("GameStateEventChannel", "ExitState");
    }

    // Abstract method for state-specific functionality
    public abstract void StateUpdate();

}
