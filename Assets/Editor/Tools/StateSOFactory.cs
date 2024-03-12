using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Compilation;

public class StateSoFactory : EditorWindow
{
    string stateName = "NewState";
    private static bool shouldCreateSO = false;
    private static string pendingSOName = "";

    [MenuItem("Tools/State SO Factory")]
    public static void ShowWindow()
    {
        GetWindow<StateSoFactory>("StateSO Factory");
    }

    private void OnEnable()
    {
        AssemblyReloadEvents.afterAssemblyReload += AfterAssemblyReload;
    }

    private void OnDisable()
    {
        AssemblyReloadEvents.afterAssemblyReload -= AfterAssemblyReload;
    }

    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        stateName = EditorGUILayout.TextField("State Name", stateName);

        if (GUILayout.Button("Generate State and Create SO"))
        {
            GenerateStateScript(stateName);
            AssetDatabase.Refresh();
            pendingSOName = stateName;
            shouldCreateSO = true;
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
        if (shouldCreateSO)
        {
            CreateSOInstance(pendingSOName);
            shouldCreateSO = false;
            pendingSOName = "";
        }
    }

    static void CreateSOInstance(string name)
    {
        string typeName = $"{name}StateSO";

        System.Type type = System.Reflection.Assembly.GetAssembly(typeof(GameStateSO)).GetType(typeName);
        if (type != null && type.IsSubclassOf(typeof(ScriptableObject)))
        {
            ScriptableObject asset = ScriptableObject.CreateInstance(type);
            if (!AssetDatabase.IsValidFolder("Assets/SO/GameState"))
                AssetDatabase.CreateFolder("Assets", "SO/GameState");
            AssetDatabase.CreateAsset(asset, $"Assets/SO/GameState/{name}State.asset");
            AssetDatabase.SaveAssets();


            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
        else
        {
            Debug.LogError("Type not found. Make sure the script compiles correctly and try again.");
        }
    }
}
