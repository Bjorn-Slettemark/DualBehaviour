using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "New GameEventChannel", menuName = "Events/Game Event Channel")]
public class GameEventChannelSO : ScriptableObject
{


    private const string AllEventsKey = "ALL"; // Special key for listeners interested in all events
    private Dictionary<Action<string>, string> listeners = new Dictionary<Action<string>, string>();

    // Raise an event to all listeners interested in uiEventName, or to those listening to all events (AllEventsKey)
    public void RaiseEvent(string uiEventName)
    {
        foreach (var listener in listeners)
        {
            if (listener.Value == uiEventName || listener.Value == AllEventsKey)
            {
                listener.Key?.Invoke(uiEventName);
            }
        }
    }

    // Register a listener for a specific event or all events
    public void RegisterListener(Action<string> listener, string eventName = AllEventsKey)
    {
        if (!listeners.ContainsKey(listener))
        {
            listeners.Add(listener, eventName);
        }
        else
        {
            // Optionally update the listener's event name if re-registering
            listeners[listener] = eventName;
        }
    }

    // Unregister a listener
    public void UnregisterListener(Action<string> listener)
    {
        if (listeners.ContainsKey(listener))
        {
            listeners.Remove(listener);
        }
    }

    // Optional: Register a listener for all events without specifying an event name
    public void RegisterListenerForAllEvents(Action<string> listener)
    {
        RegisterListener(listener, AllEventsKey);
    }

    // Optional: Unregister a listener from all events, more explicit in usage
    public void UnregisterListenerFromAllEvents(Action<string> listener)
    {
        UnregisterListener(listener);
    }

    // Debugging utility to log all registered listeners and the events they are registered for
    public void DebugRegisteredListeners()
    {
        foreach (var listener in listeners)
        {
            Debug.Log($"Listener: {listener.Key.Method.DeclaringType}.{listener.Key.Method.Name}, Event: {listener.Value}");
        }
    }

    // Return a list of tuples containing the delegate (listener) and the event name they're registered for
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
