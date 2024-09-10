using UnityEngine;
using TMPro;
using System.Text;

public class PlayerListUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerListText;

    private void Start()
    {
        if (playerListText == null)
        {
            Debug.LogError("PlayerListUI: TextMeshProUGUI component not assigned!");
            return;
        }

        // Initial update of the player list
        UpdatePlayerList();

        // Subscribe to events that might change the player list
        PlayerManager.Instance.OnPlayerInfoUpdated += UpdatePlayerList;
    }

    private void OnDestroy()
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.OnPlayerInfoUpdated -= UpdatePlayerList;
        }
    }

    private void UpdatePlayerList()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Room:{WebRTCEngine.Instance.roomName}");
        sb.AppendLine("Connected Players:");

        foreach (var player in PlayerManager.Instance.players)
        {
            string readyStatus = player.isReady ? "READY" : "NOT READY";
            string playerName = string.IsNullOrEmpty(player.playerName) ? "Unnamed Player" : player.playerName;
            sb.AppendLine($"{playerName}   -   {readyStatus}");
        }

        playerListText.text = sb.ToString();
    }
}