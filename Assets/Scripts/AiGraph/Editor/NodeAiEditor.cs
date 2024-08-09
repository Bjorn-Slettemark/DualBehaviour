using UnityEngine;
using UnityEditor;
using XNodeEditor;

[CustomNodeEditor(typeof(NodeAI))]
public class NodeAIEditor : NodeEditor
{
    private static GUIStyle editorLabelStyle;

    public override void OnHeaderGUI()
    {
        NodeAI node = target as NodeAI;
        string title = node.name;
        if (string.IsNullOrEmpty(title)) title = node.GetType().Name;

        // Create a new style based on the default node header style
        GUIStyle headerStyle = new GUIStyle(NodeEditorResources.styles.nodeHeader);

        // Set the text color based on the node's active state
        headerStyle.normal.textColor = node.IsActive ? Color.blue : Color.red;

        // Draw the header with the custom style
        GUILayout.Label(title, headerStyle, GUILayout.Height(30));
    }

    public override Color GetTint()
    {
        NodeAI node = target as NodeAI;
        // You can customize the tint color based on the node's state
        return node.IsActive ? new Color(0.8f, 0.8f, 1f) : base.GetTint();
    }

    public override void OnBodyGUI()
    {
            if (editorLabelStyle == null) editorLabelStyle = new GUIStyle(EditorStyles.label);
        EditorStyles.label.normal.textColor = Color.white;
        base.OnBodyGUI();
        EditorStyles.label.normal = editorLabelStyle.normal;
    }
}