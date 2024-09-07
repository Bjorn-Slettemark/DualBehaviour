using UnityEngine;

public class SetPlayerTag : MonoBehaviour
{
    private void Start()
    {
        // Assuming LocalWebrtcEngine and Multibehaviour are part of your project
        string localPeerId = LocalWebRTCEngine.Instance.LocalPeerId;
        //string ownerPeerId = GetComponent<MultiBehaviour>().OwnerPeerId;

        //if (localPeerId == ownerPeerId)
        //{
        //    gameObject.tag = "Player";
        //    Debug.Log("Tag set to Player");
        //}
        //else
        //{
        //    Debug.Log("Local peer ID does not match owner peer ID");
        //}
    }
}