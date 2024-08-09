using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

        // Subscribe to events from MultiplayerManager
        // You'll need to implement these events in MultiplayerManager
        MultiplayerManager.Instance.OnPeerConnected += UpdatePeerList;
        MultiplayerManager.Instance.OnPeerDisconnected += UpdatePeerList;
        MultiplayerManager.Instance.OnMessageReceived += DisplayMessage;
    }

    private async void CreateRoom()
    {
        string roomName = roomNameInput.text;
        await MultiplayerManager.Instance.TestCreateRoom(roomName);
        UpdateUI();
    }

    private async void JoinRoom()
    {
        string roomName = roomNameInput.text;
        await MultiplayerManager.Instance.TestJoinRoom(roomName);
        UpdateUI();
    }

    private void LeaveRoom()
    {
        MultiplayerManager.Instance.LeaveRoom();
        UpdateUI();
    }

    private void SendMessage()
    {
        string message = messageInput.text;
        byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
        MultiplayerManager.Instance.SendInput(messageBytes);
        DisplayMessage("You", message);
        messageInput.text = "";
    }

    private void UpdatePeerList()
    {
        // Update the peer list UI
        // You'll need to implement a method in MultiplayerManager to get the current peer list
        string peerList = string.Join("\n", MultiplayerManager.Instance.GetConnectedPeers());
        peerListText.text = "Connected Peers:\n" + peerList;
    }

    private void DisplayMessage(string sender, string message)
    {
        messageLogText.text += $"\n{sender}: {message}";
    }

    private void UpdateUI()
    {
        bool inRoom = !string.IsNullOrEmpty(MultiplayerManager.Instance.CurrentRoomName);
        createRoomButton.interactable = !inRoom;
        joinRoomButton.interactable = !inRoom;
        leaveRoomButton.interactable = inRoom;
        sendMessageButton.interactable = inRoom;
        messageInput.interactable = inRoom;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        MultiplayerManager.Instance.OnPeerConnected -= UpdatePeerList;
        MultiplayerManager.Instance.OnPeerDisconnected -= UpdatePeerList;
        MultiplayerManager.Instance.OnMessageReceived -= DisplayMessage;
    }
}