using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelLoaderManager : MonoBehaviour
{
    public static LevelLoaderManager Instance { get; private set; }
    public GameObject loadingScreenPrefab;
    private GameObject instantiatedLoadingScreenPrefab;
    private GameLevelSO currentLevel;

    [SerializeField] private GameEventChannelSO levelEventChannel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void LoadLevel(GameLevelSO newLevel)
    {
        if (currentLevel == newLevel) return; // Prevent re-loading the same level

        if (loadingScreenPrefab != null && instantiatedLoadingScreenPrefab == null)
        {
            instantiatedLoadingScreenPrefab = Instantiate(loadingScreenPrefab);
        }

        currentLevel?.ExitLevel();
        currentLevel = newLevel;
        StartCoroutine(LoadLevelAsync(newLevel.sceneName));
    }

    private IEnumerator LoadLevelAsync(string levelName)
    {
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
        levelEventChannel.RaiseEvent("LevelLoaded");

        currentLevel.EnterLevel(); // Assume GameLevelSO has an EnterLevel method to initialize the level
    }
}
