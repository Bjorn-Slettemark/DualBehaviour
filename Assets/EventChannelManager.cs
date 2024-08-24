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
    private Dictionary<GameEventChannelSO, Dictionary<string, System.Action<string>>> globalSubscriptions = new Dictionary<GameEventChannelSO, Dictionary<string, System.Action<string>>>();

    public List<EventHistory> eventHistory = new List<EventHistory>();
    public Dictionary<string, GameEventChannelSO> channelsByName = new Dictionary<string, GameEventChannelSO>();

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

        PopulateChannelsByName();

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

    }

    // Subscription

    public void SubscribeForAllChannels(GameObject subscriber, System.Action<string> callback)
    {
        foreach (var channel in eventChannels)
        {
            SubscribeChannel(subscriber, channel.name, callback);
        }
    }

    public void UnsubscribeForAllChannels(GameObject subscriber)
    {
        foreach (var channel in eventChannels)
        {
            UnSubscribeChannel(subscriber, channel.name);
        }
    }

    public void SubscribeChannel(GameObject subscriber, string channelName, System.Action<string> callback)
    {
        if (string.IsNullOrEmpty(channelName)) return;

        if (!channelsByName.TryGetValue(channelName, out GameEventChannelSO channel))
        {
            Debug.LogWarning($"Channel '{channelName}' does not exist. Creating it.");
            RegisterNewChannel(channelName);
            channel = channelsByName[channelName];
        }

        if (subscriber == null)
        {
            // Handle global subscription
            if (!globalSubscriptions.ContainsKey(channel))
            {
                globalSubscriptions[channel] = new Dictionary<string, System.Action<string>>();
            }
            const string allEventsKey = "*";
            globalSubscriptions[channel][allEventsKey] = callback;
        }
        else
        {
            // Handle GameObject-specific subscription
            if (!subscriptions.ContainsKey(subscriber))
            {
                subscriptions[subscriber] = new Dictionary<GameEventChannelSO, Dictionary<string, System.Action<string>>>();
            }
            if (!subscriptions[subscriber].ContainsKey(channel))
            {
                subscriptions[subscriber][channel] = new Dictionary<string, System.Action<string>>();
            }
            const string allEventsKey = "*";
            subscriptions[subscriber][channel][allEventsKey] = callback;
        }

        channel.RegisterListenerForAllEvents(callback);
    }

    public void UnSubscribeChannel(GameObject subscriber, string channelName)
    {
        if (!channelsByName.TryGetValue(channelName, out GameEventChannelSO channel)) return;

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
    
    public void SubscribeEvent(GameObject subscriber, GameEventChannelSO channel, string eventName, System.Action<string> callback)
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

    public void UnsubscribeEvent(GameObject subscriber, GameEventChannelSO channel, string eventName)
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
    

    // Manage Channels
    public void RaiseEvent(string channelName, string eventName)
    {
        if (channelsByName.TryGetValue(channelName, out GameEventChannelSO channel))
        {
            channel.RaiseEvent(eventName);
            eventHistory.Add(new EventHistory(channelName, eventName, "System"));

            //Debug.Log($"Raised event '{eventName}' on channel '{channelName}'");
        }
        else
        {
            Debug.LogWarning($"Attempted to raise event '{eventName}' on non-existent channel '{channelName}'");
        }
    }

    public bool ChannelExists(string channelName)
    {
        return channelsByName.ContainsKey(channelName);
    }

    public void RegisterNewChannel(string channelName)
    {
        if (!channelsByName.ContainsKey(channelName))
        {
            GameEventChannelSO newChannel = ScriptableObject.CreateInstance<GameEventChannelSO>();
            newChannel.name = channelName;

            channelsByName[channelName] = newChannel;
            eventChannels.Add(newChannel);

            Debug.Log($"Created new channel: {channelName}");
        }
    }

    private void PopulateChannelsByName()
    {
        channelsByName.Clear();
        foreach (var channel in eventChannels)
        {
            if (channel != null && !string.IsNullOrEmpty(channel.name))
            {
                channelsByName[channel.name] = channel;
            }
            else
            {
                Debug.LogWarning("Found a null or unnamed channel in eventChannels list.");
            }
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




}