using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class MultiplayerLevelSO : GameLevelSO
{
    [SerializeField] private string playerPrefabName = "Cube"; // Name of the prefab in Resources folder

    public override void EnterLevel()
    {
        base.EnterLevel();

        spawnPoints = GameObject.FindGameObjectsWithTag("Spawnpoint")
                               .Select(go => go.transform)
                               .ToList();
        SpawnLocalPlayer();
    }

    private void SpawnLocalPlayer()
    {
        Transform RandomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        MultiplayerManager.Instance.RequestMultiplayerObject(RandomSpawnPoint.transform.position, Quaternion.identity, playerPrefabName);
    }

    public override void ExitLevel()
    {
        base.ExitLevel();
    }

    public override void LoadingLevel()
    {
        base.LoadingLevel();

    }

    public override void LevelUpdate()
    {
        // Implement if needed
    }
}