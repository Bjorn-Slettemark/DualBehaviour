
using UnityEngine;

[CreateAssetMenu(fileName = "New MainMenuState", menuName = "Game States/Main Menu")]
public class MainMenuState : GameStateSO
{
    public override void EnterState()
    {
        base.EnterState();
        // Specific actions to initialize the main menu
    }

    public override void ExitState()
    {
        base.ExitState();
        // Clean up before leaving the main menu
    }

    public override void StateUpdate()
    {
        // Handle updates specific to the main menu state
    }
}
