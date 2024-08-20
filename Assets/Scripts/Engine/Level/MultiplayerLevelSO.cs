using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class MultiplayerLevelSO : GameLevelSO
{
    [SerializeField] private string playerPrefabName = "PlayerCube"; // Name of the prefab in Resources folder




    public override void EnterLevel()
    {
        base.EnterLevel();

        spawnPoints = GameObject.FindGameObjectsWithTag("Spawnpoint")
                               .Select(go => go.transform)
                               .ToList();


        SpawnLocalPlayer();

        // Start the loading process
        MultiplayerManager.Instance.BroadcastEventToAllPeers($"LevelEventChannel:LoadingLevelDone");

    }

    private void SpawnLocalPlayer()
    {
        Transform spawnPosition = GetRandomSpawnPoint();
        GameObject playerPrefab = Resources.Load<GameObject>(playerPrefabName);
        if (playerPrefab != null)
        {
            GameObject playerObject = Instantiate(playerPrefab, spawnPosition.position, Quaternion.identity);
            //playerObject.name = playerPrefabName;
            MultiBehaviour multiBehaviour = playerObject.GetComponent<MultiBehaviour>();
            if (multiBehaviour != null)
            {
                multiBehaviour.Initialize(WebRTCManager.Instance.LocalPeerId);
            }

        }
        else
        {
            Debug.LogError($"Player prefab '{playerPrefabName}' not found in Resources folder");
        }
    }



    private Transform GetRandomSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Count)];
    }

    public override void ExitLevel()
    {
        base.ExitLevel();
    }


    public override void LevelUpdate()
    {
        // Implement if needed
    }
}