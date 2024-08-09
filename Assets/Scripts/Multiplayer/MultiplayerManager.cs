using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using System.Threading.Tasks;
using Unity.WebRTC;
using System;

public class MultiplayerManager : MonoBehaviour
{
    private static MultiplayerManager _instance;
    public static MultiplayerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<MultiplayerManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("MultiplayerManager");
                    _instance = go.AddComponent<MultiplayerManager>();
                }
            }
            return _instance;
        }
    }

    public string CurrentRoomName => roomName;
    public event Action OnPeerConnected;
    public event Action OnPeerDisconnected;
    public event Action<string, string> OnMessageReceived;

    private DatabaseReference database;
    private string roomName;
    private string playerId;
    private Dictionary<string, RTCPeerConnection> peerConnections = new Dictionary<string, RTCPeerConnection>();
    private Dictionary<string, RTCDataChannel> dataChannels = new Dictionary<string, RTCDataChannel>();

    [SerializeField] private string testRoomName = "TestRoom";


    private void Start()
    {
        database = FirebaseDatabase.DefaultInstance.RootReference;
        playerId = SystemInfo.deviceUniqueIdentifier;
    }
    // New method to get connected peers
    public List<string> GetConnectedPeers()
    {
        return new List<string>(peerConnections.Keys);
    }

    // Test method to create a room
    public async Task TestCreateRoom(string testRoomName)
    {
        Debug.Log($"Attempting to create room: {testRoomName}");
        await CreateRoom(testRoomName);
        Debug.Log($"Room created: {testRoomName}");
        await TestDatabasePost();
    }

    // Test method to join a room
    public async Task TestJoinRoom(string testRoomName)
    {
        Debug.Log($"Attempting to join room: {testRoomName}");
        await JoinRoom(testRoomName);
        Debug.Log($"Joined room: {testRoomName}");
        await TestDatabasePost();
    }

    // Test method to post data to the database
    public async Task TestDatabasePost()
    {
        string testPath = $"rooms/{roomName}/testData";
        string testData = $"Test data from {playerId} at {System.DateTime.Now}";

        Debug.Log($"Attempting to post test data to {testPath}");
        await database.Child(testPath).Push().SetValueAsync(testData);
        Debug.Log("Test data posted successfully");
    }

    public async Task CreateRoom(string roomName)
    {
        this.roomName = roomName;
        await database.Child("rooms").Child(roomName).Child("host").SetValueAsync(playerId);
        await JoinRoom(roomName);
    }

    public async Task JoinRoom(string roomName)
    {
        this.roomName = roomName;
        await database.Child("rooms").Child(roomName).Child("players").Child(playerId).SetValueAsync(true);
        ListenForNewPlayers();
        ListenForOffers();
        ListenForAnswers();
        ListenForIceCandidates();
    }

    private void ListenForNewPlayers()
    {
        database.Child("rooms").Child(roomName).Child("players").ChildAdded += HandleNewPlayer;
    }

    private void HandleNewPlayer(object sender, ChildChangedEventArgs args)
    {
        string newPlayerId = args.Snapshot.Key;
        if (newPlayerId != playerId)
        {
            CreatePeerConnection(newPlayerId);
        }
    }

    private void CreatePeerConnection(string peerId)
    {
        RTCConfiguration config = default;
        config.iceServers = new RTCIceServer[]
        {
            new RTCIceServer { urls = new string[] { "stun:stun.l.google.com:19302" } }
        };

        RTCPeerConnection peerConnection = new RTCPeerConnection(ref config);
        peerConnections[peerId] = peerConnection;

        peerConnection.OnIceCandidate = candidate => SendIceCandidate(peerId, candidate);
        peerConnection.OnDataChannel = channel => HandleDataChannel(peerId, channel);

        if (string.Compare(playerId, peerId) < 0)
        {
            StartCoroutine(CreateAndSendOfferCoroutine(peerId));
        }
        OnPeerConnected?.Invoke();
    }

    private IEnumerator CreateAndSendOfferCoroutine(string peerId)
    {
        RTCPeerConnection peerConnection = peerConnections[peerId];

        var offerOp = peerConnection.CreateOffer();
        yield return offerOp;

        if (offerOp.IsError)
        {
            Debug.LogError($"Error creating offer: {offerOp.Error.message}");
            yield break;
        }

        RTCSessionDescription offer = offerOp.Desc;
        var setLocalDescOp = peerConnection.SetLocalDescription(ref offer);
        yield return setLocalDescOp;

        if (setLocalDescOp.IsError)
        {
            Debug.LogError($"Error setting local description: {setLocalDescOp.Error.message}");
            yield break;
        }

        var task = database.Child("rooms").Child(roomName).Child("offers").Child(playerId).Child(peerId).SetValueAsync(offer.sdp);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogError($"Error sending offer to Firebase: {task.Exception}");
        }
    }

    private async void SendIceCandidate(string peerId, RTCIceCandidate candidate)
    {
        await database.Child("rooms").Child(roomName).Child("candidates").Child(playerId).Child(peerId).Push().SetValueAsync(
            new Dictionary<string, object>
            {
                { "candidate", candidate.Candidate },
                { "sdpMid", candidate.SdpMid },
                { "sdpMLineIndex", candidate.SdpMLineIndex }
            });
    }

    // Modify HandleDataChannel method
    private void HandleDataChannel(string peerId, RTCDataChannel dataChannel)
    {
        dataChannels[peerId] = dataChannel;
        dataChannel.OnMessage = message => HandlePeerMessage(peerId, message);
        OnPeerConnected?.Invoke();
    }

    // Modify HandlePeerMessage method
    private void HandlePeerMessage(string peerId, byte[] message)
    {
        string playerChannel = $"player{peerId}cast";
        string decodedMessage = System.Text.Encoding.UTF8.GetString(message);
        OnMessageReceived?.Invoke(peerId, decodedMessage);
        // TODO: Implement your event system to broadcast the message on the player channel
    }

    public void SendInput(byte[] inputData)
    {
        foreach (var dataChannel in dataChannels.Values)
        {
            if (dataChannel.ReadyState == RTCDataChannelState.Open)
            {
                dataChannel.Send(inputData);
            }
        }
    }

    private void ListenForOffers()
    {
        database.Child("rooms").Child(roomName).Child("offers").ChildAdded += HandleOffer;
    }

    private void HandleOffer(object sender, ChildChangedEventArgs args)
    {
        string offererId = args.Snapshot.Key;
        if (offererId != playerId)
        {
            string offerSdp = args.Snapshot.Value.ToString();
            StartCoroutine(HandleOfferCoroutine(offererId, offerSdp));
        }
    }

    private IEnumerator HandleOfferCoroutine(string offererId, string offerSdp)
    {
        if (!peerConnections.TryGetValue(offererId, out RTCPeerConnection peerConnection))
        {
            CreatePeerConnection(offererId);
            peerConnection = peerConnections[offererId];
        }

        RTCSessionDescription offer = new RTCSessionDescription
        {
            type = RTCSdpType.Offer,
            sdp = offerSdp
        };

        var setRemoteDescOp = peerConnection.SetRemoteDescription(ref offer);
        yield return setRemoteDescOp;

        if (setRemoteDescOp.IsError)
        {
            Debug.LogError($"Error setting remote description: {setRemoteDescOp.Error.message}");
            yield break;
        }

        var answerOp = peerConnection.CreateAnswer();
        yield return answerOp;

        if (answerOp.IsError)
        {
            Debug.LogError($"Error creating answer: {answerOp.Error.message}");
            yield break;
        }

        RTCSessionDescription answer = answerOp.Desc;
        var setLocalDescOp = peerConnection.SetLocalDescription(ref answer);
        yield return setLocalDescOp;

        if (setLocalDescOp.IsError)
        {
            Debug.LogError($"Error setting local description: {setLocalDescOp.Error.message}");
            yield break;
        }

        var task = database.Child("rooms").Child(roomName).Child("answers").Child(playerId).Child(offererId).SetValueAsync(answer.sdp);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogError($"Error sending answer to Firebase: {task.Exception}");
        }
    }

    private void ListenForAnswers()
    {
        database.Child("rooms").Child(roomName).Child("answers").ChildAdded += HandleAnswer;
    }

    private void HandleAnswer(object sender, ChildChangedEventArgs args)
    {
        string answererId = args.Snapshot.Key;
        if (answererId != playerId)
        {
            string answerSdp = args.Snapshot.Value.ToString();
            StartCoroutine(HandleAnswerCoroutine(answererId, answerSdp));
        }
    }

    private IEnumerator HandleAnswerCoroutine(string answererId, string answerSdp)
    {
        if (peerConnections.TryGetValue(answererId, out RTCPeerConnection peerConnection))
        {
            RTCSessionDescription answer = new RTCSessionDescription
            {
                type = RTCSdpType.Answer,
                sdp = answerSdp
            };

            var setRemoteDescOp = peerConnection.SetRemoteDescription(ref answer);
            yield return setRemoteDescOp;

            if (setRemoteDescOp.IsError)
            {
                Debug.LogError($"Error setting remote description: {setRemoteDescOp.Error.message}");
            }
        }
    }

    private void ListenForIceCandidates()
    {
        database.Child("rooms").Child(roomName).Child("candidates").ChildAdded += HandleIceCandidate;
    }

    private void HandleIceCandidate(object sender, ChildChangedEventArgs args)
    {
        string senderId = args.Snapshot.Key;
        if (senderId != playerId)
        {
            foreach (var candidateData in args.Snapshot.Children)
            {
                Dictionary<string, object> candidateDict = candidateData.Value as Dictionary<string, object>;
                if (candidateDict != null)
                {
                    RTCIceCandidateInit candidateInit = new RTCIceCandidateInit
                    {
                        candidate = candidateDict["candidate"] as string,
                        sdpMid = candidateDict["sdpMid"] as string,
                        sdpMLineIndex = Convert.ToInt32(candidateDict["sdpMLineIndex"])
                    };

                    RTCIceCandidate candidate = new RTCIceCandidate(candidateInit);

                    if (peerConnections.TryGetValue(senderId, out RTCPeerConnection peerConnection))
                    {
                        peerConnection.AddIceCandidate(candidate);
                    }
                }
            }
        }
    }

    public void LeaveRoom()
    {
        StopAllCoroutines();

        foreach (var peerId in peerConnections.Keys)
        {
            OnPeerDisconnected?.Invoke();
        }

        foreach (var peerConnection in peerConnections.Values)
        {
            peerConnection.Close();
        }
        peerConnections.Clear();

        foreach (var dataChannel in dataChannels.Values)
        {
            dataChannel.Close();
        }
        dataChannels.Clear();

        if (!string.IsNullOrEmpty(roomName))
        {
            database.Child("rooms").Child(roomName).Child("players").Child(playerId).RemoveValueAsync();
            database.Child("rooms").Child(roomName).Child("offers").Child(playerId).RemoveValueAsync();
            database.Child("rooms").Child(roomName).Child("answers").Child(playerId).RemoveValueAsync();
            database.Child("rooms").Child(roomName).Child("candidates").Child(playerId).RemoveValueAsync();
        }
 

        roomName = null;
    }

    private void OnDestroy()
    {
        LeaveRoom();
    }
}