using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class MultiplayerManager : MonoBehaviour
{
    private static MultiplayerManager _instance;
    public static MultiplayerManager Instance => _instance;
    private string localPeerId;

    private Dictionary<string, string> multiplayerObjectsHost = new Dictionary<string, string>(); // ObjectName, ObjectId
    private Dictionary<string, string> multiplayerObjectOwners = new Dictionary<string, string>(); // ObjectId, PeerIdOwner

    private int nextObjectId = 1;

    private bool isInitialized = false;


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(Initialize());
        }
        else
        {
            Destroy(gameObject);
        }
        localPeerId = LocalWebRTCManager.Instance.LocalPeerId;

    }
    private IEnumerator Initialize()
    {
        while (LocalWebRTCManager.Instance == null || string.IsNullOrEmpty(LocalWebRTCManager.Instance.LocalPeerId))
        {
            yield return null;
        }

        localPeerId = LocalWebRTCManager.Instance.LocalPeerId;
        Debug.Log($"[MultiplayerManager] Initialized with local peer ID: {localPeerId}");

        while (WebRTCManager.Instance == null)
        {
            yield return null;
        }

        WebRTCManager.Instance.OnPeerListUpdated += HandlePeerListUpdated;
        LocalWebRTCManager.Instance.OnLocalConnectionEstablished += OnLocalConnectionEstablished;

        isInitialized = true;
        Debug.Log("[MultiplayerManager] Initialization complete");
    }


    //private void Start()
    //{
    //    WebRTCManager.Instance.OnPeerListUpdated += HandlePeerListUpdated;
    //    LocalWebRTCManager.Instance.OnLocalConnectionEstablished += OnLocalConnectionEstablished;

    //}

    public void BroadcastLocalPeer(string senderPeerId, string message)
    {
        string channelName = $"PeerChannel_{senderPeerId}";
        EventChannelManager.Instance.RaiseEvent(channelName, message);
    }

    public void BroadcastEventToAllPeers(string eventData)
    {
        Debug.Log($"[MultiplayerManager] Broadcasting event to all peers: {eventData}");
        WebRTCManager.Instance.SendDataMessage(eventData);
    }


    private void OnLocalConnectionEstablished()
    {
        Debug.Log("MultiplayerManager: Local WebRTC connection established");

        // Create a channel for the local peer
        CreatePeerChannel(localPeerId);

    }

    private void HandlePeerListUpdated(List<string> peers)
    {
        Debug.Log($"[MultiplayerManager] HandlePeerListUpdated called. Number of peers: {peers.Count}");
        foreach (var peerId in peers)
        {
            Debug.Log($"[MultiplayerManager] Processing peer: {peerId}");
            CreatePeerChannel(peerId);
        }

        // Remove channels for peers that are no longer connected
        var allChannels = EventChannelManager.Instance.EventChannels;
        foreach (var channel in allChannels)
        {
            if (channel.name.StartsWith("PeerChannel_"))
            {
                string peerId = channel.name.Substring("PeerChannel_".Length);
                if (!peers.Contains(peerId) && peerId != localPeerId)
                {
                    RemovePeerChannel(peerId);
                }
            }
        }

        // Log the final list of connected peers
        Debug.Log($"[MultiplayerManager] Connected peers: {string.Join(", ", GetConnectedPeers())}");
    }
    private void CreatePeerChannel(string peerId)
    {
        string channelName = GetPeerChannelName(peerId);
        EventChannelManager.Instance.CreateChannelIfNotExists(channelName);
        Debug.Log($"Created channel for peer: {peerId}");
    }

    private void RemovePeerChannel(string peerId)
    {
        string channelName = GetPeerChannelName(peerId);
        EventChannelManager.Instance.UnregisterFromChannel(null, channelName);
        Debug.Log($"Removed channel for peer: {peerId}");
    }

    public string GetPeerChannelName(string peerId)
    {
        return $"PeerChannel_{peerId}";
    }


    public void HandleWebRTCMessage(string senderPeerId, string message)
    {
        Debug.Log($"Received WebRTC message from {senderPeerId}: {message}");

        int colonIndex = message.IndexOf(':');
        if (colonIndex != -1)
        {
            string channelName = message.Substring(0, colonIndex);
            string eventData = message.Substring(colonIndex + 1);

            if (channelName == "LevelChannel")
            {
                Debug.Log("LevelChannel recieved a message!" + eventData);
                HandleLevelChannelMessage(senderPeerId, eventData);
            }
            else if (EventChannelManager.Instance.channelsByName.ContainsKey(channelName))
            {
                EventChannelManager.Instance.RaiseEvent(channelName, eventData);
            }
            else
            {
                Debug.LogWarning($"Attempted to raise event on non-existent channel: {channelName}");
            }
        }
        else
        {
            // Handle general peer message
            string peerChannelName = $"PeerChannel_{senderPeerId}";
            if (EventChannelManager.Instance.channelsByName.ContainsKey(peerChannelName))
            {
                EventChannelManager.Instance.RaiseEvent(peerChannelName, message);
            }
            else
            {
                Debug.LogWarning($"Attempted to raise event on non-existent peer channel: {peerChannelName}");
            }
        }
    }


    public void HandleLevelChannelMessage(string senderPeerId, string eventData)
    {
        string[] parts = eventData.Split(':');
        switch (parts[0])
        {
            case "RequestMultiplayerObjectId":
                Debug.Log("RequestMultiplayerObjectId registered! and this is: " + WebRTCManager.Instance.IsHost);
                if (WebRTCManager.Instance.IsHost)
                {
                    HandleRequestMultiplayerObjectId(parts);
                }
                break;
            case "NewMultiplayerObject":
                HandleNewMultiplayerObject(parts);
                break;
        }
        EventChannelManager.Instance.RaiseEvent("LevelChannel", eventData);
    }

    private void HandleRequestMultiplayerObjectId(string[] parts)
    {
        Debug.Log("HandleRequestMultiplayerObjectId registered!" + parts.Length);

        //if (parts.Length < 3) return;
        string peerId = parts[1];
        string objectId = $"Obj_{nextObjectId++}";
        multiplayerObjectOwners[objectId] = peerId;
        Debug.Log("Raise event with objectId: " + peerId + " " + objectId);
        BroadcastEventToAllPeers($"LevelChannel:NewMultiplayerObjectId:{peerId}:{objectId}");
    }

    public void CreateMultiplayerObject(string objectId, string objectName)
    {
        multiplayerObjectsHost[objectName] = objectId;
        BroadcastEventToAllPeers($"LevelChannel:NewMultiplayerObject:{objectId}:{objectName}");
    }

    private void HandleNewMultiplayerObject(string[] parts)
    {
        if (parts.Length < 4) return;
        string objectId = parts[2];
        string objectName = parts[3];

        if (multiplayerObjectOwners[objectId] != WebRTCManager.Instance.LocalPeerId)
        {
            GameObject prefab = Resources.Load<GameObject>(objectName);
            if (prefab != null)
            {
                GameObject newObject = Instantiate(prefab);
                newObject.name = objectName;
                MultiBehaviour multiBehaviour = newObject.GetComponent<MultiBehaviour>();
                if (multiBehaviour != null)
                {
                    multiBehaviour.Initialize(multiplayerObjectOwners[objectId], objectId);
                }
                else
                {
                    Debug.LogWarning($"[MultiplayerManager] MultiBehaviour component not found on {objectName}");
                }
            }
            else
            {
                Debug.LogError($"[MultiplayerManager] Prefab {objectName} not found in Resources folder");
            }
        }
    }



        public List<string> GetConnectedPeers()
    {
        return WebRTCManager.Instance.GetConnectedPeers();
    }

    public bool IsLocalPeer(string peerId)
    {
        return peerId == localPeerId;
    }

    private void OnDestroy()
    {
        if (WebRTCManager.Instance != null)
        {
            WebRTCManager.Instance.OnMessageReceived -= BroadcastLocalPeer;
            WebRTCManager.Instance.OnPeerListUpdated -= HandlePeerListUpdated;

        }

    }
   
}