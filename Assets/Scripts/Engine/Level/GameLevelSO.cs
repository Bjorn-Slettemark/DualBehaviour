using System.Collections.Generic;
using UnityEngine;

public abstract class GameLevelSO : ScriptableObject
{


    public string sceneName;

    public string gameLevelName;

    public GameObject loadingScreenPrefab;

    public List<Transform> spawnPoints;

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
