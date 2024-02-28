using UnityEngine;
using UnityEngine.AI;
using System;
public class AISenseSystem : MonoBehaviour
{
    public Transform player; // Assign the player's transform in the inspector
    public Vector3 playerPosition; // To store the last known position of the player

    public float visionRange = 10f; // How far the AI can see
    public float fieldOfView = 120f; // Field of view angle

    // Using System.Action for simplicity and direct invocation
    public Action OnPlayerSpotted;
    public Action OnPlayerVisible;
    public Action OnHitReceived;
    public Action OnPlayerLost;

    private bool playerInVisionRange = false; // Flag to track if player is within vision range
    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        if (GameManager.instance != null && GameManager.instance.player != null)
        {
            player = GameManager.instance.player.transform;
        }
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        VisionSense();
    }

    void VisionSense()
    {
        if (player == null) return;

        Vector3 dirToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(dirToPlayer, transform.forward);

        if (angle < fieldOfView / 2f && dirToPlayer.magnitude <= visionRange)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, dirToPlayer.normalized, out hit, visionRange))
            {
                if (hit.transform == player)
                {
                    playerPosition = player.position;
                    OnPlayerSpotted?.Invoke();
                    OnPlayerVisible?.Invoke();
                    playerInVisionRange = true;
                }
            }
        }
        else
        {
            if (playerInVisionRange)
            {
                playerInVisionRange = false;
                OnPlayerLost?.Invoke();
            }
        }
    }

    public bool PlayerVisible()
    {
        return playerInVisionRange;
    }

    public void ReceiveHit()
    {
        OnHitReceived?.Invoke();
    }
    // Visualize the AI's field of view and vision range in the Scene View
    private void OnDrawGizmos()
    {
        if (player == null) return;

        // Draw vision cone
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, (player.position - transform.position).normalized * visionRange);
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView / 2, 0) * transform.forward * visionRange;
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView / 2, 0) * transform.forward * visionRange;
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);

        // Draw vision range circle
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        // Draw debug visualization for NavMesh destination
        if (navMeshAgent != null && navMeshAgent.hasPath)
        {
            Debug.DrawLine(transform.position, navMeshAgent.destination, Color.blue);
            DebugDrawCircle(navMeshAgent.destination, 0.5f, Color.blue);
        }
    }

    // Helper method to draw a circle in the scene view
    private void DebugDrawCircle(Vector3 center, float radius, Color color)
    {
        int segments = 36;
        float angleIncrement = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);

        for (int i = 0; i < segments + 1; i++)
        {
            float angle = i * angleIncrement;
            Vector3 nextPoint = center + Quaternion.Euler(0, angle, 0) * new Vector3(radius, 0, 0);
            Debug.DrawLine(prevPoint, nextPoint, color);
            prevPoint = nextPoint;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("blimp!");
            OnHitReceived?.Invoke();
     
    }
}
