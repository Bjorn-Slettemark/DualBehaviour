using UnityEngine;

[CreateAssetMenu(fileName = "New Attack State", menuName = "AI/States/Attack State")]
public class AiStateAttackSO : AiStateSO
{
    public override string stateName => "Attack";

    public override void Enter()
    {
        if (controlledObject != null)
        {
            Debug.Log($"{controlledObject.name} entering Attack state.");
        }
        else
        {
            Debug.LogError("Attack state entered but controlledObject is null!");
        }
    }

    public override void UpdateState()
    {
        // Attack state logic
    }

    public override void Exit()
    {
        if (controlledObject != null)
        {
            Debug.Log($"{controlledObject.name} exiting Attack state.");
        }
    }
}