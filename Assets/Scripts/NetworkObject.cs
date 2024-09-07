//using UnityEngine;
//using System.Collections;

//public class NetworkObject : MonoBehaviour
//{
//    public int ObjectId { get; private set; }
//    public string OwnerPeerId { get; private set; }

//    public int displayObjectId;
//    public string displayOwnerPeerId;

//    private Transform objectTransform;

//    private void Awake()
//    {
//        objectTransform = transform;
//    }

//    public void Initialize(int objectId, string ownerPeerId)
//    {
//        Debug.Log("NetworkObject: " + objectId + " " + ownerPeerId);
//        ObjectId = objectId;
//        OwnerPeerId = ownerPeerId;

//        displayOwnerPeerId = ownerPeerId;
//        displayObjectId = objectId;
//        // Subscribe to the owner's peer channel
//        string peerChannel = NetworkEngine.Instance.GetPeerChannelName(OwnerPeerId);
//        EventChannelManager.Instance.SubscribeChannel(gameObject, peerChannel, HandleSyncMessage);

//    }

//    public bool IsOwner()
//    {
//        return OwnerPeerId == LocalWebRTCEngine.Instance.LocalPeerId;
//    }

//    private void HandleSyncMessage(string eventData)
//    {
//        //NetworkMessage syncMessage = NetworkMessage.Deserialize(eventData);

//        //if (syncMessage.MessageType == "SyncObject" && syncMessage.ObjectId == ObjectId)
//        //{
//        //    Vector3 newPosition = syncMessage.GetData<Vector3>("Position");
//        //    Quaternion newRotation = syncMessage.GetData<Quaternion>("Rotation");

//        //    ApplyTransform(newPosition, newRotation);
//        }
//    }

//    private void ApplyTransform(Vector3 newPosition, Quaternion newRotation)
//    {

//            objectTransform.position = newPosition;
//            objectTransform.rotation = newRotation;

//    }

//    private void OnDestroy()
//    {
//        // Unsubscribe from the peer channel
//        string peerChannel = NetworkEngine.Instance.GetPeerChannelName(OwnerPeerId);
//        EventChannelManager.Instance.UnSubscribeChannel(gameObject, peerChannel);
//    }
//}