using UnityEngine;
using System.Collections;

public class AttachToPlayer : MonoBehaviour
{
    private GameObject player;

    void Start()
    {
        StartCoroutine(FindAndFollowPlayer());
    }

    IEnumerator FindAndFollowPlayer()
    {
        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.Log("Searching for player...");
                yield return new WaitForSeconds(0.5f); // Wait for half a second before trying again
            }
        }

        Debug.Log($"Player found: {player.name}");

        while (true)
        {
            if (player != null)
            {
                transform.position = player.transform.position;
            }
            else
            {
                Debug.LogWarning("Player object lost. Restarting search...");
                player = null;
                StartCoroutine(FindAndFollowPlayer());
                yield break;
            }
            yield return null; // Wait for the next frame
        }
    }
}