using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;
    public static PlayerManager Instance => _instance;

    public event System.Action OnPlayerInfoUpdated;


    [System.Serializable]
    public class PlayerInfo
    {
        public string peerId;
        public string playerName;
        public string prefabName;
        public int playerNumber;
        public int score;
        public bool isAlive;
        public bool isReady;

        [NonSerialized]
        public GameObject playerObject;

        public string Serialize()
        {
            return $"{peerId}|{playerName}|{prefabName}|{playerNumber}|{score}|{isAlive}|{isReady}";
        }

        public static PlayerInfo Deserialize(string data)
        {
            string[] parts = data.Split('|');
            return new PlayerInfo
            {
                peerId = parts[0],
                playerName = parts[1],
                prefabName = parts[2],
                playerNumber = int.Parse(parts[3]),
                score = int.Parse(parts[4]),
                isAlive = bool.Parse(parts[5]),
                isReady = bool.Parse(parts[6])
            };
        }
    }
    public void RequestPlayerInfo()
    {
        if (!WebRTCEngine.Instance.IsHost)
        {
            string message = $"RequestPlayerInfo|||{WebRTCEngine.Instance.LocalPeerId}";
            EventChannelManager.Instance.RaiseNetworkEvent(MultiplayerChannelName, message);
            Debug.Log("Requesting player info from host");
        }
        else
        {
            Debug.LogWarning("Host should not request player info from itself");
        }
    }

    public List<PlayerInfo> players = new List<PlayerInfo>();

    private const string MultiplayerChannelName = "MultiplayerChannel";

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        WebRTCEngine.Instance.OnPeerListUpdated += HandlePeerListUpdated;
        EventChannelManager.Instance.SubscribeToChannel(MultiplayerChannelName, HandlePlayerMessage);

        // Initialize local player info if host
        if (WebRTCEngine.Instance.IsHost)
        {
            InitializeLocalPlayerInfo();
        }
    }

    private void InitializeLocalPlayerInfo()
    {
        string localPeerId = WebRTCEngine.Instance.LocalPeerId;
        if (!players.Any(p => p.peerId == localPeerId))
        {
            players.Add(new PlayerInfo
            {
                peerId = localPeerId,
                playerName = "Host", // You might want to set this to a saved name
                prefabName = "", // Set to default or saved prefab
                playerNumber = 1,
                score = 0,
                isAlive = true,
                isReady = false
            });
        }
    }

    private void HandlePlayerMessage(string message)
    {
        string[] parts = message.Split(new[] { "|||" }, StringSplitOptions.None);
        if (parts.Length < 2) return;

        string messageType = parts[0];
        string senderPeerId = parts[1];

        switch (messageType)
        {
            case "UpdatePlayerInfo":
                if (WebRTCEngine.Instance.IsHost && parts.Length == 3)
                {
                    PlayerInfo updatedPlayer = PlayerInfo.Deserialize(parts[2]);
                    ProcessPlayerUpdate(updatedPlayer);
                }
                break;
            case "PlayerInfo":
                if (parts.Length == 5)
                {
                    string playerName = parts[2];
                    string prefabName = parts[3];
                    bool isReady = bool.Parse(parts[4]);
                    SetPlayerInfo(senderPeerId, playerName, prefabName, isReady);
                }
                break;
            case "PlayerReady":
                SetPlayerReady(senderPeerId);
                break;
            case "PlayerNotReady":
                SetPlayerNotReady(senderPeerId);
                break;
            case "RequestPlayerInfo":
                if (WebRTCEngine.Instance.IsHost)
                {
                    SendAllPlayerInfo();
                }
                break;
            case "AllPlayerInfo":
                if (!WebRTCEngine.Instance.IsHost && parts.Length == 3)
                {
                    UpdateAllPlayerInfo(parts[2]);
                }
                break;
        }
    }

    public void SetLocalPlayerInfo(string playerName, string prefabName, bool isReady)
    {
        string localPeerId = WebRTCEngine.Instance.LocalPeerId;
        PlayerInfo localPlayer = players.Find(p => p.peerId == localPeerId);

        if (localPlayer == null)
        {
            localPlayer = new PlayerInfo
            {
                peerId = localPeerId,
                playerNumber = players.Count + 1,
                score = 0,
                isAlive = true
            };
            players.Add(localPlayer);
        }

        localPlayer.playerName = playerName;
        localPlayer.prefabName = prefabName;
        localPlayer.isReady = isReady;

        // Send update to host
        string message = $"UpdatePlayerInfo|||{localPeerId}|||{localPlayer.Serialize()}";
        EventChannelManager.Instance.RaiseNetworkEvent(MultiplayerChannelName, message);

        // If this is the host, process the update immediately
        if (WebRTCEngine.Instance.IsHost)
        {
            ProcessPlayerUpdate(localPlayer);
        }

        OnPlayerInfoUpdated?.Invoke();
    }
    private void ProcessPlayerUpdate(PlayerInfo updatedPlayer)
    {
        if (!WebRTCEngine.Instance.IsHost) return;

        PlayerInfo existingPlayer = players.Find(p => p.peerId == updatedPlayer.peerId);
        if (existingPlayer != null)
        {
            // Update existing player
            existingPlayer.playerName = updatedPlayer.playerName;
            existingPlayer.prefabName = updatedPlayer.prefabName;
            existingPlayer.isReady = updatedPlayer.isReady;
            existingPlayer.score = updatedPlayer.score;
            existingPlayer.isAlive = updatedPlayer.isAlive;
        }
        else
        {
            // Add new player
            updatedPlayer.playerNumber = players.Count + 1;
            players.Add(updatedPlayer);
        }

        // Broadcast updated player list to all peers
        SendAllPlayerInfo();
    }
    public void SetPlayerReady(string peerId)
    {
        PlayerInfo player = players.Find(p => p.peerId == peerId);
        if (player != null && !player.isReady)
        {
            player.isReady = true;
            SetLocalPlayerInfo(player.playerName, player.prefabName, true);
        }
    }



    public void SetPlayerNotReady(string peerId)
    {
        PlayerInfo player = players.Find(p => p.peerId == peerId);
        if (player != null && player.isReady)
        {
            player.isReady = false;
            SetLocalPlayerInfo(player.playerName, player.prefabName, false);
        }
    }

    private void UpdateAllPlayerInfo(string serializedPlayers)
    {
        List<PlayerInfo> newPlayerList = DeserializeAllPlayers(serializedPlayers);

        // Update existing players and add new ones
        foreach (var newPlayer in newPlayerList)
        {
            PlayerInfo existingPlayer = players.Find(p => p.peerId == newPlayer.peerId);
            if (existingPlayer != null)
            {
                // Update existing player info
                existingPlayer.playerName = newPlayer.playerName;
                existingPlayer.prefabName = newPlayer.prefabName;
                existingPlayer.playerNumber = newPlayer.playerNumber;
                existingPlayer.score = newPlayer.score;
                existingPlayer.isAlive = newPlayer.isAlive;
                existingPlayer.isReady = newPlayer.isReady;
            }
            else
            {
                // Add new player
                players.Add(newPlayer);
            }
        }

        // Remove players that no longer exist
        players.RemoveAll(p => !newPlayerList.Any(np => np.peerId == p.peerId));

        Debug.Log($"Updated all player info: {serializedPlayers}");
        OnPlayerInfoUpdated?.Invoke();

    }

    private string SerializeAllPlayers()
    {
        return string.Join("$$$", players.Select(p => p.Serialize()));
    }

    private List<PlayerInfo> DeserializeAllPlayers(string data)
    {
        return data.Split(new[] { "$$$" }, StringSplitOptions.None).Select(PlayerInfo.Deserialize).ToList();
    }

    public void CreateEmptyPlayerInfo(string peerId)
    {
        if (!players.Any(p => p.peerId == peerId))
        {
            players.Add(new PlayerInfo
            {
                peerId = peerId,
                playerNumber = players.Count + 1,
                score = 0,
                isAlive = true,
                isReady = false
            });
            Debug.Log($"Created empty PlayerInfo for peer: {peerId}");
        }
    }

    public void SetPlayerInfo(string peerId, string playerName, string prefabName, bool isReady)
    {
        PlayerInfo player = players.Find(p => p.peerId == peerId);
        bool isNewPlayer = false;
        if (player == null)
        {
            player = new PlayerInfo
            {
                peerId = peerId,
                playerNumber = players.Count + 1,
                score = 0,
                isAlive = true
            };
            players.Add(player);
            isNewPlayer = true;
        }

        bool infoChanged = player.playerName != playerName || player.prefabName != prefabName || player.isReady != isReady;

        player.playerName = playerName;
        player.prefabName = prefabName;
        player.isReady = isReady;

        Debug.Log($"Player info set for {playerName} with prefab {prefabName}, Ready: {isReady}");

        if (WebRTCEngine.Instance.IsHost && (isNewPlayer || infoChanged))
        {
            SendAllPlayerInfo();
        }

    }

    public bool AreAllPlayersReady()
    {
        return players.Count > 0 && players.All(p => p.isReady);
    }

    public void SendAllPlayerInfo()
    {
        string serializedPlayers = SerializeAllPlayers();
        string message = $"AllPlayerInfo|||{WebRTCEngine.Instance.LocalPeerId}|||{serializedPlayers}";
        EventChannelManager.Instance.RaiseNetworkEvent(MultiplayerChannelName, message);
        Debug.Log($"Sent AllPlayerInfo: {serializedPlayers}");
    }

    private void HandlePeerListUpdated(List<string> peerIds)
    {
        foreach (var peerId in peerIds)
        {
            if (!players.Any(p => p.peerId == peerId))
            {
                CreateEmptyPlayerInfo(peerId);
            }
        }
        players.RemoveAll(p => !peerIds.Contains(p.peerId) && p.peerId != WebRTCEngine.Instance.LocalPeerId);

        if (WebRTCEngine.Instance.IsHost)
        {
            SendAllPlayerInfo();
        }
        else
        {
            RequestPlayerInfo();
        }
        OnPlayerInfoUpdated?.Invoke();

    }


    private void OnDestroy()
    {
        if (WebRTCEngine.Instance != null)
        {
            WebRTCEngine.Instance.OnPeerListUpdated -= HandlePeerListUpdated;
        }
        EventChannelManager.Instance.UnsubscribeFromChannel(MultiplayerChannelName, HandlePlayerMessage);
    }
}