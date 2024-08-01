using UnityEngine;

public abstract class AiStateSO : ScriptableObject
{
    public abstract string stateName { get; }

    protected GameObject controlledObject;

    public virtual void Initialize(GameObject obj)
    {
        controlledObject = obj;
        Debug.Log($"Initialized {stateName} with {obj.name}");
    }

    public abstract void Enter();
    public abstract void UpdateState();
    public abstract void Exit();
}