using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MultiplayerGameManager : MonoBehaviour
{
    private static MultiplayerGameManager _instance;
    public static MultiplayerGameManager Instance => _instance;

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
    }

    public List<PlayerInfo> players = new List<PlayerInfo>();
    private List<Transform> spawnPoints = new List<Transform>();
    private const int MaxPlayers = 8;
    private bool isHost;

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
        EventChannelManager.Instance.SubscribeToChannel("MutliplayerEventChannel", HandleGameEvents);
        isHost = WebRTCEngine.Instance.IsHost;

        if (isHost)
        {
            SubscribeToAllPeerChannels();
        }
    }

    private void SubscribeToAllPeerChannels()
    {
        foreach (var peerId in WebRTCEngine.Instance.GetConnectedPeers())
        {
            SubscribeToPeerChannel(peerId);
        }
    }

    private void SubscribeToPeerChannel(string peerId)
    {
        string channelName = NetworkEngine.Instance.GetPeerChannelName(peerId);
        EventChannelManager.Instance.SubscribeToChannel(channelName, HandlePeerMessage);
    }

    private void HandlePeerMessage(string message)
    {
        string[] parts = message.Split(':');
        if (parts.Length < 3) return;

        string messageType = parts[0];
        string senderPeerId = parts[1];

        switch (messageType)
        {
            case "SetPlayerInfo":
                if (parts.Length == 4)
                {
                    string playerName = parts[2];
                    string prefabName = parts[3];
                    SetPlayerInfo(senderPeerId, playerName, prefabName);
                }
                break;
        }
    }

    private void SetPlayerInfo(string peerId, string playerName, string prefabName)
    {
        if (!isHost) return;

        PlayerInfo player = players.Find(p => p.peerId == peerId);
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
        }

        player.playerName = playerName;
        player.prefabName = prefabName;

        // Broadcast updated player info to all peers
        BroadcastGameEvent("PlayerInfoUpdated", $"{peerId}:{playerName}:{prefabName}");

        // If all players have set their info, start spawning
        if (players.All(p => !string.IsNullOrEmpty(p.prefabName)))
        {
            InitializeLevel();
        }
    }

    public void SetLocalPlayerInfo(string playerName, string prefabName)
    {
        string localPeerId = WebRTCEngine.Instance.LocalPeerId;
        if (isHost)
        {
            SetPlayerInfo(localPeerId, playerName, prefabName);
        }
        else
        {
            string message = $"SetPlayerInfo:{localPeerId}:{playerName}:{prefabName}";
            NetworkEngine.Instance.BroadcastToPeer(WebRTCEngine.Instance.HostPeerId, message);
        }
    }

    private void HandlePeerListUpdated(List<string> peerIds)
    {
        if (isHost)
        {
            foreach (var peerId in peerIds)
            {
                if (!players.Any(p => p.peerId == peerId))
                {
                    SubscribeToPeerChannel(peerId);
                }
            }

            players.RemoveAll(p => !peerIds.Contains(p.peerId));

            // If any player disconnected, we might need to respawn
            if (players.Any(p => p.playerObject == null && !string.IsNullOrEmpty(p.prefabName)))
            {
                SpawnPlayers();
            }
        }
    }

    public void InitializeLevel()
    {
        if (!isHost) return;

        FindSpawnPoints();
        SpawnPlayers();
    }

    private void FindSpawnPoints()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawnpoint")
                               .Select(go => go.transform)
                               .ToList();

        if (spawnPoints.Count == 0)
        {
            Debug.LogError("No spawn points found! Make sure to tag spawn points with 'Spawnpoint'");
        }
        else if (spawnPoints.Count < MaxPlayers)
        {
            Debug.LogWarning($"Only {spawnPoints.Count} spawn points found. This is less than the maximum of {MaxPlayers} players.");
        }
    }

    private void SpawnPlayers()
    {
        if (!isHost || spawnPoints.Count == 0) return;

        List<Transform> availableSpawnPoints = new List<Transform>(spawnPoints);

        foreach (var player in players)
        {
            if (player.playerObject == null && !string.IsNullOrEmpty(player.prefabName) && availableSpawnPoints.Count > 0)
            {
                int spawnIndex = Random.Range(0, availableSpawnPoints.Count);
                Transform spawnPoint = availableSpawnPoints[spawnIndex];
                SpawnPlayer(player, spawnPoint.position, spawnPoint.forward);
                availableSpawnPoints.RemoveAt(spawnIndex);
            }
        }
    }

    private void SpawnPlayer(PlayerInfo player, Vector3 position, Vector3 direction)
    {
        string spawnEventData = $"{player.peerId}:{player.prefabName}:{position.x},{position.y},{position.z}:{direction.x},{direction.y},{direction.z}";
        BroadcastGameEvent("SpawnPlayer", spawnEventData);
    }

    public void RegisterPlayerObject(string peerId, GameObject playerObject)
    {
        PlayerInfo player = players.Find(p => p.peerId == peerId);
        if (player != null)
        {
            player.playerObject = playerObject;
            player.isAlive = true;
        }
    }

    public void UpdatePlayerScore(string peerId, int scoreChange)
    {
        PlayerInfo player = players.Find(p => p.peerId == peerId);
        if (player != null)
        {
            player.score += scoreChange;
            BroadcastGameEvent("ScoreUpdated", $"{peerId}:{player.score}");
        }
    }

    public void PlayerDied(string peerId)
    {
        PlayerInfo player = players.Find(p => p.peerId == peerId);
        if (player != null)
        {
            player.isAlive = false;
            BroadcastGameEvent("PlayerDied", peerId);
            CheckGameOver();
        }
    }

    private void CheckGameOver()
    {
        int alivePlayers = players.Count(p => p.isAlive);
        if (alivePlayers <= 1)
        {
            string winnerPeerId = players.FirstOrDefault(p => p.isAlive)?.peerId;
            BroadcastGameEvent("GameOver", winnerPeerId);
        }
    }

    private void BroadcastGameEvent(string eventType, string eventData)
    {
        string fullEventData = $"{eventType}:{eventData}";
        EventChannelManager.Instance.RaiseNetworkEvent("GameEvents", fullEventData);
    }

    private void HandleGameEvents(string eventData)
    {
        string[] parts = eventData.Split(':');
        if (parts.Length < 2) return;

        string eventType = parts[0];
        string data = string.Join(":", parts.Skip(1));

        switch (eventType)
        {
            case "PlayerInfoUpdated":
                HandlePlayerInfoUpdated(data);
                break;
            case "ScoreUpdated":
                HandleScoreUpdated(data);
                break;
            case "PlayerDied":
                HandlePlayerDied(data);
                break;
            case "GameOver":
                HandleGameOver(data);
                break;
            case "SpawnPlayer":
                HandleSpawnPlayer(data);
                break;
        }
    }

    private void HandlePlayerInfoUpdated(string data)
    {
        string[] infoParts = data.Split(':');
        if (infoParts.Length == 3)
        {
            string peerId = infoParts[0];
            string playerName = infoParts[1];
            string prefabName = infoParts[2];

            PlayerInfo player = players.Find(p => p.peerId == peerId);
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
            }

            player.playerName = playerName;
            player.prefabName = prefabName;
        }
    }

    private void HandleScoreUpdated(string data)
    {
        string[] scoreParts = data.Split(':');
        if (scoreParts.Length == 2)
        {
            string peerId = scoreParts[0];
            int newScore = int.Parse(scoreParts[1]);
            PlayerInfo player = players.Find(p => p.peerId == peerId);
            if (player != null)
            {
                player.score = newScore;
            }
        }
    }

    private void HandlePlayerDied(string peerId)
    {
        PlayerInfo player = players.Find(p => p.peerId == peerId);
        if (player != null)
        {
            player.isAlive = false;
        }
    }

    private void HandleGameOver(string winnerPeerId)
    {
        Debug.Log($"Game Over! Winner: {winnerPeerId}");
        // Implement game over logic (e.g., show game over screen, restart game, etc.)
    }

    private void HandleSpawnPlayer(string data)
    {
        string[] spawnParts = data.Split(':');
        if (spawnParts.Length == 4)
        {
            string peerId = spawnParts[0];
            string prefabName = spawnParts[1];
            Vector3 spawnPosition = NetworkUtility.DeserializeVector3(spawnParts[2]);
            Vector3 spawnDirection = NetworkUtility.DeserializeVector3(spawnParts[3]);

            if (peerId == WebRTCEngine.Instance.LocalPeerId)
            {
                MultiplayerManager.Instance.RequestObjectSpawn(prefabName, spawnPosition, spawnDirection);
            }
        }
    }

    private void OnDestroy()
    {
        if (WebRTCEngine.Instance != null)
        {
            WebRTCEngine.Instance.OnPeerListUpdated -= HandlePeerListUpdated;
        }
        if (EventChannelManager.Instance != null)
        {
            EventChannelManager.Instance.UnsubscribeFromChannel("MutliplayerEventChannel", HandleGameEvents);
        }
    }
}