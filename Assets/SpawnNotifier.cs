using UnityEngine;

public class SpawnNotifier : MonoBehaviour
{
    public string eventName = "ObjectSpawned"; // Default event name, can be overridden in the inspector for specific cases
    [SerializeField]
    private GameEventChannelSO playerEventChannel;
    // Use this for initialization
    void Start()
    {
        NotifySpawned();
    }

    private void NotifySpawned()
    {
        if (playerEventChannel != null)
        {
            EventChannelManager.Instance.RaiseEvent(playerEventChannel.name, "PlayerSpawned");
            //eventChannel.RaiseEvent(eventName);
            //Debug.Log($"{gameObject.name} has spawned, event {eventName} raised.");
        }
        else
        {
            Debug.LogError("EventChannel is not assigned to SpawnNotifier on " + gameObject.name);
        }
    }
}
