//using UnityEngine;

//public class NetworkInputController : MonoBehaviour
//{
    

//    private void Start()
//    {
//        string localChannelName;
//        localChannelName = $"PeerChannel_{LocalWebRTCManager.Instance.LocalPeerId}";
//        EventChannelManager.Instance.RegisterChannelByName(localChannelName);
//        // The channel should already be created by the MultiplayerManager, so we don't need to create it here
//        Debug.Log($"NetworkInputController initialized for channel: {localChannelName}");
//    }

//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.W)) SendInputEvent("UpPressed");
//        if (Input.GetKeyUp(KeyCode.W)) SendInputEvent("UpReleased");
//        if (Input.GetKeyDown(KeyCode.S)) SendInputEvent("DownPressed");
//        if (Input.GetKeyUp(KeyCode.S)) SendInputEvent("DownReleased");
//        if (Input.GetKeyDown(KeyCode.A)) SendInputEvent("LeftPressed");
//        if (Input.GetKeyUp(KeyCode.A)) SendInputEvent("LeftReleased");
//        if (Input.GetKeyDown(KeyCode.D)) SendInputEvent("RightPressed");
//        if (Input.GetKeyUp(KeyCode.D)) SendInputEvent("RightReleased");
//        if (Input.GetKeyDown(KeyCode.Space)) SendInputEvent("JumpPressed");
//        if (Input.GetKeyUp(KeyCode.Space)) SendInputEvent("JumpReleased");
//    }

//    private void SendInputEvent(string inputEvent)
//    {
//        // Raise the event locally
//        //EventChannelManager.Instance.RaiseEventByName(localChannelName, inputEvent);
        
//        // Broadcast the event to all peers
//        MultiplayerManager.Instance.BroadcastEventToAllPeers(inputEvent);
//    }
//}