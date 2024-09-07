using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerDataSO playerData;
    // Start is called before the first frame update

    [SerializeField]
    GameEventChannelSO gameStateEventChannel;
    [SerializeField]
    GameEventChannelSO playerEventChannel;

    void Start()
    {
        EventChannelManager.Instance.SubscribeToChannel(gameStateEventChannel.name, ChannelEvent);
        EventChannelManager.Instance.SubscribeToChannel(playerEventChannel.name, ChannelEvent);

    }

    void ChannelEvent (string eventName)
    {
        if (eventName == "SpawnPlayer")
        {
            Debug.Log("Spawning player");
            Instantiate(playerData.playerPrefab, Vector3.zero, Quaternion.identity);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
