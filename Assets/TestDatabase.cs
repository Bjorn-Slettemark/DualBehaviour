using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TestDatabase : MonoBehaviour
{
    public Button createRoomButton;
    public Button joinRoomButton;
    public Button leaveRoomButton;

    public string roomName;

    void Start()
    {
        // Attach button click listeners
        createRoomButton.onClick.AddListener(StartTestCreateRoom);
        joinRoomButton.onClick.AddListener(StartTestJoinRoom);
        leaveRoomButton.onClick.AddListener(TestLeaveRoom);
    }

    public async void TestCreateRoom()
    {
        try
        {
            await MultiplayerManager.Instance.TestCreateRoom(roomName);
            Debug.Log("Room creation test completed");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in TestCreateRoom: {e.Message}");
        }
    }

    public async void TestJoinRoom()
    {
        try
        {
            await MultiplayerManager.Instance.TestJoinRoom(roomName);
            Debug.Log("Room join test completed");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in TestJoinRoom: {e.Message}");
        }
    }

    public void TestLeaveRoom()
    {
        MultiplayerManager.Instance.LeaveRoom();
        Debug.Log("Leave room test completed");
    }

    // Coroutine methods for button clicks
    public void StartTestCreateRoom()
    {
        StartCoroutine(TestCreateRoomCoroutine());
    }

    public void StartTestJoinRoom()
    {
        StartCoroutine(TestJoinRoomCoroutine());
    }

    private IEnumerator TestCreateRoomCoroutine()
    {
        createRoomButton.interactable = false;
        TestCreateRoom();
        yield return new WaitUntil(() => !createRoomButton.interactable);
        createRoomButton.interactable = true;
    }

    private IEnumerator TestJoinRoomCoroutine()
    {
        joinRoomButton.interactable = false;
        TestJoinRoom();
        yield return new WaitUntil(() => !joinRoomButton.interactable);
        joinRoomButton.interactable = true;
    }

    void OnDestroy()
    {
        // Remove button click listeners to prevent memory leaks
        createRoomButton.onClick.RemoveListener(StartTestCreateRoom);
        joinRoomButton.onClick.RemoveListener(StartTestJoinRoom);
        leaveRoomButton.onClick.RemoveListener(TestLeaveRoom);
    }
}