using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "New GameEventChannel", menuName = "Events/Game Event Channel")]
public class GameEventChannelSO : ScriptableObject
{
    private Dictionary<Action<string>, string> listeners = new Dictionary<Action<string>, string>();

    public void RaiseEvent(string uiEventName)
    {
        foreach (var listener in listeners)
        {
            if (listener.Value == uiEventName || listener.Value == "ALL")
            {
                listener.Key?.Invoke(uiEventName);
            }
        }
    }

    public void RegisterListener(Action<string> listener, string eventName)
    {
        if (!listeners.ContainsKey(listener))
        {
            listeners.Add(listener, eventName);
        }
    }

    public void UnregisterListener(Action<string> listener)
    {
        if (listeners.ContainsKey(listener))
        {
            listeners.Remove(listener);
        }
    }

    public void DebugRegisteredListeners()
    {
        foreach (var listener in listeners)
        {
            Debug.Log($"Listener: {listener.Key.Method.DeclaringType}.{listener.Key.Method.Name}, Event: {listener.Value}");
        }
    }

    public List<(Delegate Listener, string EventName)> GetListenersInfo()
    {
        var infoList = new List<(Delegate, string)>();
        foreach (var listener in listeners)
        {
            infoList.Add((listener.Key, listener.Value));
        }
        return infoList;
    }

}
