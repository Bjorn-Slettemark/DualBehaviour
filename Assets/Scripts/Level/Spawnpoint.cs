using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("Spawn Settings")]
    public List<GameObject> enemyPrefabs; // List of enemy prefabs to spawn
    public int spawnCount = 5;
    public float spawnRadius = 5f;
    public bool aggressive = true; // Flag to set spawned enemies' aggressive behavior

    [Header("Respawn Timer")]
    public bool respawnTimerEnabled = false;
    public float respawnDelay = 10f;
    public bool respawnWhenAllDead = true;
    public int maxRespawns = -1;

    [Header("Spawn on Trigger")]
    public bool spawnOnTrigger = false;
    public GameObject triggerObject;

    private int currentRespawnCount = 0;
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private Coroutine respawnCoroutine = null;

    private void Start()
    {
        if (!spawnOnTrigger)
        {
            SpawnEnemies();
        }
    }

    private void SpawnEnemies()
    {
        if (maxRespawns != -1 && currentRespawnCount >= maxRespawns) return;

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
            spawnPosition.y = transform.position.y;
            GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            spawnedEnemies.Add(spawnedEnemy);

            // Set aggressive behavior if the enemy has the IAIBehavior interface or AIBehavior base class
            IAIBehavior enemyBehavior = spawnedEnemy.GetComponent<IAIBehavior>();
            if (enemyBehavior != null)
            {
                enemyBehavior.aggressive = aggressive;
            }
        }
        currentRespawnCount++;

        if (respawnTimerEnabled && (maxRespawns == -1 || currentRespawnCount < maxRespawns))
        {
            if (respawnCoroutine != null) StopCoroutine(respawnCoroutine);
            respawnCoroutine = StartCoroutine(RespawnCoroutine());
        }
    }

    IEnumerator RespawnCoroutine()
    {
        while (maxRespawns == -1 || currentRespawnCount < maxRespawns)
        {
            yield return new WaitForSeconds(respawnDelay);
            if (respawnWhenAllDead && spawnedEnemies.Exists(enemy => enemy != null))
            {
                continue; // Wait more if any enemy is still alive
            }
            spawnedEnemies.RemoveAll(enemy => enemy == null); // Clean up dead enemies from the list
            SpawnEnemies();
        }
    }

    public void OnPlayerEnterTrigger(GameObject trigger)
    {
        if (trigger == triggerObject && spawnOnTrigger && (respawnCoroutine == null || !respawnTimerEnabled))
        {
            SpawnEnemies();
        }
    }

    private void OnDrawGizmos()
    {
        // Visualize the spawn area
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
