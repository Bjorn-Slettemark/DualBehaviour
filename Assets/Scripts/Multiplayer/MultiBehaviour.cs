using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

[AttributeUsage(AttributeTargets.Field)]
public class SyncVarAttribute : Attribute { }

public abstract class MultiBehaviour : MonoBehaviour
{
    [SerializeField] private string objectId;
    [SerializeField] private string ownerPeerId;

    public string ObjectId { get => objectId; set => objectId = value; }
    public string OwnerPeerId { get => ownerPeerId; set => ownerPeerId = value; }

    private Dictionary<string, SyncVarInfo> syncVars = new Dictionary<string, SyncVarInfo>();
    private string ownerChannel;

    [SerializeField] private float interpolationSpeed = 10f;
    [SerializeField] private float updateInterval = 0.1f; // New: Update interval in seconds
    private float lastUpdateTime;

    private class SyncVarInfo
    {
        public FieldInfo Field;
        public object NetworkValue;
        public object LocalValue;
        public object InterpolationTarget;
        public float InterpolationTime;
        public bool IsDirty; // New: Flag to track if the value has changed
    }

    public void OnInitialize(string objectId, string ownerPeerId)
    {
        this.objectId = objectId;
        this.ownerPeerId = ownerPeerId;
        this.ownerChannel = $"PeerChannel{ownerPeerId}";
        InitializeSyncVars();
        SubscribeToOwnerChannel();
        Debug.Log($"Initialized MultiBehaviour for object {objectId} owned by {ownerPeerId}");
    }

    private void InitializeSyncVars()
    {
        var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (Attribute.IsDefined(field, typeof(SyncVarAttribute)))
            {
                object initialValue = field.GetValue(this);
                syncVars[field.Name] = new SyncVarInfo
                {
                    Field = field,
                    NetworkValue = initialValue,
                    LocalValue = initialValue,
                    InterpolationTarget = initialValue,
                    InterpolationTime = 0f
                };
                Debug.Log($"Initialized SyncVar {field.Name} for {GetType().Name}");
            }
        }
    }

    private void SubscribeToOwnerChannel()
    {
        EventChannelManager.Instance.SubscribeToChannel(ownerChannel, OnSyncMessageReceived);
        Debug.Log($"Subscribed to owner channel {ownerChannel}");
    }

    private void OnSyncMessageReceived(string message)
    {
        string[] parts = message.Split(':');
        if (parts.Length == 4 && parts[0] == objectId)
        {
            string fieldName = parts[1];
            string serializedValue = parts[2];
            string senderPeerId = parts[3];
            if (senderPeerId == ownerPeerId && senderPeerId != LocalWebRTCEngine.Instance.LocalPeerId)
            {
                object deserializedValue = DeserializeValue(fieldName, serializedValue);
                if (deserializedValue is Quaternion q)
                {
                    Debug.Log($"Received Quaternion update for {fieldName}: Serialized={serializedValue}, Deserialized={q}, Euler={q.eulerAngles}");
                }
                ReceiveNetworkUpdate(fieldName, deserializedValue);
            }
        }
    }

    private void ReceiveNetworkUpdate(string fieldName, object value)
    {
        if (syncVars.TryGetValue(fieldName, out SyncVarInfo syncVarInfo))
        {
            syncVarInfo.NetworkValue = value;
            syncVarInfo.InterpolationTarget = value;
            syncVarInfo.InterpolationTime = 0f;

            syncVarInfo.Field.SetValue(this, value);

            if (value is Quaternion q)
            {
                Debug.Log($"Applied Quaternion update for {fieldName}: Value={q}, Euler={q.eulerAngles}, Current Field Value={syncVarInfo.Field.GetValue(this)}");
            }
        }
        else
        {
            Debug.LogError($"Failed to find SyncVar for field: {fieldName}");
        }
    }
    private void SyncVarUpdate()
    {
        if (IsOwner())
        {
            bool hasChanges = false;
            foreach (var kvp in syncVars)
            {
                string fieldName = kvp.Key;
                SyncVarInfo syncVarInfo = kvp.Value;
                object currentValue = syncVarInfo.Field.GetValue(this);

                if (!object.Equals(currentValue, syncVarInfo.LocalValue))
                {
                    syncVarInfo.LocalValue = currentValue;
                    syncVarInfo.NetworkValue = currentValue;
                    syncVarInfo.InterpolationTarget = currentValue;
                    syncVarInfo.IsDirty = true;
                    hasChanges = true;
                }
            }

            // Only send updates if there are changes and the update interval has passed
            if (hasChanges && Time.time - lastUpdateTime >= updateInterval)
            {
                SendNetworkUpdates();
                lastUpdateTime = Time.time;
            }
        }
    }

    private void SendNetworkUpdates()
    {
        foreach (var kvp in syncVars)
        {
            string fieldName = kvp.Key;
            SyncVarInfo syncVarInfo = kvp.Value;
            if (syncVarInfo.IsDirty)
            {
                string serializedValue = SerializeValue(syncVarInfo.LocalValue);
                string message = $"{objectId}:{fieldName}:{serializedValue}:{ownerPeerId}";
                EventChannelManager.Instance.RaiseNetworkEvent(ownerChannel, message);
                syncVarInfo.IsDirty = false;
            }
        }
    }



    public bool IsOwner()
    {
        return ownerPeerId == LocalWebRTCEngine.Instance.LocalPeerId;
    }

    protected virtual void Update()
    {
        SyncVarUpdate();
        InterpolateSyncVars();
    }

    private string SerializeValue(object value)
    {
        if (value is Vector3 v3)
            return NetworkUtility.SerializeVector3(v3);
        if (value is Quaternion q)
        {
            string serialized = NetworkUtility.SerializeQuaternion(q);
            return serialized;
        }
        return value.ToString();
    }

    private object DeserializeValue(string fieldName, string serializedValue)
    {
        if (syncVars.TryGetValue(fieldName, out SyncVarInfo syncVarInfo))
        {
            Type fieldType = syncVarInfo.Field.FieldType;
            if (fieldType == typeof(Vector3))
                return NetworkUtility.DeserializeVector3(serializedValue);
            if (fieldType == typeof(Quaternion))
            {
                Quaternion deserialized = NetworkUtility.DeserializeQuaternion(serializedValue);
                return deserialized;
            }
            return Convert.ChangeType(serializedValue, fieldType);
        }
        return null;
    }

    private void InterpolateSyncVars()
    {
        foreach (var kvp in syncVars)
        {
            SyncVarInfo syncVarInfo = kvp.Value;
            if (!IsOwner() && !object.Equals(syncVarInfo.NetworkValue, syncVarInfo.InterpolationTarget))
            {
                syncVarInfo.InterpolationTime += Time.deltaTime * interpolationSpeed;
                object interpolatedValue = InterpolateValue(syncVarInfo.NetworkValue, syncVarInfo.InterpolationTarget, syncVarInfo.InterpolationTime);
                syncVarInfo.Field.SetValue(this, interpolatedValue);
                syncVarInfo.NetworkValue = interpolatedValue;

                if (syncVarInfo.InterpolationTime >= 1f)
                {
                    syncVarInfo.NetworkValue = syncVarInfo.InterpolationTarget;
                    syncVarInfo.Field.SetValue(this, syncVarInfo.InterpolationTarget);
                    syncVarInfo.InterpolationTime = 0f;
                }
            }
        }
    }

    private object InterpolateValue(object start, object end, float t)
    {
        t = Mathf.Clamp01(t);
        if (start is Vector3 v3Start && end is Vector3 v3End)
        {
            return Vector3.Lerp(v3Start, v3End, t);
        }
        else if (start is Quaternion qStart && end is Quaternion qEnd)
        {
            Quaternion result = Quaternion.Slerp(qStart, qEnd, t);
            return result;
        }
        else if (start is float fStart && end is float fEnd)
        {
            return Mathf.Lerp(fStart, fEnd, t);
        }
        return end;
    }

    private void OnDestroy()
    {
        EventChannelManager.Instance.UnsubscribeFromChannel(ownerChannel, OnSyncMessageReceived);
        Debug.Log($"Unsubscribed from owner channel {ownerChannel}");
    }
}