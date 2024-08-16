using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

[AttributeUsage(AttributeTargets.Field)]
public class SyncAttribute : Attribute { }

public class MultiBehaviour : MonoBehaviour
{
    [SerializeField] private string objectId;
    [SerializeField] private string ownerId; // The ID of the peer that owns this object
    private Dictionary<string, object> lastSentValues = new Dictionary<string, object>();
    private float syncInterval = 0.03f; // 30 times per second
    private float lastSyncTime;

    private void Start()
    {
        if (string.IsNullOrEmpty(objectId))
        {
            objectId = System.Guid.NewGuid().ToString();
        }
        RegisterNetworkObject();
    }

    private void Update()
    {
        if (Time.time - lastSyncTime > syncInterval)
        {
            SyncFields();
            lastSyncTime = Time.time;
        }
    }

    private void SyncFields()
    {
        var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(field => Attribute.IsDefined(field, typeof(SyncAttribute)));
        var changedFields = new Dictionary<string, object>();
        foreach (var field in fields)
        {
            var currentValue = field.GetValue(this);
            if (!lastSentValues.TryGetValue(field.Name, out var lastValue) || !object.Equals(currentValue, lastValue))
            {
                changedFields[field.Name] = currentValue;
                lastSentValues[field.Name] = currentValue;
            }
        }
        if (changedFields.Count > 0)
        {
            SendSyncMessage(changedFields);
        }
    }

    private void SendSyncMessage(Dictionary<string, object> changedFields)
    {
        string message = $"SYNC|{objectId}";
        foreach (var kvp in changedFields)
        {
            message += $"|{kvp.Key}:{SerializeValue(kvp.Value)}";
        }
        MultiplayerManager.Instance.BroadcastSyncMessage(ownerId, message);
    }

    public void ReceiveSyncMessage(Dictionary<string, object> receivedFields)
    {
        var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(field => Attribute.IsDefined(field, typeof(SyncAttribute)))
            .ToDictionary(field => field.Name);
        foreach (var kvp in receivedFields)
        {
            if (fields.TryGetValue(kvp.Key, out FieldInfo field))
            {
                object newValue = DeserializeValue(kvp.Value, field.FieldType);
                field.SetValue(this, newValue);
                lastSentValues[kvp.Key] = newValue; // Update last sent value to avoid re-sending
            }
        }
    }

    private string SerializeValue(object value)
    {
        // Basic serialization, expand as needed
        return value.ToString();
    }

    private object DeserializeValue(object value, Type targetType)
    {
        // Basic deserialization, expand as needed
        return Convert.ChangeType(value, targetType);
    }

    private void RegisterNetworkObject()
    {
        MultiplayerManager.Instance.RegisterNetworkObject(objectId, this);
    }

    public void SetOwnerId(string peerId)
    {
        ownerId = peerId;
    }

    // Method to be called when you want to change a synced variable
    protected void SetSyncedValue<T>(ref T field, T newValue, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
    {
        if (!object.Equals(field, newValue))
        {
            // Send the change to the network
            SendSyncMessage(new Dictionary<string, object> { { propertyName, newValue } });
            // The actual field will be updated when the sync message is received back
        }
    }
}