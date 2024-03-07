using UnityEngine;
using UnityEngine.AI;

public static class AIUtility
{
    public static void FaceTarget(Transform agentTransform, Vector3 targetPosition, float rotationSpeed)
    {
        Vector3 directionToTarget = targetPosition - agentTransform.position;
        directionToTarget.y = 0; // Ensure the rotation is only on the Y axis
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
        agentTransform.rotation = Quaternion.Slerp(agentTransform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
    // Returns a random point within a specified distance from a given center.
    public static Vector3 GetRandomPoint(Vector3 center, float distance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance + center;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, distance, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return Vector3.zero;
    }
}
