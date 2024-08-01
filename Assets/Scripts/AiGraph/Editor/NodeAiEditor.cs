using UnityEngine;
using UnityEditor;
using XNodeEditor;

[CustomNodeEditor(typeof(NodeAI))]
public class NodeAIEditor : NodeEditor
{
    private NodeAI nodeAI;

    private void OnEnable()
    {
        nodeAI = target as NodeAI;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            // Reset the node state when exiting play mode
            if (nodeAI != null)
            {
                nodeAI.ResetState();
                NodeEditorWindow.current?.Repaint();
            }
        }
    }
    public override void OnBodyGUI()
    {
        serializedObject.Update();

        NodeAI nodeAI = target as NodeAI;

        // Draw a green border if the node is active
        if (nodeAI.IsActive)
        {
            // Get the size of the node from the nodeSizes dictionary
            if (NodeEditorWindow.current.nodeSizes.TryGetValue(nodeAI, out Vector2 nodeSize))
            {
                // Create a rect using the node's size
                Rect borderRect = new Rect(0, 0, nodeSize.x, nodeSize.y);

                // Expand the rect slightly to create a border effect
                borderRect.x -= 2;
                borderRect.y -= 2;
                borderRect.width += 4;
                borderRect.height += 4;

                // Draw the border
                EditorGUI.DrawRect(borderRect, new Color(0, 1, 0, 0.5f)); // Semi-transparent green
            }
        }

        // Call the base OnBodyGUI to ensure standard node drawing
        base.OnBodyGUI();

        serializedObject.ApplyModifiedProperties();

        // Force repaint if the GUI has changed
        if (GUI.changed)
        {
            EditorUtility.SetDirty(nodeAI);
            EditorUtility.SetDirty(nodeAI.graph);
            NodeEditorWindow.current?.Repaint();
        }
    }
}