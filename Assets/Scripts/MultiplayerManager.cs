using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MultiplayerManager : MonoBehaviour
{
    private static MultiplayerManager _instance;
    public static MultiplayerManager Instance => _instance;

    private int nextObjectId = 0;
    private const string MultiplayerChannelName = "MultiplayerChannel";
    private const string PrefabResourcePath = "MultiplayerPrefabs/";
    private List<Transform> spawnPoints = new List<Transform>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeMultiplayerChannel();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (WebRTCEngine.Instance.IsHost)
        {
            SubscribeToAllPeerChannels();
        }
    }

    private void InitializeMultiplayerChannel()
    {
        if (!EventChannelManager.Instance.ChannelExists(MultiplayerChannelName))
        {
            EventChannelManager.Instance.RegisterNewChannel(MultiplayerChannelName);
            Debug.Log($"Created new channel: {MultiplayerChannelName}");
        }
        EventChannelManager.Instance.SubscribeToChannel(MultiplayerChannelName, OnMultiplayerChannelEvent);
        Debug.Log($"Subscribed to channel: {MultiplayerChannelName}");
    }

    private void SubscribeToAllPeerChannels()
    {
        foreach (var peerId in WebRTCEngine.Instance.GetConnectedPeers())
        {
            SubscribeToPeerChannel(peerId);
        }
    }

    private void SubscribeToPeerChannel(string peerId)
    {
        string channelName = NetworkEngine.Instance.GetPeerChannelName(peerId);
        EventChannelManager.Instance.SubscribeToChannel(channelName, HandlePeerMessage);
    }

    private void HandlePeerMessage(string message)
    {
        string[] parts = message.Split(':');
        if (parts.Length < 3) return;

        string messageType = parts[0];
        string senderPeerId = parts[1];

        switch (messageType)
        {
            case "SetPlayerInfo":
                if (parts.Length == 4)
                {
                    string playerName = parts[2];
                    string prefabName = parts[3];
                    PlayerManager.Instance.SetPlayerInfo(senderPeerId, playerName, prefabName);
                    if (PlayerManager.Instance.AreAllPlayersReady())
                    {
                        SpawnPlayers();
                    }
                }
                break;
        }
    }

    private void OnMultiplayerChannelEvent(string eventData)
    {
        string[] parts = eventData.Split(':');
        if (parts.Length < 2) return;

        switch (parts[0])
        {
            case "RequestSpawnObject":
                if (WebRTCEngine.Instance.IsHost && parts.Length == 6)
                {
                    HandleSpawnRequest(parts[1], parts[2], parts[3], parts[4], parts[5]);
                }
                break;
            case "SpawnObject":
                if (parts.Length == 6)
                {
                    SpawnObject(parts[1], parts[2], parts[3], parts[4], parts[5]);
                }
                break;
            case "RequestDestroyGameObject":
                if (WebRTCEngine.Instance.IsHost && parts.Length == 3)
                {
                    HandleDestroyRequest(parts[1], parts[2]);
                }
                break;
            case "DestroyGameObject":
                if (parts.Length == 2)
                {
                    DestroyGameObject(parts[1]);
                }
                break;
            case "GameEvent":
                if (parts.Length >= 3)
                {
                    //HandleGameEvent(parts[1], string.Join(":", parts.Skip(2)));
                }
                break;

        }
    }
    public void BroadcastGameEvent(string eventType, string eventData)
    {
        string fullEventData = $"GameEvent:{eventType}:{eventData}";
        EventChannelManager.Instance.RaiseNetworkEvent(MultiplayerChannelName, fullEventData);
    }
    public void RequestObjectSpawn(string prefabName, Vector3 spawnPosition, Vector3 spawnDirection, string realOwnerPeerId = null)
    {
        string spawnPosString = NetworkUtility.SerializeVector3(spawnPosition);
        string spawnDirString = NetworkUtility.SerializeVector3(spawnDirection);
        if (realOwnerPeerId == null) realOwnerPeerId = WebRTCEngine.Instance.LocalPeerId;
        string eventData = $"RequestSpawnObject:{WebRTCEngine.Instance.LocalPeerId}:{prefabName}:{spawnPosString}:{spawnDirString}:{realOwnerPeerId}";
        EventChannelManager.Instance.RaiseNetworkEvent(MultiplayerChannelName, eventData);
    }

    private void HandleSpawnRequest(string senderPeerId, string prefabName, string spawnPosString, string spawnDirString, string ownerPeerId)
    {
        string objectId = GenerateObjectId();
        string eventData = $"SpawnObject:{prefabName}:{spawnPosString}:{spawnDirString}:{objectId}:{ownerPeerId}";
        EventChannelManager.Instance.RaiseNetworkEvent(MultiplayerChannelName, eventData);
    }
    private void SpawnObject(string prefabName, string spawnPosString, string spawnDirString, string objectId, string ownerPeerId)
    {
        GameObject prefab = Resources.Load<GameObject>(PrefabResourcePath + prefabName);
        if (prefab != null)
        {
            Vector3 spawnPos = NetworkUtility.DeserializeVector3(spawnPosString);
            Vector3 spawnDir = NetworkUtility.DeserializeVector3(spawnDirString);
            GameObject spawnedObject = Instantiate(prefab, spawnPos, Quaternion.LookRotation(spawnDir));

            MultiBehaviour multiBehaviour = spawnedObject.GetComponent<MultiBehaviour>();
            if (multiBehaviour != null)
            {
                multiBehaviour.OnInitialize(objectId, ownerPeerId);
            }
            else
            {
                Debug.LogError($"Prefab {prefabName} does not have a MultiBehaviour component!");
            }

        }
        else
        {
            Debug.LogError($"Prefab {prefabName} not found in the Resources folder!");
        }
    }



    private string GenerateObjectId()
    {
        return (nextObjectId++).ToString();
    }

    public void RequestDestroyGameObject(string objectId)
    {
        string eventData = $"RequestDestroyGameObject:{WebRTCEngine.Instance.LocalPeerId}:{objectId}";
        EventChannelManager.Instance.RaiseNetworkEvent(MultiplayerChannelName, eventData);
    }

    private void HandleDestroyRequest(string peerId, string objectId)
    {
        string eventData = $"DestroyGameObject:{objectId}";
        EventChannelManager.Instance.RaiseNetworkEvent(MultiplayerChannelName, eventData);
    }

    private void DestroyGameObject(string objectId)
    {
        MultiBehaviour objectToDestroy = FindObjectWithId(objectId);
        if (objectToDestroy != null)
        {
            Destroy(objectToDestroy.gameObject);
        }
        else
        {
            Debug.LogWarning($"Object with id {objectId} not found for destruction.");
        }
    }

    private MultiBehaviour FindObjectWithId(string objectId)
    {
        MultiBehaviour[] allObjects = FindObjectsOfType<MultiBehaviour>();
        foreach (MultiBehaviour obj in allObjects)
        {
            if (obj.ObjectId == objectId)
            {
                return obj;
            }
        }
        return null;
    }

    private void SpawnPlayers()
    {
        if (!WebRTCEngine.Instance.IsHost) return;

        FindSpawnPoints();

        if (spawnPoints.Count == 0) return;

        List<Transform> availableSpawnPoints = new List<Transform>(spawnPoints);
        var playersToSpawn = PlayerManager.Instance.GetPlayersNeedingSpawn();

        foreach (var player in playersToSpawn)
        {
            if (availableSpawnPoints.Count > 0)
            {
                int spawnIndex = Random.Range(0, availableSpawnPoints.Count);
                Transform spawnPoint = availableSpawnPoints[spawnIndex];
                RequestObjectSpawn(player.prefabName, spawnPoint.position, spawnPoint.forward);
                availableSpawnPoints.RemoveAt(spawnIndex);
            }
        }
    }

    private void FindSpawnPoints()
    {
        spawnPoints = new List<Transform>(GameObject.FindGameObjectsWithTag("Spawnpoint").Select(go => go.transform));

        if (spawnPoints.Count == 0)
        {
            Debug.LogError("No spawn points found! Make sure to tag spawn points with 'Spawnpoint'");
        }
    }


    private void OnDestroy()
    {
        EventChannelManager.Instance.UnsubscribeFromChannel(MultiplayerChannelName, OnMultiplayerChannelEvent);
    }
}