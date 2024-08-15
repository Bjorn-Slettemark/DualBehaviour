using UnityEngine;
using System.Collections.Generic;
using System;

public class MultiplayerManager : MonoBehaviour
{
    private static MultiplayerManager _instance;
    public static MultiplayerManager Instance => _instance;

    private const string MultiplayerChannelName = "MultiplayerChannel";
    private Dictionary<string, Action<string>> peerEventHandlers = new Dictionary<string, Action<string>>();


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
    public void HandleWebRTCMessage(string senderPeerId, string eventName)
    {
        Debug.Log($"Received WebRTC message from {senderPeerId}: {eventName}");
        if (peerEventHandlers.TryGetValue(senderPeerId, out var handler))
        {
            handler.Invoke(eventName);
        }
        else
        {
            Debug.LogWarning($"No handler registered for peer {senderPeerId}");
        }
    }

    public void SendEventToPeer(string peerId, string eventName)
    {
        WebRTCManager.Instance.SendDataMessage(eventName);
    }

    public void BroadcastEventToAllPeers(string eventName)
    {
        WebRTCManager.Instance.SendDataMessage(eventName);
        Debug.Log($"Broadcasting event to all peers: {eventName}");
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