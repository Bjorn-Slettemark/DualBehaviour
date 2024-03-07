
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public GameStateSO currentState;

    private void Update()
    {
        currentState?.StateUpdate();
    }

    public void ChangeState(GameStateSO newState)
    {
        currentState?.ExitState();
        currentState = newState;
        currentState.EnterState();
    }
}
