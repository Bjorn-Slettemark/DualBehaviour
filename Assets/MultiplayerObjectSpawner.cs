using UnityEngine;

public class RandomFieldMultiplayerSpawner : MonoBehaviour
{
    [SerializeField] private string prefabName = "Cube"; // Name of the prefab in Resources folder
    [SerializeField] private KeyCode spawnKey = KeyCode.Space; // Key to press for spawning
    [SerializeField] private float fieldSize = 20f; // Size of the spawn field (20x20)
    [SerializeField] private float spawnHeight = 1f; // Height at which objects spawn

    private void Update()
    {
        if (Input.GetKeyDown(spawnKey))
        {
            SpawnRandomMultiplayerObject();
        }
    }

    private void SpawnRandomMultiplayerObject()
    {
        // Calculate random position within the field
        float halfSize = fieldSize / 2f;
        Vector3 randomPosition = new Vector3(
            Random.Range(-halfSize, halfSize),
            spawnHeight,
            Random.Range(-halfSize, halfSize)
        );

        // Generate a random rotation
        Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

        // Request the multiplayer object
        MultiplayerManager.Instance.RequestMultiplayerObject(randomPosition, randomRotation, prefabName);

        Debug.Log($"Requested multiplayer object: {prefabName} at position {randomPosition}");
    }
}