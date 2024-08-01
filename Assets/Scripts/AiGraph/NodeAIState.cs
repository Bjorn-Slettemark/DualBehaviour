using UnityEngine;
using XNode;

public class NodeAIState : NodeAI
{
    [Input(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.None)]
    public string input;

    [Output(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.None)]
    public string output;

    [SerializeField] private AiStateSO aiState;

    public AiStateSO AiState { get => aiState;  }

    public void SetAiState(AiStateSO _aiState)
    {
        aiState = _aiState;
    }
    
    public override void InitializeNode()
    {
        if (aiState != null)
        {
            aiState.Initialize(aiController.gameObject);
        }
    }

    protected override void OnActivate()
    {
        if (aiState != null)
        {
            aiState.Enter();
        }
        else
        {
            Debug.LogError($"No AiStateSO assigned to NodeAIState in {aiController.name}");
        }
    }

    protected override void OnUpdate()
    {
        if (aiState != null)
        {
            Debug.Log("Updating state");

            aiState.UpdateState();
        }
    }

    protected override void OnDeactivate()
    {
        if (aiState != null)
        {
            aiState.Exit();
        }
    }

    public void CompleteState()
    {
        SignalInputComplete();
        aiController.OnStateCompleted(this);
        aiGraph.DeactivateNode(this);
    }

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "output")
        {
            return aiState != null ? aiState.stateName : "Empty State";
        }
        return base.GetValue(port);
    }
}