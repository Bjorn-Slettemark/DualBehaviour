using UnityEngine;

public class SpawnNotifier : MonoBehaviour
{
    public GameEventChannelSO eventChannel; // Assign this in the inspector
    public string eventName = "ObjectSpawned"; // Default event name, can be overridden in the inspector for specific cases

    // Use this for initialization
    void Start()
    {
        NotifySpawned();
    }

    private void NotifySpawned()
    {
        if (eventChannel != null)
        {
            eventChannel.RaiseEvent(eventName);
            //Debug.Log($"{gameObject.name} has spawned, event {eventName} raised.");
        }
        else
        {
            Debug.LogError("EventChannel is not assigned to SpawnNotifier on " + gameObject.name);
        }
    }
}
