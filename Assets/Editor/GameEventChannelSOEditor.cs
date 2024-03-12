using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(GameEventChannelSO))]
public class GameEventChannelSOEditor : Editor
{
    private bool listenersFoldout = true; // To toggle visibility of listeners section

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameEventChannelSO script = (GameEventChannelSO)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Refresh Listeners"))
        {
            // This button now just acts as a way to refresh the inspector to show updated listener info
            // Assuming DebugRegisteredListeners logs to the console
            script.DebugRegisteredListeners();
            Repaint(); // Refreshes the inspector
        }

        // Optionally, directly display listeners in the editor
        EditorGUILayout.Space();
        listenersFoldout = EditorGUILayout.Foldout(listenersFoldout, "Registered Listeners", true);
        if (listenersFoldout)
        {
            var listenersInfo = script.GetListenersInfo();
            if (listenersInfo.Count > 0)
            {
                foreach (var listenerInfo in listenersInfo)
                {
                    EditorGUILayout.LabelField($"Event: {listenerInfo.EventName}, Listener: {listenerInfo.Listener.Target.GetType()}.{listenerInfo.Listener.Method.Name}");
                }
            }
            else
            {
                EditorGUILayout.LabelField("No listeners registered.");
            }
        }
    }
}
