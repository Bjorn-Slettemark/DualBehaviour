using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;
    public static PlayerManager Instance => _instance;

    [System.Serializable]
    public class PlayerInfo
    {
        public string peerId;
        public string playerName;
        public string prefabName;
        public int playerNumber;
        public GameObject playerObject;
        public int score;
        public bool isAlive;
        public bool isReady;
    }

    public List<PlayerInfo> players = new List<PlayerInfo>();
    private const int MaxPlayers = 8;

    public event System.Action OnPlayersReadyChanged;

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

        // Subscribe to necessary channels
        EventChannelManager.Instance.SubscribeToChannel(MultiplayerChannelName, HandlePlayerMessage);
    }

    private void HandlePlayerMessage(string message)
    {
        Debug.Log("HandlePlayerMessage: " + message);
        string[] parts = message.Split(':');
        if (parts.Length < 2)
        {
            Debug.LogError($"Invalid player message format: {message}");
            return;
        }

        string messageType = parts[0];
        string senderPeerId = parts[1];

        switch (messageType)
        {
            case "PlayerInfo":
                if (parts.Length == 4)
                {
                    string playerName = parts[2];
                    string prefabName = parts[3];
                    SetPlayerInfo(senderPeerId, playerName, prefabName);
                }
                else
                {
                    Debug.LogError($"Invalid PlayerInfo message format: {message}");
                }
                break;

            case "PlayerReady":
                SetPlayerReady(senderPeerId);
                break;
        }
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

    public void SetPlayerInfo(string peerId, string playerName, string prefabName)
    {
        PlayerInfo player = players.Find(p => p.peerId == peerId);
        if (player == null)
        {
            player = new PlayerInfo
            {
                peerId = peerId,
                playerNumber = players.Count + 1,
                score = 0,
                isAlive = true,
                isReady = false
            };
            players.Add(player);
        }
        player.playerName = playerName;
        player.prefabName = prefabName;

        Debug.Log($"Player info set for {playerName} with prefab {prefabName}");
    }

    public void SetPlayerReady(string peerId)
    {
        PlayerInfo player = players.Find(p => p.peerId == peerId);
        if (player != null && !player.isReady)
        {
            player.isReady = true;
            Debug.Log($"Player {player.playerName} (PeerID: {peerId}) is ready.");
            OnPlayersReadyChanged?.Invoke();

        }
    }

    public bool AreAllPlayersReady()
    {
        return players.Count > 0 && players.All(p => p.isReady);
    }


    public void SetPlayerObject(string peerId, GameObject playerObject)
    {
        PlayerInfo player = players.Find(p => p.peerId == peerId);
        if (player != null)
        {
            player.playerObject = playerObject;
            player.isAlive = true;
        }
    }

    public List<PlayerInfo> GetPlayersNeedingSpawn()
    {
        return players.Where(p => p.playerObject == null && !string.IsNullOrEmpty(p.prefabName) && p.isReady).ToList();
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
        players.RemoveAll(p => !peerIds.Contains(p.peerId));
    }

    private void ResetPlayer(PlayerInfo player)
    {
        player.score = 0;
        player.isAlive = true;
        player.isReady = false;
        if (player.playerObject != null)
        {
            Destroy(player.playerObject);
            player.playerObject = null;
        }
    }

    public void ResetPlayerReadyStatus()
    {
        foreach (var player in players)
        {
            player.isReady = false;
        }
        OnPlayersReadyChanged?.Invoke();
    }

    private void OnDestroy()
    {
        if (WebRTCEngine.Instance != null)
        {
            WebRTCEngine.Instance.OnPeerListUpdated -= HandlePeerListUpdated;
        }

        // Unsubscribe from channels
        EventChannelManager.Instance.UnsubscribeFromChannel(MultiplayerChannelName, HandlePlayerMessage);
    }
}