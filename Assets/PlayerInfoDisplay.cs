//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using System.Collections.Generic;

//public class PlayerInfoDisplay : MonoBehaviour
//{
//    [SerializeField] private GameObject playerInfoPrefab;
//    [SerializeField] private Transform contentParent;
//    private Dictionary<string, GameObject> playerInfoObjects = new Dictionary<string, GameObject>();

//    private void Start()
//    {
//        if (playerInfoPrefab == null)
//        {
//            Debug.LogError("PlayerInfoDisplay: playerInfoPrefab is not assigned in the Inspector!");
//            return;
//        }

//        if (contentParent == null)
//        {
//            Debug.LogError("PlayerInfoDisplay: contentParent is not assigned in the Inspector!");
//            return;
//        }

//        if (PlayerManager.Instance == null)
//        {
//            Debug.LogError("PlayerInfoDisplay: PlayerManager.Instance is null!");
//            return;
//        }

//        PlayerManager.Instance.OnPlayersUpdated += UpdatePlayerInfoDisplay;
//        UpdatePlayerInfoDisplay();
//    }

//    private void UpdatePlayerInfoDisplay()
//    {
//        Debug.Log($"UpdatePlayerInfoDisplay called. Current player count: {PlayerManager.Instance.players.Count}");

//        // Remove old player info objects that are no longer in the player list
//        List<string> peersToRemove = new List<string>();
//        foreach (var kvp in playerInfoObjects)
//        {
//            if (!PlayerManager.Instance.players.Exists(p => p.peerId == kvp.Key))
//            {
//                peersToRemove.Add(kvp.Key);
//            }
//        }

//        foreach (var peerId in peersToRemove)
//        {
//            if (playerInfoObjects[peerId] != null)
//            {
//                Destroy(playerInfoObjects[peerId]);
//            }
//            playerInfoObjects.Remove(peerId);
//            Debug.Log($"Removed player info object for peerId: {peerId}");
//        }

//        // Update or create player info objects
//        foreach (var player in PlayerManager.Instance.players)
//        {
//            GameObject playerInfoObj;
//            if (!playerInfoObjects.TryGetValue(player.peerId, out playerInfoObj) || playerInfoObj == null)
//            {
//                playerInfoObj = Instantiate(playerInfoPrefab, contentParent);
//                playerInfoObjects[player.peerId] = playerInfoObj;
//                Debug.Log($"Created new player info object for {player.playerName} (PeerID: {player.peerId})");
//            }

//            TextMeshProUGUI nameText = playerInfoObj.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
//            TextMeshProUGUI readyText = playerInfoObj.transform.Find("ReadyText")?.GetComponent<TextMeshProUGUI>();

//            if (nameText != null && readyText != null)
//            {
//                nameText.text = string.IsNullOrEmpty(player.playerName) ? $"Player {player.playerNumber}" : player.playerName;
//                readyText.text = player.isReady ? "Ready" : "Not Ready";
//                readyText.color = player.isReady ? Color.green : Color.red;
//                Debug.Log($"Updated player info for {player.playerName} (PeerID: {player.peerId}), Ready: {player.isReady}");
//            }
//            else
//            {
//                Debug.LogWarning($"NameText or ReadyText component not found for player {player.playerName} (PeerID: {player.peerId})");
//            }
//        }
//    }

//    private void OnDestroy()
//    {
//        if (PlayerManager.Instance != null)
//        {
//            PlayerManager.Instance.OnPlayersUpdated -= UpdatePlayerInfoDisplay;
//        }
//    }
//}