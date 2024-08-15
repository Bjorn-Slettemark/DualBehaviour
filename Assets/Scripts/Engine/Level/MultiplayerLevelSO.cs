using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MultiplayerTest", menuName = "GameLevel/MultiplayerTest", order = 1)]
public class MultiplayerLevelSO : GameLevelSO
{
    [SerializeField]
    private GameEventChannelSO playerEventChannel;
    [SerializeField]
    private GameObject playerCubePrefab;
    private List<PlayerCube> spawnedPlayers = new List<PlayerCube>();
    [SerializeField]
    private float spawnAreaSize = 20f; // Size of the spawn area (20x20)

    public override void EnterLevel()
    {
        base.EnterLevel();
        Debug.Log("Entering level " + gameLevelName + ", spawning players");
        // Register for multiplayer events
        MultiplayerManager.Instance.RegisterForMultiplayerEvents(HandleMultiplayerEvent);
        // Spawn local player
        SpawnLocalPlayer();
        // Spawn remote players
        SpawnRemotePlayers();
        // Notify other peers that a new player has joined
        MultiplayerManager.Instance.BroadcastEventToAllPeers($"NewPlayerJoined:{WebRTCManager.Instance.LocalPeerId}");
    }

    private void SpawnLocalPlayer()
    {
        Vector3 randomPosition = GetRandomSpawnPosition();
        GameObject localPlayerObject = Instantiate(playerCubePrefab, randomPosition, Quaternion.identity);
        PlayerCube localPlayerCube = localPlayerObject.GetComponent<PlayerCube>();
        localPlayerCube.Initialize(WebRTCManager.Instance.LocalPeerId);
        spawnedPlayers.Add(localPlayerCube);
        Debug.Log($"Spawned local player with PeerId: {localPlayerCube.PeerId} at position: {randomPosition}");
    }

    private void SpawnRemotePlayers()
    {
        List<string> connectedPeers = MultiplayerManager.Instance.GetConnectedPeers();
        Debug.Log(connectedPeers.Count);
        foreach (string peerId in connectedPeers)
        {
            if (peerId != WebRTCManager.Instance.LocalPeerId)
            {
                Debug.Log("spawning remote player");

                SpawnRemotePlayer(peerId);
            }
        }
    }

    private void SpawnRemotePlayer(string peerId)
    {
        Vector3 randomPosition = GetRandomSpawnPosition();
        GameObject remotePlayerObject = Instantiate(playerCubePrefab, randomPosition, Quaternion.identity);
        PlayerCube remotePlayerCube = remotePlayerObject.GetComponent<PlayerCube>();
        remotePlayerCube.Initialize(peerId);
        spawnedPlayers.Add(remotePlayerCube);
        Debug.Log($"Spawned remote player with PeerId: {peerId} at position: {randomPosition}");
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float halfSize = spawnAreaSize / 2f;
        float x = Random.Range(-halfSize, halfSize);
        float z = Random.Range(-halfSize, halfSize);
        return new Vector3(x, 1f, z); // Y is set to 1 to spawn slightly above the ground
    }

    private void HandleMultiplayerEvent(string eventName)
    {
        if (eventName.StartsWith("NewPlayerJoined:"))
        {
            string newPeerId = eventName.Split(':')[1];
            if (newPeerId != WebRTCManager.Instance.LocalPeerId)
            {
                SpawnRemotePlayer(newPeerId);
            }
        }
    }

    public override void ExitLevel()
    {
        base.ExitLevel();
        // Clean up spawned players
        foreach (PlayerCube player in spawnedPlayers)
        {
            if (player != null)
            {
                Destroy(player.gameObject);
            }
        }
        spawnedPlayers.Clear();
        // Unregister from multiplayer events
        MultiplayerManager.Instance.UnregisterFromMultiplayerEvents(HandleMultiplayerEvent);
    }

    public override void LevelUpdate()
    {
        // Add any level-specific update logic here
    }
}