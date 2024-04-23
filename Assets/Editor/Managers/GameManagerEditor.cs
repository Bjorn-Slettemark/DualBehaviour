using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;

[CustomEditor(typeof(GameStateManager))]
public class GameManagerEditor : Editor
{
    private int selectedStateIndex = 0;
    private string[] stateNames;
    private GameStateSO[] gameStates;
    private Texture2D iconTexture;
    private Texture2D debugIconTexture; // Additional icon specifically for the debug section
    private bool showDebugSection = false; // For toggling visibility of the Debug section

    private void OnEnable()
    {
        iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Icons/GameManagerIcon.png");
        debugIconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Icons/DebugIcon.png"); // Ensure this path matches your DebugIcon location
    }

    public override void OnInspectorGUI()
    {
        GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 18,
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white },
            fixedHeight = EditorGUIUtility.singleLineHeight * 2,
            padding = new RectOffset(0, 0, (int)(EditorGUIUtility.singleLineHeight * 0.5f), 0)
        };

        EditorGUILayout.BeginVertical(GUI.skin.box);
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(iconTexture, GUILayout.Width(40), GUILayout.Height(40));
        GUILayout.Label("Game State Manager", titleStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();

        base.OnInspectorGUI();
        GameStateManager manager = (GameStateManager)target;

        // Assuming GameState enum names should be displayed instead of GameStateSO names:
        if (stateNames == null || stateNames.Length == 0)
        {
            InitializeGameStates(manager); // Now passing manager to use its stateDictionary for initialization
        }
        // Create a blue texture for the background
        Texture2D blueTexture = new Texture2D(1, 1);
        blueTexture.SetPixel(0, 0, new Color(55f / 255f, 90f / 255f, 90f / 255f, 255));
        blueTexture.Apply();

        // Create a GUIStyle that uses the blue texture
        GUIStyle blueBackgroundStyle = new GUIStyle();
        blueBackgroundStyle.normal.background = blueTexture;

        showDebugSection = EditorGUILayout.Foldout(showDebugSection, new GUIContent(" Debug", debugIconTexture), true);
        if (showDebugSection)
        {
            EditorGUILayout.BeginVertical(blueBackgroundStyle); // Begin with blue background
            if (manager.CurrentState != null)
            {
                EditorGUILayout.LabelField("Current State:", manager.CurrentState.name);
            }
            else
            {
                EditorGUILayout.LabelField("No current state set.");
            }

            // Adjust popup to use enum names
            selectedStateIndex = EditorGUILayout.Popup("Change State", selectedStateIndex, stateNames);

            if (GUILayout.Button("Change to Selected State", GUILayout.ExpandWidth(true)))
            {
                if (selectedStateIndex >= 0 && selectedStateIndex < stateNames.Length) // Ensure within range of stateNames
                {
                    GameState selectedEnumState = (GameState)Enum.GetValues(typeof(GameState)).GetValue(selectedStateIndex);
                    manager.ChangeState(selectedEnumState); // Change state using the selected enum
                }
            }

            EditorGUILayout.EndVertical();
        }
    }
private void InitializeGameStates(GameStateManager manager)
{
    if (manager != null)
    {
        // Directly use AvailableGameStates to initialize stateNames
        stateNames = manager.AvailableGameStates.Select(state => state.ToString()).ToArray();
    }
}

    private void InitializeGameStates()
    {
        string[] guids = AssetDatabase.FindAssets("t:GameStateSO");
        gameStates = new GameStateSO[guids.Length];
        stateNames = new string[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            gameStates[i] = AssetDatabase.LoadAssetAtPath<GameStateSO>(path);
            stateNames[i] = gameStates[i].name;
        }
    }
}
