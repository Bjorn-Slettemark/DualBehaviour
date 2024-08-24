using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class MultiplayerManager : MonoBehaviour
{
    private static MultiplayerManager _instance;
    public static MultiplayerManager Instance => _instance;

    private Queue<int> objectIdQueue = new Queue<int>();
    private Dictionary<int, MultiBehaviour> networkObjects = new Dictionary<int, MultiBehaviour>();
    private int nextObjectId = 1;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
    }
    public void RequestLevelChange(string levelName)
    {
        string localPeerId = LocalWebRTCEngine.Instance.LocalPeerId;
        NetworkMessage message = NetworkMessageFactory.LevelChange(localPeerId, levelName);
        NetworkEngine.Instance.BroadcastEventToAllPeers(message);
    }
    public void RequestMultiplayerObject(Vector3 position, Quaternion rotation, string prefabName)
    {
        
        string localPeerId = LocalWebRTCEngine.Instance.LocalPeerId;
        NetworkMessage message = NetworkMessageFactory.CreateRequestMultiplayerObjectId(
            localPeerId, 
            prefabName, 
            position, 
            rotation
        );
        Debug.Log(message);
        NetworkEngine.Instance.BroadcastEventToAllPeers(message);
    }

    public void HandleIncomingMessage(string serializedMessage)
    {
        //Debug.Log($"Received serialized message: {serializedMessage}");
        try
        {
            NetworkMessage message = NetworkMessage.Deserialize(serializedMessage);

            switch (message.MessageType)
            {
                case "RequestMultiplayerObjectId":
                    if (WebRTCEngine.Instance.IsHost)
                    {
                        HandleRequestMultiplayerObjectId(message);
                    }
                    break;
                case "NewMultiplayerObject":
                    HandleNewMultiplayerObject(message);
                    break;
                case "ChangeLevel":
                    HandleLevelChannel(message);
                    break;
                case "SyncObject":
                    HandleSyncObject(message);
                    break;
                default:
                    Debug.LogWarning($"Unknown message type: {message.MessageType}");
                    break;
            }
        }
        catch (ArgumentException ex)
        {
            Debug.LogError($"Error deserializing message: {ex.Message}");
        }
    }
    private void HandleSyncObject(NetworkMessage message)
    {
        int objectId = message.ObjectId ?? 0;
        Debug.Log($"Handling sync for ObjectId: {objectId}");

        if (networkObjects.TryGetValue(objectId, out MultiBehaviour multiBehaviour))
        {
            Debug.Log($"Found MultiBehaviour: {multiBehaviour.name} with ObjectId: {objectId}");
            multiBehaviour.ReceiveSyncUpdate(message);
        }
        else
        {
            Debug.LogWarning($"No MultiBehaviour found for ObjectId: {objectId}");
            // Log the current state of networkObjects for debugging
            foreach (var kvp in networkObjects)
            {
                Debug.Log($"Registered object - Key: {kvp.Key}, Value: {kvp.Value.name}");
            }
        }
    }

    private void HandleRequestMultiplayerObjectId(NetworkMessage message)
    {
        string requesterPeerId = message.LocalPeerId;
        string prefabName = message.GetData<string>("PrefabName");
        Vector3 position = message.GetData<Vector3>("Position");
        Quaternion rotation = message.GetData<Quaternion>("Rotation");

        Debug.Log($"Received RequestMultiplayerObjectId: PrefabName={prefabName}, Position={position}, Rotation={rotation}");


        int newObjectId = nextObjectId++;
        objectIdQueue.Enqueue(newObjectId);

        // Process the queue
        while (objectIdQueue.Count > 0)
        {
            int objectIdToProcess = objectIdQueue.Dequeue();
            NetworkMessage newObjectMessage = NetworkMessageFactory.CreateNewMultiplayerObject(
                requesterPeerId,
                objectIdToProcess,
                prefabName,
                position,
                rotation,
                ""  // ExtraData, if needed
            );
            NetworkEngine.Instance.BroadcastEventToAllPeers(newObjectMessage);
        }
    }

    private void HandleNewMultiplayerObject(NetworkMessage message)
    {
        int objectId = message.ObjectId ?? throw new InvalidOperationException("ObjectId is null");
        string ownerPeerId = message.LocalPeerId;
        string prefabName = message.GetData<string>("PrefabName");
        Vector3 position = message.GetData<Vector3>("Position");
        Quaternion rotation = message.GetData<Quaternion>("Rotation");

        Debug.Log($"Handling new multiplayer object: ID={objectId}, PrefabName={prefabName}, Position={position}, Rotation={rotation}");

        GameObject prefab = Resources.Load<GameObject>(prefabName);
        if (prefab != null)
        {
            GameObject spawnedObject = Instantiate(prefab, position, rotation);

            MultiBehaviour networkObject = spawnedObject.GetComponent<MultiBehaviour>();
            networkObjects[objectId] = networkObject;


            networkObject.Initialize(objectId, ownerPeerId);

            Debug.Log($"Spawned object: {prefabName} with ID: {objectId} owned by: {ownerPeerId}");
        }
        else
        {
            Debug.LogError($"Prefab not found: {prefabName}");
        }
    }
    private void HandleLevelChannel(NetworkMessage message)
    {
        switch (message.MessageType)
        {
            case "ChangeLevel":
                string levelName = message.GetData<string>("LevelName");
                // Call your level loading method here
                LevelLoaderManager.Instance.LoadLevel(levelName);
                break;
            default:
                Debug.LogWarning($"Unknown message type: {message.MessageType}");
                break;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (EventChannelManager.Instance != null)
        {
            EventChannelManager.Instance.UnSubscribeChannel(this.gameObject, "LevelEventChannel");
        }
    }
}

