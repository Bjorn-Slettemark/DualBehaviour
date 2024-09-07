using UnityEngine;
using UnityEngine.UI;

public class LevelChangeButton : MonoBehaviour
{
    [SerializeField] private Button changeLevelButton;
    public MultiplayerLevelSO desiredLevelSO;

    private void Start()
    {
        changeLevelButton.onClick.AddListener(AttemptLevelChange);
        UpdateButtonInteractability();
        PlayerManager.Instance.OnPlayersReadyChanged += UpdateButtonInteractability;
    }

    private void OnDestroy()
    {
        PlayerManager.Instance.OnPlayersReadyChanged -= UpdateButtonInteractability;
    }

    public void SetDesiredLevel(MultiplayerLevelSO eventLevelSO)
    {
        desiredLevelSO = eventLevelSO;
    }

    private void AttemptLevelChange()
    {
        ChangeLevel(desiredLevelSO);

        if (PlayerManager.Instance.AreAllPlayersReady())
        {
        }
        else
        {
            Debug.Log("Cannot change level: Not all players are ready");
        }
    }

    public void ChangeLevel(MultiplayerLevelSO level)
    {
        if (desiredLevelSO != null)
        {
            EventChannelManager.Instance.RaiseNetworkEvent("LevelEventChannel", "ChangeLevel:" + level.sceneName);
        }
        else
        {
            Debug.LogError("Desired level SO is not set");
        }
    }

    private void UpdateButtonInteractability()
    {
        changeLevelButton.interactable = PlayerManager.Instance.AreAllPlayersReady();
    }
}