# Table of Contents
- \Dev\Unity\DualBehaviour\Assets\Scripts\GameStateManager.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AI\AiController.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AI\AISenseSystem.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AI\AIState.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AI\AiStateAttackSO.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AI\AiStateIdleSO.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AI\AiStateSO.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\AiGraph.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\NodeAI.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\NodeAiConditional.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\NodeAIEvent.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\NodeAILocalEvent.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\NodeAIState.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Common\HealthSystem.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\FloatReference.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\FloatVariable.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Level\DestructibleObject.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Level\DoorController.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Level\HideInGame.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Level\Spawnpoint.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Level\Trigger.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Level\WinPoint.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Player\BulletController.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Player\DynamicCameraFollow.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Player\GunController.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Player\PlayerController.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Player\PlayerDataSO.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Player\PlayerStatsSO.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\UI\GameControllButtons.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\UI\HealthBar.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\UI\LevelChangeButton.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\UI\UIManager.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AI\Agents\AIBehaviorMelee.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AI\Agents\AIBehaviorRanged.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AI\Agents\IAIBehavior.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AI\Helper\AIHelperFunctions.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AI\Weapon\EnemyGunController.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AI\Weapon\SwordController.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\Editor\AiGraphEditor.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\Editor\AiGraphEditorExtensions.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\Editor\NodeAiEditor.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\Editor\NodeAIEventEditor.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\Editor\NodeAiStateEditor.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\Core\DualBehaviour.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\EventSystem\GameEventChannelSO.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\EventSystem\GameEventListener.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\GameState\DefaultStateSO.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\GameState\GameOverStateSO.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\GameState\GameStateMachine.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\GameState\GameStateSO.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\GameState\InGameStateSO.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\GameState\IntroStateSO.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\GameState\LevelLoadingStateSO.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\GameState\LoadingStateSO.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\GameState\MainMenuStateSO.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\GameState\PauseStateSO.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\GameState\SavingStateSO.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\Level\GameLevelSO.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\Level\IngameGameLevelSO.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\Level\MainMenuGameLevelSO.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\SaveLoad\GameDataSO.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\SaveLoad\LevelSaveBlueprintSO.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\SaveLoad\MigrationConfigBaseSO.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\SaveLoad\MigrationConfig_v1_to_v2_SO.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\SaveLoad\SaveBlueprintSO.cs

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\GameStateManager.cs

- Extension: .cs
- Language: csharp
- Size: 4555 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum GameState
{
    Default,
    Intro,
    MainMenu,
    InGame,
    Pause,
    GameOver,
    Saving,
    Loading,
    LevelLoading
    // Add more states as needed
}
[Icon("Assets/Editor/Icons/GameManagerIcon.png")]
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public GameStateSO CurrentState { get => currentState; }

    [SerializeField] private GameState initialState; // Use the enum for the initial state
    private GameStateSO currentState;

    [SerializeField] private List<GameStateSO> gameStates = new List<GameStateSO>();

    // This dictionary maps GameState enums to their corresponding GameStateSO instances.
    private Dictionary<GameState, GameStateSO> stateDictionary = new Dictionary<GameState, GameStateSO>();
    [SerializeField] private GameStateSO defaultStateSO;
    public GameStateSO DefaultStateSO => defaultStateSO;


    public GameState[] AvailableGameStates
    {
        get { return gameStates.Where(gs => gs != null).Select(gs => gs.gameState).Distinct().ToArray(); }
    }

    public GameState InitialState { get ; }
    public List<GameStateSO> GameStates { get ;  }

    // Add a public method or property to access the GameState keys

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeStateDictionary();  // Make sure to initialize it here to handle editor and play mode initialization

        // Set to default state if initialState isn't available
        if (!stateDictionary.ContainsKey(initialState))
        {
            Debug.LogWarning($"Initial state '{initialState}' does not have a corresponding GameStateSO. Using DefaultStateSO.");
            currentState = defaultStateSO; // Use the defaultStateSO directly
        }
        else
        {
            ChangeState(initialState); // Change to initialState as usual
        }

        EventChannelManager.Instance.RegisterForAllChannels(this.gameObject, eventCheck);



    }
    void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        // Additional setup as necessary, ensuring it's editor-friendly.
    }
    private void eventCheck(string eventName)
    {
    }

    private void SetupSingleton()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject); // Use DestroyImmediate to handle cleanup directly in editor mode.
            return;
        }
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeStateDictionary()
    {
        if (gameStates == null)
        {
            Debug.LogError("gameStates list is null.");
            return;
        }

        foreach (var gameState in gameStates)
        {
            if (gameState == null)
            {
                Debug.LogError("Found a null GameStateSO in the gameStates list.");
                continue;
            }

            if (!stateDictionary.ContainsKey(gameState.gameState))
            {
                stateDictionary.Add(gameState.gameState, gameState);
            }
        }
    }


    public void ChangeState(GameState newStateEnum)
    {
        GameStateSO newState;
        if (!stateDictionary.TryGetValue(newStateEnum, out newState))
        {
            Debug.LogWarning($"GameState {newStateEnum} not found in state dictionary. Using DefaultStateSO.");
            newState = defaultStateSO; // Fallback to defaultStateSO
        }

        // Proceed with state transition
        currentState?.ExitState();
        currentState = newState;
        currentState?.EnterState();
    }

    private void Update()
    {
        currentState?.StateUpdate();
    }

    private void OnDisable()
    {
        EventChannelManager.Instance.UnregisterForAllChannels(this.gameObject);
        if (Instance == this)
        {
            Instance = null;  // Clear the static instance if this object is disabled
        }
    }
}
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AI\AiController.cs

- Extension: .cs
- Language: csharp
- Size: 1332 bytes
- Created: 2024-07-23 07:48:27
- Modified: 2024-08-02 08:23:22

### Code

```csharp
using UnityEngine;
using System.Collections.Generic;

public class AIController : MonoBehaviour
{
    public AiGraph aiGraph;

    private void Awake()
    {
        if (aiGraph != null)
        {
            aiGraph.ResetAllNodes();
        }
    }

    private void Start()
    {
        if (aiGraph != null)
        {
            InitializeAIGraph();
        }
        else
        {
            Debug.LogError("AIGraph not assigned to AIController!");
        }
    }

    private void InitializeAIGraph()
    {
        aiGraph.Initialize(this);
    }

    private void Update()
    {
        if (aiGraph != null)
        {
            aiGraph.UpdateNodes();
        }
    }

    public void OnStateCompleted(NodeAIState state)
    {
        aiGraph.OnStateCompleted(state);
    }

    public List<NodeAIState> GetActiveStates()
    {
        return aiGraph.GetActiveStates();
    }

    // Test method to activate a node
    public void TestNodeActivation()
    {
        Debug.Log("Testing node activation");
        if (aiGraph != null && aiGraph.nodes.Count > 0)
        {
            NodeAI firstNode = aiGraph.nodes[0] as NodeAI;
            if (firstNode != null)
            {
                aiGraph.ActivateNode(firstNode);
            }
        }
    }
}
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AI\AISenseSystem.cs

- Extension: .cs
- Language: csharp
- Size: 4480 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using UnityEngine;
using UnityEngine.AI;
using System;
public class AISenseSystem : MonoBehaviour
{
    public Transform player; // Assign the player's transform in the inspector
    public Vector3 playerPosition; // To store the last known position of the player

    public float visionRange = 10f; // How far the AI can see
    public float fieldOfView = 120f; // Field of view angle

    // Using System.Action for simplicity and direct invocation
    public Action OnPlayerSpotted;
    public Action OnPlayerVisible;
    public Action OnHitReceived;
    public Action OnPlayerLost;

    private bool playerInVisionRange = false; // Flag to track if player is within vision range
    private NavMeshAgent navMeshAgent;

    [SerializeField]
    private GameEventChannelSO playerEventChannel;
    private void Start()
    {

        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        VisionSense();
    }

    void VisionSense()
    {
        if (player == null) return;

        Vector3 dirToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(dirToPlayer, transform.forward);

        if (angle < fieldOfView / 2f && dirToPlayer.magnitude <= visionRange)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, dirToPlayer.normalized, out hit, visionRange))
            {
                if (hit.transform == player)
                {
                    playerPosition = player.position;
                    OnPlayerSpotted?.Invoke();
                    OnPlayerVisible?.Invoke();
                    playerInVisionRange = true;
                }
            }
        }
        else
        {
            if (playerInVisionRange)
            {
                playerInVisionRange = false;
                OnPlayerLost?.Invoke();
            }
        }
    }

    public bool PlayerVisible()
    {
        return playerInVisionRange;
    }

    public void ReceiveHit()
    {
        OnHitReceived?.Invoke();
    }
    // Visualize the AI's field of view and vision range in the Scene View
    private void OnDrawGizmos()
    {
        if (player == null) return;

        // Draw vision cone
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, (player.position - transform.position).normalized * visionRange);
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView / 2, 0) * transform.forward * visionRange;
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView / 2, 0) * transform.forward * visionRange;
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);

        // Draw vision range circle
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        // Draw debug visualization for NavMesh destination
        if (navMeshAgent != null && navMeshAgent.hasPath)
        {
            Debug.DrawLine(transform.position, navMeshAgent.destination, Color.blue);
            DebugDrawCircle(navMeshAgent.destination, 0.5f, Color.blue);
        }
    }

    // Helper method to draw a circle in the scene view
    private void DebugDrawCircle(Vector3 center, float radius, Color color)
    {
        int segments = 36;
        float angleIncrement = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);

        for (int i = 0; i < segments + 1; i++)
        {
            float angle = i * angleIncrement;
            Vector3 nextPoint = center + Quaternion.Euler(0, angle, 0) * new Vector3(radius, 0, 0);
            Debug.DrawLine(prevPoint, nextPoint, color);
            prevPoint = nextPoint;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("blimp!");
            OnHitReceived?.Invoke();
     
    }


    private void HandlePlayerSpawnEvent(string eventName)
    {
        if (eventName == "PlayerSpawned")
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

    }
    private void OnEnable()
    {
        playerEventChannel.RegisterListener(HandlePlayerSpawnEvent, "PlayerSpawned");
    }

    private void OnDisable()
    {
        playerEventChannel.UnregisterListener(HandlePlayerSpawnEvent);
    }
}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AI\AIState.cs

- Extension: .cs
- Language: csharp
- Size: 1027 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public struct Transition
{
    public string eventName;          // Event that triggers the transition
    public AIState targetState;       // State to transition to when the event is fired
}

[CreateAssetMenu(fileName = "AIState", menuName = "AI/State")]
public class AIState : ScriptableObject
{
    public LocalEventChannel listenChannel; // Channel to listen for events
    public string triggerEventName;         // Specific event name to trigger transitions

    public List<Transition> transitions = new List<Transition>();

    public Action OnEnter;
    public Action OnExit;
    public Action OnUpdate;

    public void Enter()
    {
        OnEnter?.Invoke();
        Debug.Log($"Entering state: {name}");
    }

    public void Exit()
    {
        OnExit?.Invoke();
        Debug.Log($"Exiting state: {name}");
    }

    public void Update()
    {
        OnUpdate?.Invoke();
    }
}
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AI\AiStateAttackSO.cs

- Extension: .cs
- Language: csharp
- Size: 782 bytes
- Created: 2024-07-23 08:12:34
- Modified: 2024-07-28 21:09:54

### Code

```csharp
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
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AI\AiStateIdleSO.cs

- Extension: .cs
- Language: csharp
- Size: 519 bytes
- Created: 2024-07-24 06:51:22
- Modified: 2024-07-28 22:39:16

### Code

```csharp
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
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AI\AiStateSO.cs

- Extension: .cs
- Language: csharp
- Size: 448 bytes
- Created: 2024-07-23 08:12:13
- Modified: 2024-07-29 05:43:15

### Code

```csharp
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
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\AiGraph.cs

- Extension: .cs
- Language: csharp
- Size: 5287 bytes
- Created: 2024-07-23 07:33:49
- Modified: 2024-08-08 09:35:59

### Code

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using System.Linq;
using UnityEditor;
using XNodeEditor;

public enum PriorityLevel
{
    Highest = 1,
    High = 2,
    Medium = 3,
    Low = 4,
    Lowest = 5
}
[CreateAssetMenu(fileName = "New AI Graph", menuName = "AI/Graph")]
public class AiGraph : NodeGraph
{
    private List<NodeAI> activeNodes = new List<NodeAI>();
    private List<NodeAIState> activeStates = new List<NodeAIState>();
    private int currentPriorityLevel = 5; // Default to lowest priority
    public int CurrentPriorityLevel => currentPriorityLevel;

    public event Action OnPriorityChanged;
    public event System.Action OnGraphChanged;

    private AIController aiController;
    private List<NodeAI> triggeredNodes = new List<NodeAI>();

    public void Initialize(AIController controller)
    {
        aiController = controller;
        SetCurrentPriorityLevel(5, true); // Reset to lowest priority
        Debug.Log($"Initializing AiGraph with {nodes.Count} nodes. Starting priority: {CurrentPriorityLevel}");

        foreach (NodeAI node in nodes)
        {
            if (node == null)
            {
                Debug.LogWarning("Encountered a null node in AiGraph.");
                continue;
            }

            try
            {
                Debug.Log($"Initializing node: {node.name} of type {node.GetType().Name}");
                node.InitializeNode(this, aiController);
                if (node is NodeAIEvent || node is NodeAILocalEvent)
                {
                    ActivateNode(node);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error initializing node {node.name}: {e.Message}\n{e.StackTrace}");
            }
        }
    }

    public void OnNodeActiveStateChanged()
    {
        Debug.Log("Node active state changed");
        if (XNodeEditor.NodeEditorWindow.current != null)
        {
            EditorApplication.delayCall += XNodeEditor.NodeEditorWindow.current.Repaint;
        }
        OnGraphChanged?.Invoke();
    }
    public void SetCurrentPriorityLevel(int level, bool forceSet = false)
    {
        if (forceSet || level < currentPriorityLevel)
        {
            currentPriorityLevel = Mathf.Clamp(level, 1, 5);
            Debug.Log($"Priority level set to: {currentPriorityLevel}");
            OnPriorityChanged?.Invoke();
        }
    }

    public void ActivateNode(NodeAI node)
    {
        Debug.Log($"AiGraph activating node: {node.name}");
        if (!activeNodes.Contains(node))
        {
            activeNodes.Add(node);
            node.Activate();
            OnNodeActiveStateChanged();

            if (node is NodeAIState stateNode)
            {
                ActivateState(stateNode);
            }
        }
    }

    public void TriggerNode(NodeAI node)
    {
        if (!triggeredNodes.Contains(node))
        {
            triggeredNodes.Add(node);
        }
    }

    public void UpdateNodes()
    {
        triggeredNodes = triggeredNodes.OrderBy(n => GetNodePriority(n)).ToList();

        foreach (var node in triggeredNodes)
        {
            node.Execute();
        }

        triggeredNodes.Clear();

        foreach (NodeAI node in activeNodes)
        {
            node.Update();
        }
    }

    private int GetNodePriority(NodeAI node)
    {
        if (node is NodeAIEvent eventNode)
        {
            return eventNode.overridePriority ? 0 : (int)eventNode.priorityLevel;
        }
        else if (node is NodeAILocalEvent localEventNode)
        {
            return localEventNode.overridePriority ? 0 : (int)localEventNode.priorityLevel;
        }
        return 5;
    }

    public void ResetAllNodes()
    {
        Debug.Log("Resetting all nodes");
        foreach (NodeAI node in nodes)
        {
            if (node != null)
            {
                node.ResetState();
            }
        }
        activeNodes.Clear();
        activeStates.Clear();
    }

    public void DeactivateNode(NodeAI node)
    {
        Debug.Log($"AiGraph deactivating node: {node.name}");
        if (activeNodes.Contains(node))
        {
            activeNodes.Remove(node);
            node.Deactivate();
            OnNodeActiveStateChanged();

            if (node is NodeAIState stateNode)
            {
                DeactivateState(stateNode);
            }
        }
    }

    private void ActivateState(NodeAIState state)
    {
        if (!activeStates.Contains(state))
        {
            activeStates.Add(state);
        }
    }

    private void DeactivateState(NodeAIState state)
    {
        activeStates.Remove(state);
    }

    public void OnStateCompleted(NodeAIState state)
    {
        state.TriggerOutputs();
        DeactivateState(state);
    }

    public List<NodeAIState> GetActiveStates()
    {
        return new List<NodeAIState>(activeStates);
    }
    public void SetNodeActive(NodeAI node, bool active)
    {
        node.IsActive = active;
        NodeEditorWindow.current.Repaint();
    }

}
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\NodeAI.cs

- Extension: .cs
- Language: csharp
- Size: 3143 bytes
- Created: 2024-07-26 08:11:18
- Modified: 2024-08-08 09:35:46

### Code

```csharp
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

public abstract class NodeAI : Node
{
    [HideInInspector] public AiGraph aiGraph;
    [HideInInspector] public AIController aiController;


    private bool _isActive;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive != value)
            {
                _isActive = value;
                if (NodeEditorWindow.current != null)
                {
                    NodeEditorWindow.current.Repaint();
                }
            }
        }
    }

    public void InitializeNode(AiGraph graph, AIController controller)
    {
        if (graph == null)
        {
            Debug.LogError("Graph is null in NodeAI.InitializeNode");
            return;
        }
        if (controller == null)
        {
            Debug.LogError("Controller is null in NodeAI.InitializeNode");
            return;
        }

        Debug.Log("NodeAi Initializing");
        aiGraph = graph;
        aiController = controller;
        InitializeNode();
    }

    public virtual void InitializeNode() { }

    public virtual void Execute()
    {
        Debug.Log($"Executing node: {name}");
        SignalInputComplete();
        TriggerOutputs();
    }

    public virtual void Activate()
    {
        Debug.Log($"Activating node: {name}");
        if (!IsActive)
        {
            IsActive = true;
            OnActivate();
        }
    }

    public virtual void ResetState()
    {
        Debug.Log($"Resetting state for node: {name}");
        IsActive = false;
    }

    public virtual void Deactivate()
    {
        Debug.Log($"Deactivating node: {name}");
        IsActive = false;
        OnDeactivate();
    }

    public virtual void Update()
    {
        if (IsActive)
        {
            OnUpdate();
        }
    }

    protected virtual void OnActivate() { }
    protected virtual void OnDeactivate() { }
    protected virtual void OnUpdate() { }

    protected void SignalInputComplete()
    {
        var inputPort = GetInputPort("input");
        if (inputPort != null)
        {
            foreach (var connection in inputPort.GetConnections())
            {
                if (connection.node is NodeAI nodeAI)
                {
                    aiGraph.DeactivateNode(nodeAI);
                }
            }
        }
    }

    public void TriggerOutputs()
    {
        Debug.Log($"Triggering outputs for {name}");
        var outputPort = GetOutputPort("output");
        if (outputPort != null)
        {
            foreach (var connection in outputPort.GetConnections())
            {
                if (connection.node is NodeAI nodeAI)
                {
                    Debug.Log($"Activating connected node: {nodeAI.name}");
                    aiGraph.ActivateNode(nodeAI);
                }
            }
        }
    }

    public override object GetValue(NodePort port)
    {
        return null;
    }
}
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\NodeAiConditional.cs

- Extension: .cs
- Language: csharp
- Size: 683 bytes
- Created: 2024-07-29 05:52:06
- Modified: 2024-07-29 10:20:09

### Code

```csharp
using System;
using System.Collections;
using UnityEngine;

public abstract class NodeAIConditional : NodeAI
{
    //protected abstract bool EvaluateCondition();

    //protected override void OnActivate()
    //{
    //    StartCoroutine(ConditionalCoroutine());
    //}

    //private IEnumerator ConditionalCoroutine()
    //{
    //    while (isActive && !EvaluateCondition())
    //    {
    //        yield return null;
    //    }

    //    if (isActive)
    //    {
    //        OnConditionMet();
    //    }
    //}

    //protected virtual void OnConditionMet()
    //{
    //    SignalInputComplete();
    //    TriggerOutputs();
    //}
}
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\NodeAIEvent.cs

- Extension: .cs
- Language: csharp
- Size: 2436 bytes
- Created: 2024-07-23 07:37:25
- Modified: 2024-08-08 09:14:29

### Code

```csharp
using UnityEngine;
using XNode;
using System.Collections.Generic;
using UnityEditor;

public class NodeAIEvent : NodeAI
{
    [Input(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.None)]
    public string input;

    [Output(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.None)]
    public string output;

    public GameEventChannelSO eventChannel;
    public string eventName;
    public  PriorityLevel priorityLevel = PriorityLevel.Lowest;
    public bool overridePriority = false;

    private bool isListening = false;

    public override void InitializeNode()
    {
        base.InitializeNode();
        SubscribeToEvent();
    }

    private void SubscribeToEvent()
    {
        if (eventChannel != null)
        {
            eventChannel.RegisterListener(OnEventRaised);
            isListening = true;
        }
    }
    private void OnEventRaised(string raisedEventName)
    {
        if (raisedEventName == eventName && (overridePriority || CanTriggerEvent()))
        {
            aiGraph.TriggerNode(this);
        }
    }

    public override void Execute()
    {
        Debug.Log($"NodeAIEvent {name} executed. Priority level: {(int)priorityLevel}");
        aiGraph.SetCurrentPriorityLevel((int)priorityLevel, overridePriority);
        base.Execute();
    }
    private bool CanTriggerEvent()
    {
        var inputPort = GetInputPort("input");
        if (inputPort != null && inputPort.GetConnections().Count > 0)
        {
            foreach (var connection in inputPort.GetConnections())
            {
                if (connection.node is NodeAI nodeAI && !nodeAI.IsActive)
                {
                    return false;
                }
            }
        }

        return overridePriority || (int)priorityLevel <= aiGraph.CurrentPriorityLevel;
    }

    private void TriggerEvent()
    {
        aiGraph.SetCurrentPriorityLevel((int)priorityLevel);
        Debug.Log($"NodeAIEvent {name} triggered. New priority level: {(int)priorityLevel}");
        SignalInputComplete();
        TriggerOutputs();

        // Notify Unity that the graph has changed
        EditorUtility.SetDirty(aiGraph);
    }
    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "output")
        {
            return eventName;
        }
        return null;
    }
}
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\NodeAILocalEvent.cs

- Extension: .cs
- Language: csharp
- Size: 3042 bytes
- Created: 2024-07-24 08:14:49
- Modified: 2024-08-08 06:42:48

### Code

```csharp
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNode;
public class NodeAILocalEvent : NodeAI
{
    [Input(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.None)]
    public string input;

    [Output(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.None)]
    public string output;

    [SerializeField] private string eventName;
    [SerializeField] private LocalEventChannel eventChannel;
    public PriorityLevel priorityLevel = PriorityLevel.Lowest;
    public bool overridePriority = false;

    private LocalEventHandler localEventHandler;

    public override void InitializeNode()
    {
        base.InitializeNode();
        SetupEventListener();
    }

    private void SetupEventListener()
    {
        if (!Application.isPlaying) return;

        localEventHandler = aiController.GetComponent<LocalEventHandler>();
        if (localEventHandler == null)
        {
            Debug.LogError($"LocalEventHandler not found on AIController for event: {eventName}");
            return;
        }

        Debug.Log($"Setting up listener for LocalEventNode: {eventName} on channel {eventChannel}");
        localEventHandler.RegisterListener(eventChannel, OnLocalEventRaised);
    }

    private void OnLocalEventRaised(string receivedEventName)
    {
        if (receivedEventName == eventName && (overridePriority || CanTriggerEvent()))
        {
            aiGraph.TriggerNode(this);
        }
    }

    public override void Execute()
    {
        Debug.Log($"NodeAIEvent {name} executed. Priority level: {(int)priorityLevel}");
        aiGraph.SetCurrentPriorityLevel((int)priorityLevel, overridePriority);
        base.Execute();
    }

    private bool CanTriggerEvent()
    {
        var inputPort = GetInputPort("input");
        if (inputPort != null && inputPort.GetConnections().Count > 0)
        {
            foreach (var connection in inputPort.GetConnections())
            {
                if (connection.node is NodeAI nodeAI && !nodeAI.IsActive)
                {
                    return false;
                }
            }
        }

        return overridePriority || (int)priorityLevel <= aiGraph.CurrentPriorityLevel;
    }

    private void TriggerEvent()
    {
        aiGraph.SetCurrentPriorityLevel((int)priorityLevel);
        Debug.Log($"NodeAILocalEvent {name} triggered. New priority level: {(int)priorityLevel}");
        SignalInputComplete();
        TriggerOutputs();

        // Notify Unity that the graph has changed
        EditorUtility.SetDirty(aiGraph);
    }

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "output")
        {
            return eventName;
        }
        return null;
    }

    private void OnDisable()
    {
        if (localEventHandler != null)
        {
            localEventHandler.UnregisterListener(eventChannel, OnLocalEventRaised);
        }
    }
}
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\NodeAIState.cs

- Extension: .cs
- Language: csharp
- Size: 1672 bytes
- Created: 2024-07-23 07:37:55
- Modified: 2024-08-08 08:35:22

### Code

```csharp
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
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Common\HealthSystem.cs

- Extension: .cs
- Language: csharp
- Size: 1005 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100f;
    [SerializeField]
    public float currentHealth;

    void Start()
    {
        // Initialize health when the game starts
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        // Reduce health by the damage amount
        currentHealth -= amount;

        // Check if health has dropped below zero
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (this.CompareTag("Player"))
        {
            //GameManager.Instance.PlayerDied();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Optional: Method to heal the AI
    public void Heal(float amount)
    {
        currentHealth += amount;
        // Ensure we do not exceed max health
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }
}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\FloatReference.cs

- Extension: .cs
- Language: csharp
- Size: 829 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
[System.Serializable]
public class FloatReference
{
    public bool UseConstant = true;
    public float ConstantValue;
    public FloatVariable Variable;

    public FloatReference(float value)
    {
        UseConstant = true;
        ConstantValue = value;
    }

    public float Value
    {
        get { return UseConstant ? ConstantValue : Variable.Value; }
    }

    public void SetValue(float value)
    {
        if (UseConstant)
        {
            ConstantValue = value;
        }
        else
        {
            Variable.SetValue(value);
        }
    }

    public void ApplyChange(float amount)
    {
        if (UseConstant)
        {
            ConstantValue += amount;
        }
        else
        {
            Variable.ApplyChange(amount);
        }
    }
}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\FloatVariable.cs

- Extension: .cs
- Language: csharp
- Size: 442 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "New FloatVariable", menuName = "Variables/FloatVariable")]
public class FloatVariable : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif
    public float Value;

    public void SetValue(float value)
    {
        Value = value;
    }

    public void ApplyChange(float amount)
    {
        Value += amount;
    }
}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Level\DestructibleObject.cs

- Extension: .cs
- Language: csharp
- Size: 187 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp

using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    private void Start()
    {
        
    }

    private void OnDestroy()
    {
        
    }
}


```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Level\DoorController.cs

- Extension: .cs
- Language: csharp
- Size: 2494 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float slideDistance = 5f; // How far the door slides down
    public float slideSpeed = 3f; // Speed of the sliding motion
    public float openDuration = 5f; // Time in seconds the door stays open before closing

    private Vector3 closedPosition; // Starting position of the door, assumed to be 'closed'
    private Vector3 openPosition; // Target position when 'open'
    private bool isOpening = false; // Is the door currently opening?
    private bool isClosing = false; // Is the door currently closing?
    private float openTimer = 0f; // Timer to track how long the door stays open

    void Start()
    {
        closedPosition = transform.position; // Initialize closedPosition to the starting position
        openPosition = new Vector3(transform.position.x, transform.position.y - slideDistance, transform.position.z); // Calculate the open position
    }

    void Update()
    {
        if (isOpening)
        {
            // Smoothly translate the door to the open position
            transform.position = Vector3.MoveTowards(transform.position, openPosition, slideSpeed * Time.deltaTime);
            if (transform.position == openPosition)
            {
                isOpening = false; // Stop opening when the door reaches the open position
                openTimer = openDuration; // Reset and start the open timer
            }
        }
        else if (isClosing)
        {
            // Smoothly translate the door to the closed position
            transform.position = Vector3.MoveTowards(transform.position, closedPosition, slideSpeed * Time.deltaTime);
            if (transform.position == closedPosition)
            {
                isClosing = false; // Stop closing when the door reaches the closed position
            }
        }

        // Timer countdown
        if (openTimer > 0)
        {
            openTimer -= Time.deltaTime;
            if (openTimer <= 0)
            {
                // Time's up, start closing the door
                isClosing = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpening && !isClosing)
        {
            isOpening = true; // Trigger the door to start opening
            // No need to change the openTimer here as it's reset when the door fully opens
        }
    }
}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Level\HideInGame.cs

- Extension: .cs
- Language: csharp
- Size: 704 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class HideInGame : MonoBehaviour
{
    private MeshRenderer meshRenderer;

    private void Start()
    {
        // Get the MeshRenderer component attached to this GameObject
        meshRenderer = GetComponent<MeshRenderer>();

        // Check if we are in the Unity Editor
        if (!Application.isPlaying)
        {
            // If in the Unity Editor, ensure the MeshRenderer is enabled
            meshRenderer.enabled = true;
        }
        else
        {
            // If in play mode, disable the MeshRenderer to hide it in the game
            meshRenderer.enabled = false;
        }
    }
}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Level\Spawnpoint.cs

- Extension: .cs
- Language: csharp
- Size: 3133 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("Spawn Settings")]
    public List<GameObject> enemyPrefabs; // List of enemy prefabs to spawn
    public int spawnCount = 5;
    public float spawnRadius = 5f;
    public bool aggressive = true; // Flag to set spawned enemies' aggressive behavior

    [Header("Respawn Timer")]
    public bool respawnTimerEnabled = false;
    public float respawnDelay = 10f;
    public bool respawnWhenAllDead = true;
    public int maxRespawns = -1;

    [Header("Spawn on Trigger")]
    public bool spawnOnTrigger = false;
    public GameObject triggerObject;

    private int currentRespawnCount = 0;
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private Coroutine respawnCoroutine = null;

    private void Start()
    {
        if (!spawnOnTrigger)
        {
            SpawnEnemies();
        }
    }

    private void SpawnEnemies()
    {
        if (maxRespawns != -1 && currentRespawnCount >= maxRespawns) return;

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
            spawnPosition.y = transform.position.y;
            GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            spawnedEnemies.Add(spawnedEnemy);

            // Set aggressive behavior if the enemy has the IAIBehavior interface or AIBehavior base class
            IAIBehavior enemyBehavior = spawnedEnemy.GetComponent<IAIBehavior>();
            if (enemyBehavior != null)
            {
                enemyBehavior.aggressive = aggressive;
            }
        }
        currentRespawnCount++;

        if (respawnTimerEnabled && (maxRespawns == -1 || currentRespawnCount < maxRespawns))
        {
            if (respawnCoroutine != null) StopCoroutine(respawnCoroutine);
            respawnCoroutine = StartCoroutine(RespawnCoroutine());
        }
    }

    IEnumerator RespawnCoroutine()
    {
        while (maxRespawns == -1 || currentRespawnCount < maxRespawns)
        {
            yield return new WaitForSeconds(respawnDelay);
            if (respawnWhenAllDead && spawnedEnemies.Exists(enemy => enemy != null))
            {
                continue; // Wait more if any enemy is still alive
            }
            spawnedEnemies.RemoveAll(enemy => enemy == null); // Clean up dead enemies from the list
            SpawnEnemies();
        }
    }

    public void OnPlayerEnterTrigger(GameObject trigger)
    {
        if (trigger == triggerObject && spawnOnTrigger && (respawnCoroutine == null || !respawnTimerEnabled))
        {
            SpawnEnemies();
        }
    }

    private void OnDrawGizmos()
    {
        // Visualize the spawn area
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Level\Trigger.cs

- Extension: .cs
- Language: csharp
- Size: 698 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp

using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Trigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering collider is tagged as "Player"
        if (other.CompareTag("Player"))
        {
            // Find all SpawnPoint components in the scene
            SpawnPoint[] spawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);

            // Notify each SpawnPoint that the player has entered the trigger
            foreach (SpawnPoint spawnPoint in spawnPoints)
            {
                spawnPoint.OnPlayerEnterTrigger(this.gameObject);
            }
        }
    }


}


```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Level\WinPoint.cs

- Extension: .cs
- Language: csharp
- Size: 4491 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using UnityEngine;
using System.Collections.Generic;

public class WinPoint : MonoBehaviour
{
    [Header("Win Conditions")]
    public bool enterTriggerToWin = false; // Enter a trigger to win
    public bool killAllEnemiesToWin = false; // Kill all enemies to win
    public bool winAfterTimelimit = false; // Win within a certain time limit
    public float timeLimit = 60f; // Time limit to win (in seconds)

    [Header("Object Destruction Conditions")]
    public bool destroyObjectsToWin = false; // Enable this win condition
    public List<GameObject> objectsToDestroy = new List<GameObject>();

    [Header("Win Condition Mode")]
    public bool requireAllActiveConditions = true; // If true, all conditions must be met to win. If false, any condition will suffice.

    [Header("Internal Variables")]
    private bool isLevelWon = false;
    private float startTime;
    private bool triggerEntered = false; // Flag to indicate the player has entered the win trigger
    private int totalEnemiesToKill;
    private int enemiesLeftToKill;
    private void Start()
    {
       // totalEnemiesToKill = GameManager.Instance.totalEnemies.Count;
        
        if (winAfterTimelimit)
        {
            startTime = Time.time;
        }
    }
    private void Update()
    {
        //enemiesLeftToKill = totalEnemiesToKill - GameManager.instance.enemiesKilled;

        if (!isLevelWon && CheckWinConditions())
        {
            WinLevel();
        }
    }


    // Called when an enemy is killed
    public void OnEnemyKilled()
    {
        // Check if killing all enemies is a win condition
        if (killAllEnemiesToWin && AreAllEnemiesKilled())
        {
            WinLevel();
        }
    }

    // Check if all enemies are killed
    private bool AreAllEnemiesKilled()
    {
        //if (GameManager.Instance.enemiesKilled == GameManager.instance.totalEnemies.Count)
        //{
        //    return true;
        //}

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && enterTriggerToWin)
        {
            triggerEntered = true; // Mark this win condition as met

            // If not requiring all conditions, win immediately upon entering the trigger
            if (!requireAllActiveConditions)
            {
                WinLevel();
            }
            else
            {
                // If requiring all conditions, the regular check will handle if this was the last needed condition
            }
        }
    }



    private void WinLevel()
    {
        if (!isLevelWon) // Check to prevent multiple calls
        {
            Debug.Log("Level Won!");
            isLevelWon = true;
            //GameManager.Instance.OnPlayerWin();
            // Additional logic for handling level completion can be added here if needed
        }
    }
    private bool AreAllObjectsDestroyed()
    {
        objectsToDestroy.RemoveAll(item => item == null); // Clean up any null references

        if (objectsToDestroy.Count == 0)
        {
            return true;
        }

        return false;
    }

    private bool CheckWinConditions()
    {
        // Track how many conditions are met
        int conditionsMetCount = 0;
        int activeConditionsCount = 0;

        // Check each condition

        // Trigger entry condition
        if (enterTriggerToWin)
        {
            activeConditionsCount++;
            if (triggerEntered)
            {
                conditionsMetCount++;
            }
        }

        // Killing all enemies condition
        if (killAllEnemiesToWin)
        {
            activeConditionsCount++;
            if (AreAllEnemiesKilled()) conditionsMetCount++;
        }

        // Destroying objects condition
        if (destroyObjectsToWin)
        {
            activeConditionsCount++;
            if (AreAllObjectsDestroyed()) conditionsMetCount++;
        }

        // Add additional condition checks here

        // If require all conditions, check that all active conditions are met
        if (requireAllActiveConditions)
        {
            return conditionsMetCount == activeConditionsCount;
        }
        else // If not requiring all, check that at least one condition is met
        {
            return conditionsMetCount > 0;
        }
    }


}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Player\BulletController.cs

- Extension: .cs
- Language: csharp
- Size: 1555 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 20f;
    public float damage = 10f; // The damage this bullet will deal
    public float maxRange = 10f; // Maximum distance the bullet can travel

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position; // Store the starting position of the bullet
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime); // Use Vector3 for 3D movement

        // Check if the bullet has exceeded its maximum range
        if (Vector3.Distance(startPosition, transform.position) > maxRange)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider hitInfo)
    {
        // Check if the hit object has a HealthSystem component
        AISenseSystem aISense = hitInfo.GetComponent<AISenseSystem>();

        HealthSystem healthSystem = hitInfo.GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            // Damage the hit object
            healthSystem.TakeDamage(damage);
        }
        if (aISense != null)
        {
            // Damage the hit object
            aISense.OnHitReceived?.Invoke();
        }
        // Destroy the bullet after hitting something
        Destroy(gameObject);
    }


    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Player\DynamicCameraFollow.cs

- Extension: .cs
- Language: csharp
- Size: 5547 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicCameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    public float damping = 1;
    public float lookAheadFactor = 3;
    public float lookAheadReturnSpeed = 0.5f;
    public float lookAheadMoveThreshold = 0.1f;
    public bool averagePositionEnabled = false;
    public float averagePositionWeight = 0.5f;
    public Vector3 cameraOffset;
    public bool lockToXZPlane = false;

    [Header("Zoom Settings")]
    public float zoomSpeed = 10f;
    public float minZoom = 5f;
    public float maxZoom = 20f;

    [Header("X Offset Zoom Settings")]
    public float minZOffset = -10f;
    public float maxZOffset = 10f;

    [Header("Perspective Bias")]
    public AnimationCurve perspectiveBiasCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public float perspectiveBiasScale = 1.0f;

    private Vector3 m_LastTargetPosition;
    private Vector3 m_CurrentVelocity;
    private Vector3 m_LookAheadPos;
    private Camera m_Camera;
    private Vector3 lastValidMouseWorldPosition;

    [Header("Event Channels")]
    public GameEventChannelSO spawnEventChannel; // Assign in the inspector

    private void OnEnable()
    {
        spawnEventChannel.RegisterListener(HandleEvent, "PlayerSpawned");
    }

    private void OnDisable()
    {
        spawnEventChannel.UnregisterListener(HandleEvent);
    }
    private void Start()
    {
        m_Camera = Camera.main;

        // Initialize cameraOffset.y to start zoomed out
        cameraOffset.y = maxZoom;

        // If you want to also set an initial Z Offset based on the max zoom, do it here
        float initialZoomFactor = (maxZoom - minZoom) / (maxZoom - minZoom); // This will be 1, but it's shown for consistency
        cameraOffset.z = Mathf.Lerp(minZOffset, maxZOffset, initialZoomFactor);


    }


    public void InitializeCamera(Transform newTarget)
    {
        target = newTarget;
        Vector3 originalOffsetFromOrigin = transform.position - Vector3.zero;
        transform.position = target.position + originalOffsetFromOrigin;
        m_LastTargetPosition = target.position;
    }

    private void Update()
    {
        if (target == null) return;

        // Handle zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cameraOffset.y -= scroll * zoomSpeed;
        cameraOffset.y = Mathf.Clamp(cameraOffset.y, minZoom, maxZoom);

        // Adjust X Offset based on Zoom
        float zoomFactor = (cameraOffset.y - minZoom) / (maxZoom - minZoom);
        cameraOffset.z = Mathf.Lerp(minZOffset, maxZOffset, zoomFactor);

        Vector3 moveDelta = target.position - m_LastTargetPosition;
        bool updateLookAheadTarget = moveDelta.magnitude > lookAheadMoveThreshold;

        if (updateLookAheadTarget)
        {
            m_LookAheadPos = lookAheadFactor * new Vector3(moveDelta.x, 0, moveDelta.z);
        }
        else
        {
            m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
        }

        Vector3 aheadTargetPos = target.position + m_LookAheadPos + cameraOffset;

        // Apply perspective bias
        float relativeZ = (target.position - transform.position).z;
        float bias = perspectiveBiasCurve.Evaluate(Mathf.InverseLerp(-perspectiveBiasScale, perspectiveBiasScale, relativeZ));
        aheadTargetPos += Vector3.forward * bias * perspectiveBiasScale;

        if (averagePositionEnabled)
        {
            Vector3 averagePosition = CalculateAveragePosition(aheadTargetPos);
            aheadTargetPos = Vector3.Lerp(aheadTargetPos, averagePosition, averagePositionWeight);
        }

        Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);
        transform.position = newPos;

        m_LastTargetPosition = target.position;
    }

    private Vector3 CalculateAveragePosition(Vector3 aheadTargetPos)
    {
        Vector3 averagePosition;
        // Check if the EventSystem.current is not null and then if the mouse is over a UI element
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            // Return the last valid average position if the mouse is over UI
            averagePosition = (target.position + lastValidMouseWorldPosition) / 2;
            return averagePosition;
        }

        Vector3 mousePosition = Input.mousePosition;
        // Ensure m_Camera is not null before using it
        if (m_Camera == null)
        {
            Debug.LogWarning("Camera is not assigned in DynamicCameraFollow script.");
            return aheadTargetPos; // Return aheadTargetPos or some default value to avoid breaking the flow
        }
        mousePosition.z = m_Camera.WorldToScreenPoint(target.position).z;
        Vector3 mouseWorldPosition = m_Camera.ScreenToWorldPoint(mousePosition);

        // Update lastValidMouseWorldPosition with the current valid mouseWorldPosition
        lastValidMouseWorldPosition = mouseWorldPosition;

        averagePosition = (target.position + mouseWorldPosition) / 2;
        return averagePosition;
    }
    private void HandleEvent(string eventName)
    {
        if (eventName == "PlayerSpawned")
        {
            InitializeCamera(GameObject.FindGameObjectWithTag("Player").transform);
        }
        
    }
}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Player\GunController.cs

- Extension: .cs
- Language: csharp
- Size: 2118 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float fireRate = 1f;
    public float damage = 10f;
    public float rotationSpeed = 10f;

    private float timeUntilFire = 0f;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
       RotateGunTowardsMouse();
    }

    public void AttemptShoot()
    {
        // Check if the current time is greater than or equal to the time until the next shot is allowed
        if (Time.time >= timeUntilFire)
        {
            // Update timeUntilFire to the current time plus the interval derived from fireRate
            timeUntilFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }


    void RotateGunTowardsMouse()
    {
        // Create a plane at the gun's position facing up
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        // Generate a ray from the cursor position
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // Determine the point where the cursor ray intersects the plane
        float hitDist;
        if (groundPlane.Raycast(ray, out hitDist))
        {
            // Find the point along the ray that hits the calculated distance
            Vector3 targetPoint = ray.GetPoint(hitDist);

            // Determine the target rotation. This is the rotation if the gun looks at the target point
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);

            // Set the gun's rotation to this new rotation but only rotate around Y axis
            targetRotation.x = 0;
            targetRotation.z = 0;

            // Smoothly rotate towards the target point
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void Shoot()
    {
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }
}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Player\PlayerController.cs

- Extension: .cs
- Language: csharp
- Size: 3899 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private CharacterController characterController;

    public GunController gunController; // Reference to the GunController component

    private Vector3 movement;
    private bool positionLoaded = false;

    [SerializeField]
    private GameEventChannelSO saveEventChannel;
    [SerializeField]
    private GameEventChannelSO loadEventChannel;
    [SerializeField]
    private PlayerDataSO playerData;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Subscribe to the event when game data is loaded. Adjust "GameEventChannel" and "GameDataLoaded" as needed.
        if (EventChannelManager.Instance != null)
        {
            EventChannelManager.Instance.RegisterEvent(gameObject, saveEventChannel, "PlayerSave", SavePlayerData);

            EventChannelManager.Instance.RegisterEvent(gameObject, loadEventChannel, "PlayerData", UpdatePlayerPositionFromEventData);
        }
    }

    void Update()
    {

        // Check if position has been loaded, to skip one update cycle
        if (positionLoaded)
        {
            positionLoaded = false;
            return;
        }

        //playerData.playerPosition = this.transform.position;

        // Input handling for movement in 3D
        movement.x = Input.GetAxis("Horizontal");
        movement.z = Input.GetAxis("Vertical");

        if (Input.GetButton("Fire1") && Time.timeScale != 0)
        {
            gunController.AttemptShoot();
        }

        // Apply gravity manually
        if (!characterController.isGrounded)
        {
            movement.y += Physics.gravity.y * Time.deltaTime;
        }
        else
        {
            movement.y = 0f; // Reset the Y movement when grounded to prevent accumulating gravity
        }

        // Move the player using the CharacterController
        characterController.Move(movement * moveSpeed * Time.deltaTime);

        // Handle rotation to face the direction of movement
        HandleRotation();
    }

    void SavePlayerData(string eventName)
    {
        Debug.Log(eventName + "!!!!");
        playerData.playerPosition = this.transform.position;
        SaveLoadManager.Instance.SaveGame(playerData);
    }
    // This method will be called when the event is raised
    void UpdatePlayerPositionFromEventData(string gameDataName)
    {
        positionLoaded = false;

        // Assuming gameDataName can be used to identify or retrieve the relevant PlayerData
        // This part needs customization based on how your GameDataSOs are structured and how PlayerData is retrieved
        Debug.Log("LOAD!!" + playerData.playerPosition);
        ;

        if (playerData != null)
        {
            //SaveLoadManager.Instance.(playerData)
            LoadPosition(playerData.playerPosition);
        }
    }
    void HandleRotation()
    {
        Vector3 direction = new Vector3(movement.x, 0, movement.z);
        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime * 100);
        }
    }

    // Method to apply force to the player
    public void ApplyForce(Vector3 force)
    {
        characterController.Move(force * Time.deltaTime);
    }
    public void LoadPosition(Vector3 loadedPosition)
    {
        characterController.enabled = false; // Disable to set position outside bounds
        transform.position = loadedPosition;
        characterController.enabled = true; // Re-enable
        positionLoaded = true;
    }
}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Player\PlayerDataSO.cs

- Extension: .cs
- Language: csharp
- Size: 303 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Game/Player Data", order = 1)]
public class PlayerDataSO : GameDataSO
{
    [SerializeField]
    public GameObject playerPrefab;
    [SerializeField]
    public Vector3 playerPosition;
    public string dataName;
}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Player\PlayerStatsSO.cs

- Extension: .cs
- Language: csharp
- Size: 2086 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "Game/Player Stats", order = 0)]
public class PlayerStatsSO : ScriptableObject
{
    [Header("Health")]
    public float maxHealth = 100f;
    [SerializeField]
    private float health;

    [Header("Quantum Energy")]
    public float maxQuantumLevel = 100f;
    [SerializeField]
    private float quantumLevel;

    [Header("Movement")]
    public float maxSpeed = 3f;
    public float sprintSpeedMultiplier = 2f;
    public float deceleration = 5.0f;
    public float acceleration = 10.0f;

    [Header("Dash")]
    public float dashForce = 10.0f;
    public float dashDuration = 0.5f;
    public float dashCooldown = 2f;

    [Header("Inventory")]
    public int offensiveSlots = 1;
    public int defensiveSlots = 1;
    public int modifierSlots = 1;

    public delegate void PlayerDeathHandler();
    public event PlayerDeathHandler OnPlayerDeath;

    public float Health
    {
        get => health;
        private set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);
                if (health <= 0)
                {
                    OnPlayerDeath?.Invoke();
                }
            }
        }
    }

    public float QuantumLevel
    {
        get => quantumLevel;
        private set => quantumLevel = Mathf.Clamp(value, 0, maxQuantumLevel);
    }

    private void OnEnable()
    {
        ResetStats();
    }

    public void ResetStats()
    {
        Health = maxHealth;
        QuantumLevel = maxQuantumLevel;
    }

    public void IncreaseQuantumLevel(float amount)
    {
        QuantumLevel += amount;
    }

    public void DecreaseQuantumLevel(float amount)
    {
        QuantumLevel -= amount;
    }

    public void TakeDamage(float amount)
    {
        Debug.Log("Taking damage: " + amount);
        Health -= amount;
    }

    public void Heal(float amount)
    {
        Health += amount;
    }
}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\UI\GameControllButtons.cs

- Extension: .cs
- Language: csharp
- Size: 540 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControlButtons : MonoBehaviour
{
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reloads the current scene
        Time.timeScale = 1; // Ensure the game is unpaused
    }

    public void ExitGame()
    {
        // If running in the Unity editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // Quit the game
#endif
    }
}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\UI\HealthBar.cs

- Extension: .cs
- Language: csharp
- Size: 1140 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public PlayerStatsSO playerStats; // Reference to the HealthSystem component of the player
    public RectTransform barFill; // Reference to the fill image of the health bar

    private void Start()
    {
        if (playerStats == null)
        {
            Debug.LogError("HealthSystem component not found on the player!");
            return;
        }

        // Update the health bar initially
        UpdateHealthBar();
    }

    private void Update()
    {
        // Continuously update the health bar to reflect any changes in the player's health
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        // Calculate the fill amount based on the current health percentage
        float fillAmount = playerStats.Health / playerStats.maxHealth;

        // Clamp the fill amount between 0 and 1
        fillAmount = Mathf.Clamp01(fillAmount);

        // Update the width of the fill image of the health bar
        barFill.localScale = new Vector3(fillAmount, 1f, 1f);
    }
}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\UI\LevelChangeButton.cs

- Extension: .cs
- Language: csharp
- Size: 392 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-28 22:04:01

### Code

```csharp
using UnityEngine;

public class LevelChangeButton : MonoBehaviour
{
    [SerializeField]
    private GameLevelSO desiredLevelSO;
    public void ChangeLevel(GameLevelSO eventLevelSO)
    {

            LevelLoaderManager.Instance.LoadLevel(eventLevelSO);

       

        // Assuming GameManager has a method to change levels that accepts a level name or identifier
    }
}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\UI\UIManager.cs

- Extension: .cs
- Language: csharp
- Size: 2151 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject gameOverScreen;
    public GameObject victoryScreen;
    public GameObject mainMenuScreen;

    public GameEventListener Listener;

    private void Awake()
    {
        Listener.Response.AddListener(HandleUIEvent);
    }

    private void Start()
    {
        gameOverScreen.SetActive(false);
        victoryScreen.SetActive(false);
        mainMenuScreen.SetActive(false);
        Time.timeScale = 1;
    }

    private void Update()
    {
        // Check if the Escape key was pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Toggle the main menu visibility
            ToggleMainMenu();
        }
    }

    private void OnEnable()
    {
        //GameManager.PlayerIsDead += ShowGameOverScreen;
        //GameManager.PlayerWon += ShowVictoryScreen;
    }

    private void OnDisable()
    {
        //GameManager.PlayerIsDead -= ShowGameOverScreen;
        //GameManager.PlayerWon -= ShowVictoryScreen;
    }

    private void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0;

    }

    private void ShowVictoryScreen()
    {
        victoryScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void ShowMainMenu(bool show)
    {
        mainMenuScreen.SetActive(show);
        Time.timeScale = show ? 0 : 1; // Pause the game when the main menu is shown
    }

    // Method to toggle the main menu
    private void ToggleMainMenu()
    {
        // Check the current state of the main menu screen and toggle it
        bool isMainMenuActive = mainMenuScreen.activeSelf;
        ShowMainMenu(!isMainMenuActive);
    }

    private void HandleUIEvent(string eventName)
    {
        switch (eventName)
        {
            case "Test":
                Debug.Log("Test pressed");
                break;
            case "SettingsOpened":
                // Code to open settings
                break;
                // Add cases for other event names as needed
        }
    }
}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AI\Agents\AIBehaviorMelee.cs

- Extension: .cs
- Language: csharp
- Size: 3704 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using MonsterLove.StateMachine;
using UnityEngine;
using UnityEngine.AI;

public class AIBehaviorMelee : MonoBehaviour, IAIBehavior
{
    public enum States
    {
        Spawning,
        Idle,
        Chasing,
        Attacking,
        Cooldown
    }

    [Header("Enemy Settings")]
    public Transform player;
    public float chaseSpeed = 3f;
    public float rotationSpeed = 1f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public float attackDuration = 2f;

    private StateMachine<States, Driver> fsm;

    private NavMeshAgent navMeshAgent;
    private SwordController swordController;
    public bool aggressive { get; set; }

    private void Awake()
    {
        fsm = new StateMachine<States, Driver>(this);
        fsm.ChangeState(States.Spawning);
    }

    private void Start()
    {
        AISenseSystem AISense = GetComponent<AISenseSystem>();

        AISense.OnPlayerVisible += fsm.Driver.OnPlayerVisible.Invoke;
        AISense.OnPlayerLost += fsm.Driver.OnPlayerVisible.Invoke;

        if (aggressive) { fsm.Driver.OnAggressive.Invoke();  }

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = chaseSpeed;

        swordController = GetComponentInChildren<SwordController>();


       

        if (navMeshAgent == null)
        {
            Debug.LogError("Missing NavMeshAgent component on enemy.", this);
        }
    }
    private void Update()
    {
        fsm.Driver.Update.Invoke(); //Tap the state machine into Unity's update loop. We could choose to call this from anywhere though!
    }
    public class Driver
    {
        public StateEvent OnPlayerVisible;
        public StateEvent OnPlayerLost;
        public StateEvent OnhitReceived;
        public StateEvent OnAggressive;
        public StateEvent Update;
    }
    

    // Draw debug graphics in editor mode
    private void OnDrawGizmos()
    {
        // Draw attack range sphere
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);


    }
    void OnGUI()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2); // Adjust the Vector3.up * 2 to position the text above the AI
        screenPosition.y = Screen.height - screenPosition.y; // Convert to GUI coordinates

        GUIStyle style = new GUIStyle();
        style.normal.textColor = aggressive ? Color.red : Color.white; // Change text color based on the aggressive state
        style.alignment = TextAnchor.MiddleCenter;

        GUI.Label(new Rect(screenPosition.x - 50, screenPosition.y - 25, 100, 50), fsm.State.ToString(), style);
    }


    #region Finite State Machine

    private void Spawning_Enter()
    {
        fsm.ChangeState(States.Idle);
    }

    private void Idle_OnAggressive()
    {
        fsm.ChangeState(States.Chasing);
    }

    private void Idle_OnPlayerVisible()
    {
        fsm.ChangeState(States.Chasing);
    }
    private void Idle_Exit()
    {
    }
    private void Chasing_Enter()
    {
    }

    private void Chasing_Update()
    {
        navMeshAgent.SetDestination(player.position);


        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            fsm.ChangeState(States.Attacking);
        }
    }

    private void Attacking_Enter()
    {
        swordController.StartSwinging();
        Invoke("EndAttackCooldown", attackDuration);
    }


    private void EndAttackCooldown()
    {
        fsm.ChangeState(States.Chasing);
    }

    #endregion

}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AI\Agents\AIBehaviorRanged.cs

- Extension: .cs
- Language: csharp
- Size: 5225 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
    using System.Collections;
    using UnityEngine;
    using UnityEngine.AI;
    using MonsterLove.StateMachine;

    public class AIBehaviorRanged : MonoBehaviour, IAIBehavior
    {
        public enum States
        {
            Idle,
            Patrol,
            Chasing,
            Attacking,
            Dodging,
            Cooldown
        }

        [Header("Movement Settings")]
        public float speed = 3f;
        public float rotationSpeed = 1f;
        public float dodgeDuration = 2f;

        [Header("Patrol Settings")]
        public float minPatrolDistance = 10f;
        public float maxPatrolDistance = 20f;
        public float idleDuration = 2f;

        [Header("Attack Settings")]
        public float attackDuration = 5f;
        public float attackCooldown = 5f;
        public float attackRange = 10f;
        public float dodgingCirclingSpeed = 100f;

        public bool aggressive { get; set; }

        private NavMeshAgent navMeshAgent;
        private AISenseSystem aiSense;
        private EnemyGunController enemyGunController;

        private StateMachine<States, Driver> fsm;
        //private bool isAttacking = false;
        //private bool isOnAttackCooldown = false;
        private Vector3 patrolDestination;

        private void Awake()
        {
            fsm = new StateMachine<States, Driver>(this);
            fsm.ChangeState(States.Idle);
        }

        private void Start()
        {
            AISenseSystem AISense = GetComponent<AISenseSystem>();

            AISense.OnHitReceived += fsm.Driver.OnHitReceived.Invoke;
            AISense.OnPlayerVisible += fsm.Driver.OnPlayerVisible.Invoke;
            AISense.OnPlayerLost += fsm.Driver.OnPlayerVisible.Invoke;

            if (aggressive) { fsm.Driver.OnAggressive.Invoke(); }

            navMeshAgent = GetComponent<NavMeshAgent>();
            aiSense = GetComponent<AISenseSystem>();
            enemyGunController = GetComponentInChildren<EnemyGunController>();

            navMeshAgent.speed = speed;
            //navMeshAgent.updateRotation = false; // Disable NavMeshAgent automatic rotation    }
        }


        private void Update()
        {
            fsm.Driver.Update.Invoke();
        }


        public class Driver
        {
            public StateEvent OnPlayerVisible;
            public StateEvent OnAggressive;

            public StateEvent OnHitReceived;
            public StateEvent Update;
        }

        #region StateMachineMethods
        private void Idle_OnPlayerVisible()
        {
            fsm.ChangeState(States.Chasing);
        }
        private void Idle_OnAggressive()
        {
            fsm.ChangeState(States.Chasing);
        }

        private void Idle_OnHitReceived()
        {
            fsm.ChangeState(States.Chasing);
        }
        private void Patrol_Enter()
        {
            SetNewPatrolDestination();
        }
        private void Patrol_Update()
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                SetNewPatrolDestination();
            }
        }
        private void Chasing_Enter()
        {
        }

        private void Chasing_Update()
        {
            navMeshAgent.SetDestination(aiSense.player.position);

            if (Vector3.Distance(transform.position, aiSense.player.position) <= attackRange)
            {
                fsm.ChangeState(States.Attacking);
            }
        }

        private void Attacking_Update()
        {
            navMeshAgent.SetDestination(this.transform.position);

            enemyGunController.AttemptShoot(); // Initiates the attack, EnemyGunController manages firing rate
            Invoke("EndAttack", attackDuration);
        }

        private void EndAttack()
        {
            fsm.ChangeState(States.Chasing);
        }

        #endregion

        private void SetNewPatrolDestination()
        {
            patrolDestination = AIUtility.GetRandomPoint(transform.position, maxPatrolDistance);
            if (patrolDestination != Vector3.zero)
            {
                navMeshAgent.SetDestination(patrolDestination);
            }
        }

        private void OnDrawGizmos()
        {
            // Draw attack range sphere
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);


        }
        void OnGUI()
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2); // Adjust the Vector3.up * 2 to position the text above the AI
            screenPosition.y = Screen.height - screenPosition.y; // Convert to GUI coordinates

            GUIStyle style = new GUIStyle();
            style.normal.textColor = aggressive ? Color.red : Color.white; // Change text color based on the aggressive state
            style.alignment = TextAnchor.MiddleCenter;

            GUI.Label(new Rect(screenPosition.x - 50, screenPosition.y - 25, 100, 50), fsm.State.ToString(), style);
        }

    }

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AI\Agents\IAIBehavior.cs

- Extension: .cs
- Language: csharp
- Size: 69 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
public interface IAIBehavior
{
    bool aggressive { get; set; }
}
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AI\Helper\AIHelperFunctions.cs

- Extension: .cs
- Language: csharp
- Size: 1013 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using UnityEngine;
using UnityEngine.AI;

public static class AIUtility
{
    public static void FaceTarget(Transform agentTransform, Vector3 targetPosition, float rotationSpeed)
    {
        Vector3 directionToTarget = targetPosition - agentTransform.position;
        directionToTarget.y = 0; // Ensure the rotation is only on the Y axis
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
        agentTransform.rotation = Quaternion.Slerp(agentTransform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
    // Returns a random point within a specified distance from a given center.
    public static Vector3 GetRandomPoint(Vector3 center, float distance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance + center;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, distance, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return Vector3.zero;
    }
}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AI\Weapon\EnemyGunController.cs

- Extension: .cs
- Language: csharp
- Size: 2119 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using UnityEngine;

public class EnemyGunController : MonoBehaviour
{
    public Transform firePoint;
    public GameObject projectilePrefab;
    public AISenseSystem aiSense; // Reference to the AISense component
    public float fireRate = 1f;
    public float rotationSpeed = 10f;
    public float damage = 10f; // Damage output of the bullet

    private float timeUntilFire = 0f;

    private void Start()
    {
        // Get reference to the AISense component attached to the same GameObject
        aiSense = gameObject.GetComponentInParent<AISenseSystem>();
    }

    void Update()
    {
        if (aiSense != null) // Check if the player is visible to the AI
        {
            AimTowardsPlayer();
        }
    }

    void AimTowardsPlayer()
    {
        if (aiSense.player != null) // Ensure player reference is not null
        {
            Vector3 directionToPlayer = aiSense.player.position - transform.position;
            directionToPlayer.y = 0; // Remove vertical component to ensure we're only rotating on the Y axis

            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }


    public void AttemptShoot()
    {
        // Check if it's time to fire based on the fire rate
        if (Time.time >= timeUntilFire)
        {
            timeUntilFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (firePoint != null && projectilePrefab != null)
        {
            // Instantiate a projectile at the fire point
            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            // Set the bullet's damage
            BulletController bulletController = bullet.GetComponent<BulletController>();
            if (bulletController != null)
            {
                bulletController.SetDamage(damage); // Set the bullet's damage
            }
        }
    }
}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AI\Weapon\SwordController.cs

- Extension: .cs
- Language: csharp
- Size: 1559 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using UnityEngine;

public class SwordController : MonoBehaviour
{
    public Transform attackPoint; // Point from where the sword will attack
    public float swingSpeed = -1000f;
    public float damage = 20f;
    public float knockbackForce = 100f;
    public float attackRange = 2f;

    private bool isSwinging = false;

    void Update()
    {
        if (isSwinging)
        {
            // Perform swinging motion
            transform.RotateAround(attackPoint.position, Vector3.up, swingSpeed * Time.deltaTime);
        }
    }

    public void StartSwinging()
    {
        isSwinging = true;
        Invoke("StopSwinging", 1.0f); // Swing for 1 second
    }

    private void StopSwinging()
    {
        isSwinging = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Damage the player
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // Knock back the player if it has a PlayerController component
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
                playerController.ApplyForce(knockbackDirection * knockbackForce);
            }
        }
    }

}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\Editor\AiGraphEditor.cs

- Extension: .cs
- Language: csharp
- Size: 2608 bytes
- Created: 2024-07-23 07:39:01
- Modified: 2024-08-08 09:34:42

### Code

```csharp
using UnityEngine;
using UnityEditor;
using XNodeEditor;

[CustomNodeGraphEditor(typeof(AiGraph))]
public class AIGraphEditor : NodeGraphEditor
{
    private AiGraph aiGraph;


    public override void OnOpen()
    {
        base.OnOpen();
        window.titleContent.text = "AI Graph";
        aiGraph = target as AiGraph;
        if (aiGraph != null)
        {
            aiGraph.OnGraphChanged += OnGraphChanged;
        }
    }


    private void OnGraphChanged()
    {
        if (NodeEditorWindow.current != null)
        {
            NodeEditorWindow.current.Repaint();
        }
    }

    public override void OnGUI()
    {
        base.OnGUI();

        // Draw the info bar
        DrawInfoBar();
    }

    private void DrawInfoBar()
    {
        AiGraph graph = target as AiGraph;
        if (graph == null) return;

        float infoBarHeight = 24; // Increased height for better visibility
        Rect infoBarRect = new Rect(0, window.position.height - infoBarHeight, window.position.width, infoBarHeight);

        // Draw the grey background
        EditorGUI.DrawRect(infoBarRect, new Color(0.3f, 0.3f, 0.3f, 1)); // Dark grey color

        GUILayout.BeginArea(infoBarRect);
        GUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace(); // Push the priority display to the right

        // Display current priority
        GUIStyle style = new GUIStyle(EditorStyles.label);
        style.alignment = TextAnchor.MiddleRight;
        style.normal.textColor = GetPriorityColor(graph.CurrentPriorityLevel);
        style.fontSize = 12; // Slightly larger font
        style.fontStyle = FontStyle.Bold; // Make it bold

        GUILayout.Label($"Current Priority: {(PriorityLevel)graph.CurrentPriorityLevel}", style, GUILayout.Height(infoBarHeight));

        // Add some padding to the right
        GUILayout.Space(10);

        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        // Force repaint to update the info bar
        window.Repaint();
    }

    public override string GetNodeMenuName(System.Type type)
    {
        if (type == typeof(NodeAIEvent)) return "Events/Event";
        if (type == typeof(NodeAIState)) return "AI/State";
        if (type == typeof(NodeAILocalEvent)) return "Events/LocalEvent";

        return null; // Don't show in context menu
    }

    private Color GetPriorityColor(int priorityLevel)
    {
        float t = (priorityLevel - 1) / 4f; // Normalize to 0-1 range
        return Color.Lerp(Color.red, Color.green, t);
    }



}
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\Editor\AiGraphEditorExtensions.cs

- Extension: .cs
- Language: csharp
- Size: 436 bytes
- Created: 2024-08-05 11:39:20
- Modified: 2024-08-05 11:40:12

### Code

```csharp
using UnityEngine;
using UnityEditor;
using XNodeEditor;

[InitializeOnLoad]
public static class AiGraphEditorExtension
{
    static AiGraphEditorExtension()
    {
        EditorApplication.update += Update;
    }

    private static void Update()
    {
        if (NodeEditorWindow.current != null && NodeEditorWindow.current.graph != null)
        {
            NodeEditorWindow.current.Repaint();
        }
    }
}
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\Editor\NodeAiEditor.cs

- Extension: .cs
- Language: csharp
- Size: 1355 bytes
- Created: 2024-07-28 22:31:39
- Modified: 2024-08-08 10:48:19

### Code

```csharp
using UnityEngine;
using UnityEditor;
using XNodeEditor;

[CustomNodeEditor(typeof(NodeAI))]
public class NodeAIEditor : NodeEditor
{
    private static GUIStyle editorLabelStyle;

    public override void OnHeaderGUI()
    {
        NodeAI node = target as NodeAI;
        string title = node.name;
        if (string.IsNullOrEmpty(title)) title = node.GetType().Name;

        // Create a new style based on the default node header style
        GUIStyle headerStyle = new GUIStyle(NodeEditorResources.styles.nodeHeader);

        // Set the text color based on the node's active state
        headerStyle.normal.textColor = node.IsActive ? Color.blue : Color.red;

        // Draw the header with the custom style
        GUILayout.Label(title, headerStyle, GUILayout.Height(30));
    }

    public override Color GetTint()
    {
        NodeAI node = target as NodeAI;
        // You can customize the tint color based on the node's state
        return node.IsActive ? new Color(0.8f, 0.8f, 1f) : base.GetTint();
    }

    public override void OnBodyGUI()
    {
            if (editorLabelStyle == null) editorLabelStyle = new GUIStyle(EditorStyles.label);
        EditorStyles.label.normal.textColor = Color.white;
        base.OnBodyGUI();
        EditorStyles.label.normal = editorLabelStyle.normal;
    }
}
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\Editor\NodeAIEventEditor.cs

- Extension: .cs
- Language: csharp
- Size: 892 bytes
- Created: 2024-08-01 07:48:12
- Modified: 2024-08-08 09:24:44

### Code

```csharp
using static UnityEngine.GraphicsBuffer;
using static XNodeEditor.NodeEditor;
using UnityEditor;
using XNodeEditor;

[CustomNodeEditor(typeof(NodeAIEvent))]
public class NodeAIEventEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        serializedObject.Update();

        NodeAIEvent nodeAIEvent = target as NodeAIEvent;

        // Draw default inspector properties
        base.OnBodyGUI();


        serializedObject.ApplyModifiedProperties();
    }
}

[CustomNodeEditor(typeof(NodeAILocalEvent))]
public class NodeAILocalEventEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        serializedObject.Update();

        NodeAILocalEvent nodeAILocalEvent = target as NodeAILocalEvent;

        // Draw default inspector properties
        base.OnBodyGUI();

 
        serializedObject.ApplyModifiedProperties();
    }
}
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\Editor\NodeAiStateEditor.cs

- Extension: .cs
- Language: csharp
- Size: 4395 bytes
- Created: 2024-07-24 06:47:28
- Modified: 2024-07-29 10:28:55

### Code

```csharp
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using XNodeEditor;

[CustomNodeEditor(typeof(NodeAIState))]
public class NodeAiStateEditor : NodeEditor
{
    private NodeAIState stateNode;
    private string[] stateTypeNames;
    private Type[] stateTypes;
    private int selectedIndex = -1;
    private Editor stateLogicEditor;
    private bool stateTypesInitialized = false;

    public override void OnCreate()
    {
        base.OnCreate();
        stateNode = target as NodeAIState;
        if (stateNode == null)
        {
            Debug.LogError("Failed to initialize AiStateNode");
        }
        InitializeStateTypes();
    }

    public override void OnBodyGUI()
    {

        if (stateNode == null)
        {
            stateNode = target as NodeAIState;
            if (stateNode == null)
            {
                EditorGUILayout.HelpBox("Failed to initialize AiStateNode", MessageType.Error);
                return;
            }
        }

        serializedObject.Update();
        NodeEditorGUILayout.PortField(stateNode.GetPort("input"));
        NodeEditorGUILayout.PortField(stateNode.GetPort("output"));
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("State Logic", EditorStyles.boldLabel);

        if (!stateTypesInitialized)
            InitializeStateTypes();

        EditorGUI.BeginChangeCheck();
        selectedIndex = EditorGUILayout.Popup("Select State Logic", selectedIndex, stateTypeNames);
        if (EditorGUI.EndChangeCheck() && selectedIndex != -1)
        {
            SetStateLogic(stateTypes[selectedIndex]);
        }

        if (stateNode.AiState != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("State Logic Properties", EditorStyles.boldLabel);

            if (stateLogicEditor == null || stateLogicEditor.target != stateNode.AiState)
            {
                if (stateLogicEditor != null)
                    UnityEngine.Object.DestroyImmediate(stateLogicEditor);
                stateLogicEditor = Editor.CreateEditor(stateNode.AiState);
            }

            stateLogicEditor.OnInspectorGUI();
        }

        serializedObject.ApplyModifiedProperties();



        if (GUI.changed)
        {
            EditorUtility.SetDirty(stateNode);
            EditorUtility.SetDirty(stateNode.graph);
        }
    }

    private void InitializeStateTypes()
    {
        if (stateTypesInitialized) return;

        try
        {
            stateTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type != null && typeof(AiStateSO).IsAssignableFrom(type) && !type.IsAbstract)
                .ToArray();

            stateTypeNames = stateTypes.Select(t => t.Name).ToArray();

            if (stateNode.AiState != null)
            {
                selectedIndex = Array.FindIndex(stateTypes, t => t == stateNode.AiState.GetType());
            }
            else if (selectedIndex == -1 && stateTypes.Length > 0)
            {
                selectedIndex = 0;
            }

            stateTypesInitialized = true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error initializing state types: {e.Message}");
            stateTypesInitialized = false;
        }
    }

    private void SetStateLogic(Type stateType)
    {
        if (stateNode.AiState != null && stateNode.AiState.GetType() == stateType)
            return;

        Undo.RecordObject(stateNode, "Change State Logic");

        if (stateNode.AiState != null)
        {
            Undo.DestroyObjectImmediate(stateNode.AiState);
        }

        AiStateSO newStateLogic = ScriptableObject.CreateInstance(stateType) as AiStateSO;
        newStateLogic.name = stateType.Name;

        stateNode.SetAiState(newStateLogic);

        AssetDatabase.AddObjectToAsset(newStateLogic, stateNode.graph);

        EditorUtility.SetDirty(stateNode);
        EditorUtility.SetDirty(stateNode.graph);

        AssetDatabase.SaveAssets();
    }

    private void OnDisable()
    {
        if (stateLogicEditor != null)
        {
            UnityEngine.Object.DestroyImmediate(stateLogicEditor);
        }
    }
}
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\Core\DualBehaviour.cs

- Extension: .cs
- Language: csharp
- Size: 356 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using UnityEngine;

public class DualBehaviour : MonoBehaviour
{
    GameStateManager GameStateManager = GameStateManager.Instance;
    LevelLoaderManager LevelLoaderManager = LevelLoaderManager.Instance;
    EventChannelManager EventChannelManager = EventChannelManager.Instance;
    SaveLoadManager SaveLoadManager = SaveLoadManager.Instance;
}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\EventSystem\GameEventChannelSO.cs

- Extension: .cs
- Language: csharp
- Size: 2701 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-29 16:40:50

### Code

```csharp
using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "New GameEventChannel", menuName = "Events/Game Event Channel")]
public class GameEventChannelSO : ScriptableObject
{


    private const string AllEventsKey = "ALL"; // Special key for listeners interested in all events
    private Dictionary<Action<string>, string> listeners = new Dictionary<Action<string>, string>();

    // Raise an event to all listeners interested in uiEventName, or to those listening to all events (AllEventsKey)
    public void RaiseEvent(string uiEventName)
    {
        foreach (var listener in listeners)
        {
            if (listener.Value == uiEventName || listener.Value == AllEventsKey)
            {
                listener.Key?.Invoke(uiEventName);
            }
        }
    }

    // Register a listener for a specific event or all events
    public void RegisterListener(Action<string> listener, string eventName = AllEventsKey)
    {
        if (!listeners.ContainsKey(listener))
        {
            listeners.Add(listener, eventName);
        }
        else
        {
            // Optionally update the listener's event name if re-registering
            listeners[listener] = eventName;
        }
    }

    // Unregister a listener
    public void UnregisterListener(Action<string> listener)
    {
        if (listeners.ContainsKey(listener))
        {
            listeners.Remove(listener);
        }
    }

    // Optional: Register a listener for all events without specifying an event name
    public void RegisterListenerForAllEvents(Action<string> listener)
    {
        RegisterListener(listener, AllEventsKey);
    }

    // Optional: Unregister a listener from all events, more explicit in usage
    public void UnregisterListenerFromAllEvents(Action<string> listener)
    {
        UnregisterListener(listener);
    }

    // Debugging utility to log all registered listeners and the events they are registered for
    public void DebugRegisteredListeners()
    {
        foreach (var listener in listeners)
        {
            Debug.Log($"Listener: {listener.Key.Method.DeclaringType}.{listener.Key.Method.Name}, Event: {listener.Value}");
        }
    }

    // Return a list of tuples containing the delegate (listener) and the event name they're registered for
    public List<(Delegate Listener, string EventName)> GetListenersInfo()
    {
        var infoList = new List<(Delegate, string)>();
        foreach (var listener in listeners)
        {
            infoList.Add((listener.Key, listener.Value));
        }
        return infoList;
    }
}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\EventSystem\GameEventListener.cs

- Extension: .cs
- Language: csharp
- Size: 663 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GameEventResponse : UnityEvent<string> { }

public class GameEventListener : MonoBehaviour
{
    public GameEventChannelSO EventChannel;
    public GameEventResponse Response;
    public List<string> eventName;
    private void OnEnable()
    {
        EventChannel.RegisterListener(OnEventRaised, eventName[0]);
    }

    private void OnDisable()
    {
        EventChannel.UnregisterListener(OnEventRaised);
    }

    private void OnEventRaised(string uiEventName)
    {
        Response.Invoke(uiEventName);
    }
}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\GameState\DefaultStateSO.cs

- Extension: .cs
- Language: csharp
- Size: 538 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp

using UnityEngine;

[CreateAssetMenu(fileName = "New IngameState", menuName = "Game States/Default")]
public class DefaultState : GameStateSO
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

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\GameState\GameOverStateSO.cs

- Extension: .cs
- Language: csharp
- Size: 524 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp

using UnityEngine;

[CreateAssetMenu(fileName = "New GameOverState", menuName = "Game States/GameOver")]
public class GameOverStateSO : GameStateSO
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
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\GameState\GameStateMachine.cs

- Extension: .cs
- Language: csharp
- Size: 365 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp

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

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\GameState\GameStateSO.cs

- Extension: .cs
- Language: csharp
- Size: 830 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class GameStateSO : ScriptableObject
{
    public GameState gameState;
    [SerializeField]
    private GameEventChannelSO gameStateEventChannel;

    // Method to be called when entering the state
    public virtual void EnterState()
    {
        // Use gameState.ToString() to ensure dynamic state name is passed
        EventChannelManager.Instance.RaiseEvent(gameStateEventChannel, gameState.ToString());
    }

    // Method to be called when exiting the state
    public virtual void ExitState()
    {
        EventChannelManager.Instance.RaiseEvent(gameStateEventChannel, "ExitState");
    }

    // Abstract method for state-specific functionality
    public abstract void StateUpdate();

}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\GameState\InGameStateSO.cs

- Extension: .cs
- Language: csharp
- Size: 536 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp

using UnityEngine;

[CreateAssetMenu(fileName = "New IngameState", menuName = "Game States/Ingame")]
public class IngameState : GameStateSO
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

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\GameState\IntroStateSO.cs

- Extension: .cs
- Language: csharp
- Size: 515 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp

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
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\GameState\LevelLoadingStateSO.cs

- Extension: .cs
- Language: csharp
- Size: 536 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp

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
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\GameState\LoadingStateSO.cs

- Extension: .cs
- Language: csharp
- Size: 521 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp

using UnityEngine;

[CreateAssetMenu(fileName = "New LoadingState", menuName = "Game States/Loading")]
public class LoadingStateSO : GameStateSO
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
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\GameState\MainMenuStateSO.cs

- Extension: .cs
- Language: csharp
- Size: 524 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp

using UnityEngine;

[CreateAssetMenu(fileName = "New MainMenuState", menuName = "Game States/MainMenu")]
public class MainMenuStateSO : GameStateSO
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
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\GameState\PauseStateSO.cs

- Extension: .cs
- Language: csharp
- Size: 515 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp

using UnityEngine;

[CreateAssetMenu(fileName = "New PauseState", menuName = "Game States/Pause")]
public class PauseStateSO : GameStateSO
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
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\GameState\SavingStateSO.cs

- Extension: .cs
- Language: csharp
- Size: 518 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp

using UnityEngine;

[CreateAssetMenu(fileName = "New SavingState", menuName = "Game States/Saving")]
public class SavingStateSO : GameStateSO
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
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\Level\GameLevelSO.cs

- Extension: .cs
- Language: csharp
- Size: 981 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using UnityEngine;

public abstract class GameLevelSO : ScriptableObject
{


    public string sceneName;

    public string gameLevelName;

    public GameObject loadingScreenPrefab;

    [SerializeField]
    private GameEventChannelSO levelEventChannel;

    public virtual GameObject GetLoadingScreenPrefab()
    {
        return loadingScreenPrefab;
    }
    // Method to be called when entering the state
    public virtual void EnterLevel()
    {
        EventChannelManager.Instance.RaiseEvent(levelEventChannel, "EnterLevel");
        // Additional enter logic
    }

    // Method to be called when exiting the state
    public virtual void ExitLevel()
    {
        EventChannelManager.Instance.RaiseEvent(levelEventChannel, "ExitLevel");
        // Additional exit logic
    }

    // Abstract method for state-specific functionality
    public abstract void LevelUpdate();
    // Abstract method for state-specific functionality
}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\Level\IngameGameLevelSO.cs

- Extension: .cs
- Language: csharp
- Size: 553 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "IngameGameLevel", menuName = "GameLevel/IngameGameLevel", order = 1)]
public class IngameGameLevelSO : GameLevelSO
{

    [SerializeField]
    private GameEventChannelSO playerEventChannel;

    public override void EnterLevel()
    {
        base.EnterLevel();
        Debug.Log("Entering level" + gameLevelName + " , spawning player");
        EventChannelManager.Instance.RaiseEvent(playerEventChannel, "SpawnPlayer");
    }
    public override void LevelUpdate()
    {
    }


}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\Level\MainMenuGameLevelSO.cs

- Extension: .cs
- Language: csharp
- Size: 315 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp

using UnityEngine;

[CreateAssetMenu(fileName = "GameLevel", menuName = "GameLevel/MainMenuGameLevel", order = 1)]
public class MainMenuGameLevelSO : GameLevelSO
{
    public override void LevelUpdate()
    {
    }

    public override void EnterLevel()
    {
        base.EnterLevel();
    }


}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\SaveLoad\GameDataSO.cs

- Extension: .cs
- Language: csharp
- Size: 869 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using UnityEngine;
using System.Collections.Generic;
public interface IGameData
{
    string DataName { get; set; }
    int DataVersion { get; set; }
    string UserId { get; set; }
    string SaveTimestamp { get; set ; }
}

[CreateAssetMenu(fileName = "GameData", menuName = "Save Load/GameData", order = 1)]
public class GameDataSO : ScriptableObject, IGameData
{
    public virtual string DataName { get ; set ; }
    public virtual int DataVersion { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public virtual string UserId { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public virtual string SaveTimestamp { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }



}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\SaveLoad\LevelSaveBlueprintSO.cs

- Extension: .cs
- Language: csharp
- Size: 986 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSaveBlueprint", menuName = "Game/SaveBlueprintSO")]
public class LevelSaveBlueprintSO : SaveBlueprintSO
{
    // Define any other parameters necessary for saving/loading

    // Method to execute save logic
    public new void Save()
    {

        for (int i = 0; i < gameDataSOList.Count; i++)
        {
            SaveLoadManager.Instance.SaveGame(gameDataSOList[i]);
        }
        // Custom save logic here
        //SaveLoadManager.Instance.SaveGame(gameDataSO[0]);
    }

    // Method to execute load logic
    public new void Load()
    {
        for (int i = 0; i < gameDataSOList.Count; i++)
        {
            SaveLoadManager.Instance.LoadGame(gameDataSOList[i]);
        }
        // Custom load logic here
        //SaveLoadManager.Instance.LoadGame(gameDataSO[0]);
        postLoadGameState = GameState.InGame;
    }
}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\SaveLoad\MigrationConfigBaseSO.cs

- Extension: .cs
- Language: csharp
- Size: 394 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using UnityEngine;

public abstract class MigrationConfigBaseSO : ScriptableObject
{
    // Define any common fields or methods that all migration configurations will share
    // This could include settings or parameters needed for migration

    // Define an abstract method for migration that derived classes must implement
    public abstract void Migrate(GameDataSO gameData);
}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\SaveLoad\MigrationConfig_v1_to_v2_SO.cs

- Extension: .cs
- Language: csharp
- Size: 513 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "MigrationConfig_v1_to_v2", menuName = "Save Load/Migration Configuration/Version 1 to Version 2")]
public class MigrationConfig_v1_to_v2_SO : MigrationConfigBaseSO
{
    public override void Migrate(GameDataSO gameData)
    {
        // Implement migration logic from version 1 to version 2
        // Example:
        // If you need to add a new field to the game data, you can initialize it here
        // gameData.newField = defaultValue;
    }
}

```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\SaveLoad\SaveBlueprintSO.cs

- Extension: .cs
- Language: csharp
- Size: 989 bytes
- Created: 2024-07-22 07:36:43
- Modified: 2024-07-22 07:36:43

### Code

```csharp
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "NewSaveConfigSO", menuName = "Game/SaveConfigSO")]
public class SaveBlueprintSO : ScriptableObject
{
    public List<GameDataSO> gameDataSOList; // Reference to the specific game data
    [SerializeField]
    private GameEventChannelSO saveEventChannel;
    [SerializeField]
    private GameEventChannelSO loadEventChannel;

    public GameState postLoadGameState;
    // Define any other parameters necessary for saving/loading

    // Method to execute save logic
    public void Save()
    {
        // Custom save logic here
        EventChannelManager.Instance.RaiseEvent(saveEventChannel, "PlayerSave");
    }

    // Method to execute load logic
    public void Load()
    {
        // Custom load logic here
        // Example: SaveLoadManager.Instance.LoadGame(gameDataSO);
        EventChannelManager.Instance.RaiseEvent(loadEventChannel, "PlayerData");

    }
}

```

