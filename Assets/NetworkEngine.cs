using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
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
        localPeerId = LocalWebRTCEngine.Instance.LocalPeerId;

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
        if (WebRTCEngine.Instance.IsHost)
        {
            NetworkMessage message = NetworkMessageFactory.CreateLevelChange(localPeerId, levelName);
            BroadcastEventToAllPeers(message);
        }
        else
        {
            Debug.LogWarning("Only the host can broadcast level changes.");
        }
    }

    public void BroadcastEventToAllPeers(NetworkMessage message)
    {
        string serializedMessage = message.Serialize();
        Debug.Log("Outgoing DATA serialized message: " + serializedMessage);

        // Use the existing method to send the serialized message
        WebRTCEngine.Instance.SendDataMessage(serializedMessage);

    }

    //public void BroadcastEventToAllPeers(string eventData)
    //{
    //    Debug.Log($"[MultiplayerManager] Broadcasting event to all peers: {eventData}");
    //    WebRTCEngine.Instance.SendDataMessage(eventData);
    //}
    public void BroadcastToPeer(string targetPeer, string eventData)
    {
        //Debug.Log($"[MultiplayerManager] Broadcasting event to all peers: {eventData}");
        WebRTCEngine.Instance.SendDataMessage(targetPeer, eventData);
    }
    // Recieving

    public void HandleWebRTCMessage(string message)
    {

        MultiplayerManager.Instance.HandleIncomingMessage(message);
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
        var allChannels = EventChannelManager.Instance.EventChannels;
        foreach (var channel in allChannels)
        {
            if (channel.name.StartsWith("PeerChannel:"))
            {
                string peerId = channel.name.Substring("PeerChannel:".Length);
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
        EventChannelManager.Instance.UnSubscribeChannel(null, channelName);
        Debug.Log($"Removed channel for peer: {peerId}");
    }

    public string GetPeerChannelName(string peerId)
    {
        return $"PeerChannel:{peerId}";
    }



    private void OnDestroy()
    {
        if (WebRTCEngine.Instance != null)
        {
            WebRTCEngine.Instance.OnPeerListUpdated -= HandlePeerListUpdated;

        }

    }

}