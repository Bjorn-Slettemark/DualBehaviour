using System.Linq;
using UnityEngine;

public class MultiplayerLevelSO : GameLevelSO
{
    [SerializeField] private GameObject playerPrefab;

    public override void EnterLevel()
    {
        base.EnterLevel();

        spawnPoints = GameObject.FindGameObjectsWithTag("Spawnpoint")
                               .Select(go => go.transform)
                               .ToList();        // Register for multiplayer events

        EventChannelManager.Instance.RegisterForChannel(null, "LevelEventChannel", HandlePlayerEvent);
        Debug.Log(PeerManager.Instance.LocalPeerChannelName);
        // Notify other peers that a new player has joined
        MultiplayerChannelManager.Instance.BroadcastEvent("LevelEventChannel", $"NewPlayerJoined:{LocalWebRTCManager.Instance.LocalPeerId}");
    }

    private void SpawnLocalPlayer()
    {
        Transform spawnPosition = GetRandomSpawnPoint();
        GameObject playerObject = Instantiate(playerPrefab, spawnPosition.position, Quaternion.identity);
        playerObject.GetComponent<PlayerCube>().Initialize(LocalWebRTCManager.Instance.LocalPeerId, true);
        //playerObject.GetComponent<PlayerController>().Initialize(LocalWebRTCManager.Instance.LocalPeerId, true);
    }

    private void HandlePlayerEvent(string eventData)
    {
        string[] parts = eventData.Split(':');
        if (parts[0] == "NewPlayerJoined")
        {
            string peerId = parts[1];


            if (peerId != LocalWebRTCManager.Instance.LocalPeerId)
            {
                SpawnRemotePlayer(peerId);
            } else
            {
                SpawnLocalPlayer();
            }
        }
    }

    private void SpawnRemotePlayer(string peerId)
    {
        Transform spawnPosition = GetRandomSpawnPoint();
        GameObject playerObject = Instantiate(playerPrefab, spawnPosition.position, Quaternion.identity);
        //playerObject.GetComponent<PlayerController>().Initialize(peerId, false);
    }

    private Transform GetRandomSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Count)];
    }

    public override void ExitLevel()
    {
        base.ExitLevel();
        EventChannelManager.Instance.UnregisterFromChannel(null, "PlayerChannel");
    }

    public override void LevelUpdate()
    {
        throw new System.NotImplementedException();
    }
}