//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using System.Collections.Generic;
//using System.Linq;

//public class PlayerStatusUI : MonoBehaviour
//{
//    [SerializeField] public TextMeshProUGUI roomNameText;
//    [SerializeField] public Transform playerListContent;
//    [SerializeField] public  GameObject playerStatusPrefab;

//    private Dictionary<string, GameObject> playerStatusObjects = new Dictionary<string, GameObject>();

//    private void Start()
//    {
//        UpdateRoomName();
//        PlayerManager.Instance.OnPlayersReadyChanged += UpdatePlayerList;
//        WebRTCEngine.Instance.OnPeerListUpdated += HandlePeerListUpdated;
//        UpdatePlayerList();

//        // Subscribe to room-related events
//        EventChannelManager.Instance.SubscribeToChannel("MultiplayerChannel", OnMultiplayerEvent);
//    }

//    private void OnMultiplayerEvent(string eventData)
//    {
//        string[] parts = eventData.Split(':');
//        if (parts.Length >= 2)
//        {
//            switch (parts[0])
//            {
//                case "RoomCreated":
//                case "RoomJoined":
//                case "RoomLeft":
//                    UpdateRoomName();
//                    break;
//            }
//        }
//    }

//    private void UpdateRoomName()
//    {
//        string roomName = WebRTCEngine.Instance.roomName;
//        roomNameText.text = string.IsNullOrEmpty(roomName) ? "Not in a room" : $"Room: {roomName}";
//    }

//    private void HandlePeerListUpdated(List<string> peers)
//    {
//        UpdatePlayerList();
//        UpdateRoomName(); // Update room name when peer list changes
//    }

//    private void UpdatePlayerList()
//    {
//        // Clear existing player status objects
//        foreach (var obj in playerStatusObjects.Values)
//        {
//            Destroy(obj);
//        }
//        playerStatusObjects.Clear();

//        // Create new player status objects
//        foreach (var player in PlayerManager.Instance.players)
//        {
//            GameObject playerStatusObj = Instantiate(playerStatusPrefab, playerListContent);
//            TextMeshProUGUI nameText = playerStatusObj.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
//            TextMeshProUGUI readyText = playerStatusObj.transform.Find("ReadyText").GetComponent<TextMeshProUGUI>();

//            nameText.text = string.IsNullOrEmpty(player.playerName) ? $"Player {player.playerNumber}" : player.playerName;
//            readyText.text = player.isReady ? "Ready" : "Not Ready";
//            readyText.color = player.isReady ? Color.green : Color.red;

//            playerStatusObjects[player.peerId] = playerStatusObj;
//        }
//    }

//    private void OnDestroy()
//    {
//        if (PlayerManager.Instance != null)
//        {
//            PlayerManager.Instance.OnPlayersReadyChanged -= UpdatePlayerList;
//        }
//        if (WebRTCEngine.Instance != null)
//        {
//            WebRTCEngine.Instance.OnPeerListUpdated -= HandlePeerListUpdated;
//        }
//        EventChannelManager.Instance.UnsubscribeFromChannel("MultiplayerChannel", OnMultiplayerEvent);
//    }
//}