using System.Linq;
using UnityEngine;

public class MultiplayerLevelSO : GameLevelSO
{
    [SerializeField] private string playerPrefabName = "PlayerCube"; // Name of the prefab in Resources folder

    public override void EnterLevel()
    {
        base.EnterLevel();

        spawnPoints = GameObject.FindGameObjectsWithTag("Spawnpoint")
                               .Select(go => go.transform)
                               .ToList();

        // Register for level events
        EventChannelManager.Instance.RegisterForChannel(null, "LevelChannel", HandleLevelEvent);

        // Spawn local player
        SpawnLocalPlayer();

        Debug.Log($"Entered multiplayer level. Local peer channel: {WebRTCManager.Instance.LocalPeerChannelName}");
    }

    private void SpawnLocalPlayer()
    {
        Transform spawnPosition = GetRandomSpawnPoint();
        GameObject playerPrefab = Resources.Load<GameObject>(playerPrefabName);

        if (playerPrefab != null)
        {
            GameObject playerObject = Instantiate(playerPrefab, spawnPosition.position, Quaternion.identity);
            MultiBehaviour multiBehaviour = playerObject.GetComponent<MultiBehaviour>();

            if (multiBehaviour != null)
            {
                Debug.Log("Setting ownerId: " + WebRTCManager.Instance.LocalPeerId);
               multiBehaviour.Initialize(WebRTCManager.Instance.LocalPeerId);
            }
            else
            {
                Debug.LogError($"MultiBehaviour component not found on {playerPrefabName} prefab");
            }
        }
        else
        {
            Debug.LogError($"Player prefab '{playerPrefabName}' not found in Resources folder");
        }
    }

    private void HandleLevelEvent(string eventData)
    {
        // This method is now primarily for debugging or additional level-specific logic
        // MultiBehaviour will handle most network-related events
        Debug.Log($"Received level event: {eventData}");
    }

    private Transform GetRandomSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Count)];
    }

    public override void ExitLevel()
    {
        base.ExitLevel();
        EventChannelManager.Instance.UnregisterFromChannel(null, "LevelChannel");
    }

    public override void LevelUpdate()
    {
        // Implement if needed
    }
}