using UnityEngine;
using UnityEditor; // Required for the Handles class
using System.Collections.Generic;

public class WinPoint : MonoBehaviour
{
    [Header("Win Conditions")]
    public bool enterTriggerToWin = false; // Enter a trigger to win
    public bool killAllEnemiesToWin = false; // Kill all enemies to win
    public bool winAfterTimelimit = false; // Win within a certain time limit
    public float timeLimit = 60f; // Time limit to win (in seconds)

    [Header("Object Destruction Conditions")]
    public bool destroyObjectsToWin = false; // Enable this win condition
    public List<GameObject> objectsToDestroy = new List<GameObject>();

    [Header("Win Condition Mode")]
    public bool requireAllActiveConditions = true; // If true, all conditions must be met to win. If false, any condition will suffice.

    [Header("Internal Variables")]
    private bool isLevelWon = false;
    private float startTime;
    private bool triggerEntered = false; // Flag to indicate the player has entered the win trigger
    private int totalEnemiesToKill;
    private int enemiesLeftToKill;
    private void Start()
    {
        totalEnemiesToKill = GameManager.instance.totalEnemies.Count;
        
        if (winAfterTimelimit)
        {
            startTime = Time.time;
        }
    }
    private void Update()
    {
        enemiesLeftToKill = totalEnemiesToKill - GameManager.instance.enemiesKilled;

        if (!isLevelWon && CheckWinConditions())
        {
            WinLevel();
        }
    }


    // Called when an enemy is killed
    public void OnEnemyKilled()
    {
        // Check if killing all enemies is a win condition
        if (killAllEnemiesToWin && AreAllEnemiesKilled())
        {
            WinLevel();
        }
    }

    // Check if all enemies are killed
    private bool AreAllEnemiesKilled()
    {
        if (GameManager.instance.enemiesKilled == GameManager.instance.totalEnemies.Count)
        {
            return true;
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && enterTriggerToWin)
        {
            triggerEntered = true; // Mark this win condition as met

            // If not requiring all conditions, win immediately upon entering the trigger
            if (!requireAllActiveConditions)
            {
                WinLevel();
            }
            else
            {
                // If requiring all conditions, the regular check will handle if this was the last needed condition
            }
        }
    }



    private void WinLevel()
    {
        if (!isLevelWon) // Check to prevent multiple calls
        {
            Debug.Log("Level Won!");
            isLevelWon = true;
            GameManager.instance.OnPlayerWin();
            // Additional logic for handling level completion can be added here if needed
        }
    }
    private bool AreAllObjectsDestroyed()
    {
        objectsToDestroy.RemoveAll(item => item == null); // Clean up any null references

        if (objectsToDestroy.Count == 0)
        {
            return true;
        }

        return false;
    }

    private bool CheckWinConditions()
    {
        // Track how many conditions are met
        int conditionsMetCount = 0;
        int activeConditionsCount = 0;

        // Check each condition

        // Trigger entry condition
        if (enterTriggerToWin)
        {
            activeConditionsCount++;
            if (triggerEntered)
            {
                conditionsMetCount++;
            }
        }

        // Killing all enemies condition
        if (killAllEnemiesToWin)
        {
            activeConditionsCount++;
            if (AreAllEnemiesKilled()) conditionsMetCount++;
        }

        // Destroying objects condition
        if (destroyObjectsToWin)
        {
            activeConditionsCount++;
            if (AreAllObjectsDestroyed()) conditionsMetCount++;
        }

        // Add additional condition checks here

        // If require all conditions, check that all active conditions are met
        if (requireAllActiveConditions)
        {
            return conditionsMetCount == activeConditionsCount;
        }
        else // If not requiring all, check that at least one condition is met
        {
            return conditionsMetCount > 0;
        }
    }

    private void OnDrawGizmos()
    {

        // Visualize the win conditions
        Handles.color = Color.green;
        Vector3 startPos = transform.position + Vector3.up * 3;
        Vector3 offset = Vector3.down * 0.2f;

        Handles.Label(startPos + offset, "Win Conditions", new GUIStyle()
        {
            normal = { textColor = Color.white },
            alignment = TextAnchor.MiddleCenter,
            fontSize = 20
        });

        offset += Vector3.down * 0.4f;

        if (enterTriggerToWin)
        {
            Handles.Label(startPos + offset, "- Enter Trigger to Win", new GUIStyle()
            {
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14
            });
            offset += Vector3.down * 0.4f;
        }

        if (killAllEnemiesToWin)
        {
            Handles.Label(startPos + offset, "- Kill All Enemies to Win", new GUIStyle()
            {
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14
            });
            offset += Vector3.down * 0.4f;
        }

        if (totalEnemiesToKill > 0)
        {
            Handles.Label(startPos + offset, "- Kill " + enemiesLeftToKill + " Enemies to Win", new GUIStyle()
            {
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14
            });
            offset += Vector3.down * 0.4f;
        }

        if (winAfterTimelimit)
        {
            Handles.Label(startPos + offset, "- Win within " + timeLimit + " seconds", new GUIStyle()
            {
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14
            });
        }
    }
}
