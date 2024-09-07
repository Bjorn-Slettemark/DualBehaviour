using UnityEngine;
using System;
using System.Globalization;
using System.Linq;

public static class NetworkUtility
{
    public static Vector3 DeserializeVector3(string vectorString)
    {
        // Remove parentheses and split by comma
        string[] components = vectorString.Trim('(', ')').Split(',');
        if (components.Length != 3)
        {
            Debug.LogError($"Invalid Vector3 string format: {vectorString}");
            return Vector3.zero;
        }
        try
        {
            float x = float.Parse(components[0].Trim(), CultureInfo.InvariantCulture);
            float y = float.Parse(components[1].Trim(), CultureInfo.InvariantCulture);
            float z = float.Parse(components[2].Trim(), CultureInfo.InvariantCulture);
            return new Vector3(x, y, z);
        }
        catch (FormatException e)
        {
            Debug.LogError($"Error parsing Vector3 components: {e.Message}");
            return Vector3.zero;
        }
    }

    public static string SerializeVector3(Vector3 vector)
    {
        return $"({vector.x.ToString(CultureInfo.InvariantCulture)}, {vector.y.ToString(CultureInfo.InvariantCulture)}, {vector.z.ToString(CultureInfo.InvariantCulture)})";
    }

    public static Quaternion DeserializeQuaternion(string quaternionString)
    {
        // Remove parentheses and split by comma
        string[] components = quaternionString.Trim('(', ')').Split(',');
        if (components.Length != 4)
        {
            Debug.LogError($"Invalid Quaternion string format: {quaternionString}");
            return Quaternion.identity;
        }
        try
        {
            float x = float.Parse(components[0].Trim(), CultureInfo.InvariantCulture);
            float y = float.Parse(components[1].Trim(), CultureInfo.InvariantCulture);
            float z = float.Parse(components[2].Trim(), CultureInfo.InvariantCulture);
            float w = float.Parse(components[3].Trim(), CultureInfo.InvariantCulture);
            return new Quaternion(x, y, z, w);
        }
        catch (FormatException e)
        {
            Debug.LogError($"Error parsing Quaternion components: {e.Message}");
            return Quaternion.identity;
        }
    }

    public static string SerializeQuaternion(Quaternion quaternion)
    {
        return $"({quaternion.x.ToString(CultureInfo.InvariantCulture)}, {quaternion.y.ToString(CultureInfo.InvariantCulture)}, {quaternion.z.ToString(CultureInfo.InvariantCulture)}, {quaternion.w.ToString(CultureInfo.InvariantCulture)})";
    }

    public static bool TryDeserializeVector3(string vectorString, out Vector3 result)
    {
        result = Vector3.zero;
        string[] components = vectorString.Trim('(', ')').Split(',');
        if (components.Length != 3)
        {
            return false;
        }
        if (float.TryParse(components[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
            float.TryParse(components[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float y) &&
            float.TryParse(components[2].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float z))
        {
            result = new Vector3(x, y, z);
            return true;
        }
        return false;
    }

    public static bool TryDeserializeQuaternion(string quaternionString, out Quaternion result)
    {
        result = Quaternion.identity;
        string[] components = quaternionString.Trim('(', ')').Split(',');
        if (components.Length != 4)
        {
            return false;
        }
        if (float.TryParse(components[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
            float.TryParse(components[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float y) &&
            float.TryParse(components[2].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float z) &&
            float.TryParse(components[3].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float w))
        {
            result = new Quaternion(x, y, z, w);
            return true;
        }
        return false;
    }

    public static string[] SplitEventData(string eventData)
    {
        if (string.IsNullOrEmpty(eventData))
        {
            Debug.LogWarning("NetworkUtility: Attempted to split empty or null event data.");
            return new string[0];
        }

        // Split the event data by colon
        string[] parts = eventData.Split(':');

        // Trim each part to remove any leading or trailing whitespace
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i] = parts[i].Trim();
        }

        return parts;
    }
}