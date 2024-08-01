using UnityEngine;
using UnityEditor;
using XNodeEditor;

[CustomNodeGraphEditor(typeof(AiGraph))]
public class AIGraphEditor : NodeGraphEditor
{
    public override void OnOpen()
    {
        base.OnOpen();
        window.titleContent.text = "AI Graph";
    }

    public override string GetNodeMenuName(System.Type type)
    {
        if (type == typeof(NodeAIEvent)) return "Events/Event";
        if (type == typeof(NodeAIState)) return "AI/State";
        if (type == typeof(NodeAILocalEvent)) return "Events/LocalEvent";

        return null; // Don't show in context menu
    }

    public override void OnGUI()
    {
        base.OnGUI();

        // Draw the info bar
        DrawInfoBar();
    }

    private void DrawInfoBar()
    {
        AiGraph graph = target as AiGraph;
        if (graph == null) return;

        float infoBarHeight = 24; // Increased height for better visibility
        Rect infoBarRect = new Rect(0, window.position.height - infoBarHeight, window.position.width, infoBarHeight);

        // Draw the grey background
        EditorGUI.DrawRect(infoBarRect, new Color(0.3f, 0.3f, 0.3f, 1)); // Dark grey color

        GUILayout.BeginArea(infoBarRect);
        GUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace(); // Push the priority display to the right

        // Display current priority
        GUIStyle style1 = new GUIStyle(EditorStyles.label);
        style1.alignment = TextAnchor.MiddleRight;
        style1.fontSize = 12; // Slightly larger font
        style1.fontStyle = FontStyle.Bold; // Make it bold
        GUILayout.Label($"Current Priority: ", style1, GUILayout.Height(infoBarHeight));

        // Display current priority
        GUIStyle style = new GUIStyle(EditorStyles.label);
        style.alignment = TextAnchor.MiddleRight;
        style.normal.textColor = GetPriorityColor(graph.CurrentPriorityLevel);
        style.fontSize = 12; // Slightly larger font
        style.fontStyle = FontStyle.Bold; // Make it bold

        GUILayout.Label($"{(PriorityLevel)graph.CurrentPriorityLevel}", style, GUILayout.Height(infoBarHeight));

        // Add some padding to the right
        GUILayout.Space(10);

        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        // Force repaint to update the info bar
        window.Repaint();
    }

    private Color GetPriorityColor(int priorityLevel)
    {
        float t = (priorityLevel - 1) / 4f; // Normalize to 0-1 range
        return Color.Lerp(Color.red, Color.green, t);
    }
}