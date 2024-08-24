using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;

public class MultiBehaviour : MonoBehaviour
{
    [SerializeField][ReadOnly] private int objectId;
    public int ObjectId { get => objectId; set => objectId = value; }

    [SerializeField][ReadOnly] private string ownerPeerId;
    public string OwnerPeerId { get => ownerPeerId; set => ownerPeerId = value; }

    private Dictionary<string, FieldInfo> syncFields = new Dictionary<string, FieldInfo>();
    private Dictionary<string, object> syncReferences = new Dictionary<string, object>();
    private const float SyncInterval = 0.1f; // Adjust as needed
    private float nextSyncTime;

    protected virtual void Awake()
    {
        CollectSyncFields();
    }

    public virtual void Initialize(int objectId, string ownerPeerId)
    {
        ObjectId = objectId;
        OwnerPeerId = ownerPeerId;
        OnInitialized();
    }

    protected virtual void OnInitialized() { }

    private void CollectSyncFields()
    {
        FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        foreach (var field in fields)
        {
            if (Attribute.IsDefined(field, typeof(SyncAttribute)))
            {
                syncFields[field.Name] = field;
            }
        }
    }

    protected virtual void Update()
    {
        if (IsOwner() && Time.time >= nextSyncTime)
        {
            SyncVariables();
            nextSyncTime = Time.time + SyncInterval;
        }
    }

    public bool IsOwner()
    {
        return OwnerPeerId == LocalWebRTCEngine.Instance.LocalPeerId;
    }

    protected void SetSyncReference(string fieldName, object reference)
    {
        syncReferences[fieldName] = reference;
    }

    private void SyncVariables()
    {
        Dictionary<string, object> syncData = new Dictionary<string, object>();
        foreach (var kvp in syncFields)
        {
            string fieldName = kvp.Key;
            FieldInfo field = kvp.Value;

            if (syncReferences.TryGetValue(fieldName, out object reference))
            {
                syncData[fieldName] = reference;
            }
            else
            {
                syncData[fieldName] = field.GetValue(this);
            }
        }
        if (syncData.Count > 0)
        {
            string extraData = SerializeSyncData(syncData);
            NetworkMessage syncMessage = NetworkMessageFactory.CreateSyncObjectMessage(
                ObjectId,
                transform.position,
                transform.rotation,
                extraData
            );

            NetworkEngine.Instance.BroadcastEventToAllPeers(syncMessage);
        }
    }

    public virtual void ReceiveSyncUpdate(NetworkMessage message)
    {
        string extraData = message.GetData<string>("ExtraData");
        if (!string.IsNullOrEmpty(extraData))
        {
            Dictionary<string, object> syncData = DeserializeSyncData(extraData);
            ApplySyncUpdate(syncData);
        }

        // Update transform
        Vector3 position = message.GetData<Vector3>("Position");
        Quaternion rotation = message.GetData<Quaternion>("Rotation");

        if (!IsOwner())
        {
            transform.position = position;
            transform.rotation = rotation;
        }
    }

    private void ApplySyncUpdate(Dictionary<string, object> syncData)
    {
        foreach (var kvp in syncData)
        {
            if (syncFields.TryGetValue(kvp.Key, out FieldInfo field))
            {
                field.SetValue(this, kvp.Value);
            }
        }
    }

    protected void UpdateSyncField<T>(string fieldName, T value)
    {
        if (syncFields.TryGetValue(fieldName, out FieldInfo field))
        {
            field.SetValue(this, value);
            if (IsOwner())
            {
                SyncVariables();
            }
        }
    }

    private string SerializeSyncData(Dictionary<string, object> syncData)
    {
        List<string> serializedFields = new List<string>();
        foreach (var kvp in syncData)
        {
            string serializedValue = NetworkUtility.SerializeValue(kvp.Value);
            serializedFields.Add($"{kvp.Key}={serializedValue}");
        }
        return string.Join(":", serializedFields);
    }

    private Dictionary<string, object> DeserializeSyncData(string extraData)
    {
        Dictionary<string, object> result = new Dictionary<string, object>();
        string[] fields = extraData.Split(':');
        for (int i = 0; i < fields.Length; i++)
        {
            string[] keyValue = fields[i].Split('=');
            if (keyValue.Length == 2)
            {
                string key = keyValue[0];
                string serializedValue = keyValue[1];

                if (i + 1 < fields.Length && !fields[i + 1].Contains('='))
                {
                    serializedValue += ":" + fields[++i];
                }

                if (syncFields.TryGetValue(key, out FieldInfo fieldInfo))
                {
                    object deserializedValue = NetworkUtility.DeserializeValue(serializedValue, fieldInfo.FieldType);
                    result[key] = deserializedValue;
                }
            }
        }
        return result;
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class SyncAttribute : Attribute { }

public class ReadOnlyAttribute : PropertyAttribute { }