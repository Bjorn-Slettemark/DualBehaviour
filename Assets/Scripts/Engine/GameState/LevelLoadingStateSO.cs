
using UnityEngine;

[CreateAssetMenu(fileName = "New LevelLoadingState", menuName = "Game States/LevelLoading")]
public class LevelLoadingStateSO : GameStateSO
{
    public override void EnterState()
    {
        base.EnterState();
        // Specific actions to initialize the state
    }

    public override void ExitState()
    {
        base.ExitState();
        // Clean up before leaving the state
    }

    public override void StateUpdate()
    {
        // Handle updates specific to the state
    }
}