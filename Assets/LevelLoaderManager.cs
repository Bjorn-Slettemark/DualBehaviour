using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[Icon("Assets/Editor/Icons/LevelLoaderIcon.png")]
public class LevelLoaderManager : MonoBehaviour
{
    public static LevelLoaderManager Instance { get; private set; }
    private GameObject instantiatedLoadingScreenPrefab;
    private GameLevelSO currentLevel;
    [SerializeField]
    private List<GameLevelSO> gameLevels;

    [SerializeField]
    private GameEventChannelSO levelEventChannel;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        EventChannelManager.Instance.RegisterChannel(this.gameObject, levelEventChannel, LoadLevelEvents);
    }

    public void LoadLevelEvents(string eventName)
    {
        foreach (var gameLevel in gameLevels)
        {
            if (gameLevel.sceneName == eventName)
            {
                LoadLevel(gameLevel);
                break; // Exit the loop once the matching level is found and initiated for loading
            }
        }
    }

    public void LoadLevel(GameLevelSO newLevel, bool fromSaveGame = false)
    {
        //if (currentLevel == newLevel) return; // Prevent re-loading the same level

        if (newLevel.loadingScreenPrefab != null && instantiatedLoadingScreenPrefab == null)
        {
            instantiatedLoadingScreenPrefab = Instantiate(newLevel.loadingScreenPrefab);
        }

        currentLevel?.ExitLevel();
        currentLevel = newLevel;
            StartCoroutine(LoadLevelAsync(newLevel.sceneName));
    }

    private IEnumerator LoadLevelAsync(string levelName)
    {
        if (this == null) yield break;
        EventChannelManager.Instance.RaiseEvent(levelEventChannel, "LevelLoading");

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        if (instantiatedLoadingScreenPrefab != null)
        {
            Destroy(instantiatedLoadingScreenPrefab);
            instantiatedLoadingScreenPrefab = null;
        }

        // Notify the rest of the game that the level has loaded
        EventChannelManager.Instance.RaiseEvent(levelEventChannel, "LevelLoaded");

        currentLevel.EnterLevel(); // Assume GameLevelSO has an EnterLevel method to initialize the level
    }

    void OnDestroy()
    {
        EventChannelManager.Instance.UnregisterChannel(this.gameObject, levelEventChannel);
    }
}
