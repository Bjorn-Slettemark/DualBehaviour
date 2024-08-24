using System.Collections.Generic;
using UnityEngine;

public abstract class GameLevelSO : ScriptableObject
{


    public string sceneName;

    public string gameLevelName;

    public GameObject loadingScreenPrefab;

    public List<Transform> spawnPoints;

    public bool loadingDone = false;

    [SerializeField]
    private GameEventChannelSO levelEventChannel;

    public virtual GameObject GetLoadingScreenPrefab()
    {
        return loadingScreenPrefab;
    }
  
    // Method to be called when entering the state
    public virtual void EnterLevel()
    {
        EventChannelManager.Instance.RaiseEvent(levelEventChannel.name, "EnterLevel");
        // Additional enter logic
    }

    public virtual void LoadingLevel()
    {
        EventChannelManager.Instance.RaiseEvent(levelEventChannel.name, "LoadingLevel");
        // Additional enter logic
    }
    // Method to be called when exiting the state
    public virtual void ExitLevel()
    {
        EventChannelManager.Instance.RaiseEvent(levelEventChannel.name, "ExitLevel");
        // Additional exit logic
    }

    // Abstract method for state-specific functionality
    public abstract void LevelUpdate();
    // Abstract method for state-specific functionality

}
