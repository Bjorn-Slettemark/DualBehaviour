using UnityEngine;
using System;
using System.Collections.Generic;

public class MultiplayerManager : MonoBehaviour
{
    private static MultiplayerManager _instance;
    public static MultiplayerManager Instance => _instance;

    private const string MultiplayerChannelName = "MultiplayerChannel";
    private Dictionary<string, Action<string>> peerEventHandlers = new Dictionary<string, Action<string>>();
    private Dictionary<string, MultiBehaviour> networkObjects = new Dictionary<string, MultiBehaviour>();

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

    public void RegisterNetworkObject(string objectId, MultiBehaviour networkObject)
    {
        networkObjects[objectId] = networkObject;
    }


    public void BroadcastSyncMessage(string ownerId, string message)
    {
        // Broadcast to all peers
        BroadcastEventToAllPeers(message);

        // Also broadcast to the specific peer's channel
        string channelName = $"PeerChannel_{ownerId}";
        EventChannelManager.Instance.RaiseEventByName(channelName, message);
    }

    public void HandleWebRTCMessage(string senderPeerId, string message)
    {
        if (message.StartsWith("SYNC|"))
        {
            HandleSyncMessage(message);
        }
        else if (peerEventHandlers.TryGetValue(senderPeerId, out var handler))
        {
            handler.Invoke(message);
        }
        else
        {
            Debug.LogWarning($"No handler registered for peer {senderPeerId}");
        }
    }

    private void HandleSyncMessage(string message)
    {
        string[] parts = message.Split('|');
        if (parts.Length < 3) return;

        string objectId = parts[1];
        if (!networkObjects.TryGetValue(objectId, out MultiBehaviour networkObject))
        {
            Debug.LogWarning($"No network object found with ID: {objectId}");
            return;
        }

        Dictionary<string, object> receivedFields = new Dictionary<string, object>();
        for (int i = 2; i < parts.Length; i++)
        {
            string[] fieldParts = parts[i].Split(':');
            if (fieldParts.Length == 2)
            {
                receivedFields[fieldParts[0]] = fieldParts[1];
            }
        }

        networkObject.ReceiveSyncMessage(receivedFields);
    }

    public void BroadcastEventToAllPeers(string eventName)
    {
        WebRTCManager.Instance.SendDataMessage(eventName);
    }

    private void Start()
    {
        WebRTCManager.Instance.OnMessageReceived += HandleWebRTCMessage;
        WebRTCManager.Instance.OnPeerListUpdated += HandlePeerListUpdated;

        EventChannelManager.Instance.RegisterChannelByName(MultiplayerChannelName);

        // Create a channel for the local peer
        //string localChannelName = $"PeerChannel_{WebRTCManager.Instance.LocalPeerId}";
        //EventChannelManager.Instance.RegisterChannelByName(localChannelName);
    }

    private void HandlePeerListUpdated(List<string> peers)
    {
        foreach (var peerId in peers)
        {
            CreatePeerChannel(peerId);
        }

        // Remove channels for peers that are no longer connected
        var allChannels = EventChannelManager.Instance.EventChannels;
        foreach (var channel in allChannels)
        {
            if (channel.name.StartsWith("PeerChannel_"))
            {
                string peerId = channel.name.Substring("PeerChannel_".Length);
                if (!peers.Contains(peerId) && peerId != WebRTCManager.Instance.LocalPeerId)
                {
                    RemovePeerChannel(peerId);
                }
            }
        }
    }

    private void CreatePeerChannel(string peerId)
    {
        string channelName = $"PeerChannel_{peerId}";
        EventChannelManager.Instance.RegisterChannelByName(channelName);
        Debug.Log($"Created channel for peer: {peerId}");
    }

    private void RemovePeerChannel(string peerId)
    {
        string channelName = $"PeerChannel_{peerId}";
        // Note: EventChannelManager doesn't have a method to remove channels. 
        // You might want to add this functionality or handle it differently.
        Debug.Log($"Removed channel for peer: {peerId}");
    }


    public void SendEventToPeer(string peerId, string eventName)
    {
        WebRTCManager.Instance.SendDataMessage(eventName);
    }


    public void RaiseEventLocally(string peerId, string eventName)
    {
        string channelName = $"PeerChannel_{peerId}";
        EventChannelManager.Instance.RaiseEventByName(channelName, eventName);
        Debug.Log($"Raised event locally for peer {peerId}: {eventName}");
    }


    public void RegisterForPeerEvents(string peerId, Action<string> listener)
    {
        peerEventHandlers[peerId] = listener;
        Debug.Log($"Registered peer event handler for {peerId}");
    }

    public void UnregisterFromPeerEvents(string peerId, Action<string> listener)
    {
        string channelName = $"PeerChannel_{peerId}";
        EventChannelManager.Instance.UnregisterEventByName(gameObject, channelName, "*", listener);
        Debug.Log($"Unregistered from peer events on channel: {channelName}");
    }

    public void RegisterForMultiplayerEvents(Action<string> listener)
    {
        EventChannelManager.Instance.RegisterEventByName(gameObject, MultiplayerChannelName, "*", listener);
    }

    public void UnregisterFromMultiplayerEvents(Action<string> listener)
    {
        EventChannelManager.Instance.UnregisterEventByName(gameObject, MultiplayerChannelName, "*", listener);
    }


    public List<string> GetConnectedPeers()
    {
        return WebRTCManager.Instance.GetConnectedPeers();
    }

    public bool IsLocalPeer(string peerId)
    {
        return peerId == WebRTCManager.Instance.LocalPeerId;
    }

    private void OnDestroy()
    {
        if (WebRTCManager.Instance != null)
        {
            WebRTCManager.Instance.OnMessageReceived -= HandleWebRTCMessage;
        }
    }
}