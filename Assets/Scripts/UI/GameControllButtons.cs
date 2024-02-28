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
