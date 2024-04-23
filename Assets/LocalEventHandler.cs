using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum LocalEventChannel
{
    State,
    Sense,
    Combat,
    Interaction,
    Inventory,
    Animation,
    Mission
}

public class LocalEventHandler : MonoBehaviour
{
    private Dictionary<LocalEventChannel, Dictionary<string, Action<string>>> channelDictionary = new Dictionary<LocalEventChannel, Dictionary<string, Action<string>>>();
    private const string AllEventsKey = "*";  // Wildcard key for subscribing to all events in a channel
    private List<EventHistory> eventHistory = new List<EventHistory>();  // List to keep track of event history for debugging

    // Define an EventHistory struct to record events
public struct EventHistory
{
    public string ChannelName { get; }
    public string EventName { get; }
    public string SenderName { get; }
    public DateTime Timestamp { get; }

    public EventHistory(string channelName, string eventName, string senderName, DateTime timestamp)
    {
        ChannelName = channelName;
        EventName = eventName;
        SenderName = senderName;
        Timestamp = timestamp;
    }
}

    // Subscribe to a specific event within a channel
    public void SubscribeEvent(LocalEventChannel channel, string eventName, Action<string> callback)
    {
        if (!channelDictionary.ContainsKey(channel))
        {
            channelDictionary[channel] = new Dictionary<string, Action<string>>();
        }

        if (channelDictionary[channel].ContainsKey(eventName))
        {
            channelDictionary[channel][eventName] += callback;
        }
        else
        {
            channelDictionary[channel][eventName] = callback;
        }
    }

    // Subscribe to all events within a channel
    public void SubscribeChannel(LocalEventChannel channel, Action<string> callback)
    {
        SubscribeEvent(channel, AllEventsKey, callback);
    }

    // Unsubscribe from a specific event or all events in a channel
    public void Unsubscribe(LocalEventChannel channel, string eventName, Action<string> callback)
    {
        if (channelDictionary.ContainsKey(channel) && channelDictionary[channel].ContainsKey(eventName))
        {
            channelDictionary[channel][eventName] -= callback;

            if (channelDictionary[channel][eventName] == null)
            {
                channelDictionary[channel].Remove(eventName);
            }
        }
    }

    // Publi an event to a specific channel
    public void Publish(LocalEventChannel channel, string eventName, string eventArg)
    {
        EnsureEventRegistered(channel, eventName); // Ensure the event is registered before publishing

        if (channelDictionary.ContainsKey(channel))
        {
            // Notify specific event subscribers
            if (channelDictionary[channel].ContainsKey(eventName))
            {
                channelDictionary[channel][eventName]?.Invoke(eventArg);
            }

            // Notify subscribers to all events within the channel
            if (channelDictionary[channel].ContainsKey(AllEventsKey))
            {
                channelDictionary[channel][AllEventsKey]?.Invoke(eventArg);
            }

            // Record the event in the history for debugging
            // add timestamp to the event history
            eventHistory.Add(new EventHistory(channel.ToString(), eventName, eventArg, DateTime.Now)); 
            Debug.Log($"Event recorded: {channel}, {eventName}, {eventArg}, Total Events: {eventHistory.Count}");

        }
        else
        {
            Debug.Log("Publish called with unregistered channel or event name.");
        }
    }

    // Get all channels as a list of strings (for editor usage)
    public List<LocalEventChannel> GetChannels()
    {
        return Enum.GetValues(typeof(LocalEventChannel)).Cast<LocalEventChannel>().ToList();
    }

    // Get event history (for editor usage)
    public List<EventHistory> GetEventHistory()
    {
        return eventHistory;
    }

    // Clear event history (for editor usage)
    public void ClearEventHistory()
    {
        eventHistory.Clear();
    }

    public void EnsureChannelRegistered(LocalEventChannel channel)
    {
        if (!channelDictionary.ContainsKey(channel))
        {
            channelDictionary.Add(channel, new Dictionary<string, Action<string>>());
        }
    }

    public void EnsureEventRegistered(LocalEventChannel channel, string eventName)
    {
        EnsureChannelRegistered(channel); // First ensure the channel is registered
        if (!channelDictionary[channel].ContainsKey(eventName))
        {
            channelDictionary[channel].Add(eventName, null); // Initialize with null, assuming no listeners yet
        }
    }

    // Unsubscribe from all events across all channels
    public void UnsubscribeAll()
    {
        foreach (var channel in channelDictionary.Keys)
        {
            var eventsList = new List<string>(channelDictionary[channel].Keys); // Create a copy to modify during iteration
            foreach (var eventName in eventsList)
            {
                channelDictionary[channel][eventName] = null; // Clear delegates
                channelDictionary[channel].Remove(eventName); // Remove the event entry
            }
        }
    }

    public void Unsubscribe(LocalEventChannel channel)
    {
        if (channelDictionary.ContainsKey(channel))
        {
            channelDictionary[channel].Clear(); // Clears all events within the channel
            channelDictionary.Remove(channel); // Optionally remove the channel if no events are needed
        }
    }

    public void UnsubscribeEvents(LocalEventChannel channel, params string[] eventNames)
    {
        if (channelDictionary.ContainsKey(channel))
        {
            foreach (var eventName in eventNames)
            {
                if (channelDictionary[channel].ContainsKey(eventName))
                {
                    channelDictionary[channel][eventName] = null; // Clear delegates
                    channelDictionary[channel].Remove(eventName); // Remove event entry
                }
            }
        }
    }

}
