using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MultiplayerLevelSO : GameLevelSO
{
    [SerializeField] protected int maxPlayers = 8;

    public override void EnterLevel()
    {
        base.EnterLevel();
        InitializeLevel();
    }

    protected virtual void InitializeLevel()
    {
        FindSpawnPoints();
        if (WebRTCEngine.Instance.IsHost)
        {
            SpawnPlayers();
        }
    }

    protected virtual void FindSpawnPoints()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawnpoint")
                               .Select(go => go.transform)
                               .ToList();
        if (spawnPoints.Count == 0)
        {
            Debug.LogError("No spawn points found! Make sure to tag spawn points with 'Spawnpoint'");
        }
        else if (spawnPoints.Count < maxPlayers)
        {
            Debug.LogWarning($"Only {spawnPoints.Count} spawn points found. This is less than the maximum of {maxPlayers} players.");
        }
    }
    protected virtual void SpawnPlayers()
    {
        if (!WebRTCEngine.Instance.IsHost) return;

        List<PlayerManager.PlayerInfo> playersToSpawn = PlayerManager.Instance.players;
        List<Transform> availableSpawnPoints = new List<Transform>(spawnPoints);

        foreach (var player in playersToSpawn)
        {
            if (availableSpawnPoints.Count > 0)
            {
                int spawnIndex = Random.Range(0, availableSpawnPoints.Count);
                Transform spawnPoint = availableSpawnPoints[spawnIndex];
                MultiplayerManager.Instance.RequestObjectSpawn(player.prefabName, spawnPoint.position, spawnPoint.forward, player.peerId);
                availableSpawnPoints.RemoveAt(spawnIndex);
            }
            else
            {
                Debug.LogWarning($"No available spawn points for player {player.playerName}");
            }
        }

        BroadcastGameStart();
    }
    protected virtual void BroadcastGameStart()
    {
        MultiplayerManager.Instance.BroadcastGameEvent("GameStart", "");
    }

    public override void ExitLevel()
    {
        base.ExitLevel();
        CleanupLevel();
    }

    protected virtual void CleanupLevel()
    {
        spawnPoints.Clear();
    }

    public override void LoadingLevel()
    {
        base.LoadingLevel();
        // Add any multiplayer-specific level loading logic here
    }

    public override void LevelUpdate()
    {
        // Add any multiplayer-specific level update logic here
        // This could include checking for game end conditions, updating scores, etc.
    }

    // Additional helper methods as needed
    protected virtual void HandlePlayerDisconnect(string peerId)
    {
        // Implement logic for handling player disconnects
        // This could include removing the player, adjusting the game state, etc.
    }

    protected virtual void HandleLateJoinPlayer(string peerId)
    {
        // Implement logic for handling players who join late
        // This could include spawning them at a safe location, syncing game state, etc.
    }
}