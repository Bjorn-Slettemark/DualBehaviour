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
