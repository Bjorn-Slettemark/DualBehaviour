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

    [SerializeField] private List<GameStateSO> gameStates;

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
        SetupSingleton();
        InitializeStateDictionary();

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
    }

    private void SetupSingleton()
    {
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
        foreach (var gameState in gameStates)
        {
            if (!stateDictionary.ContainsKey(gameState.gameState)) // Assuming GameStateSO has a public GameState enum field
            {
                stateDictionary.Add(gameState.gameState, gameState);
            }
        }

        // Optionally, log if the initialStateEnum is not in gameStates
        if (!stateDictionary.ContainsKey(initialState))
        {
            Debug.LogError("Initial state enum value not found in gameStates list!");
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
}