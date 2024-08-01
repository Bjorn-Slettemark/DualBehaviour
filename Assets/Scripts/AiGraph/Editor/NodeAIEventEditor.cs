using static UnityEngine.GraphicsBuffer;
using static XNodeEditor.NodeEditor;
using UnityEditor;
using XNodeEditor;

[CustomNodeEditor(typeof(NodeAIEvent))]
public class NodeAIEventEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        serializedObject.Update();

        NodeAIEvent nodeAIEvent = target as NodeAIEvent;

        // Draw default inspector properties
        base.OnBodyGUI();


        serializedObject.ApplyModifiedProperties();
    }
}

[CustomNodeEditor(typeof(NodeAILocalEvent))]
public class NodeAILocalEventEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        serializedObject.Update();

        NodeAILocalEvent nodeAILocalEvent = target as NodeAILocalEvent;

        // Draw default inspector properties
        base.OnBodyGUI();

 
        serializedObject.ApplyModifiedProperties();
    }
}