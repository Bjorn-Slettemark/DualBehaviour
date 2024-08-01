using UnityEngine;

[CreateAssetMenu(fileName = "New Idle State", menuName = "AI/States/Idle State")]
public class AiStateIdleSO : AiStateSO
{
    public override string stateName => "Idle";

    public override void Enter()
    {
        Debug.Log($"{controlledObject.name} entering Idle state.");
    }

    public override void UpdateState()
    {
        // Idle state logic
    }

    public override void Exit()
    {
        Debug.Log($"{controlledObject.name} exiting Idle state.");
    }
}