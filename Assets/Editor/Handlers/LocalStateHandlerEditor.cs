using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(LocalStateHandler))]
public class LocalStateHandlerEditor : Editor
{
    // This will hold the index of the currently selected state in the dropdown
    private int selectedStateIndex = 0;
    private string[] stateNames;
    private bool showControlPanel = false; // Start with the control panel collapsed

    private void OnEnable()
    {
        // Populate the state names array with names of the states from the LocalState enum
        stateNames = Enum.GetNames(typeof(LocalState));
    }
    private void DrawHeader()
    {
        GUIStyle headerStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 16,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.UpperCenter,
            margin = new RectOffset(0, 0, 10, 10)
        };

        EditorGUILayout.LabelField("Local State Handler", headerStyle);
    }

    public override void OnInspectorGUI()
    {
        DrawHeader();
        base.OnInspectorGUI(); // Draw the default inspector
        
        LocalStateHandler handler = (LocalStateHandler)target;

        // Foldout to toggle visibility of the control panel
        showControlPanel = EditorGUILayout.Foldout(showControlPanel, "Control Panel", true);

        if (showControlPanel)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.LabelField("Current State: " + handler.GetCurrentState().ToString(), EditorStyles.boldLabel);

                // Dropdown menu to select a state
                selectedStateIndex = EditorGUILayout.Popup("Select State", selectedStateIndex, stateNames);

                // Button to switch to the selected state
                if (GUILayout.Button("Switch State"))
                {
                    LocalState newState = (LocalState)Enum.GetValues(typeof(LocalState)).GetValue(selectedStateIndex);
                    handler.ChangeState(newState);
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}
