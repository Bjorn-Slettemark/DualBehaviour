using UnityEngine;

public abstract class GameLevelSO : ScriptableObject
{
    public GameEventChannelSO levelEventChannel;

    public string sceneName;

    public GameObject loadingScreenPrefab;

    public virtual GameObject GetLoadingScreenPrefab()
    {
        return loadingScreenPrefab;
    }
    // Method to be called when entering the state
    public virtual void EnterLevel()
    {
        EventChannelManager.Instance.RaiseEvent("LevelEventChannel", "EnterLevel");
        // Additional enter logic
    }

    // Method to be called when exiting the state
    public virtual void ExitLevel()
    {
        levelEventChannel.RaiseEvent($"ExitLevel");
        // Additional exit logic
    }

    // Abstract method for state-specific functionality
    public abstract void LevelUpdate();

}
