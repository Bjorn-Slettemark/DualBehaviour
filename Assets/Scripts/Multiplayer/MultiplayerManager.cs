using UnityEngine;
using Unity.WebRTC;
using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;

public class MultiplayerManager : MonoBehaviour
{
    private static MultiplayerManager _instance;
    public static MultiplayerManager Instance => _instance;

    private DatabaseReference database;
    private string roomName;
    private string localPeerId;
    private Dictionary<string, RTCPeerConnection> peerConnections = new Dictionary<string, RTCPeerConnection>();
    private Dictionary<string, RTCDataChannel> dataChannels = new Dictionary<string, RTCDataChannel>();

    public event Action<string> OnMessageReceived;
    public event Action<List<string>> OnPeerListUpdated;

    private Dictionary<string, List<RTCIceCandidate>> iceCandidateQueue = new Dictionary<string, List<RTCIceCandidate>>();
   
    private const float KEEP_ALIVE_INTERVAL = 5f; // Send keep-alive message every 5 seconds
    private const string KEEP_ALIVE_MESSAGE = "keep-alive";
    private Dictionary<string, Coroutine> keepAliveCoroutines = new Dictionary<string, Coroutine>();



    private bool isHost;

    private void Awake()
    {
        Debug.Log("MultiplayerManager: Awake called");
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Multiple instances of MultiplayerManager detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Debug.Log("MultiplayerManager: Start called");
        if (Firebase.FirebaseApp.DefaultInstance == null)
        {
            Debug.LogError("Firebase has not been initialized. Please make sure Firebase is set up correctly.");
            return;
        }
        database = FirebaseDatabase.DefaultInstance.RootReference;
        localPeerId = DualBehaviourUtils.GenerateRandomString(5);
        Debug.Log($"MultiplayerManager: Local Peer ID is {localPeerId}");
        StartCoroutine(WebRTC.Update());
        Debug.Log("MultiplayerManager: WebRTC Update coroutine started");
    }

    public void CreateRoom(string roomName)
    {
        if (database == null)
        {
            Debug.LogError("MultiplayerManager: Firebase database not initialized");
            return;
        }

        Debug.Log($"MultiplayerManager: Creating room {roomName}");
        this.roomName = roomName;
        isHost = true;
        database.Child("rooms").Child(roomName).Child("peers").Child(localPeerId).SetValueAsync(true).ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log($"MultiplayerManager: Successfully created room {roomName} in Firebase");
                ListenForPeers();
            }
            else
            {
                Debug.LogError($"MultiplayerManager: Failed to create room in Firebase: {task.Exception}");
            }
        });
    }


    public void JoinRoom(string roomName)
    {
        Debug.Log($"MultiplayerManager: Joining room {roomName}");
        this.roomName = roomName;
        isHost = false;
        database.Child("rooms").Child(roomName).Child("peers").Child(localPeerId).SetValueAsync(true).ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log($"MultiplayerManager: Successfully joined room {roomName} in Firebase");
                ListenForPeers();
            }
            else
            {
                Debug.LogError($"MultiplayerManager: Failed to join room in Firebase: {task.Exception}");
            }
        });
    }

    public void LeaveRoom()
    {
        if (string.IsNullOrEmpty(roomName)) return;

        Debug.Log($"MultiplayerManager: Leaving room {roomName}");
        foreach (var peer in peerConnections)
        {
            ClosePeerConnection(peer.Key);
        }

        database.Child("rooms").Child(roomName).Child("peers").Child(localPeerId).RemoveValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log($"MultiplayerManager: Successfully left room {roomName} in Firebase");
            }
            else
            {
                Debug.LogError($"MultiplayerManager: Failed to leave room in Firebase: {task.Exception}");
            }
        });
        roomName = null;
    }

    private void ListenForPeers()
    {
        Debug.Log($"MultiplayerManager: Start listening for peers in room {roomName}");
        database.Child("rooms").Child(roomName).Child("peers").ValueChanged += HandlePeersChanged;
    }

    private void HandlePeersChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError($"MultiplayerManager: Database error - {args.DatabaseError.Message}");
            return;
        }

        if (args.Snapshot.Value == null) return;

        Debug.Log("MultiplayerManager: Peer list changed");
        Debug.Log($"MultiplayerManager: Snapshot value: {args.Snapshot.GetRawJsonValue()}");

        var peers = new List<string>();
        foreach (var child in args.Snapshot.Children)
        {
            string peerId = child.Key;
            Debug.Log($"MultiplayerManager: Peer found - {peerId}");
            if (peerId != localPeerId)
            {
                peers.Add(peerId);
                if (!peerConnections.ContainsKey(peerId))
                {
                    Debug.Log($"MultiplayerManager: New peer found - {peerId}");
                    CreatePeerConnection(peerId);
                    if (isHost)
                    {
                        StartCoroutine(CreateOffer(peerId));
                    }
                }
            }
        }

        foreach (var peer in peerConnections.Keys)
        {
            if (!peers.Contains(peer))
            {
                Debug.Log($"MultiplayerManager: Peer left - {peer}");
                ClosePeerConnection(peer);
            }
        }

        Debug.Log($"MultiplayerManager: Updated peer list - {string.Join(", ", peers)}");
        OnPeerListUpdated?.Invoke(peers);
    }

    private void CreatePeerConnection(string peerId)
    {
        Debug.Log($"MultiplayerManager: Creating peer connection for {peerId}");
        RTCConfiguration config = default;
        config.iceServers = new[] { new RTCIceServer { urls = new[] { "stun:stun.l.google.com:19302" } } };

        var pc = new RTCPeerConnection(ref config);
        peerConnections[peerId] = pc;

        pc.OnIceCandidate = candidate => HandleIceCandidate(peerId, candidate);
        pc.OnDataChannel = channel => HandleDataChannel(peerId, channel);
        pc.OnIceConnectionChange = state => Debug.Log($"MultiplayerManager: ICE Connection state changed for {peerId}: {state}");
        pc.OnConnectionStateChange = state => Debug.Log($"MultiplayerManager: Connection state changed for {peerId}: {state}");

        var dataChannel = pc.CreateDataChannel("data");
        Debug.Log($"MultiplayerManager: Data channel created for {peerId}");
        dataChannel.OnOpen = () => Debug.Log($"MultiplayerManager: Data channel opened for {peerId}");
        dataChannel.OnClose = () => Debug.Log($"MultiplayerManager: Data channel closed for {peerId}");
        dataChannel.OnMessage = message => HandleMessage(peerId, message);
        dataChannels[peerId] = dataChannel;

        pc.OnIceConnectionChange = state =>
        {
            Debug.Log($"MultiplayerManager: ICE Connection state changed for {peerId}: {state}");
            if (state == RTCIceConnectionState.Connected || state == RTCIceConnectionState.Completed)
            {
                Debug.Log($"MultiplayerManager: ICE Connection established with {peerId}");
            }
        };

        pc.OnConnectionStateChange = state =>
        {
            Debug.Log($"MultiplayerManager: Connection state changed for {peerId}: {state}");
            if (state == RTCPeerConnectionState.Connected)
            {
                Debug.Log($"MultiplayerManager: WebRTC connection established with {peerId}");
            }
        };

        dataChannel.OnOpen = () =>
        {
            Debug.Log($"MultiplayerManager: Data channel opened for {peerId}. Ready for data exchange.");
        };

        StartCoroutine(CreateOffer(peerId));
        ListenForIceCandidates(peerId);
    }

    private IEnumerator CreateOffer(string peerId)
    {
        Debug.Log($"MultiplayerManager: Creating offer for {peerId}");
        var pc = peerConnections[peerId];
        var op = pc.CreateOffer();
        yield return op;

        if (!op.IsError)
        {
            Debug.Log($"MultiplayerManager: Offer created successfully for {peerId}");
            yield return StartCoroutine(OnCreateOfferSuccess(pc, peerId, op.Desc));
        }
        else
        {
            OnCreateSessionDescriptionError(op.Error);
        }

        if (!isHost)
        {
            ListenForOffer(peerId);
        }
        ListenForIceCandidates(peerId);
    }


    private IEnumerator OnCreateOfferSuccess(RTCPeerConnection pc, string peerId, RTCSessionDescription desc)
    {
        Debug.Log($"MultiplayerManager: Setting local description for {peerId}");
        var op = pc.SetLocalDescription(ref desc);
        yield return op;

        if (!op.IsError)
        {
            Debug.Log($"MultiplayerManager: Local description set for {peerId}");
            string sdpMessage = JsonUtility.ToJson(new RTCSessionDescriptionInit
            {
                sdp = desc.sdp,
                type = desc.type.ToString().ToLower()
            });

            Debug.Log($"MultiplayerManager: Sending offer to {peerId}");
            database.Child("rooms").Child(roomName).Child("offers").Child(localPeerId).Child(peerId).SetValueAsync(sdpMessage).ContinueWith(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    Debug.Log($"MultiplayerManager: Offer sent successfully to {peerId}");
                    ListenForAnswer(peerId);
                }
                else
                {
                    Debug.LogError($"MultiplayerManager: Failed to send offer to {peerId}: {task.Exception}");
                }
            });
        }
        else
        {
            var error = op.Error;
            OnSetSessionDescriptionError(ref error);
        }
    }

    private void ListenForAnswer(string peerId)
    {
        Debug.Log($"MultiplayerManager: Listening for answer from {peerId}");
        database.Child("rooms").Child(roomName).Child("answers").Child(peerId).Child(localPeerId)
            .ValueChanged += (sender, args) => HandleRemoteSessionDescription(peerId, args, RTCSdpType.Answer);
    }

    private void ListenForOffer(string peerId)
    {
        Debug.Log($"MultiplayerManager: Listening for offer from {peerId}");
        database.Child("rooms").Child(roomName).Child("offers").Child(peerId).Child(localPeerId)
            .ValueChanged += (sender, args) => HandleRemoteSessionDescription(peerId, args, RTCSdpType.Offer);
    }

    private void HandleRemoteSessionDescription(string peerId, ValueChangedEventArgs args, RTCSdpType type)
    {
        if (args.DatabaseError != null || args.Snapshot.Value == null) return;

        Debug.Log($"MultiplayerManager: Received {type} from {peerId}");
        string sdpMessage = args.Snapshot.Value.ToString();
        var init = JsonUtility.FromJson<RTCSessionDescriptionInit>(sdpMessage);

        var pc = peerConnections[peerId];
        var remoteDesc = new RTCSessionDescription { sdp = init.sdp, type = type };
        StartCoroutine(SetRemoteDescriptionAndHandleCandidates(pc, peerId, remoteDesc));
    }

    private IEnumerator SetRemoteDescriptionAndHandleCandidates(RTCPeerConnection pc, string peerId, RTCSessionDescription remoteDesc)
    {
        Debug.Log($"MultiplayerManager: Setting remote description for {peerId}");
        var op = pc.SetRemoteDescription(ref remoteDesc);
        yield return op;

        if (!op.IsError)
        {
            Debug.Log($"MultiplayerManager: Remote description set for {peerId}");
            ProcessIceCandidateQueue(peerId);

            if (remoteDesc.type == RTCSdpType.Offer)
            {
                yield return StartCoroutine(CreateAnswer(pc, peerId));
            }
        }
        else
        {
            var error = op.Error;
            OnSetSessionDescriptionError(ref error);
        }
    }
    private IEnumerator SetRemoteDescription(RTCPeerConnection pc, string peerId, RTCSessionDescription remoteDesc)
    {
        Debug.Log($"MultiplayerManager: Setting remote description for {peerId}");
        var op = pc.SetRemoteDescription(ref remoteDesc);
        yield return op;

        if (!op.IsError)
        {
            Debug.Log($"MultiplayerManager: Remote description set for {peerId}");
            ProcessIceCandidateQueue(peerId);
            if (remoteDesc.type == RTCSdpType.Offer)
            {
                yield return StartCoroutine(CreateAnswer(pc, peerId));
            }
        }
        else
        {
            var error = op.Error;
            OnSetSessionDescriptionError(ref error);
        }
    }
    private IEnumerator CreateAnswer(RTCPeerConnection pc, string peerId)
    {
        Debug.Log($"MultiplayerManager: Creating answer for {peerId}");
        var op = pc.CreateAnswer();
        yield return op;

        if (!op.IsError)
        {
            Debug.Log($"MultiplayerManager: Answer created for {peerId}");
            yield return StartCoroutine(OnCreateAnswerSuccess(pc, peerId, op.Desc));
        }
        else
        {
            OnCreateSessionDescriptionError(op.Error);
        }
    }

    private IEnumerator OnCreateAnswerSuccess(RTCPeerConnection pc, string peerId, RTCSessionDescription desc)
    {
        Debug.Log($"MultiplayerManager: Setting local description (answer) for {peerId}");
        var op = pc.SetLocalDescription(ref desc);
        yield return op;

        if (!op.IsError)
        {
            Debug.Log($"MultiplayerManager: Local description (answer) set for {peerId}");
            string answerMessage = JsonUtility.ToJson(new RTCSessionDescriptionInit
            {
                sdp = desc.sdp,
                type = desc.type.ToString().ToLower()
            });

            Debug.Log($"MultiplayerManager: Sending answer to {peerId}");
            database.Child("rooms").Child(roomName).Child("answers").Child(localPeerId).Child(peerId).SetValueAsync(answerMessage).ContinueWith(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    Debug.Log($"MultiplayerManager: Answer sent successfully to {peerId}");
                }
                else
                {
                    Debug.LogError($"MultiplayerManager: Failed to send answer to {peerId}: {task.Exception}");
                }
            });
        }
        else
        {
            var error = op.Error;
            OnSetSessionDescriptionError(ref error);
        }
    }

    private void HandleIceCandidate(string peerId, RTCIceCandidate iceCandidate)
    {
        Debug.Log($"MultiplayerManager: ICE candidate gathered for {peerId}: {iceCandidate.Candidate}");

        // Send only the candidate string
        database.Child("rooms").Child(roomName).Child("iceCandidates").Child(localPeerId).Child(peerId).Push().SetValueAsync(iceCandidate.Candidate);
    }

    private void HandleDataChannel(string peerId, RTCDataChannel channel)
    {
        Debug.Log($"MultiplayerManager: Handling data channel for {peerId}");
        dataChannels[peerId] = channel;
        channel.OnMessage = message => HandleMessage(peerId, message);
        channel.OnOpen = () =>
        {
            Debug.Log($"MultiplayerManager: Data channel opened for {peerId}");
            StartKeepAlive(peerId);
        };
        channel.OnClose = () =>
        {
            Debug.Log($"MultiplayerManager: Data channel closed for {peerId}");
            StopKeepAlive(peerId);
        };
    }

    private void StartKeepAlive(string peerId)
    {
        if (!keepAliveCoroutines.ContainsKey(peerId))
        {
            Debug.Log("Starting the keepalive function");
            keepAliveCoroutines[peerId] = StartCoroutine(KeepAliveCoroutine(peerId));
        }
    }

    private void StopKeepAlive(string peerId)
    {
        if (keepAliveCoroutines.TryGetValue(peerId, out Coroutine coroutine))
        {
            StopCoroutine(coroutine);
            keepAliveCoroutines.Remove(peerId);
        }
    }

    private IEnumerator KeepAliveCoroutine(string peerId)
    {
        while (true)
        {
            yield return new WaitForSeconds(KEEP_ALIVE_INTERVAL);
            Debug.Log("message sent keepalive!");
            SendKeepAliveMessage(peerId);
        }
    }

    private void SendKeepAliveMessage(string peerId)
    {
        if (dataChannels.TryGetValue(peerId, out RTCDataChannel channel) &&
            channel.ReadyState == RTCDataChannelState.Open)
        {
            byte[] keepAliveMessage = System.Text.Encoding.UTF8.GetBytes(KEEP_ALIVE_MESSAGE);
            channel.Send(keepAliveMessage);
            Debug.Log($"MultiplayerManager: Sent keep-alive message to {peerId}");
        }
    }

    private void HandleMessage(string peerId, byte[] bytes)
    {
        string message = System.Text.Encoding.UTF8.GetString(bytes);
        if (message == KEEP_ALIVE_MESSAGE)
        {
            Debug.Log($"MultiplayerManager: Received keep-alive message from {peerId}");
            // Optionally, you can reset a timer here to track the last received keep-alive
        }
        else
        {
            Debug.Log($"MultiplayerManager: Received message from {peerId}: {message}");
            OnMessageReceived?.Invoke($"{peerId}: {message}");
        }
    }

    public void SendDataMessage(string message)
    {
        Debug.Log($"MultiplayerManager: Sending message to all peers: {message}");
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(message);
        foreach (var kvp in dataChannels)
        {
            string peerId = kvp.Key;
            RTCDataChannel channel = kvp.Value;
            if (channel.ReadyState == RTCDataChannelState.Open)
            {
                channel.Send(bytes);
                Debug.Log($"MultiplayerManager: Message sent to {peerId} through channel: {channel.Label}");
            }
            else
            {
                Debug.LogWarning($"MultiplayerManager: Cannot send message to {peerId}. Channel state: {channel.ReadyState}");
            }
        }
    }

    private void ClosePeerConnection(string peerId)
    {
        Debug.Log($"MultiplayerManager: Closing peer connection for {peerId}");
        StopKeepAlive(peerId);
        if (peerConnections.TryGetValue(peerId, out var pc))
        {
            pc.Close();
            peerConnections.Remove(peerId);
            Debug.Log($"MultiplayerManager: Peer connection closed and removed for {peerId}");
        }

        if (dataChannels.TryGetValue(peerId, out var channel))
        {
            channel.Close();
            dataChannels.Remove(peerId);
            Debug.Log($"MultiplayerManager: Data channel closed and removed for {peerId}");
        }

        // Remove all signaling data for this peer
        database.Child("rooms").Child(roomName).Child("offers").Child(localPeerId).Child(peerId).RemoveValueAsync();
        database.Child("rooms").Child(roomName).Child("answers").Child(localPeerId).Child(peerId).RemoveValueAsync();
        database.Child("rooms").Child(roomName).Child("iceCandidates").Child(localPeerId).Child(peerId).RemoveValueAsync();
    }

    private void OnDestroy()
    {
        Debug.Log("MultiplayerManager: Destroying and leaving room");
        foreach (var peerId in keepAliveCoroutines.Keys)
        {
            StopKeepAlive(peerId);
        }
        LeaveRoom();
    }

    private void OnCreateSessionDescriptionError(RTCError error)
    {
        Debug.LogError($"MultiplayerManager: Failed to create session description: {error.message}");
    }

    private void OnSetSessionDescriptionError(ref RTCError error)
    {
        Debug.LogError($"MultiplayerManager: Failed to set session description: {error.message}");
    }

    // Helper method to parse and add remote ICE candidates
    private void AddRemoteIceCandidate(string peerId, string candidateString)
    {
        Debug.Log($"Received ICE candidate for {peerId}: {candidateString}");

        if (peerConnections.TryGetValue(peerId, out var pc))
        {
            RTCIceCandidateInit init = new RTCIceCandidateInit
            {
                candidate = candidateString,
                sdpMid = "0",
                sdpMLineIndex = 0
            };

            RTCIceCandidate candidate = new RTCIceCandidate(init);

            bool remoteDescriptionSet = false;
            try
            {
                remoteDescriptionSet = (pc.RemoteDescription.type == RTCSdpType.Offer || pc.RemoteDescription.type == RTCSdpType.Answer);
            }
            catch (System.Exception)
            {
                // If accessing RemoteDescription throws an exception, we assume it's not set
                remoteDescriptionSet = false;
            }

            if (!remoteDescriptionSet)
            {
                Debug.Log($"MultiplayerManager: Buffering ICE candidate for {peerId} as RemoteDescription is not set yet");
                if (!iceCandidateQueue.ContainsKey(peerId))
                    iceCandidateQueue[peerId] = new List<RTCIceCandidate>();
                iceCandidateQueue[peerId].Add(candidate);
            }
            else
            {
                pc.AddIceCandidate(candidate);
                Debug.Log($"MultiplayerManager: Added remote ICE candidate for {peerId}");
            }
        }
        else
        {
            Debug.LogWarning($"MultiplayerManager: Received ICE candidate for unknown peer {peerId}");
        }
    }
    private void ProcessIceCandidateQueue(string peerId)
{
    if (iceCandidateQueue.TryGetValue(peerId, out var queue))
    {
        Debug.Log($"MultiplayerManager: Processing queued ICE candidates for {peerId}. Count: {queue.Count}");
        foreach (var candidate in queue)
        {
            peerConnections[peerId].AddIceCandidate(candidate);
            Debug.Log($"MultiplayerManager: Added queued ICE candidate for {peerId}");
        }
        iceCandidateQueue.Remove(peerId);
    }
}    // Add this class to help with JSON deserialization
    [System.Serializable]
    private class CandidateWrapper
    {
        public string candidate;
        public string sdpMid;
        public ushort? sdpMLineIndex;
    }

    [System.Serializable]
    private class CandidateContainer
    {
        public string candidate;
        public string sdpMid;
    }
    // Temporary class to match the structure of the received JSON
    [System.Serializable]
    private class TempIceCandidate
    {
        public string candidate;
        public string sdpMid;
    }

    // Modify the ListenForIceCandidates method
    private void ListenForIceCandidates(string peerId)
    {
        Debug.Log($"MultiplayerManager: Start listening for ICE candidates from {peerId}");
        database.Child("rooms").Child(roomName).Child("iceCandidates").Child(peerId).Child(localPeerId)
            .ChildAdded += (sender, args) =>
            {
                if (args.Snapshot.Value != null)
                {
                    string candidateString = args.Snapshot.Value.ToString();
                    AddRemoteIceCandidate(peerId, candidateString);
                }
            };
    }


}
    [Serializable]
    public class RTCSessionDescriptionInit
    {
        public string sdp;
        public string type;
    }


[Serializable]
public class IceCandidateInfo
{
    public string candidate;
    public string sdpMid;
    public ushort? sdpMLineIndex;
}

