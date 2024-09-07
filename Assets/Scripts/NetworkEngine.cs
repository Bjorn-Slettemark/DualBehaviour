using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class NetworkEngine : MonoBehaviour
{
    private static NetworkEngine _instance;
    public static NetworkEngine Instance => _instance;
    private string localPeerId;

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
    
    }

    private IEnumerator Initialize()
    {
        while (LocalWebRTCEngine.Instance == null || string.IsNullOrEmpty(LocalWebRTCEngine.Instance.LocalPeerId))
        {
            yield return null;
        }

        localPeerId = LocalWebRTCEngine.Instance.LocalPeerId;

        while (WebRTCEngine.Instance == null)
        {
            yield return null;
        }

        WebRTCEngine.Instance.OnPeerListUpdated += HandlePeerListUpdated;
        LocalWebRTCEngine.Instance.OnLocalConnectionEstablished += OnLocalConnectionEstablished;

        isInitialized = true;
        Debug.Log("[MultiplayerManager] Initialization complete");
    }

    // Broadcasting
    public void BroadcastLevelChange(string levelName)
    {
            BroadcastEventToAllPeers(levelName);
    }

    public void BroadcastEventToAllPeers(string eventData)
    {
        // Use the existing method to send the serialized message
        WebRTCEngine.Instance.SendDataMessage(eventData);
    }

    public void BroadcastToPeer(string targetPeer, string eventData)
    {
        //Debug.Log($"[MultiplayerManager] Broadcasting event to all peers: {eventData}");
        WebRTCEngine.Instance.SendDataMessage(targetPeer, eventData);
    }

    // Recieving
    public void HandleWebRTCMessage(string message)
    {
        string[] parts = message.Split(new char[] { ':' }, 2);
        if (parts.Length != 2)
        {
            Debug.LogError($"Invalid message format: {message}");
            return;
        }

        string channelName = parts[0];
        string eventData = parts[1];

        EventChannelManager.Instance.RaiseEvent(channelName, eventData);
    }




    // Connection

    private void EnsurePeerChannelExists(string peerId)
    {
        string channelName = GetPeerChannelName(peerId);
        if (!EventChannelManager.Instance.ChannelExists(channelName))
        {
            EventChannelManager.Instance.RegisterNewChannel(channelName);
            Debug.Log($"[MultiplayerManager] Created event channel for peer: {peerId}");
        }
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
        var allChannels = EventChannelManager.Instance.AllChannels;
        foreach (var channel in allChannels)
        {
            if (channel.name.StartsWith("PeerChannel"))
            {
                string peerId = channel.name.Substring("PeerChannel".Length);
                if (!peers.Contains(peerId) && peerId != localPeerId)
                {
                    RemovePeerChannel(peerId);
                }
            }
        }

        // Log the final list of connected peers
        Debug.Log($"[MultiplayerManager] Connected peers: {string.Join(", ", GetConnectedPeers())}");
    }

    public List<string> GetConnectedPeers()
    {
        return WebRTCEngine.Instance.GetConnectedPeers();
    }

    public bool IsLocalPeer(string peerId)
    {
        return peerId == localPeerId;
    }

    // Channels

    private void CreatePeerChannel(string peerId)
    {
        string channelName = GetPeerChannelName(peerId);
        EventChannelManager.Instance.RegisterNewChannel(channelName);
        Debug.Log($"Created event channel for peer: {peerId}");
    }

    private void RemovePeerChannel(string peerId)
    {
        string channelName = GetPeerChannelName(peerId);
        Debug.Log($"Removed channel for peer: {peerId}");
    }

    public string GetPeerChannelName(string peerId)
    {
        return $"PeerChannel{peerId}";
    }

    public bool IsLocalPeerId(string peerId)
    {
        return peerId == LocalWebRTCEngine.Instance.LocalPeerId;
    }

    private void OnDestroy()
    {
        if (WebRTCEngine.Instance != null)
        {
            WebRTCEngine.Instance.OnPeerListUpdated -= HandlePeerListUpdated;

        }

    }

}