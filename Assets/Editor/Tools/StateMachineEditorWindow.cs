using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class NodeGraphEditorWindow : EditorWindow
{
    private float gridSize = 50f; // Size of each grid cell
    private Vector2 scrollPosition = Vector2.zero;
    private List<Vector2> nodePositions = new List<Vector2>(); // List to store node positions

    [MenuItem("Window/Node Graph Editor")]
    public static void ShowWindow()
    {
        GetWindow<NodeGraphEditorWindow>("Node Graph Editor");
    }

    private void OnGUI()
    {
        DrawGrid();
        DrawNodes(); // Draw nodes after drawing grid

        ProcessEvents(Event.current);

        if (GUI.changed)
        {
            Repaint();
        }
    }

    private void DrawGrid()
    {
        int width = Mathf.CeilToInt(position.width / gridSize);
        int height = Mathf.CeilToInt(position.height / gridSize);

        Handles.color = Color.gray;

        // Draw vertical grid lines
        for (int x = 0; x < width; x++)
        {
            Handles.DrawLine(new Vector3(x * gridSize, 0, 0), new Vector3(x * gridSize, position.height, 0));
        }

        // Draw horizontal grid lines
        for (int y = 0; y < height; y++)
        {
            Handles.DrawLine(new Vector3(0, y * gridSize, 0), new Vector3(position.width, y * gridSize, 0));
        }
    }

    private void DrawNodes()
    {
        Handles.color = Color.blue;

        foreach (Vector2 position in nodePositions)
        {
            Handles.DrawSolidDisc(position, Vector3.forward, 10f); // Draw a circle at node position
        }
    }

    private void ProcessEvents(Event e)
    {
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            CreateNode(e.mousePosition);
        }
    }

    private void CreateNode(Vector2 position)
    {
        Vector2 roundedPosition = RoundPositionToGrid(position);
        nodePositions.Add(roundedPosition); // Add node position to the list
        Debug.Log("Node created at position: " + roundedPosition);
    }

    private Vector2 RoundPositionToGrid(Vector2 position)
    {
        float x = Mathf.Round(position.x / gridSize) * gridSize;
        float y = Mathf.Round(position.y / gridSize) * gridSize;
        return new Vector2(x, y);
    }
}
