using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using XNodeEditor;

[CustomNodeEditor(typeof(NodeAIState))]
public class NodeAiStateEditor : NodeEditor
{
    private NodeAIState stateNode;
    private string[] stateTypeNames;
    private Type[] stateTypes;
    private int selectedIndex = -1;
    private Editor stateLogicEditor;
    private bool stateTypesInitialized = false;

    public override void OnCreate()
    {
        base.OnCreate();
        stateNode = target as NodeAIState;
        if (stateNode == null)
        {
            Debug.LogError("Failed to initialize AiStateNode");
        }
        InitializeStateTypes();
    }

    public override void OnBodyGUI()
    {

        if (stateNode == null)
        {
            stateNode = target as NodeAIState;
            if (stateNode == null)
            {
                EditorGUILayout.HelpBox("Failed to initialize AiStateNode", MessageType.Error);
                return;
            }
        }

        serializedObject.Update();
        NodeEditorGUILayout.PortField(stateNode.GetPort("input"));
        NodeEditorGUILayout.PortField(stateNode.GetPort("output"));
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("State Logic", EditorStyles.boldLabel);

        if (!stateTypesInitialized)
            InitializeStateTypes();

        EditorGUI.BeginChangeCheck();
        selectedIndex = EditorGUILayout.Popup("Select State Logic", selectedIndex, stateTypeNames);
        if (EditorGUI.EndChangeCheck() && selectedIndex != -1)
        {
            SetStateLogic(stateTypes[selectedIndex]);
        }

        if (stateNode.AiState != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("State Logic Properties", EditorStyles.boldLabel);

            if (stateLogicEditor == null || stateLogicEditor.target != stateNode.AiState)
            {
                if (stateLogicEditor != null)
                    UnityEngine.Object.DestroyImmediate(stateLogicEditor);
                stateLogicEditor = Editor.CreateEditor(stateNode.AiState);
            }

            stateLogicEditor.OnInspectorGUI();
        }

        serializedObject.ApplyModifiedProperties();



        if (GUI.changed)
        {
            EditorUtility.SetDirty(stateNode);
            EditorUtility.SetDirty(stateNode.graph);
        }
    }

    private void InitializeStateTypes()
    {
        if (stateTypesInitialized) return;

        try
        {
            stateTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type != null && typeof(AiStateSO).IsAssignableFrom(type) && !type.IsAbstract)
                .ToArray();

            stateTypeNames = stateTypes.Select(t => t.Name).ToArray();

            if (stateNode.AiState != null)
            {
                selectedIndex = Array.FindIndex(stateTypes, t => t == stateNode.AiState.GetType());
            }
            else if (selectedIndex == -1 && stateTypes.Length > 0)
            {
                selectedIndex = 0;
            }

            stateTypesInitialized = true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error initializing state types: {e.Message}");
            stateTypesInitialized = false;
        }
    }

    private void SetStateLogic(Type stateType)
    {
        if (stateNode.AiState != null && stateNode.AiState.GetType() == stateType)
            return;

        Undo.RecordObject(stateNode, "Change State Logic");

        if (stateNode.AiState != null)
        {
            Undo.DestroyObjectImmediate(stateNode.AiState);
        }

        AiStateSO newStateLogic = ScriptableObject.CreateInstance(stateType) as AiStateSO;
        newStateLogic.name = stateType.Name;

        stateNode.SetAiState(newStateLogic);

        AssetDatabase.AddObjectToAsset(newStateLogic, stateNode.graph);

        EditorUtility.SetDirty(stateNode);
        EditorUtility.SetDirty(stateNode.graph);

        AssetDatabase.SaveAssets();
    }

    private void OnDisable()
    {
        if (stateLogicEditor != null)
        {
            UnityEngine.Object.DestroyImmediate(stateLogicEditor);
        }
    }
}