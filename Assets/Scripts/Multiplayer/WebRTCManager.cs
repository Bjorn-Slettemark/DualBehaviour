using UnityEngine;
using Unity.WebRTC;
using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using System.Linq;


public class WebRTCManager : MonoBehaviour
{
    private static WebRTCManager _instance;
    public static WebRTCManager Instance => _instance;

    private DatabaseReference database;
    private string roomName;

    private string localPeerId;
    public string LocalPeerId {  get { return localPeerId; } }

    private Dictionary<string, RTCPeerConnection> peerConnections = new Dictionary<string, RTCPeerConnection>();
    private Dictionary<string, RTCDataChannel> dataChannels = new Dictionary<string, RTCDataChannel>();

    public event Action<string, string> OnMessageReceived; // peerId, message
    public event Action<List<string>> OnPeerListUpdated;

    private Dictionary<string, List<RTCIceCandidate>> iceCandidateQueue = new Dictionary<string, List<RTCIceCandidate>>();

    private const float KEEP_ALIVE_INTERVAL = 0.1f;
    private const string KEEP_ALIVE_MESSAGE = "KEEP_ALIVE";
    private Dictionary<string, Coroutine> keepAliveCoroutines = new Dictionary<string, Coroutine>();
    private Dictionary<string, float> lastKeepAliveTime = new Dictionary<string, float>();


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
        localPeerId = DualBehaviourUtils.GenerateRandomString(5);

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
        Debug.Log($"MultiplayerManager: Local Peer ID is {localPeerId}");

        StartCoroutine(WebRTC.Update());
        Debug.Log("MultiplayerManager: WebRTC Update coroutine started");
    }

    public void AddPeerConnection(string peerId, RTCPeerConnection peerConnection)
    {
        peerConnections[peerId] = peerConnection;
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
            Debug.Log($"WebRTCManager: Peer found - {peerId}");

            if (peerId != localPeerId)
            {
                peers.Add(peerId);

                if (!peerConnections.ContainsKey(peerId))
                {
                    Debug.Log($"WebRTCManager: New peer found - {peerId}");
                    CreatePeerConnection(peerId);
                    StartCoroutine(InitiateConnection(peerId));
                }
            }
        }

        // Remove connections for peers that are no longer in the room
        foreach (var peer in peerConnections.Keys.ToList())
        {
            if (!peers.Contains(peer) && peer != LocalPeerConnectionSetup.Instance.RemotePeerId)
            {
                Debug.Log($"MultiplayerManager: Peer left - {peer}");
                ClosePeerConnection(peer);
            }
        }

        Debug.Log($"MultiplayerManager: Updated peer list - {string.Join(", ", peers)}");
        OnPeerListUpdated?.Invoke(peers);
    }
    private IEnumerator InitiateConnection(string peerId)
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 1f)); // Add a small random delay

        if (string.Compare(localPeerId, peerId) < 0)
        {
            yield return StartCoroutine(CreateOffer(peerId));
        }
        else
        {
            ListenForOffer(peerId);
        }
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
        Debug.Log($"WebRTCManager: Joining room {roomName}");
        this.roomName = roomName;
        isHost = false;
        database.Child("rooms").Child(roomName).Child("peers").Child(localPeerId).SetValueAsync(true).ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log($"WebRTCManager: Successfully joined room {roomName} in Firebase");
                ListenForPeers();
                ConnectToExistingPeers();
            }
            else
            {
                Debug.LogError($"WebRTCManager: Failed to join room in Firebase: {task.Exception}");
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

    private void ConnectToExistingPeers()
    {
        database.Child("rooms").Child(roomName).Child("peers").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted && task.Result.Value != null)
            {
                foreach (var child in task.Result.Children)
                {
                    string peerId = child.Key;
                    if (peerId != localPeerId && !peerConnections.ContainsKey(peerId))
                    {
                        CreatePeerConnection(peerId);
                        StartCoroutine(CreateOffer(peerId));
                    }
                }
            }
        });
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
        pc.OnIceConnectionChange = state => HandleIceConnectionChange(peerId, state);
        pc.OnConnectionStateChange = state => HandleConnectionStateChange(peerId, state);

        var dataChannel = pc.CreateDataChannel("data");
        SetupDataChannel(peerId, dataChannel);

        //StartCoroutine(CreateOffer(peerId));
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

        ListenForAnswer(peerId);
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
        Debug.Log($"WebRTCManager: Listening for offer from {peerId}");
        database.Child("rooms").Child(roomName).Child("offers").Child(peerId).Child(localPeerId)
            .ValueChanged += (sender, args) => HandleRemoteSessionDescription(peerId, args, RTCSdpType.Offer);
    }


    private void HandleRemoteSessionDescription(string peerId, ValueChangedEventArgs args, RTCSdpType type)
    {
        if (args.DatabaseError != null || args.Snapshot.Value == null) return;

        Debug.Log($"WebRTCManager: Received {type} from {peerId}");
        string sdpMessage = args.Snapshot.Value.ToString();
        var init = JsonUtility.FromJson<RTCSessionDescriptionInit>(sdpMessage);

        var pc = peerConnections[peerId];
        var remoteDesc = new RTCSessionDescription { sdp = init.sdp, type = type };
        StartCoroutine(SetRemoteDescriptionAndCreateAnswer(pc, peerId, remoteDesc));
    }

    private IEnumerator SetRemoteDescriptionAndCreateAnswer(RTCPeerConnection pc, string peerId, RTCSessionDescription remoteDesc)
    {
        Debug.Log($"WebRTCManager: Setting remote description for {peerId}");
        var op = pc.SetRemoteDescription(ref remoteDesc);
        yield return op;

        if (!op.IsError)
        {
            Debug.Log($"WebRTCManager: Remote description set for {peerId}");
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



    public void AddDataChannel(string peerId, RTCDataChannel dataChannel)
    {
        dataChannels[peerId] = dataChannel;
        dataChannel.OnMessage += (bytes) =>
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            OnMessageReceived?.Invoke(peerId, message);
        };
    }

    private void SetupDataChannel(string peerId, RTCDataChannel channel)
    {
        Debug.Log($"WebRTCManager: Setting up data channel for {peerId}");
        dataChannels[peerId] = channel;

        channel.OnClose = () =>
        {
            Debug.Log($"WebRTCManager: Data channel closed for {peerId}.");
            if (peerId != localPeerId)
            {
                StartCoroutine(TryReconnectDataChannel(peerId));
            }
        };

        channel.OnMessage = message => HandleDataMessage(peerId, message);
    }

    private void HandleDataChannel(string peerId, RTCDataChannel channel)
    {
        Debug.Log($"MultiplayerManager: Received data channel for {peerId}");
        SetupDataChannel(peerId, channel);
    }



    private void HandleIceConnectionChange(string peerId, RTCIceConnectionState state)
    {
        Debug.Log($"MultiplayerManager: ICE Connection state changed for {peerId}: {state}");
        if (state == RTCIceConnectionState.Connected || state == RTCIceConnectionState.Completed)
        {
            Debug.Log($"MultiplayerManager: ICE Connection established with {peerId}");
        }
    }

    private void HandleConnectionStateChange(string peerId, RTCPeerConnectionState state)
    {
        Debug.Log($"MultiplayerManager: Connection state changed for {peerId}: {state}");
        if (state == RTCPeerConnectionState.Connected)
        {
            Debug.Log($"MultiplayerManager: WebRTC connection established with {peerId}");
            StopCoroutine(nameof(ReconnectionAttempt));  // Stop any ongoing reconnection attempts
        }
        else if (state == RTCPeerConnectionState.Disconnected || state == RTCPeerConnectionState.Failed)
        {
            Debug.Log($"MultiplayerManager: WebRTC connection lost with {peerId}. Attempting to reconnect.");
            StartCoroutine(ReconnectionAttempt(peerId));
        }
    }

    private IEnumerator ReconnectionAttempt(string peerId)
    {
        int attempts = 0;
        const int maxAttempts = 5;
        const float delayBetweenAttempts = 2f;

        while (attempts < maxAttempts)
        {
            yield return new WaitForSeconds(delayBetweenAttempts);
            attempts++;

            Debug.Log($"MultiplayerManager: Reconnection attempt {attempts} for {peerId}");
            ClosePeerConnection(peerId);
            CreatePeerConnection(peerId);

            // Wait for connection to be established
            yield return new WaitForSeconds(5f);

            if (peerConnections.TryGetValue(peerId, out var pc) && pc.ConnectionState == RTCPeerConnectionState.Connected)
            {
                Debug.Log($"MultiplayerManager: Reconnection successful for {peerId}");
                yield break;
            }
        }

        Debug.LogError($"MultiplayerManager: Failed to reconnect to {peerId} after {maxAttempts} attempts");
    }

    private IEnumerator TryReconnectPeerConnection(string peerId)
    {
        yield return new WaitForSeconds(1f);
        ClosePeerConnection(peerId);
        CreatePeerConnection(peerId);
    }



    private IEnumerator TryReconnectDataChannel(string peerId)
    {
        Debug.Log($"MultiplayerManager: Attempting to reconnect data channel for {peerId}");
        yield return new WaitForSeconds(1f);

        if (peerConnections.TryGetValue(peerId, out RTCPeerConnection pc))
        {
            if (!dataChannels.ContainsKey(peerId) || dataChannels[peerId].ReadyState == RTCDataChannelState.Closed)
            {
                var dataChannel = pc.CreateDataChannel("data");
                SetupDataChannel(peerId, dataChannel);
                Debug.Log($"MultiplayerManager: Created new data channel for {peerId}");
            }
            else
            {
                Debug.Log($"MultiplayerManager: Data channel already exists for {peerId}. State: {dataChannels[peerId].ReadyState}");
            }
        }
        else
        {
            Debug.LogError($"MultiplayerManager: Failed to reconnect. No peer connection found for {peerId}");
        }
    }

    private void HandleDataMessage(string peerId, byte[] bytes)
    {
        string message = System.Text.Encoding.UTF8.GetString(bytes);
        if (message == KEEP_ALIVE_MESSAGE && peerId != localPeerId)
        {
            Debug.Log($"MultiplayerManager: Received keep-alive message from {peerId}");
            lastKeepAliveTime[peerId] = Time.time;
        }
        else
        {
            Debug.Log($"MultiplayerManager: Received message from {peerId}: {message}");
            OnMessageReceived?.Invoke(peerId, message);
        }
    }


    public void SendDataMessage(string message)
    {
        Debug.Log($"WebRTCManager: Sending data message to all peers: {message}");
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(message);

        // Send to local peer
        //LocalPeerConnectionSetup.Instance.SendLocalMessage(message);
        //Debug.Log("WebRTCManager: Data message sent to local peer");

        // Send to remote peers
        foreach (var kvp in dataChannels)
        {
            string peerId = kvp.Key;
            RTCDataChannel channel = kvp.Value;
            if (channel.ReadyState == RTCDataChannelState.Open)
            {
                channel.Send(bytes);
                Debug.Log($"WebRTCManager: Data message sent to remote peer {peerId}");
            }
            else
            {
                Debug.LogWarning($"WebRTCManager: Cannot send data message to remote peer {peerId}. Channel state: {channel.ReadyState}");
            }
        }

 
    }



    public List<string> GetConnectedPeers()
    {
        return new List<string>(peerConnections.Keys);
    }

    private void ClosePeerConnection(string peerId)
    {
        Debug.Log($"MultiplayerManager: Closing peer connection for {peerId}");

        if (dataChannels.TryGetValue(peerId, out var channel))
        {
            channel.Close();
            dataChannels.Remove(peerId);
            Debug.Log($"MultiplayerManager: Data channel closed and removed for {peerId}");
        }

        if (peerConnections.TryGetValue(peerId, out var pc))
        {
            pc.Close();
            peerConnections.Remove(peerId);
            Debug.Log($"MultiplayerManager: Peer connection closed and removed for {peerId}");
        }

        // Remove all signaling data for this peer
        database.Child("rooms").Child(roomName).Child("offers").Child(localPeerId).Child(peerId).RemoveValueAsync();
        database.Child("rooms").Child(roomName).Child("answers").Child(localPeerId).Child(peerId).RemoveValueAsync();
        database.Child("rooms").Child(roomName).Child("iceCandidates").Child(localPeerId).Child(peerId).RemoveValueAsync();
    }



    private void OnCreateSessionDescriptionError(RTCError error)
    {
        Debug.LogError($"MultiplayerManager: Failed to create session description: {error.message}");
    }

    private void OnSetSessionDescriptionError(ref RTCError error)
    {
        Debug.LogError($"MultiplayerManager: Failed to set session description: {error.message}");
    }



    private void HandleIceCandidate(string peerId, RTCIceCandidate iceCandidate)
    {
        Debug.Log($"MultiplayerManager: ICE candidate gathered for {peerId}: {iceCandidate.Candidate}");

        // Send only the candidate string
        database.Child("rooms").Child(roomName).Child("iceCandidates").Child(localPeerId).Child(peerId).Push().SetValueAsync(iceCandidate.Candidate);
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


    private void OnDestroy()
    {
        Debug.Log("WebRTCManager: Destroying and leaving room");
        foreach (var peerId in peerConnections.Keys.ToList())
        {
            ClosePeerConnection(peerId);

        }
        LeaveRoom();
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
