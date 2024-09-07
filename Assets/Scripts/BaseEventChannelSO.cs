using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class BaseEventChannelSO : ScriptableObject
{
    [SerializeField] private List<string> recentEvents = new List<string>();
    [SerializeField] private int maxRecentEvents = 50;

    public void LogEvent(string eventName, string eventData)
    {
        string logEntry = $"{DateTime.Now:HH:mm:ss} - {eventName}: {eventData}";
        recentEvents.Insert(0, logEntry);
        if (recentEvents.Count > maxRecentEvents)
        {
            recentEvents.RemoveAt(recentEvents.Count - 1);
        }
    }

    public List<string> GetRecentEvents()
    {
        return recentEvents;
    }

    public void ClearRecentEvents()
    {
        recentEvents.Clear();
    }
}