using UnityEngine;
using System.Collections.Generic;

public class EventChannelManager : MonoBehaviour
{
    public static EventChannelManager Instance { get; private set; }

    // Use a dictionary to manage multiple event channels if needed
    [SerializeField] private List<GameEventChannelSO> eventChannels;
    private Dictionary<string, GameEventChannelSO> eventChannelDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Makes this object persistent across scene loads
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

    // Optionally, methods to register and raise events through specific channels directly
    public void RaiseEvent(string channelName, string eventName)
    {
        var channel = GetEventChannel(channelName);
        channel?.RaiseEvent(eventName);
    }

    // Other utility methods can be added as needed, such as for registering/unregistering listeners, etc.
}
