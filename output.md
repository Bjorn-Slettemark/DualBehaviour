# Table of Contents
- \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\FloatReference.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\Engine\FloatVariable.cs
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
- \Dev\Unity\DualBehaviour\Assets\Scripts\AI\AiController.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AI\AISenseSystem.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AI\AIState.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AI\AiStateAttackSO.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AI\AiStateIdleSO.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AI\AiStateSO.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AI\Agents\AIBehaviorMelee.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AI\Agents\AIBehaviorRanged.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AI\Agents\IAIBehavior.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AI\Helper\AIHelperFunctions.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AI\Weapon\EnemyGunController.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AI\Weapon\SwordController.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\AiGraph.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\NodeAI.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\NodeAiConditional.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\NodeAIEntry.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\NodeAIEvent.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\NodeAILocalEvent.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\NodeAIState.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\Editor\AiGraphEditor.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\Editor\NodeAiEditor.cs
- \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\Editor\NodeAiStateEditor.cs

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

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AI\AiController.cs

- Extension: .cs
- Language: csharp
- Size: 921 bytes
- Created: 2024-07-23 07:48:27
- Modified: 2024-07-29 17:14:14

### Code

```csharp
using System.Collections.Generic;
using UnityEngine;

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
        aiGraph.UpdateNodes();
    }

    public void OnStateCompleted(NodeAIState state)
    {
        aiGraph.OnStateCompleted(state);
    }

    // New method to get active states
    public List<NodeAIState> GetActiveStates()
    {
        return aiGraph.GetActiveStates();
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

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\AiGraph.cs

- Extension: .cs
- Language: csharp
- Size: 2816 bytes
- Created: 2024-07-23 07:33:49
- Modified: 2024-07-29 17:12:17

### Code

```csharp
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using XNode;

public class AiGraph : NodeGraph
{
    private List<NodeAI> activeNodes = new List<NodeAI>();
    private List<NodeAIState> activeStates = new List<NodeAIState>();
    private AIController aiController;

    public void Initialize(AIController controller)
    {
        aiController = controller;
        UnityEngine.Debug.Log($"AIController {aiController} initialized in AiGraph!");
        UnityEngine.Debug.Log($"Number of nodes in graph: {nodes.Count}");

        foreach (NodeAI node in nodes)
        {
            if (node == null)
            {
                UnityEngine.Debug.Log($"Null node found in AiGraph");
                continue;
            }
            UnityEngine.Debug.Log($"Initializing node: {node.GetType().Name}");
            node.InitializeNode(this, aiController);

            // If it's an EntryNode, it will activate itself and its outputs
            if (node is NodeAIEntry entryNode)
            {
                UnityEngine.Debug.Log($"Found EntryNode: {entryNode.name}");
            }
        }
    }

    public void UpdateNodes()
    {
        foreach (NodeAI node in activeNodes)
        {
            node.Update();
        }
    }
    public void ResetAllNodes()
    {
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
    public void ActivateNode(NodeAI node)
    {
        if (!activeNodes.Contains(node))
        {
            UnityEngine.Debug.Log($"AiGraph activating node: {node.name}");
            activeNodes.Add(node);
            node.Activate();

            if (node is NodeAIState stateNode)
            {
                ActivateState(stateNode);
            }
        }
    }

    public void DeactivateNode(NodeAI node)
    {
        if (activeNodes.Contains(node))
        {
            activeNodes.Remove(node);
            node.Deactivate();

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
}
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\NodeAI.cs

- Extension: .cs
- Language: csharp
- Size: 2697 bytes
- Created: 2024-07-26 08:11:18
- Modified: 2024-07-29 17:12:00

### Code

```csharp
using System.Collections.Generic;
using UnityEngine;
using XNode;

public abstract class NodeAI : Node
{
    [HideInInspector] public AiGraph aiGraph;
    [HideInInspector] public AIController aiController;

    private bool isActive = false;

    public bool IsActive { get => isActive;  }

    public void InitializeNode(AiGraph graph, AIController controller)
    {
        if (graph == null)
        {
            UnityEngine.Debug.LogError("Graph is null in NodeAI.InitializeNode");
            return;
        }
        if (controller == null)
        {
            UnityEngine.Debug.LogError("Controller is null in NodeAI.InitializeNode");
            return;
        }

        Debug.Log("NodeAi Initializing");
        aiGraph = graph;
        aiController = controller;
        InitializeNode();
    }

    public virtual void InitializeNode() { }

    public virtual void Activate()
    {
        if (!isActive)
        {
            isActive = true;
            Debug.Log($"Activating node: {name}");
            OnActivate();
        }
    }
    public virtual void ResetState()
    {
        isActive = false;
        // Add any other state reset logic here
    }

    public virtual void Deactivate()
    {
        isActive = false;
        OnDeactivate();
    }

    public virtual void Update()
    {
        if (isActive)
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
        return null; // You might want to return something meaningful here depending on your use case
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

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\NodeAIEntry.cs

- Extension: .cs
- Language: csharp
- Size: 772 bytes
- Created: 2024-07-29 16:55:16
- Modified: 2024-07-29 16:55:56

### Code

```csharp
using UnityEngine;
using XNode;

public class NodeAIEntry : NodeAI
{
    [Output(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.None)]
    public string output;

    public override void InitializeNode()
    {
        base.InitializeNode();
        Debug.Log($"EntryNode {name} initialized");
        // Activate this node and its outputs immediately
        aiGraph.ActivateNode(this);
        TriggerOutputs();
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        Debug.Log($"EntryNode {name} activated");
    }

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "output")
        {
            return "Entry";
        }
        return null;
    }
}
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\NodeAIEvent.cs

- Extension: .cs
- Language: csharp
- Size: 1732 bytes
- Created: 2024-07-23 07:37:25
- Modified: 2024-07-29 17:08:56

### Code

```csharp
using UnityEngine;
using XNode;
using System.Collections.Generic;

public class NodeAIEvent : NodeAI
{
    [Input(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.None)]
    public string input;

    [Output(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.None)]
    public string output;

    public GameEventChannelSO eventChannel;
    public string eventName;

    private bool isListening = false;

    public override void InitializeNode()
    {
        // Don't activate immediately
        Debug.Log($"Initialized NodeAIEvent: {name}");
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        if (!isListening)
        {
            Debug.Log($"{name} Activated, registering event");
            eventChannel.RegisterListener(OnEventRaised);
            isListening = true;
        }
    }



    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        if (isListening)
        {
            Debug.Log($"{name} Deactivated, unregistering event");
            eventChannel.UnregisterListener(OnEventRaised);
            isListening = false;
        }
    }

    private void OnEventRaised(string raisedEventName)
    {
        Debug.Log($"Event '{raisedEventName}' is raised on: {name}");
        if (raisedEventName == eventName)
        {
            Debug.Log($"Event matches. Triggering outputs for {name}");
            SignalInputComplete();
            TriggerOutputs();
        }
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
- Size: 2563 bytes
- Created: 2024-07-24 08:14:49
- Modified: 2024-07-29 10:22:45

### Code

```csharp
using System.Collections.Generic;
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


    private LocalEventHandler localEventHandler;

    public override void InitializeNode()
    {
        if (aiController == null)
        {
            Debug.LogError($"AIController is null for LocalEventNode: {eventName}");
            return;
        }
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
        Debug.Log($"LocalEventNode received event: {receivedEventName} on channel {eventChannel}. Expecting: {eventName}");

        if (receivedEventName == eventName)
        {
            TriggerEvent();
        }
    }

    private void TriggerEvent()
    {
        Debug.Log($"Local Event '{eventName}' triggered on channel {eventChannel}!");

        List<NodeAIState> nodesToActivate = new List<NodeAIState>();
        foreach (NodePort connection in GetOutputPort("output").GetConnections())
        {
            NodeAIState stateNode = connection.node as NodeAIState;
            if (stateNode != null)
            {
                nodesToActivate.Add(stateNode);
                Debug.Log($"Added state node {stateNode.name} to activation list");
            }
        }
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
- Size: 1628 bytes
- Created: 2024-07-23 07:37:55
- Modified: 2024-07-29 10:27:06

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

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\Editor\AiGraphEditor.cs

- Extension: .cs
- Language: csharp
- Size: 710 bytes
- Created: 2024-07-23 07:39:01
- Modified: 2024-07-29 16:59:27

### Code

```csharp
using static XNodeEditor.NodeGraphEditor;
using UnityEditor.PackageManager.UI;
using XNodeEditor;

[CustomNodeGraphEditor(typeof(AiGraph))]
public class AIGraphEditor : NodeGraphEditor
{
    public override void OnOpen()
    {
        base.OnOpen();
        window.titleContent.text = "AI Graph";
    }

    public override string GetNodeMenuName(System.Type type)
    {
        if (type == typeof(NodeAIEntry)) return "AI/Entry";
        if (type == typeof(NodeAIEvent)) return "Events/Event";
        if (type == typeof(NodeAIState)) return "AI/State";
        if (type == typeof(NodeAILocalEvent)) return "Events/LocalEvent";

        return null; // Don't show in context menu
    }
}
```

## File: \Dev\Unity\DualBehaviour\Assets\Scripts\AiGraph\Editor\NodeAiEditor.cs

- Extension: .cs
- Language: csharp
- Size: 2157 bytes
- Created: 2024-07-28 22:31:39
- Modified: 2024-07-29 17:12:58

### Code

```csharp
using UnityEngine;
using UnityEditor;
using XNodeEditor;

[CustomNodeEditor(typeof(NodeAI))]
public class NodeAIEditor : NodeEditor
{
    private NodeAI nodeAI;

    private void OnEnable()
    {
        nodeAI = target as NodeAI;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            // Reset the node state when exiting play mode
            if (nodeAI != null)
            {
                nodeAI.ResetState();
                NodeEditorWindow.current?.Repaint();
            }
        }
    }
    public override void OnBodyGUI()
    {
        serializedObject.Update();

        NodeAI nodeAI = target as NodeAI;

        // Draw a green border if the node is active
        if (nodeAI.IsActive)
        {
            // Get the size of the node from the nodeSizes dictionary
            if (NodeEditorWindow.current.nodeSizes.TryGetValue(nodeAI, out Vector2 nodeSize))
            {
                // Create a rect using the node's size
                Rect borderRect = new Rect(0, 0, nodeSize.x, nodeSize.y);

                // Expand the rect slightly to create a border effect
                borderRect.x -= 2;
                borderRect.y -= 2;
                borderRect.width += 4;
                borderRect.height += 4;

                // Draw the border
                EditorGUI.DrawRect(borderRect, new Color(0, 1, 0, 0.5f)); // Semi-transparent green
            }
        }

        // Call the base OnBodyGUI to ensure standard node drawing
        base.OnBodyGUI();

        serializedObject.ApplyModifiedProperties();

        // Force repaint if the GUI has changed
        if (GUI.changed)
        {
            EditorUtility.SetDirty(nodeAI);
            EditorUtility.SetDirty(nodeAI.graph);
            NodeEditorWindow.current?.Repaint();
        }
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

