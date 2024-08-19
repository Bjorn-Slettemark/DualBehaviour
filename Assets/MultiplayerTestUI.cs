using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class MultiplayerTestUI : MonoBehaviour
{
    public TMP_InputField roomNameInput;
    public Button createRoomButton;
    public Button joinRoomButton;
    public Button leaveRoomButton;
    public Button sendMessageButton;
    public TMP_InputField messageInput;
    public TextMeshProUGUI peerListText;
    public TextMeshProUGUI messageLogText;

    private void Start()
    {
        createRoomButton.onClick.AddListener(CreateRoom);
        joinRoomButton.onClick.AddListener(JoinRoom);
        leaveRoomButton.onClick.AddListener(LeaveRoom);
        sendMessageButton.onClick.AddListener(SendMessage);

        WebRTCManager.Instance.OnPeerListUpdated += HandlePeerListUpdated;
    }

    private void CreateRoom()
    {
        PeerManager.Instance.CreateRoom(roomNameInput.text);
    }

    private void JoinRoom()
    {
        PeerManager.Instance.JoinRoom(roomNameInput.text);
    }

    private void LeaveRoom()
    {
        PeerManager.Instance.LeaveRoom();
    }

    private void SendMessage()
    {
        WebRTCManager.Instance.SendDataMessage(messageInput.text);
        messageInput.text = "";
    }



    private void HandlePeerListUpdated(List<string> peers)
    {
        peerListText.text = string.Join("\n", peers);
    }

    private void OnDestroy()
    {
        WebRTCManager.Instance.OnPeerListUpdated -= HandlePeerListUpdated;
    }
}