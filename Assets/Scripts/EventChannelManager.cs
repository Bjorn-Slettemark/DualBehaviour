using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class EventChannelManager : MonoBehaviour
{
    public static EventChannelManager Instance { get; private set; }

    [SerializeField] private List<GameEventChannelSO> allChannels = new List<GameEventChannelSO>();
    private Dictionary<string, List<Action<string>>> subscriptions = new Dictionary<string, List<Action<string>>>();

    public List<GameEventChannelSO> AllChannels => allChannels;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeChannels();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeChannels()
    {
        foreach (var channel in allChannels)
        {
            if (channel != null && !string.IsNullOrEmpty(channel.name))
            {
                RegisterNewChannel(channel.name);
            }
        }
        DumpChannels();
    }

    public bool ChannelExists(string channelName)
    {
        return subscriptions.ContainsKey(channelName);
    }

    public void RegisterNewChannel(string channelName)
    {
        if (!subscriptions.ContainsKey(channelName))
        {
            subscriptions[channelName] = new List<Action<string>>();

            // Check if the channel already exists in allChannels
            if (!allChannels.Any(c => c.name == channelName))
            {
                GameEventChannelSO newChannel = ScriptableObject.CreateInstance<GameEventChannelSO>();
                newChannel.name = channelName;
                allChannels.Add(newChannel);
            }

            Debug.Log($"Registered new channel: {channelName}");
        }
        else
        {
            Debug.Log($"Channel already exists: {channelName}");
        }
    }

    public void SubscribeToChannel(string channelName, Action<string> callback)
    {
        if (!ChannelExists(channelName))
        {
            RegisterNewChannel(channelName);
        }
        subscriptions[channelName].Add(callback);
        Debug.Log($"Subscribed to channel: {channelName}");
    }

    public void UnsubscribeFromChannel(string channelName, Action<string> callback)
    {
        if (ChannelExists(channelName))
        {
            subscriptions[channelName].Remove(callback);
        }
    }

    public void RaiseEvent(string channelName, string eventData)
    {
        if (ChannelExists(channelName))
        {

            foreach (var callback in subscriptions[channelName].ToList()) // Create a copy to avoid modification during iteration
            {
                callback.Invoke(eventData);
            }
        }
        else
        {
            Debug.LogWarning($"Attempted to raise event on non-existent channel: {channelName}");
        }
    }


    public void RaiseNetworkEvent(string channelName, string eventData)
    {
        string serializedMessage = $"{channelName}:{eventData}";
        Debug.Log(serializedMessage);
        NetworkEngine.Instance.BroadcastEventToAllPeers(serializedMessage);
    }

    public void HandleIncomingNetworkEvent(string channelName, string eventData)
    {
        RaiseEvent(channelName, eventData);
    }

    public void DumpChannels()
    {
        Debug.Log("--- All Registered Channels ---");
        foreach (var channel in allChannels)
        {
            Debug.Log($"Channel: {channel.name}, Subscribers: {(subscriptions.ContainsKey(channel.name) ? subscriptions[channel.name].Count : 0)}");
        }
    }
}

