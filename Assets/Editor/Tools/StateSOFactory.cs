

using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Compilation;
using System;
using System.Reflection;

public class StateSoFactory : EditorWindow
{
    string stateName = "NewState";

    [MenuItem("Tools/State SO Factory")]
    public static void ShowWindow()
    {
        GetWindow<StateSoFactory>("StateSO Factory");
    }

    private void OnEnable()
    {
        if (SessionState.GetBool("shouldCreateSO", false))
        {
            AssemblyReloadEvents.afterAssemblyReload += AfterAssemblyReload;
        }
    }

    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        stateName = EditorGUILayout.TextField("State Name", stateName);

        if (GUILayout.Button("Generate State and Create SO"))
        {
            GenerateStateScript(stateName);
            AssetDatabase.Refresh();
            SessionState.SetString("pendingSOName", stateName);
            SessionState.SetBool("shouldCreateSO", true);
            AssemblyReloadEvents.afterAssemblyReload += AfterAssemblyReload; // Subscribe only when needed
        }
    }
    static void GenerateStateScript(string name)
    {
        string scriptPath = $"Assets/Scripts/Engine/GameState/{name}StateSO.cs";
        string template = $@"
using UnityEngine;

[CreateAssetMenu(fileName = ""New {name}State"", menuName = ""Game States/{name}"")]
public class {name}StateSO : GameStateSO
{{
    public override void EnterState()
    {{
        base.EnterState();
        // Specific actions to initialize the state
    }}

    public override void ExitState()
    {{
        base.ExitState();
        // Clean up before leaving the state
    }}

    public override void StateUpdate()
    {{
        // Handle updates specific to the state
    }}
}}";
        // Ensure the directory exists
        string directoryPath = Path.GetDirectoryName(scriptPath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        File.WriteAllText(scriptPath, template);
    }

    private static void AfterAssemblyReload()
    {
        // Proceed to check if SO should be created
        if (SessionState.GetBool("shouldCreateSO", false))
        {
            CreateSOInstance(SessionState.GetString("pendingSOName", ""));
            SessionState.SetBool("shouldCreateSO", false);
            AssemblyReloadEvents.afterAssemblyReload -= AfterAssemblyReload; // Unsubscribe after use
        }
    }
    static void CreateSOInstance(string name)
    {
        string typeName = $"{name}StateSO";

        Debug.Log($"Looking up type: {typeName}");

        System.Type type = System.Reflection.Assembly.GetAssembly(typeof(GameStateSO)).GetType(typeName);
        if (type != null && type.IsSubclassOf(typeof(ScriptableObject)))
        {
            ScriptableObject asset = ScriptableObject.CreateInstance(type);

            // Assuming GameState is an enum or class with a static method/property to get its instance
            GameState gameState = GetGameStateByName(name); // Implement this method based on your GameState definition

            string stateNameWithoutSuffix = name.Replace("StateSO", ""); // Assuming the convention matches

            // Use reflection to set private or protected fields, if necessary
            type.GetField("gameState", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.SetValue(asset, gameState);
            type.GetField("stateName", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.SetValue(asset, stateNameWithoutSuffix);

            if (!AssetDatabase.IsValidFolder("Assets/SO"))
                AssetDatabase.CreateFolder("Assets", "SO");
            if (!AssetDatabase.IsValidFolder("Assets/SO/GameState"))
                AssetDatabase.CreateFolder("Assets/SO", "GameState");

            AssetDatabase.CreateAsset(asset, $"Assets/SO/GameState/{name}State.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(); // Ensure the AssetDatabase is refreshed

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            Debug.Log($"Created and assigned values to {name}.");
        }
        else
        {
            Debug.LogError($"Type not found: {typeName}. Ensure the script has compiled and the type name is correct.");
        }
    }

    // Example implementation if GameState is an enum
    // Adjust this method to fit your actual GameState implementation
    private static GameState GetGameStateByName(string name)
    {
        if (Enum.TryParse<GameState>(name, out GameState result))
        {
            return result;
        }
        return default; // Or a specific default value
    }

}