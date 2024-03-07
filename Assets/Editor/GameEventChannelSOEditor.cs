using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(GameEventChannelSO))]
public class GameEventChannelSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameEventChannelSO script = (GameEventChannelSO)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Registered Listeners", EditorStyles.boldLabel);

        if (GUILayout.Button("Refresh Listeners"))
        {
            script.DebugRegisteredListeners();
        }

        // Optional: Display listeners directly in the editor
        // This requires the GameEventChannelSO to have a method to return listeners
        var listeners = script.GetListenersInfo(); // This method needs to be implemented in GameEventChannelSO
        foreach (var listener in listeners)
        {
            EditorGUILayout.LabelField($"Listener: {listener.Listener.GetType()}, Event: {listener.EventName}");
        }
    }
}
