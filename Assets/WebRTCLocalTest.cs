using UnityEngine;
using Unity.WebRTC;
using System.Collections;

public class WebRTCLocalTest : MonoBehaviour
{
    private Unity.WebRTC.RTCPeerConnection peer1;
    private Unity.WebRTC.RTCPeerConnection peer2;
    private RTCDataChannel dataChannel1;
    private RTCDataChannel dataChannel2;

    private void Start()
    {
        StartCoroutine(RunTest());
    }

    private IEnumerator RunTest()
    {
        Debug.Log("Starting WebRTC test...");
        yield return new WaitForSeconds(1);

        Debug.Log("Creating peer connections...");
        RTCConfiguration config = default;
        config.iceServers = new[] { new RTCIceServer { urls = new[] { "stun:stun.l.google.com:19302" } } };

        peer1 = new Unity.WebRTC.RTCPeerConnection(ref config);
        peer2 = new Unity.WebRTC.RTCPeerConnection(ref config);

        // Set up event handlers for both peers
        SetupPeerConnectionHandlers(peer1, "Peer1");
        SetupPeerConnectionHandlers(peer2, "Peer2");

        // Create a data channel on peer1
        dataChannel1 = peer1.CreateDataChannel("test-channel", new RTCDataChannelInit());
        SetupDataChannelHandlers(dataChannel1, "Peer1");

        // Set up the offer-answer exchange
        Debug.Log("Creating offer...");
        var offerOp = peer1.CreateOffer();
        yield return offerOp;
        if (offerOp.IsError)
        {
            Debug.LogError($"Error creating offer: {offerOp.Error.message}");
            yield break;
        }

        Debug.Log($"Offer created: {offerOp.Desc.sdp}");
        yield return StartCoroutine(SetDescription(peer1, offerOp.Desc, true));
        yield return StartCoroutine(SetDescription(peer2, offerOp.Desc, false));

        Debug.Log("Creating answer...");
        var answerOp = peer2.CreateAnswer();
        yield return answerOp;
        if (answerOp.IsError)
        {
            Debug.LogError($"Error creating answer: {answerOp.Error.message}");
            yield break;
        }

        Debug.Log($"Answer created: {answerOp.Desc.sdp}");
        yield return StartCoroutine(SetDescription(peer2, answerOp.Desc, true));
        yield return StartCoroutine(SetDescription(peer1, answerOp.Desc, false));

        // Wait for the connection to be established
        yield return new WaitUntil(() => peer1.ConnectionState == RTCPeerConnectionState.Connected &&
                                          peer2.ConnectionState == RTCPeerConnectionState.Connected);

        Debug.Log("Peers connected successfully!");

        // Send a test message
        if (dataChannel1.ReadyState == RTCDataChannelState.Open)
        {
            dataChannel1.Send("Hello from Peer1!");
        }

        yield return new WaitForSeconds(5);
        Debug.Log("Test completed.");
    }

    private IEnumerator SetDescription(Unity.WebRTC.RTCPeerConnection peer, RTCSessionDescription desc, bool isLocal)
    {
        var op = isLocal ? peer.SetLocalDescription(ref desc) : peer.SetRemoteDescription(ref desc);
        yield return op;
        if (op.IsError)
        {
            Debug.LogError($"Error setting {(isLocal ? "local" : "remote")} description: {op.Error.message}");
        }
    }

    private void SetupPeerConnectionHandlers(Unity.WebRTC.RTCPeerConnection peer, string peerName)
    {
        peer.OnIceCandidate = candidate =>
        {
            Debug.Log($"{peerName} ICE candidate: {candidate.Candidate}");
            // In a real scenario, you'd send this to the other peer. For this test, we'll add it directly.
            if (peer == peer1)
                peer2.AddIceCandidate(candidate);
            else
                peer1.AddIceCandidate(candidate);
        };

        peer.OnConnectionStateChange = state => Debug.Log($"{peerName} connection state changed to {state}");
        peer.OnIceConnectionChange = state => Debug.Log($"{peerName} ICE connection state changed to {state}");
        peer.OnIceGatheringStateChange = state => Debug.Log($"{peerName} ICE gathering state changed to {state}");

        if (peer == peer2)
        {
            peer.OnDataChannel = channel =>
            {
                Debug.Log($"{peerName} received data channel: {channel.Label}");
                dataChannel2 = channel;
                SetupDataChannelHandlers(dataChannel2, peerName);
            };
        }
    }

    private void SetupDataChannelHandlers(RTCDataChannel channel, string peerName)
    {
        channel.OnMessage = bytes => Debug.Log($"{peerName} received message: {System.Text.Encoding.UTF8.GetString(bytes)}");
        channel.OnOpen = () => Debug.Log($"{peerName} data channel opened");
        channel.OnClose = () => Debug.Log($"{peerName} data channel closed");
    }
}