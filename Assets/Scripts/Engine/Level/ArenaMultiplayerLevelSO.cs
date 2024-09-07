using UnityEngine;

[CreateAssetMenu(fileName = "New Multiplayer Arena", menuName = "Multiplayer Levels/Arena")]
public class ArenaMultiplayerLevel : MultiplayerLevelSO
{
    [SerializeField] private int scoreToWin = 10;
    [SerializeField] private float hazardSpawnInterval = 30f; // Time in seconds between hazard spawns

    private float lastHazardSpawnTime;

    protected override void InitializeLevel()
    {
        base.InitializeLevel();
        // Additional arena-specific initialization
        SetupArenaHazards();
        ResetGameState();
    }

    private void SetupArenaHazards()
    {
        // Implement arena-specific hazard setup
        // For example, initialize hazard spawn points or load hazard prefabs
        Debug.Log("Setting up arena hazards");
    }

    private void ResetGameState()
    {
        lastHazardSpawnTime = Time.time;
        // Reset any other arena-specific game state variables
    }

    public override void LevelUpdate()
    {
        base.LevelUpdate();
        //CheckWinCondition();
        //UpdateHazards();
    }

    private void UpdateHazards()
    {
        if (Time.time - lastHazardSpawnTime >= hazardSpawnInterval)
        {
            SpawnHazard();
            lastHazardSpawnTime = Time.time;
        }
    }

    private void SpawnHazard()
    {
        // Implement hazard spawning logic
        Debug.Log("Spawning a new hazard");
        // Example: MultiplayerManager.Instance.RequestObjectSpawn("HazardPrefab", GetRandomSpawnPoint(), Quaternion.identity);
    }

    private void CheckWinCondition()
    {
        foreach (var player in PlayerManager.Instance.players)
        {
            if (player.score >= scoreToWin)
            {
                EndGame(player);
                break;
            }
        }
    }

    private void EndGame(PlayerManager.PlayerInfo winner)
    {
        Debug.Log($"Game Over! Winner: {winner.playerName}");
        MultiplayerManager.Instance.BroadcastGameEvent("GameEnd", winner.peerId);
        // Implement additional game end logic
        // For example, show end game UI, stop hazard spawning, etc.
    }

    protected override void HandlePlayerDisconnect(string peerId)
    {
        base.HandlePlayerDisconnect(peerId);

        // Arena-specific disconnect handling
        int remainingPlayers = PlayerManager.Instance.players.Count;
        if (remainingPlayers < 2) // Assuming we need at least 2 players for the game
        {
            EndGameDueToInsufficientPlayers();
        }
    }

    private void EndGameDueToInsufficientPlayers()
    {
        Debug.Log("Game ended due to insufficient players");
        MultiplayerManager.Instance.BroadcastGameEvent("GameEndInsufficientPlayers", "");
        // Implement logic to end the game and possibly return to lobby
    }

    protected override void HandleLateJoinPlayer(string peerId)
    {
        base.HandleLateJoinPlayer(peerId);

        // Arena-specific late join handling
        // For example, spawn the player at a safe location and sync game state
        SpawnLateJoinPlayer(peerId);
        SyncGameStateToLateJoinPlayer(peerId);
    }

    private void SpawnLateJoinPlayer(string peerId)
    {
        // Implement logic to spawn a late-joining player
        Transform spawnPoint = GetSafeSpawnPoint();
        PlayerManager.PlayerInfo player = PlayerManager.Instance.players.Find(p => p.peerId == peerId);
        if (player != null && spawnPoint != null)
        {
            MultiplayerManager.Instance.RequestObjectSpawn(player.prefabName, spawnPoint.position, spawnPoint.forward);
        }
    }

    private Transform GetSafeSpawnPoint()
    {
        // Implement logic to find a safe spawn point for late-joining players
        // This could be a designated "safe" spawn point or a randomly selected one
        return spawnPoints.Count > 0 ? spawnPoints[Random.Range(0, spawnPoints.Count)] : null;
    }

    private void SyncGameStateToLateJoinPlayer(string peerId)
    {
        // Implement logic to sync the current game state to the late-joining player
        // This could include current scores, hazard positions, time remaining, etc.
        Debug.Log($"Syncing game state to late-join player: {peerId}");
    }
}