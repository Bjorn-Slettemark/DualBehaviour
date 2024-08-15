using System.Collections.Generic;
using System;
using UnityEngine;

[ExecuteInEditMode] // Execute this script in editor mode as well
public class EventChannelManager : MonoBehaviour
{
    public static EventChannelManager Instance { get; private set; }

    public List<GameEventChannelSO> EventChannels { get => eventChannels; }
    [SerializeField] private List<GameEventChannelSO> eventChannels;

    private Dictionary<GameObject, Dictionary<GameEventChannelSO, Dictionary<string, System.Action<string>>>> subscriptions = new Dictionary<GameObject, Dictionary<GameEventChannelSO, Dictionary<string, System.Action<string>>>>();
    
    public List<EventHistory> eventHistory = new List<EventHistory>();

    public struct EventHistory
    {
        public string ChannelName;
        public string EventName;
        public string SenderName;
        public DateTime Timestamp;

        public EventHistory(string channelName, string eventName, string senderName)
        {
            ChannelName = channelName;
            EventName = eventName;
            SenderName = senderName;
            Timestamp = DateTime.Now;
        }
    }
    // Add this inside the EventChannelManager class

    private void Awake()
    {
        SetupSingleton(); // Ensure singleton setup on Awake.


        // Ensure DontDestroyOnLoad is only called when in play mode
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(gameObject);
        }

        // Initialize other components or setup as necessary
    }
    private void SetupSingleton()
    {
        if (Instance != null && Instance != this)
        {
            // If another instance exists, destroy this one and return to avoid duplicates.
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Only call DontDestroyOnLoad when the application is running.
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(gameObject);
        }

        // Additional initialization can be placed here as needed.
    }

    // Add other methods and logic as previously discussed...

public void RegisterForAllChannels(GameObject subscriber, System.Action<string> callback)
    {
        foreach (var channel in eventChannels)
        {
            RegisterChannel(subscriber, channel, callback);
        }
    }

    public void UnregisterForAllChannels(GameObject subscriber)
    {
        foreach (var channel in eventChannels)
        {
            UnregisterChannel(subscriber, channel);
        }
    }

    public void RegisterChannel(GameObject subscriber, GameEventChannelSO channel, System.Action<string> callback)
    {
        if (channel == null) return;

        if (!subscriptions.ContainsKey(subscriber))
        {
            subscriptions[subscriber] = new Dictionary<GameEventChannelSO, Dictionary<string, System.Action<string>>>();
        }

        if (!subscriptions[subscriber].ContainsKey(channel))
        {
            subscriptions[subscriber][channel] = new Dictionary<string, System.Action<string>>();
        }

        const string allEventsKey = "*"; // Special key to denote subscription to all events in the channel
        subscriptions[subscriber][channel][allEventsKey] = callback;
        channel.RegisterListenerForAllEvents(callback);
    }

    public void UnregisterChannel(GameObject subscriber, GameEventChannelSO channel)
    {
        if (!subscriptions.ContainsKey(subscriber) || !subscriptions[subscriber].ContainsKey(channel)) return;

        const string allEventsKey = "*";
        if (subscriptions[subscriber][channel].ContainsKey(allEventsKey))
        {
            var callback = subscriptions[subscriber][channel][allEventsKey];
            channel.UnregisterListenerFromAllEvents(callback);
            subscriptions[subscriber][channel].Remove(allEventsKey);

            if (subscriptions[subscriber][channel].Count == 0)
            {
                subscriptions[subscriber].Remove(channel);
                if (subscriptions[subscriber].Count == 0)
                {
                    subscriptions.Remove(subscriber);
                }
            }
        }
    }
    public void RegisterEvent(GameObject subscriber, GameEventChannelSO channel, string eventName, System.Action<string> callback)
    {
        if (channel == null) return;

        channel.RegisterListener(callback, eventName);

        if (!subscriptions.ContainsKey(subscriber))
        {
            subscriptions[subscriber] = new Dictionary<GameEventChannelSO, Dictionary<string, System.Action<string>>>();
        }

        if (!subscriptions[subscriber].ContainsKey(channel))
        {
            subscriptions[subscriber][channel] = new Dictionary<string, System.Action<string>>();
        }

        subscriptions[subscriber][channel][eventName] = callback;
    }

    public void UnregisterEvent(GameObject subscriber, GameEventChannelSO channel, string eventName)
    {
        if (channel == null || !subscriptions.ContainsKey(subscriber) || !subscriptions[subscriber].ContainsKey(channel) || !subscriptions[subscriber][channel].ContainsKey(eventName))
        {
            return;
        }

        var callback = subscriptions[subscriber][channel][eventName];
        channel.UnregisterListener(callback);

        subscriptions[subscriber][channel].Remove(eventName);

        if (subscriptions[subscriber][channel].Count == 0)
        {
            subscriptions[subscriber].Remove(channel);
            if (subscriptions[subscriber].Count == 0)
            {
                subscriptions.Remove(subscriber);
            }
        }
    }

    public void RaiseEvent(GameEventChannelSO channel, string eventDescriptor)
    {
        string[] parts = eventDescriptor.Split(':');
        string eventName = parts[0];
        string senderName = parts.Length > 1 ? parts[1] : "Unknown";

        if (channel != null)
        {
            eventHistory.Add(new EventHistory(channel.name, eventName, senderName));
            channel.RaiseEvent(eventName);
        }
    }

    private void OnDestroy()
    {
        // Properly unregister all listeners when the manager is destroyed
        foreach (var subscriber in subscriptions)
        {
            foreach (var channelSubscription in subscriber.Value)
            {
                foreach (var action in channelSubscription.Value)
                {
                    channelSubscription.Key.UnregisterListener(action.Value);
                }
            }
        }
    }

    public void ClearEventHistory()
    {
        eventHistory.Clear();
    }

    public void PrintEventHistory()
    {
        foreach (var history in eventHistory)
        {
            Debug.Log($"Event: {history.EventName} in Channel: {history.ChannelName} at {history.Timestamp}");
        }
    }



#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadMethod]
    private static void EnsureManagerExists()
    {
        if (Instance == null)
        {


            // Updated method to find the first object of type
            Instance = UnityEngine.Object.FindObjectOfType<EventChannelManager>();

            if (Instance == null)
            {
                // Create a new manager if one does not exist
                var manager = new GameObject("EventChannelManager (Editor)");
                Instance = manager.AddComponent<EventChannelManager>();
            }
            else
            {
                // Ensure it's set up properly if an instance already exists
                Instance.SetupSingleton();
            }
        }
    }
#endif




    private Dictionary<string, GameEventChannelSO> channelsByName = new Dictionary<string, GameEventChannelSO>();

    public void RegisterChannelByName(string channelName)
    {
        if (!channelsByName.ContainsKey(channelName))
        {
            GameEventChannelSO newChannel = ScriptableObject.CreateInstance<GameEventChannelSO>();
            newChannel.name = channelName;

            channelsByName[channelName] = newChannel;

            eventChannels.Add(newChannel);

            Debug.Log($"Registered new channel: {channelName}");
        }
    }

    public void RegisterEventByName(GameObject subscriber, string channelName, string eventName, Action<string> callback)
    {
        if (!channelsByName.ContainsKey(channelName))
        {
            RegisterChannelByName(channelName);
        }

        GameEventChannelSO channel = channelsByName[channelName];
        channel.RegisterListener(callback, eventName);
        Debug.Log($"Registered event '{eventName}' for channel '{channelName}'");
    }

    public void UnregisterEventByName(GameObject subscriber, string channelName, string eventName, Action<string> callback)
    {
        if (channelsByName.TryGetValue(channelName, out GameEventChannelSO channel))
        {
            channel.UnregisterListener(callback);
            Debug.Log($"Unregistered event '{eventName}' for channel '{channelName}'");
        }
    }

    public void RaiseEventByName(string channelName, string eventName)
    {
        if (channelsByName.TryGetValue(channelName, out GameEventChannelSO channel))
        {
            channel.RaiseEvent(eventName);
            Debug.Log($"Raised event '{eventName}' on channel '{channelName}'");
        }
        else
        {
            Debug.LogWarning($"Attempted to raise event '{eventName}' on non-existent channel '{channelName}'");
        }
    }
}
