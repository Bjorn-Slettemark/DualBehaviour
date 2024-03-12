using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(LevelLoaderManager))]
public class LevelLoaderManagerEditor : Editor
{
    private int selectedLevelIndex = -1; // Initialized to -1 to indicate no selection
    private string[] levelNames;
    private GameLevelSO[] gameLevels;

    // Icon textures
    private Texture2D iconTexture;
    private Texture2D debugIconTexture; // Debug icon

    private bool showDebugSection = false; // Toggle for showing the debug section

    void OnEnable()
    {
        string[] guids = AssetDatabase.FindAssets("t:GameLevelSO");
        gameLevels = new GameLevelSO[guids.Length];
        levelNames = new string[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            gameLevels[i] = AssetDatabase.LoadAssetAtPath<GameLevelSO>(path);
            levelNames[i] = gameLevels[i].name;
        }

        // Load the icon textures
        iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Icons/LevelLoaderIcon.png");
        debugIconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Icons/DebugIcon.png"); // Ensure this path matches your DebugIcon location
    }

    public override void OnInspectorGUI()
    {
        // Title style with double height and centered alignment
        GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 18,
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white },
            fixedHeight = EditorGUIUtility.singleLineHeight * 2,
            padding = new RectOffset(0, 0, (int)(EditorGUIUtility.singleLineHeight * 0.5f), 0)
        };

        // Draw the title with the icon
        EditorGUILayout.BeginVertical(GUI.skin.box);
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(iconTexture, GUILayout.Width(40), GUILayout.Height(40));
        GUILayout.Label("Level Loader Manager", titleStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();

        DrawDefaultInspector(); // Draws the default inspector

        // Debug section with DebugIcon
        EditorGUILayout.Space();
        showDebugSection = EditorGUILayout.Foldout(showDebugSection, new GUIContent(" Debug", debugIconTexture), true);
        if (showDebugSection)
        {
            // Inside the debug section
            if (gameLevels == null || gameLevels.Length == 0)
            {
                EditorGUILayout.HelpBox("No GameLevelSO instances found.", MessageType.Warning);
                return;
            }

            selectedLevelIndex = EditorGUILayout.Popup("Select Level", selectedLevelIndex, levelNames);

            if (GUILayout.Button("Load Selected Level") && selectedLevelIndex >= 0)
            {
                if (Application.isPlaying)
                {
                    LevelLoaderManager manager = (LevelLoaderManager)target;
                    if (manager != null)
                    {
                        GameLevelSO selectedLevel = gameLevels[selectedLevelIndex];
                        manager.LoadLevel(selectedLevel);
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("Not in Play Mode", "You need to be in Play Mode to load levels.", "OK");
                }
            }
        }
    }
}
