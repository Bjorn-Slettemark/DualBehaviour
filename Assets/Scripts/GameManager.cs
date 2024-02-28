using System.Collections.Generic;
using UnityEngine;
public enum GameState { MainMenu, GameRunning, Pause, GameOver, Victory }

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public GameObject playerPrefab;
    public GameState currentState;
    public GameObject player;

    public int score = 0;
    public int currentLevel = 1;
    public int playerLives = 3;

    // Enemy tracking
    public List<GameObject> totalEnemies;
    public int enemiesKilled = 0;

    // UI elements and audio sources would be set in the inspector
    // Example placeholders for UI and audio management
    // public Text scoreText; // Assuming you're using UnityEngine.UI
    // public GameObject pauseMenu;
    // public AudioSource backgroundMusic;
    // public AudioSource[] soundEffects;

    // Define the events
    public static event System.Action PlayerIsDead;
    public static event System.Action PlayerWon;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        InitGame();
    }

    private void InitGame()
    {
        player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        ChangeGameState(GameState.GameRunning);
        Camera.main.GetComponent<DynamicCameraFollow>().InitializeCamera(player.transform);
    }

    void Update()
    {
        switch (currentState)
        {
            case GameState.GameRunning:
                // Game logic here
                break;
            case GameState.Pause:
                // Pause logic here
                TogglePauseMenu(true);
                break;
            case GameState.GameOver:
                // GameOver logic here
                break;
                // Other states as needed
        }
    }

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        // Update UI, check for level up, etc.
        // Example: scoreText.text = "Score: " + score;
    }


    public void ChangeGameState(GameState newState)
    {
        currentState = newState;
        switch (newState)
        {
            case GameState.Pause:
                TogglePauseMenu(true);
                break;
            case GameState.GameRunning:
                TogglePauseMenu(false);
                break;
                // Handle other states
        }
    }

    public void TogglePauseMenu(bool show)
    {
        // pauseMenu.SetActive(show);
        if (show)
        {
            Time.timeScale = 0f; // Pause game
        }
        else
        {
            Time.timeScale = 1f; // Resume game
        }
    }

    // Example method for playing sound effects
    public void PlaySoundEffect(int index)
    {
        // if (index >= 0 && index < soundEffects.Length)
        // {
        //     soundEffects[index].Play();
        // }
    }


    // Call this method when enemies are spawned
    public void RegisterEnemySpawn(GameObject enemy)
    {
        totalEnemies.Add(enemy);
    }

    // Call this method when an enemy is killed
    public void RegisterEnemyDeath(GameObject enemy)
    {
        enemiesKilled++;
        totalEnemies.Remove(enemy);
    }

    public void PlayerDied()
    {
        playerLives -= 1;
        if (playerLives > 0)
        {
            Debug.Log("Player respawn.");
            // Respawn player logic here
        }
        else
        {
            Debug.Log("Player died.");
            ChangeGameState(GameState.GameOver);
            // Invoke the PlayerIsDead event
            PlayerIsDead?.Invoke();
        }
    }

    public void OnPlayerWin()
    {
        Debug.Log("Player has won the level!");
        // Transition to the next level, show win UI, play victory sounds, etc.
        // Invoke the PlayerWon event
        PlayerWon?.Invoke();
    }



}

