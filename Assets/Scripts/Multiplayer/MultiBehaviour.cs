using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Globalization;


[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public class SyncAttribute : Attribute
{
    // You can add additional properties here if needed in the future
    // For now, we'll keep it simple
}

public class MultiBehaviour : MonoBehaviour
{
    private const float INITIALIZATION_DELAY = 1f;

    [SerializeField] private string objectId;
    [SerializeField] private string ownerId;
    private Dictionary<string, object> syncedValues = new Dictionary<string, object>();
    private bool isInitialized = false;

    public string ObjectId => objectId;
    public string OwnerId => ownerId;
    private bool objectIdRequested = false;

    private void Awake()
    {
        InitializeSyncedProperties();
        EventChannelManager.Instance.RegisterForChannel(gameObject, "LevelEventChannel", HandleLevelChannelMessage);

    }

    public void Initialize(string ownerId, string objectId = null)
    {
        StartCoroutine(DelayedInitialize(ownerId, objectId));
    }
    private IEnumerator DelayedInitialize(string ownerId, string objectId = null)
    {
        yield return new WaitForSeconds(INITIALIZATION_DELAY);

        this.ownerId = ownerId;
        if (objectId == null && ownerId == WebRTCManager.Instance.LocalPeerId && !objectIdRequested)
        {
            objectIdRequested = true;
            RequestObjectId();
        }
        else
        {
            SetObjectId(objectId);
        }
    }


    private void RequestObjectId()
    {
        MultiplayerManager.Instance.BroadcastEventToAllPeers($"LevelEventChannel:RequestMultiplayerObjectId:{ownerId}");
    }

    private void HandleLevelChannelMessage(string eventData)
    {
        Debug.Log("MultiBehaviour is getting the levelchannel: " + eventData);
        string[] parts = eventData.Split(':');
        if (parts[0] == "NewMultiplayerObjectId" && parts[1] == ownerId && !isInitialized)
        {
            Debug.Log("Setting objectid: " + parts[2]);
            SetObjectId(parts[2]);

        }

    }

    public void SetObjectId(string id)
    {
        if (!isInitialized && id != null)
        {
            objectId = id;
            isInitialized = true;
            SubscribeToSyncEvents();
            if (ownerId == WebRTCManager.Instance.LocalPeerId)
            {
                MultiplayerManager.Instance.CreateMultiplayerObject(objectId, gameObject.name);
            }
            Debug.Log($"[MultiBehaviour] Initialized: ObjectId={objectId}, OwnerId={ownerId}");
        }
        else if (isInitialized)
        {
            Debug.LogWarning($"[MultiBehaviour] Attempted to set ObjectId after initialization: {id}");
        }
    }
    //public void SetOwnerId(string id)
    //{
    //    if (ownerId != id)
    //    {
    //        UnsubscribeFromSyncEvents();
    //        ownerId = id;
    //        SubscribeToSyncEvents();
    //        Debug.Log($"[MultiBehaviour] Set new OwnerId: {ownerId}");
    //    }
    //}

    private void InitializeSyncedProperties()
    {
        var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(prop => Attribute.IsDefined(prop, typeof(SyncAttribute)));

        foreach (var property in properties)
        {
            syncedValues[property.Name] = property.GetValue(this);
            Debug.Log($"[MultiBehaviour] Initialized synced property: {property.Name} = {syncedValues[property.Name]}");
        }
    }

    private void SubscribeToSyncEvents()
    {
        string channelName = MultiplayerManager.Instance.GetPeerChannelName(LocalWebRTCManager.Instance.LocalPeerId);
        EventChannelManager.Instance.RegisterForChannel(gameObject, channelName, HandleSyncMessage);
        Debug.Log($"[MultiBehaviour] Subscribed to channel: {channelName}");
    }

    private void UnsubscribeFromSyncEvents()
    {
        if (!string.IsNullOrEmpty(ownerId))
        {
            string channelName = MultiplayerManager.Instance.GetPeerChannelName(ownerId);
            EventChannelManager.Instance.UnregisterFromChannel(gameObject, channelName);
            Debug.Log($"[MultiBehaviour] Unsubscribed from channel: {channelName}");
        }
    }

    private void HandleSyncMessage(string message)
    {
        Debug.Log($"[MultiBehaviour] Received sync message: {message}");
        string[] parts = message.Split(':');
        if (parts.Length >= 4)
        {
            string senderId = parts[0];
            string messageObjectId = parts[1];
            string propertyName = parts[2];
            string valueString = string.Join(":", parts.Skip(3));

            // Check if this message is for this object
            if (messageObjectId != objectId)
            {
                // This message is not for this object, ignore it
                return;
            }

            var property = GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property != null && Attribute.IsDefined(property, typeof(SyncAttribute)))
            {
                object newValue = DeserializeValue(valueString, property.PropertyType);
                UpdateSyncedValue(propertyName, newValue);
            }
            else
            {
                Debug.LogWarning($"[MultiBehaviour] Property not found or not synced: {propertyName}");
            }
        }
        else
        {
            Debug.LogWarning($"[MultiBehaviour] Invalid sync message format: {message}");
        }
    }



    public void UpdateSyncedValue(string propertyName, object newValue)
    {
        if (!syncedValues.ContainsKey(propertyName) || !object.Equals(syncedValues[propertyName], newValue))
        {
            syncedValues[propertyName] = newValue;
            var property = GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            property?.SetValue(this, newValue);
            Debug.Log($"[MultiBehaviour] Updated {propertyName} to {newValue}");
        }
        else
        {
            Debug.Log($"[MultiBehaviour] No change in {propertyName}, current value: {newValue}");
        }
    }

    protected void RequestSyncedValueUpdate<T>(string propertyName, T newValue)
    {
        Debug.Log($"[MultiBehaviour] Requesting synced value update: {propertyName} = {newValue}");
        string message = $"{LocalWebRTCManager.Instance.LocalPeerId}:{objectId}:{propertyName}:{SerializeValue(newValue)}";
        MultiplayerManager.Instance.BroadcastEventToAllPeers(message);
    }

    private string SerializeValue(object value)
    {
        if (value == null)
            return "null";

        if (value is Vector3 vector3Value)
            return $"v3:{vector3Value.x},{vector3Value.y},{vector3Value.z}";

        if (value is Quaternion quaternionValue)
            return $"q:{quaternionValue.x},{quaternionValue.y},{quaternionValue.z},{quaternionValue.w}";

        if (value is float floatValue)
            return $"f:{floatValue.ToString(CultureInfo.InvariantCulture)}";

        if (value is int intValue)
            return $"i:{intValue}";

        if (value is bool boolValue)
            return $"b:{(boolValue ? "1" : "0")}";

        if (value is string stringValue)
            return $"s:{stringValue}";

        // For other types, use JSON serialization
        return $"json:{JsonUtility.ToJson(value)}";
    }

    private object DeserializeValue(string value, Type targetType)
    {
        if (value == "null")
            return null;

        string[] parts = value.Split(new[] { ':' }, 2);
        if (parts.Length != 2)
            throw new ArgumentException("Invalid serialized value format");

        string typePrefix = parts[0];
        string data = parts[1];

        switch (typePrefix)
        {
            case "v3":
                if (targetType != typeof(Vector3))
                    throw new ArgumentException("Type mismatch: expected Vector3");
                string[] vectorParts = data.Split(',');
                return new Vector3(
                    float.Parse(vectorParts[0], CultureInfo.InvariantCulture),
                    float.Parse(vectorParts[1], CultureInfo.InvariantCulture),
                    float.Parse(vectorParts[2], CultureInfo.InvariantCulture));

            case "q":
                if (targetType != typeof(Quaternion))
                    throw new ArgumentException("Type mismatch: expected Quaternion");
                string[] quatParts = data.Split(',');
                return new Quaternion(
                    float.Parse(quatParts[0], CultureInfo.InvariantCulture),
                    float.Parse(quatParts[1], CultureInfo.InvariantCulture),
                    float.Parse(quatParts[2], CultureInfo.InvariantCulture),
                    float.Parse(quatParts[3], CultureInfo.InvariantCulture));

            case "f":
                if (targetType != typeof(float))
                    throw new ArgumentException("Type mismatch: expected float");
                return float.Parse(data, CultureInfo.InvariantCulture);

            case "i":
                if (targetType != typeof(int))
                    throw new ArgumentException("Type mismatch: expected int");
                return int.Parse(data);

            case "b":
                if (targetType != typeof(bool))
                    throw new ArgumentException("Type mismatch: expected bool");
                return data == "1";

            case "s":
                if (targetType != typeof(string))
                    throw new ArgumentException("Type mismatch: expected string");
                return data;

            case "json":
                return JsonUtility.FromJson(data, targetType);

            default:
                throw new ArgumentException($"Unknown type prefix: {typePrefix}");
        }
    }
    private void OnDestroy()
    {
        EventChannelManager.Instance.UnregisterFromChannel(gameObject, "LevelEventChannel");
        UnsubscribeFromSyncEvents();
    }
}