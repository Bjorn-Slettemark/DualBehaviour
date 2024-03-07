
using UnityEngine;

public class MainMenuStateSO : GameStateSO
{
    public override void StateUpdate()
    {

    }
    public override void EnterState()
    {
        base.EnterState();
        Debug.Log("Entered mainmenu state");
    }
}
