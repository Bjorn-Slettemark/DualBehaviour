using UnityEngine;
using UnityEngine.UI;

public class LevelChangeButton : MonoBehaviour
{
    [SerializeField] private Button changeLevelButton;
    public MultiplayerLevelSO desiredLevelSO;

    private void Start()
    {
        if (changeLevelButton == null)
        {
            Debug.LogError("Change Level Button not assigned in the inspector!");
            return;
        }

        changeLevelButton.onClick.AddListener(AttemptLevelChange);
        UpdateButtonInteractability();
    }

    private void Update()
    {
        UpdateButtonInteractability();
    }

    public void SetDesiredLevel(MultiplayerLevelSO eventLevelSO)
    {
        desiredLevelSO = eventLevelSO;
    }

    private void AttemptLevelChange()
    {
        if (PlayerManager.Instance.AreAllPlayersReady() && WebRTCEngine.Instance.IsHost)
        {
            ChangeLevel(desiredLevelSO);
        }
        else
        {
            Debug.Log("Cannot change level: Not all conditions are met");
        }
    }

    public void ChangeLevel(MultiplayerLevelSO level)
    {
        if (level != null)
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
        bool canChangeLevelAreAllPlayersReady = PlayerManager.Instance.AreAllPlayersReady();
        bool isHost = WebRTCEngine.Instance.IsHost;


        changeLevelButton.interactable = canChangeLevelAreAllPlayersReady && isHost;

        if (changeLevelButton.interactable)
        {
            Debug.Log($"Level change button is interactable. All players ready: {canChangeLevelAreAllPlayersReady}, Is host: {isHost}, Player count: {PlayerManager.Instance.players.Count}");
        }
    }
}