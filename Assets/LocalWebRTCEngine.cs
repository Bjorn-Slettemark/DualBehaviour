using UnityEngine;
using Unity.WebRTC;
using System;
using System.Collections;

public class LocalWebRTCEngine : MonoBehaviour
{
    private static LocalWebRTCEngine _instance;
    public static LocalWebRTCEngine Instance => _instance;

    private RTCPeerConnection localPeer;
    private RTCPeerConnection remotePeer;
    private RTCDataChannel localDataChannel;
    private RTCDataChannel remoteDataChannel;

    public event Action OnLocalConnectionEstablished;

    public string LocalPeerId { get; private set; }
    public string RemotePeerId { get; private set; }

    private void Awake()
    {
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
        RemotePeerId = DualBehaviourUtils.GenerateRandomString(5);
        LocalPeerId = DualBehaviourUtils.GenerateRandomString(5);
    }

    private void Start()
    {
        StartCoroutine(SetupLocalConnection());
    }

    private IEnumerator SetupLocalConnection()
    {
        Debug.Log("LocalPeerConnectionSetup: Setting up local WebRTC connection...");

        RemotePeerId = "local-" + System.Guid.NewGuid().ToString().Substring(0, 8);

        RTCConfiguration config = default;
        config.iceServers = new[] { new RTCIceServer { urls = new[] { "stun:stun.l.google.com:19302" } } };

        localPeer = new RTCPeerConnection(ref config);
        remotePeer = new RTCPeerConnection(ref config);

        SetupPeerConnectionHandlers(localPeer, "LocalPeer");
        SetupPeerConnectionHandlers(remotePeer, "RemotePeer");

        localDataChannel = localPeer.CreateDataChannel("local-channel", new RTCDataChannelInit());
        SetupDataChannelHandlers(localDataChannel, "LocalPeer");

        remotePeer.OnDataChannel = channel =>
        {
            remoteDataChannel = channel;
            SetupDataChannelHandlers(remoteDataChannel, "RemotePeer");
        };

        var offerOp = localPeer.CreateOffer();
        yield return offerOp;
        if (!offerOp.IsError)
        {
            yield return StartCoroutine(SetLocalDescription(localPeer, offerOp.Desc));
            yield return StartCoroutine(SetRemoteDescription(remotePeer, offerOp.Desc));

            var answerOp = remotePeer.CreateAnswer();
            yield return answerOp;
            if (!answerOp.IsError)
            {
                yield return StartCoroutine(SetLocalDescription(remotePeer, answerOp.Desc));
                yield return StartCoroutine(SetRemoteDescription(localPeer, answerOp.Desc));
            }
            else
            {
                Debug.LogError($"Error creating answer: {answerOp.Error.message}");
            }
        }
        else
        {
            Debug.LogError($"Error creating offer: {offerOp.Error.message}");
        }

        yield return new WaitUntil(() => localPeer.ConnectionState == RTCPeerConnectionState.Connected &&
                                        remotePeer.ConnectionState == RTCPeerConnectionState.Connected &&
                                        remoteDataChannel != null &&
                                        remoteDataChannel.ReadyState == RTCDataChannelState.Open);



        Debug.Log("LocalPeerConnectionSetup: Local WebRTC connection established successfully!");

        // Add only the remote peer to WebRTCManager
        //WebRTCManager.Instance.AddPeerConnection(RemotePeerId, remotePeer);
        WebRTCEngine.Instance.AddDataChannel(LocalWebRTCEngine.Instance.LocalPeerId, remoteDataChannel);
        OnLocalConnectionEstablished?.Invoke();

    }

    private IEnumerator SetLocalDescription(RTCPeerConnection pc, RTCSessionDescription desc)
    {
        var op = pc.SetLocalDescription(ref desc);
        yield return op;
        if (op.IsError)
        {
            Debug.LogError($"Error setting local description: {op.Error.message}");
        }
    }

    private IEnumerator SetRemoteDescription(RTCPeerConnection pc, RTCSessionDescription desc)
    {
        var op = pc.SetRemoteDescription(ref desc);
        yield return op;
        if (op.IsError)
        {
            Debug.LogError($"Error setting remote description: {op.Error.message}");
        }
    }

    private void SetupPeerConnectionHandlers(RTCPeerConnection pc, string peerName)
    {
        pc.OnIceCandidate = candidate =>
        {
            Debug.Log($"{peerName} ICE candidate: {candidate.Candidate}");
            if (pc == localPeer)
                remotePeer.AddIceCandidate(candidate);
            else
                localPeer.AddIceCandidate(candidate);
        };

        pc.OnConnectionStateChange = state => Debug.Log($"{peerName} connection state changed to {state}");
        pc.OnIceConnectionChange = state => Debug.Log($"{peerName} ICE connection state changed to {state}");
    }

    private void SetupDataChannelHandlers(RTCDataChannel channel, string peerName)
    {
        channel.OnMessage = bytes =>
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            //Debug.Log($"{peerName} received message: {message}");
            Debug.Log("Local Webrtc engine got the message: " + message);

            if (peerName == "LocalPeer")
            {
                NetworkEngine.Instance.HandleWebRTCMessage(message);

                // Forward local messages directly to MultiplayerManager
            }
            // Remote messages are handled by WebRTCManager
        };
        channel.OnOpen = () => Debug.Log($"{peerName} data channel opened");
        channel.OnClose = () => Debug.Log($"{peerName} data channel closed");
    }

}