
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Trigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering collider is tagged as "Player"
        if (other.CompareTag("Player"))
        {
            // Find all SpawnPoint components in the scene
            SpawnPoint[] spawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);

            // Notify each SpawnPoint that the player has entered the trigger
            foreach (SpawnPoint spawnPoint in spawnPoints)
            {
                spawnPoint.OnPlayerEnterTrigger(this.gameObject);
            }
        }
    }


}

