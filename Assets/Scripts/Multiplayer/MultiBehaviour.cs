using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

public class MultiBehaviour : MonoBehaviour
{
    [SerializeField][ReadOnly] private int objectId;
    public int ObjectId { get => objectId; set => objectId = value; }

    [SerializeField][ReadOnly] private string ownerPeerId;
    public string OwnerPeerId { get => ownerPeerId; set => ownerPeerId = value; }

    private Dictionary<string, FieldInfo> syncFields = new Dictionary<string, FieldInfo>();
    private Dictionary<string, Func<object>> getters = new Dictionary<string, Func<object>>();
    private Dictionary<string, Action<object>> setters = new Dictionary<string, Action<object>>();

    protected virtual void Awake()
    {
        CollectSyncFields();
    }

    public void Initialize(int objectId, string ownerPeerId)
    {
        ObjectId = objectId;
        OwnerPeerId = ownerPeerId;
        OnInitialized();
    }

    protected virtual void OnInitialized() { }

    public bool IsOwner()
    {
        return OwnerPeerId == LocalWebRTCEngine.Instance.LocalPeerId;
    }

    private void CollectSyncFields()
    {
        syncFields = GetType()
            .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Where(field => Attribute.IsDefined(field, typeof(SyncAttribute)))
            .ToDictionary(field => field.Name, field => field);
    }

    protected void SetSyncLink(string fieldName, Func<object> getter, Action<object> setter)
    {
        if (syncFields.ContainsKey(fieldName))
        {
            getters[fieldName] = getter;
            setters[fieldName] = setter;
            Debug.Log($"Sync link set for {fieldName}");
        }
        else
        {
            Debug.LogWarning($"Attempted to set sync link for non-synced field: {fieldName}");
        }
    }


    public virtual void ReceiveSyncUpdate(NetworkMessage message)
    {
        foreach (var field in syncFields)
        {
            string fieldName = field.Key;
            FieldInfo fieldInfo = field.Value;

            object value = message.GetData<object>(fieldName);
            if (value != null)
            {
                UpdateSyncField(fieldName, value);
            }
        }
    }
    protected void UpdateSyncField(string fieldName, object value)
    {
        if (syncFields.TryGetValue(fieldName, out FieldInfo field))
        {
            field.SetValue(this, value);
            if (setters.TryGetValue(fieldName, out Action<object> setter))
            {
                setter(value);
            }

            Debug.Log($"Sync field updated: {fieldName} = {value}");
        }
        else
        {
            Debug.LogWarning($"Attempted to update non-synced field: {fieldName}");
        }
    }

}

[AttributeUsage(AttributeTargets.Field)]
public class SyncAttribute : Attribute { }

public class ReadOnlyAttribute : PropertyAttribute { }