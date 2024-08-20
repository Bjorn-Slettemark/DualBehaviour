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

    public Dictionary<string, RTCPeerConnection> peerConnections = new Dictionary<string, RTCPeerConnection>();
    private Dictionary<string, RTCDataChannel> dataChannels = new Dictionary<string, RTCDataChannel>();

    public event Action<string, string> OnMessageReceived; // peerId, message
    public event Action<List<string>> OnPeerListUpdated;


    private Dictionary<string, List<RTCIceCandidate>> iceCandidateQueue = new Dictionary<string, List<RTCIceCandidate>>();
    public string roomName { get; private set; }


    private string localPeerId;
    public string LocalPeerId => localPeerId;

    private bool isHost;

    public bool IsHost => isHost;

    private string localPeerChannelName;
    public string LocalPeerChannelName => localPeerChannelName;

    private string hostPeerId;
    public string HostPeerId => hostPeerId;

    private DatabaseReference database;

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
        // Now it's safe to access PeerManager properties
        // Perform any necessary initialization here
        database = FirebaseDatabase.DefaultInstance.RootReference;

        localPeerId = LocalWebRTCManager.Instance.LocalPeerId;
        localPeerChannelName = "PeerChannel_" + LocalPeerId;
        peerConnections = WebRTCManager.Instance.peerConnections;


        Debug.Log($"WebRTCManager: Local Peer ID is {localPeerId}");
        StartCoroutine(WebRTC.Update());
        Debug.Log("WebRTCManager: WebRTC Update coroutine started");
    }
    public void CreateRoom(string roomName)
    {
        if (database == null)
        {
            Debug.LogError("WebRTCManager: Firebase database not initialized");
            return;
        }

        Debug.Log($"WebRTCManager: Creating room {roomName}");
        this.roomName = roomName;
        isHost = true;
        hostPeerId = localPeerId;
        database.Child("rooms").Child(roomName).Child("host").SetValueAsync(localPeerId);

        database.Child("rooms").Child(roomName).Child("peers").Child(localPeerId).SetValueAsync(true).ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log($"WebRTCManager: Successfully created room {roomName} in Firebase");
                database.Child("rooms").Child(roomName).Child("host").SetValueAsync(localPeerId);
                //WebRTCManager.Instance.ListenForPeers();
                //ListenForPeers();
                database.Child("rooms").Child(roomName).Child("peers").ValueChanged += WebRTCManager.Instance.HandlePeersChanged;

            }
            else
            {
                Debug.LogError($"WebRTCManager: Failed to create room in Firebase: {task.Exception}");
            }
        });
    }

    public void JoinRoom(string roomName)
    {
        Debug.Log($"WebRTCManager: Joining room {roomName}");
        this.roomName = roomName;
        isHost = false;
        database.Child("rooms").Child(roomName).Child("host").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted && task.Result.Value != null)
            {
                hostPeerId = task.Result.Value.ToString();
            }
        });

        database.Child("rooms").Child(roomName).Child("peers").Child(localPeerId).SetValueAsync(true).ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log($"WebRTCManager: Successfully joined room {roomName} in Firebase");
                //WebRTCManager.Instance.ListenForPeers();
                database.Child("rooms").Child(roomName).Child("peers").ValueChanged += WebRTCManager.Instance.HandlePeersChanged;

                WebRTCManager.Instance.ConnectToExistingPeers();
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

        Debug.Log($"WebRTCManager: Leaving room {roomName}");
        foreach (var peer in peerConnections)
        {
            WebRTCManager.Instance.ClosePeerConnection(peer.Key);
        }

        if (isHost)
        {
            // Trigger host migration before leaving
            database.Child("rooms").Child(roomName).Child("host").RemoveValueAsync();
        }

        database.Child("rooms").Child(roomName).Child("peers").Child(localPeerId).RemoveValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log($"WebRTCManager: Successfully left room {roomName} in Firebase");
            }
            else
            {
                Debug.LogError($"WebRTCManager: Failed to leave room in Firebase: {task.Exception}");
            }
        });
        roomName = null;
        isHost = false;
    }


    public void HandleHostMigration()
    {
        if (isHost) return; // Already the host

        var peers = new List<string>(peerConnections.Keys);
        peers.Add(localPeerId);
        peers.Sort();

        if (peers[0] == localPeerId)
        {
            isHost = true;
            hostPeerId = localPeerId;
            database.Child("rooms").Child(roomName).Child("host").SetValueAsync(localPeerId);

        }
    }
    public void AddPeerConnection(string peerId, RTCPeerConnection peerConnection)
    {
        peerConnections[peerId] = peerConnection;
    }

    public string GetCurrentHost()
    {
        return isHost ? localPeerId : null; // Only the host knows for sure it's the host
    }

    public bool IsPeerHost(string peerId)
    {
        return isHost && peerId == localPeerId;
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

    public void ConnectToExistingPeers()
    {
        Debug.Log("connect to existing peers.. ");
        database.Child("rooms").Child(roomName).Child("peers").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted && task.Result.Value != null)
            {
                Debug.Log("Found some peers...");
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
    public void HandlePeersChanged(object sender, ValueChangedEventArgs args)
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
                    WebRTCManager.Instance.CreatePeerConnection(peerId);
                    StartCoroutine(InitiateConnection(peerId));
                }
            }
            else
            {
                Debug.Log($"WebRTCManager: Skipping local peer - {peerId}");
            }
        }

        // Check if the host is still in the room
        database.Child("rooms").Child(roomName).Child("host").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted && task.Result.Value != null)
            {
                string hostPeerId = task.Result.Value.ToString();
                if (!peers.Contains(hostPeerId))
                {
                    WebRTCManager.Instance.HandleHostMigration();
                }
            }
        });


        // Remove connections for peers that are no longer in the room
        foreach (var peer in peerConnections.Keys.ToList())
        {
            if (!peers.Contains(peer) && peer != LocalWebRTCManager.Instance.RemotePeerId)
            {
                Debug.Log($"MultiplayerManager: Peer left - {peer}");
                ClosePeerConnection(peer);
            }
        }

        Debug.Log($"MultiplayerManager: Updated peer list - {string.Join(", ", peers)}");
        OnPeerListUpdated?.Invoke(peers);
    }

    public void CreatePeerConnection(string peerId)
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

        if (string.Compare(localPeerId, peerId) < 0)
        {
            var dataChannel = pc.CreateDataChannel("data");
            SetupDataChannel(peerId, dataChannel);
        }

        //StartCoroutine(CreateOffer(peerId));
        ListenForIceCandidates(peerId);
    }
    private IEnumerator CreateOffer(string peerId)
    {
        Debug.Log($"Creating offer for {peerId}");
        var pc = peerConnections[peerId];

        if (pc.SignalingState != RTCSignalingState.Stable)
        {
            Debug.LogWarning($"Cannot create offer for {peerId}, signaling state is {pc.SignalingState}");
            yield break;
        }

        var op = pc.CreateOffer();
        yield return op;

        if (!op.IsError)
        {
            yield return StartCoroutine(OnCreateOfferSuccess(pc, peerId, op.Desc));
        }
        else
        {
            OnCreateSessionDescriptionError(op.Error);
        }

        ListenForAnswer(peerId);
    }

    private IEnumerator OnCreateOfferSuccess(RTCPeerConnection pc, string peerId, RTCSessionDescription desc)
    {
        Debug.Log($"Setting local description (offer) for {peerId}");
        var op = pc.SetLocalDescription(ref desc);
        yield return op;

        if (!op.IsError)
        {
            string sdpMessage = JsonUtility.ToJson(new RTCSessionDescriptionInit
            {
                sdp = desc.sdp,
                type = desc.type.ToString().ToLower()
            });

            Debug.Log($"Sending offer to {peerId}");
            database.Child("rooms").Child(roomName).Child("offers").Child(localPeerId).Child(peerId).SetValueAsync(sdpMessage);
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

        if (type == RTCSdpType.Offer)
        {
            StartCoroutine(HandleOffer(pc, peerId, init));
        }
        else if (type == RTCSdpType.Answer)
        {
            StartCoroutine(HandleAnswer(pc, peerId, init));
        }
    }

    private IEnumerator HandleOffer(RTCPeerConnection pc, string peerId, RTCSessionDescriptionInit offer)
    {
        Debug.Log($"Handling offer from {peerId}");
        var remoteDesc = new RTCSessionDescription { sdp = offer.sdp, type = RTCSdpType.Offer };
        var op = pc.SetRemoteDescription(ref remoteDesc);
        yield return op;

        if (!op.IsError)
        {
            yield return StartCoroutine(CreateAnswer(pc, peerId));
        }
        else
        {
            Debug.LogError($"Error setting remote offer: {op.Error.message}");
        }
    }

    private IEnumerator HandleAnswer(RTCPeerConnection pc, string peerId, RTCSessionDescriptionInit answer)
    {
        Debug.Log($"Handling answer from {peerId}");
        var remoteDesc = new RTCSessionDescription { sdp = answer.sdp, type = RTCSdpType.Answer };
        var op = pc.SetRemoteDescription(ref remoteDesc);
        yield return op;

        if (!op.IsError)
        {
            Debug.Log($"Successfully set remote answer for peer {peerId}");
            ProcessIceCandidateQueue(peerId);
        }
        else
        {
            Debug.LogError($"Error setting remote answer: {op.Error.message}");
        }
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
        Debug.Log($"Creating answer for {peerId}");
        var op = pc.CreateAnswer();
        yield return op;

        if (!op.IsError)
        {
            yield return StartCoroutine(OnCreateAnswerSuccess(pc, peerId, op.Desc));
        }
        else
        {
            OnCreateSessionDescriptionError(op.Error);
        }
    }
    private IEnumerator OnCreateAnswerSuccess(RTCPeerConnection pc, string peerId, RTCSessionDescription desc)
    {
        Debug.Log($"Setting local description (answer) for {peerId}");
        var op = pc.SetLocalDescription(ref desc);
        yield return op;

        if (!op.IsError)
        {
            string answerMessage = JsonUtility.ToJson(new RTCSessionDescriptionInit
            {
                sdp = desc.sdp,
                type = desc.type.ToString().ToLower()
            });

            Debug.Log($"Sending answer to {peerId}");
            database.Child("rooms").Child(roomName).Child("answers").Child(localPeerId).Child(peerId).SetValueAsync(answerMessage);
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
            //string message = System.Text.Encoding.UTF8.GetString(bytes);
            //OnMessageReceived?.Invoke(peerId, message);
        };
    }

    private void SetupDataChannel(string peerId, RTCDataChannel channel)
    {
        if (dataChannels.ContainsKey(peerId))
        {
            Debug.Log($"Data channel for {peerId} already exists. Skipping setup.");
            return;
        }

        Debug.Log($"WebRTCManager: Setting up data channel for {peerId}");
        dataChannels[peerId] = channel;
        channel.OnOpen = () => Debug.Log($"Data channel opened for {peerId}");
        channel.OnClose = () =>
        {
            Debug.Log($"WebRTCManager: Data channel closed for {peerId}.");
            if (peerId != localPeerId)
            {
                StartCoroutine(TryReconnectDataChannel(peerId));
            }
        };

        channel.OnError = (error) => Debug.LogError($"Data channel error for {peerId}: {error}");
        channel.OnMessage = message => HandleDataMessage(peerId, message);
    }

    private void HandleDataChannel(string peerId, RTCDataChannel channel)
    {
        Debug.Log($"MultiplayerManager: Received data channel for {peerId}");
        if (!dataChannels.ContainsKey(peerId))
        {
            SetupDataChannel(peerId, channel);
        }
        else
        {
            Debug.Log($"Data channel for {peerId} already exists. Using existing channel.");
        }
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
                if (string.Compare(localPeerId, peerId) < 0)  // Only create if we're the 'lower' peer
                {
                    var dataChannel = pc.CreateDataChannel("data");
                    SetupDataChannel(peerId, dataChannel);
                    Debug.Log($"MultiplayerManager: Created new data channel for {peerId}");
                }
                else
                {
                    Debug.Log($"MultiplayerManager: Waiting for {peerId} to create new data channel");
                }
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

        Debug.Log($"MultiplayerManager: Received message from {peerId}: {message}");
        //OnMessageReceived?.Invoke(peerId, message);
        
        MultiplayerManager.Instance.HandleWebRTCMessage(peerId, message);

    }


    public void SendDataMessage(string message)
    {
        Debug.Log($"WebRTCManager: Sending data message to all peers: {message}");
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(message);

        foreach (var kvp in dataChannels)
        {
            string peerId = kvp.Key;
            RTCDataChannel channel = kvp.Value;
            if (channel.ReadyState == RTCDataChannelState.Open)
            {

                    channel.Send(bytes);
                    Debug.Log($"WebRTCManager: Data message sent to remote peer {peerId} + {message}");

            }
            else
            {
                Debug.LogWarning($"WebRTCManager: Cannot send data message to remote peer {peerId}. Channel state: {channel.ReadyState}");
            }
        }
    }
    public void SendDataMessage(string targetPeerId, string message)
    {
        Debug.Log($"WebRTCManager: Sending data message to peer {targetPeerId}: {message}");
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(message);

        if (dataChannels.TryGetValue(targetPeerId, out RTCDataChannel channel))
        {
            if (channel.ReadyState == RTCDataChannelState.Open)
            {
                try
                {
                    channel.Send(bytes);
                    Debug.Log($"WebRTCManager: Data message sent to peer {targetPeerId}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error sending message to {targetPeerId}: {e.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"WebRTCManager: Cannot send data message to peer {targetPeerId}. Channel state: {channel.ReadyState}");
            }
        }
        else
        {
            Debug.LogWarning($"WebRTCManager: No data channel found for peer {targetPeerId}");
        }
    }
    public List<string> GetConnectedPeers()
    {
        return new List<string>(peerConnections.Keys);
    }

    public void ClosePeerConnection(string peerId)
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

    private void ListenForIceCandidates(string peerId)
    {
        Debug.Log($"MultiplayerManager: Start listening for ICE candidates from {peerId}");
        try
        {
            string path = $"rooms/{roomName}/iceCandidates/{peerId}/{localPeerId}";
            database.Child(path).ChildAdded += (sender, args) =>
            {
                if (args.Snapshot.Value != null)
                {
                    string candidateString = args.Snapshot.Value.ToString();
                    AddRemoteIceCandidate(peerId, candidateString);
                }
            };
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error setting up ICE candidate listener for {peerId}: {ex.Message}");
        }
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