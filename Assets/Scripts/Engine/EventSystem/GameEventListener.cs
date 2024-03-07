using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GameEventResponse : UnityEvent<string> { }

public class GameEventListener : MonoBehaviour
{
    public GameEventChannelSO EventChannel;
    public GameEventResponse Response;
    public List<string> eventName;
    private void OnEnable()
    {
        EventChannel.RegisterListener(OnEventRaised, eventName[0]);
    }

    private void OnDisable()
    {
        EventChannel.UnregisterListener(OnEventRaised);
    }

    private void OnEventRaised(string uiEventName)
    {
        Response.Invoke(uiEventName);
    }
}
