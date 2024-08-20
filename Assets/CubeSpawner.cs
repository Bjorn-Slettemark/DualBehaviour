using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public GameObject cubePrefab;  // Assign this in the Inspector
    public int numberOfCubes = 100;
    public float spawnAreaSize = 100f;
    public KeyCode spawnKey = KeyCode.P;

    private void Update()
    {
        if (Input.GetKeyUp(spawnKey))
        {
            SpawnCubes();
        }
    }

    private void SpawnCubes()
    {
        for (int i = 0; i < numberOfCubes; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-spawnAreaSize / 2, spawnAreaSize / 2),
                0,  // Assuming you want them on the same Y level
                Random.Range(-spawnAreaSize / 2, spawnAreaSize / 2)
            );

            GameObject cubeGO = Instantiate(cubePrefab, randomPosition, Quaternion.identity);
            MultiBehaviour multiBehaviour = cubeGO.GetComponent<MultiBehaviour>();
            if (multiBehaviour != null)
            {
                multiBehaviour.Initialize(WebRTCManager.Instance.LocalPeerId);
            }
        }

        Debug.Log($"Spawned {numberOfCubes} cubes in a {spawnAreaSize}x{spawnAreaSize} area.");
    }
}