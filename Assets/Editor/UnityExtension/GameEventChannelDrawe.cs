using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class GameEventChannelDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Set the color tint for the drawer's background
        Color originalColor = GUI.color;
        Color tint = new Color(1.0f, 1.0f, 0.8f, 0.4f); // Light yellow tint
        GUI.color = tint;

        // Draw a rectangle with the tint color
        EditorGUI.DrawRect(position, tint);

        // Reset the color to the original
        GUI.color = originalColor;

        // Check if the target object is an EventChannelManager
        if (property.serializedObject.targetObject is EventChannelManager)
        {
            // If it is, simply draw the default property field
            EditorGUI.PropertyField(position, property, label, true);
        }
        else
        {
            // Otherwise, continue with the existing drawing code for dropdown
            GameEventChannelSO currentChannel = property.objectReferenceValue as GameEventChannelSO;
            if (EventChannelManager.Instance != null)
            {
                List<GameEventChannelSO> channels = EventChannelManager.Instance.AllChannels;
                int currentIndex = channels != null ? channels.IndexOf(currentChannel) : -1;
                List<string> channelNames = channels != null ? channels.ConvertAll(ch => ch.name) : new List<string>();

                int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, channelNames.ToArray());

                if (selectedIndex >= 0 && selectedIndex < channels.Count)
                {
                    property.objectReferenceValue = channels[selectedIndex];
                }
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "EventChannelManager not initialized");
            }
        }

        EditorGUI.EndProperty();
    }
}
