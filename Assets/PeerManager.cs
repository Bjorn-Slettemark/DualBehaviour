using Firebase.Database;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.WebRTC;
using System.Collections;

public class PeerManager : MonoBehaviour
{
    //public static PeerManager Instance { get; private set; }

    //public DatabaseReference database { get; private set; }

    //private Dictionary<string, RTCPeerConnection> peerConnections = new Dictionary<string, RTCPeerConnection>();

    //public bool IsInitialized { get; private set; }

    //public string roomName { get; private set; }

    //private string localPeerId;
    //public string LocalPeerId => localPeerId;

    //private bool isHost;

    //public bool IsHost => isHost;

    //private string localPeerChannelName;
    //public string LocalPeerChannelName => localPeerChannelName;

    //private string hostPeerId;
    //public string HostPeerId => hostPeerId;

    //private void Awake()
    //{
    //    if (Instance == null)
    //    {
    //        Instance = this;
    //        DontDestroyOnLoad(gameObject);
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }

    //    StartCoroutine(Initialize());

 
    //}

    //private IEnumerator Initialize()
    //{
    //    // Perform any necessary initialization here
    //    database = FirebaseDatabase.DefaultInstance.RootReference;

    //    localPeerId = LocalWebRTCManager.Instance.LocalPeerId;
    //    localPeerChannelName = "PeerChannel_" + LocalPeerId;
    //    peerConnections = WebRTCManager.Instance.peerConnections;

    //    // Wait for a frame to ensure everything is set up
    //    yield return null;

    //    // Trigger the initialization event
    //    IsInitialized = true;
    //}

    //private void Start()
    //{
    //}

    //public void CreateRoom(string roomName)
    //{
    //    if (database == null)
    //    {
    //        Debug.LogError("WebRTCManager: Firebase database not initialized");
    //        return;
    //    }

    //    Debug.Log($"WebRTCManager: Creating room {roomName}");
    //    this.roomName = roomName;
    //    isHost = true;
    //    hostPeerId = localPeerId;
    //    database.Child("rooms").Child(roomName).Child("host").SetValueAsync(localPeerId);

    //    database.Child("rooms").Child(roomName).Child("peers").Child(localPeerId).SetValueAsync(true).ContinueWith(task =>
    //    {
    //        if (task.IsCompleted && !task.IsFaulted)
    //        {
    //            Debug.Log($"WebRTCManager: Successfully created room {roomName} in Firebase");
    //            database.Child("rooms").Child(roomName).Child("host").SetValueAsync(localPeerId);
    //            //WebRTCManager.Instance.ListenForPeers();
    //            //ListenForPeers();
    //            database.Child("rooms").Child(roomName).Child("peers").ValueChanged += WebRTCManager.Instance.HandlePeersChanged;

    //        }
    //        else
    //        {
    //            Debug.LogError($"WebRTCManager: Failed to create room in Firebase: {task.Exception}");
    //        }
    //    });
    //}

    //public void JoinRoom(string roomName)
    //{
    //    Debug.Log($"WebRTCManager: Joining room {roomName}");
    //    this.roomName = roomName;
    //    isHost = false;
    //    //database.Child("rooms").Child(roomName).Child("host").GetValueAsync().ContinueWith(task =>
    //    //{
    //    //    if (task.IsCompleted && !task.IsFaulted && task.Result.Value != null)
    //    //    {
    //    //        hostPeerId = task.Result.Value.ToString();
    //    //    }
    //    //});

    //    database.Child("rooms").Child(roomName).Child("peers").Child(localPeerId).SetValueAsync(true).ContinueWith(task =>
    //    {
    //        if (task.IsCompleted && !task.IsFaulted)
    //        {
    //            Debug.Log($"WebRTCManager: Successfully joined room {roomName} in Firebase");
    //            //WebRTCManager.Instance.ListenForPeers();
    //            database.Child("rooms").Child(roomName).Child("peers").ValueChanged += WebRTCManager.Instance.HandlePeersChanged;

    //            WebRTCManager.Instance.ConnectToExistingPeers();
    //        }
    //        else
    //        {
    //            Debug.LogError($"WebRTCManager: Failed to join room in Firebase: {task.Exception}");
    //        }
    //    });
    //}

    //public void LeaveRoom()
    //{
    //    if (string.IsNullOrEmpty(roomName)) return;

    //    Debug.Log($"WebRTCManager: Leaving room {roomName}");
    //    foreach (var peer in peerConnections)
    //    {
    //        WebRTCManager.Instance.ClosePeerConnection(peer.Key);
    //    }

    //    if (isHost)
    //    {
    //        // Trigger host migration before leaving
    //        database.Child("rooms").Child(roomName).Child("host").RemoveValueAsync();
    //    }

    //    database.Child("rooms").Child(roomName).Child("peers").Child(localPeerId).RemoveValueAsync().ContinueWith(task =>
    //    {
    //        if (task.IsCompleted && !task.IsFaulted)
    //        {
    //            Debug.Log($"WebRTCManager: Successfully left room {roomName} in Firebase");
    //        }
    //        else
    //        {
    //            Debug.LogError($"WebRTCManager: Failed to leave room in Firebase: {task.Exception}");
    //        }
    //    });
    //    roomName = null;
    //    isHost = false;
    //}


    //public void HandleHostMigration()
    //{
    //    if (isHost) return; // Already the host

    //    var peers = new List<string>(peerConnections.Keys);
    //    peers.Add(localPeerId);
    //    peers.Sort();

    //    if (peers[0] == localPeerId)
    //    {
    //        isHost = true;
    //        hostPeerId = localPeerId;
    //        database.Child("rooms").Child(roomName).Child("host").SetValueAsync(localPeerId);

    //    }
    //}
    //public void AddPeerConnection(string peerId, RTCPeerConnection peerConnection)
    //{
    //    peerConnections[peerId] = peerConnection;
    //}

    //public string GetCurrentHost()
    //{
    //    return isHost ? localPeerId : null; // Only the host knows for sure it's the host
    //}

    //public bool IsPeerHost(string peerId)
    //{
    //    return isHost && peerId == localPeerId;
    //}
}