using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerDataSO playerData;
    // Start is called before the first frame update
    void Start()
    {
        EventChannelManager.Instance.RegisterChannel(this.gameObject, "GameStateChannel", GameStateChannelEvent);
    }

    void GameStateChannelEvent (string eventName)
    {
        if (eventName == "SpawnPlayer")
        {
            Instantiate(playerData.playerPrefab, Vector3.zero, Quaternion.identity);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
