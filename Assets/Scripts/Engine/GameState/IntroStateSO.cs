
using UnityEngine;

[CreateAssetMenu(fileName = "New IntroState", menuName = "Game States/Intro")]
public class IntroStateSO : GameStateSO
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