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

        WebRTCManager.Instance.OnMessageReceived += HandleMessageReceived;
        WebRTCManager.Instance.OnPeerListUpdated += HandlePeerListUpdated;
    }

    private void CreateRoom()
    {
        WebRTCManager.Instance.CreateRoom(roomNameInput.text);
    }

    private void JoinRoom()
    {
        WebRTCManager.Instance.JoinRoom(roomNameInput.text);
    }

    private void LeaveRoom()
    {
        WebRTCManager.Instance.LeaveRoom();
    }

    private void SendMessage()
    {
        WebRTCManager.Instance.SendDataMessage(messageInput.text);
        messageInput.text = "";
    }

    private void HandleMessageReceived(string peerId, string message)
    {
        messageLogText.text += $"{message}\n";
    }

    private void HandlePeerListUpdated(List<string> peers)
    {
        peerListText.text = string.Join("\n", peers);
    }

    private void OnDestroy()
    {
        WebRTCManager.Instance.OnMessageReceived -= HandleMessageReceived;
        WebRTCManager.Instance.OnPeerListUpdated -= HandlePeerListUpdated;
    }
}