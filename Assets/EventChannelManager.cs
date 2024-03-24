using UnityEngine;
using System.Collections.Generic;
using System;

public enum EventChannels
{
    GameState,
    UI,
    Player,
    Level
}
[Icon("Assets/Editor/Icons/EventChannelIcon.png")]
public class EventChannelManager : MonoBehaviour
{
    public static EventChannelManager Instance { get; private set; }
    public List<GameEventChannelSO> EventChannels { get => eventChannels; }

    [SerializeField] private List<GameEventChannelSO> eventChannels;
    private Dictionary<string, GameEventChannelSO> eventChannelDictionary;
    private Dictionary<GameObject, Dictionary<string, Dictionary<string, System.Action<string>>>> subscriptions = new Dictionary<GameObject, Dictionary<string, Dictionary<string, System.Action<string>>>>();
    // List to store the history of raised events
    public List<EventHistory> eventHistory = new List<EventHistory>();

    // Define the EventHistory structure
    public struct EventHistory
    {
        public string ChannelName;
        public string EventName;
        public DateTime Timestamp;

        public EventHistory(string channelName, string eventName)
        {
            ChannelName = channelName;
            EventName = eventName;
            Timestamp = DateTime.Now; // Capture the current time
        }
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeEventChannels();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeEventChannels()
    {
        eventChannelDictionary = new Dictionary<string, GameEventChannelSO>();
        foreach (var channel in eventChannels)
        {
            if (channel != null && !eventChannelDictionary.ContainsKey(channel.name))
            {
                eventChannelDictionary.Add(channel.name, channel);
            }
        }
    }

    public GameEventChannelSO GetEventChannel(string channelName)
    {
        if (eventChannelDictionary.TryGetValue(channelName, out var channel))
        {
            return channel;
        }
        Debug.LogWarning($"EventChannelManager: Channel not found - {channelName}");
        return null;
    }
    public void RegisterForAllChannels(GameObject subscriber, System.Action<string> callback)
    {
        foreach (var channelName in eventChannelDictionary.Keys)
        {
            RegisterChannel(subscriber, channelName, callback);
        }
    }
    public void UnregisterForAllChannels(GameObject subscriber, System.Action<string> callback)
    {
        foreach (var channelName in eventChannelDictionary.Keys)
        {
            UnregisterChannel(subscriber, channelName);
        }
    }

    public void RegisterChannel(GameObject subscriber, string channelName, System.Action<string> callback)
    {
        var channel = GetEventChannel(channelName);
        if (channel == null) return;

        if (!subscriptions.ContainsKey(subscriber))
        {
            subscriptions[subscriber] = new Dictionary<string, Dictionary<string, System.Action<string>>>();
        }

        if (!subscriptions[subscriber].ContainsKey(channelName))
        {
            subscriptions[subscriber][channelName] = new Dictionary<string, System.Action<string>>();
        }

        // A special key to denote subscription to all events in the channel
        const string allEventsKey = "*";
        subscriptions[subscriber][channelName][allEventsKey] = callback;

        // Register the listener for all current and future events in the channel
        channel.RegisterListenerForAllEvents(callback);
    }

    public void RegisterEvent(GameObject subscriber, string channelName, string eventName, System.Action<string> callback)
    {
        var channel = GetEventChannel(channelName);
        if (channel == null) return;

        channel.RegisterListener(callback, eventName);

        if (!subscriptions.ContainsKey(subscriber))
        {
            subscriptions[subscriber] = new Dictionary<string, Dictionary<string, System.Action<string>>>();
        }
        if (!subscriptions[subscriber].ContainsKey(channelName))
        {
            subscriptions[subscriber][channelName] = new Dictionary<string, System.Action<string>>();
        }
        subscriptions[subscriber][channelName][eventName] = callback;
    }

    public void UnregisterChannel(GameObject subscriber, string channelName)
    {
        if (!subscriptions.ContainsKey(subscriber) || !subscriptions[subscriber].ContainsKey(channelName)) return;

        var channel = GetEventChannel(channelName);
        if (channel != null)
        {
            const string allEventsKey = "*";
            if (subscriptions[subscriber][channelName].ContainsKey(allEventsKey))
            {
                var callback = subscriptions[subscriber][channelName][allEventsKey];
                channel.UnregisterListenerFromAllEvents(callback);
                subscriptions[subscriber][channelName].Remove(allEventsKey);

                if (subscriptions[subscriber][channelName].Count == 0)
                {
                    subscriptions[subscriber].Remove(channelName);
                    if (subscriptions[subscriber].Count == 0)
                    {
                        subscriptions.Remove(subscriber);
                    }
                }
            }
        }
    }

    public void UnregisterEvent(GameObject subscriber, string channelName, string eventName)
    {
        if (!subscriptions.ContainsKey(subscriber) || !subscriptions[subscriber].ContainsKey(channelName) || !subscriptions[subscriber][channelName].ContainsKey(eventName)) return;

        var channel = GetEventChannel(channelName);
        if (channel != null)
        {
            var callback = subscriptions[subscriber][channelName][eventName];
            channel.UnregisterListener(callback);
        }

        subscriptions[subscriber][channelName].Remove(eventName);
        if (subscriptions[subscriber][channelName].Count == 0)
        {
            subscriptions[subscriber].Remove(channelName);
            if (subscriptions[subscriber].Count == 0)
            {
                subscriptions.Remove(subscriber);
            }
        }
    }

    // Modified RaiseEvent method to log events into history
    public void RaiseEvent(string channelName, string eventName)
    {
        var channel = GetEventChannel(channelName);
        if (channel != null)
        {
            // Log the event before raising it
            eventHistory.Add(new EventHistory(channelName, eventName));
            channel?.RaiseEvent(eventName);
        }
    }


    private void OnDestroy()
    {
        // Loop through all subscriptions to unregister them properly
        foreach (var subscriber in subscriptions)
        {
            foreach (var channelSubscription in subscriber.Value)
            {
                var eventChannel = GetEventChannel(channelSubscription.Key);
                if (eventChannel != null)
                {
                    // Now iterating over each action/callback in the dictionary
                    foreach (var action in channelSubscription.Value)
                    {
                        eventChannel.UnregisterListener(action.Value); // Correctly passing the action to UnregisterListener
                    }
                }
            }
        }
    }


    public void UnregisterAllListenersForGameObject(GameObject subscriber)
    {
        // Assuming 'subscriptions' is structured as: GameObject -> (ChannelName -> Action<string>)
        if (subscriptions.TryGetValue(subscriber, out var channelSubscriptions))
        {
            foreach (var channelName in channelSubscriptions.Keys)
            {
                var eventChannel = GetEventChannel(channelName);
                if (eventChannel != null)
                {
                    var actions = channelSubscriptions[channelName];
                    foreach (var action in actions.Values)
                    {
                        eventChannel.UnregisterListener(action);
                    }
                }
            }
            subscriptions.Remove(subscriber);
        }
    }
    // Optional: Method to clear the event history
    public void ClearEventHistory()
    {
        eventHistory.Clear();
    }

    // Optional: Method to print the event history to the console for debugging
    public void PrintEventHistory()
    {
        foreach (var history in eventHistory)
        {
            Debug.Log($"Event: {history.EventName} in Channel: {history.ChannelName} at {history.Timestamp}");
        }
    }
}
