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
    private HashSet<string> knownPeers = new HashSet<string>();

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
        WebRTCEngine.Instance.OnPeerListUpdated += HandlePeerListUpdated;


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
    private void HandlePeerListUpdated(List<string> peerIds)
    {
        var newPeers = peerIds.Except(knownPeers).ToList();

        foreach (var newPeerId in newPeers)
        {
            Debug.Log($"New peer detected: {newPeerId}");
            if (WebRTCEngine.Instance.IsHost)
            {
                // Host sends current player info to all peers
                PlayerManager.Instance.SendAllPlayerInfo();
            }
            else if (newPeerId == WebRTCEngine.Instance.LocalPeerId)
            {
                // If we're the new peer, request player info update
                PlayerManager.Instance.RequestPlayerInfo();
            }
        }

        knownPeers = new HashSet<string>(peerIds);
    }


    private void SubscribeToPeerChannel(string peerId)
    {
        string channelName = NetworkEngine.Instance.GetPeerChannelName(peerId);
        //EventChannelManager.Instance.SubscribeToChannel(channelName, HandlePeerMessage);
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




    private void OnDestroy()
    {
        if (WebRTCEngine.Instance != null)
        {
            WebRTCEngine.Instance.OnPeerListUpdated -= HandlePeerListUpdated;
        }
        EventChannelManager.Instance.UnsubscribeFromChannel(MultiplayerChannelName, OnMultiplayerChannelEvent);
    }
}