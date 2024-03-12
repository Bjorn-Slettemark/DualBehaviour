using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SaveLoadManager))]
public class SaveLoadManagerEditor : Editor
{
    private int selectedBlueprintIndex = 0; // Index of the selected blueprint in the dropdown
    private Texture2D iconTexture; // Custom icon for the header

    private void OnEnable()
    {
        // Load the custom icon, make sure the path matches your icon's location
        iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Icons/SaveLoadIcon.png");
    }

    public override void OnInspectorGUI()
    {
        SaveLoadManager script = (SaveLoadManager)target;

        // Header style configuration, similar to GameManagerEditor
        GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 18,
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white },
            fixedHeight = EditorGUIUtility.singleLineHeight * 2,
            padding = new RectOffset(0, 0, (int)(EditorGUIUtility.singleLineHeight * 0.5f), 0)
        };

        // Box style for the header
        EditorGUILayout.BeginVertical(GUI.skin.box);
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(iconTexture, GUILayout.Width(40), GUILayout.Height(40));
        GUILayout.Label("Save & Load Manager", titleStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();

        base.OnInspectorGUI();


        // Dropdown for selecting a GameSaveBlueprintSO
        if (script.gameSaveBlueprintSO != null && script.gameSaveBlueprintSO.Count > 0)
        {
            string[] blueprintNames = new string[script.gameSaveBlueprintSO.Count];
            for (int i = 0; i < script.gameSaveBlueprintSO.Count; i++)
            {
                blueprintNames[i] = script.gameSaveBlueprintSO[i] != null ? script.gameSaveBlueprintSO[i].name : "<None>";
            }

            EditorGUILayout.Space();
            selectedBlueprintIndex = EditorGUILayout.Popup("Select Blueprint", selectedBlueprintIndex, blueprintNames);

            // Save and Load buttons for the selected blueprint
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save Selected Blueprint"))
            {
                var selectedBlueprint = script.gameSaveBlueprintSO[selectedBlueprintIndex];
                if (selectedBlueprint != null)
                {
                    script.SaveBlueprint(selectedBlueprint);
                    Debug.Log($"Saved {selectedBlueprint.name}.");
                }
            }

            if (GUILayout.Button("Load Selected Blueprint"))
            {
                var selectedBlueprint = script.gameSaveBlueprintSO[selectedBlueprintIndex];
                if (selectedBlueprint != null)
                {
                    script.LoadBlueprint(selectedBlueprint);
                    Debug.Log($"Loaded {selectedBlueprint.name}.");
                }
            }
            GUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.LabelField("No Game Save Blueprints found.");
        }
    }
}
