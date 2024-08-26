using UnityEngine;
using System;
using System.Globalization;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using System.Linq;
using UnityEngine.UIElements;

public static class NetworkUtility
{
    public static string SerializeValue(object value)
    {
        if (value == null)
            return "null";

        if (value is Vector3 vector3Value)
            return $"v3:{vector3Value.x.ToString(CultureInfo.InvariantCulture)}," +
                   $"{vector3Value.y.ToString(CultureInfo.InvariantCulture)}," +
                   $"{vector3Value.z.ToString(CultureInfo.InvariantCulture)}";

        if (value is Quaternion quaternionValue)
            return $"q:{quaternionValue.x.ToString(CultureInfo.InvariantCulture)}," +
                   $"{quaternionValue.y.ToString(CultureInfo.InvariantCulture)}," +
                   $"{quaternionValue.z.ToString(CultureInfo.InvariantCulture)}," +
                   $"{quaternionValue.w.ToString(CultureInfo.InvariantCulture)}";

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
    public static object DeserializeValue(string value, Type targetType)
    {
        if (value == "null")
            return null;

        // Special case for strings: if the value starts with 's:', remove the prefix
        if (value.StartsWith("s:"))
        {
            value = value.Substring(2);
        }

        // If the value doesn't contain a colon, and the target type is string, return the value as-is
        if (!value.Contains(":") && targetType == typeof(string))
        {
            return value;
        }

        string[] parts = value.Split(new[] { ':' }, 2);
        if (parts.Length != 2)
        {
            Debug.LogError($"Invalid serialized value format: {value}");
            throw new ArgumentException($"Invalid serialized value format: {value}");
        }

        string typePrefix = parts[0];
        string data = parts[1];

        switch (typePrefix)
        {
            case "v3":
                if (targetType != typeof(Vector3))
                    throw new ArgumentException($"Type mismatch: expected Vector3, got {targetType}");
                string[] vectorParts = data.Split(',');
                return new Vector3(
                    float.Parse(vectorParts[0], CultureInfo.InvariantCulture),
                    float.Parse(vectorParts[1], CultureInfo.InvariantCulture),
                    float.Parse(vectorParts[2], CultureInfo.InvariantCulture));
            case "q":
                if (targetType != typeof(Quaternion))
                    throw new ArgumentException($"Type mismatch: expected Quaternion, got {targetType}");
                string[] quatParts = data.Split(',');
                return new Quaternion(
                    float.Parse(quatParts[0], CultureInfo.InvariantCulture),
                    float.Parse(quatParts[1], CultureInfo.InvariantCulture),
                    float.Parse(quatParts[2], CultureInfo.InvariantCulture),
                    float.Parse(quatParts[3], CultureInfo.InvariantCulture));
            case "f":
                if (targetType != typeof(float))
                    throw new ArgumentException($"Type mismatch: expected float, got {targetType}");
                return float.Parse(data, CultureInfo.InvariantCulture);
            case "i":
                if (targetType != typeof(int))
                    throw new ArgumentException($"Type mismatch: expected int, got {targetType}");
                return int.Parse(data);
            case "b":
                if (targetType != typeof(bool))
                    throw new ArgumentException($"Type mismatch: expected bool, got {targetType}");
                return data == "1";
            case "s":
                if (targetType != typeof(string))
                    throw new ArgumentException($"Type mismatch: expected string, got {targetType}");
                return data;
            case "json":
                return JsonUtility.FromJson(data, targetType);
            default:
                Debug.LogError($"Unknown type prefix: {typePrefix}");
                throw new ArgumentException($"Unknown type prefix: {typePrefix}");
        }
    }
}
public class NetworkMessage
{
    public string Channel { get; private set; }
    public string MessageType { get; private set; }
    public string LocalPeerId { get; private set; }
    public int? ObjectId { get; private set; }
    private Dictionary<string, object> Data { get; set; }

    private NetworkMessage()
    {
        Data = new Dictionary<string, object>();
    }

    public static NetworkMessage Create(string channel, string messageType, string localPeerId, int? objectId = null)
    {
        return new NetworkMessage
        {
            Channel = channel,
            MessageType = messageType,
            LocalPeerId = localPeerId,
            ObjectId = objectId
        };
    }

    public NetworkMessage AddData(string key, object value)
    {
        Data[key] = value;
        return this;
    }

    public T GetData<T>(string key, T defaultValue = default)
    {
        if (Data.TryGetValue(key, out object value))
        {
            if (value is T typedValue)
            {
                return typedValue;
            }
            else if (typeof(T) == typeof(string) && value != null)
            {
                string stringValue = value.ToString();
                // For ExtraData, return the full string without removing the "s:" prefix
                if (key == "ExtraData")
                {
                    return (T)(object)stringValue;
                }
                // For other string values, remove the "s:" prefix if it exists
                return (T)(object)(stringValue.StartsWith("s:") ? stringValue.Substring(2) : stringValue);
            }
        }

        Debug.LogWarning($"Key '{key}' not found or not of type {typeof(T)}. Returning default value.");
        return defaultValue;
    }
    internal string Serialize()
    {
        var parts = new List<string> { Channel, MessageType, LocalPeerId };
        if (ObjectId.HasValue)
        {
            parts.Add(ObjectId.Value.ToString());
        }
        foreach (var kvp in Data)
        {
            parts.Add($"{kvp.Key}={NetworkUtility.SerializeValue(kvp.Value)}");
        }
        return string.Join(":", parts);
    }

    public static NetworkMessage Deserialize(string message)
    {
        //Debug.Log($"Deserializing message: {message}");

        var parts = message.Split(':');
        if (parts.Length < 3)
        {
            Debug.LogError($"Invalid message format: {message}");
            throw new ArgumentException($"Invalid message format: {message}");
        }

        var result = new NetworkMessage
        {
            Channel = parts[0],
            MessageType = parts[1],
            LocalPeerId = parts[2]
        };

        //Debug.Log($"Channel: {result.Channel}, MessageType: {result.MessageType}, LocalPeerId: {result.LocalPeerId}");

        int startIndex = 3;
        if (parts.Length > 3 && int.TryParse(parts[3], out int objectId))
        {
            result.ObjectId = objectId;
            startIndex = 4;
        }

        // Parse the rest of the message
        for (int i = startIndex; i < parts.Length; i++)
        {
            var keyValue = parts[i].Split('=');
            if (keyValue.Length == 2)
            {
                string key = keyValue[0];
                string serializedValue = keyValue[1];

                if (i + 1 < parts.Length && !parts[i + 1].Contains('='))
                {
                    // This is a complex value (Vector3 or Quaternion)
                    serializedValue += ":" + parts[++i];
                }

                //Debug.Log($"Parsing key-value pair: {key}={serializedValue}");

                Type valueType = DetermineType(serializedValue);
                object value = NetworkUtility.DeserializeValue(serializedValue, valueType);
                result.Data[key] = value;
            }
        }

        return result;
    }
    private static Type DetermineType(string serializedValue)
    {
        if (serializedValue.StartsWith("v3:")) return typeof(Vector3);
        if (serializedValue.StartsWith("q:")) return typeof(Quaternion);
        if (serializedValue.StartsWith("f:")) return typeof(float);
        if (serializedValue.StartsWith("i:")) return typeof(int);
        if (serializedValue.StartsWith("b:")) return typeof(bool);
        if (serializedValue.StartsWith("s:")) return typeof(string);
        if (serializedValue.StartsWith("json:")) return typeof(object);
        return typeof(string); // Default to string if type cannot be determined
    }

}
public static class NetworkMessageFactory
{
    public static NetworkMessage CreateRequestMultiplayerObjectId(string localPeerId, string prefabName, Vector3 position, Quaternion rotation)
    {
        return NetworkMessage.Create("LevelEventChannel", "RequestMultiplayerObjectId", localPeerId)
            .AddData("PrefabName", prefabName)
            .AddData("Position", position)
            .AddData("Rotation", rotation);
    }

    public static NetworkMessage CreateNewMultiplayerObject(string localPeerId, int objectId, string prefabName, Vector3 position, Quaternion rotation, string extraData)
    {
        return NetworkMessage.Create("LevelEventChannel", "NewMultiplayerObject", localPeerId, objectId)
            .AddData("PrefabName", prefabName)
            .AddData("Position", position)
            .AddData("Rotation", rotation)
            .AddData("ExtraData", extraData);
    }

    public static NetworkMessage CreateLevelChange(string localPeerId, string levelName)
    {
        return NetworkMessage.Create("LevelEventChannel", "ChangeLevel", localPeerId)
            .AddData("LevelName", levelName);
    }

    public static NetworkMessage CreatePlayerObjectMessage(int objectId, Vector3 position, Quaternion rotation, Quaternion turretRotation, string data)
    {
        return NetworkMessage.Create("LevelEventChannel", "SyncObject", LocalWebRTCEngine.Instance.LocalPeerId, objectId)
            .AddData("Position", position)
            .AddData("Rotation", rotation)
            .AddData("TurretRotation", turretRotation);

    }
}