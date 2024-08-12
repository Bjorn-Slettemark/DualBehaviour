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

        MultiplayerManager.Instance.OnMessageReceived += HandleMessageReceived;
        MultiplayerManager.Instance.OnPeerListUpdated += HandlePeerListUpdated;
    }

    private void CreateRoom()
    {
        MultiplayerManager.Instance.CreateRoom(roomNameInput.text);
    }

    private void JoinRoom()
    {
        MultiplayerManager.Instance.JoinRoom(roomNameInput.text);
    }

    private void LeaveRoom()
    {
        MultiplayerManager.Instance.LeaveRoom();
    }

    private void SendMessage()
    {
        MultiplayerManager.Instance.SendDataMessage(messageInput.text);
        messageInput.text = "";
    }

    private void HandleMessageReceived(string message)
    {
        messageLogText.text += $"{message}\n";
    }

    private void HandlePeerListUpdated(List<string> peers)
    {
        peerListText.text = string.Join("\n", peers);
    }

    private void OnDestroy()
    {
        MultiplayerManager.Instance.OnMessageReceived -= HandleMessageReceived;
        MultiplayerManager.Instance.OnPeerListUpdated -= HandlePeerListUpdated;
    }
}