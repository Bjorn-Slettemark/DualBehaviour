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

        EventChannelManager.Instance.SubscribeForAllChannels(this.gameObject, eventCheck);



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
        EventChannelManager.Instance.UnsubscribeForAllChannels(this.gameObject);
        if (Instance == this)
        {
            Instance = null;  // Clear the static instance if this object is disabled
        }
    }
}