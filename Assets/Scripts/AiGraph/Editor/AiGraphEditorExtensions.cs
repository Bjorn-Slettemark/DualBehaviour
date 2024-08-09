using UnityEngine;
using UnityEditor;
using XNodeEditor;

[InitializeOnLoad]
public static class AiGraphEditorExtension
{
    static AiGraphEditorExtension()
    {
        EditorApplication.update += Update;
    }

    private static void Update()
    {
        if (NodeEditorWindow.current != null && NodeEditorWindow.current.graph != null)
        {
            NodeEditorWindow.current.Repaint();
        }
    }
}