using static XNodeEditor.NodeGraphEditor;
using UnityEditor.PackageManager.UI;
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
}