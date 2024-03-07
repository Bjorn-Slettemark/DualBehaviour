using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public enum State
{
    Entering,
    Updating,
    Exiting
}
public abstract class GameStateSO : ScriptableObject
{


    [SerializeField]
    private string stateName;

    // Method to be called when entering the state
    public virtual void EnterState()
    {
        EventChannelManager.Instance.RaiseEvent("LevelEventChannel",stateName);

    }

    // Method to be called when exiting the state
    public virtual void ExitState()
    {
        EventChannelManager.Instance.RaiseEvent("LevelEventChannel", stateName);
    }

    // Abstract method for state-specific functionality
    public abstract void StateUpdate();

}
