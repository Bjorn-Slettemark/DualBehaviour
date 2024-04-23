using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;

[CustomEditor(typeof(LocalEventHandler))]
public class LocalEventHandlerEditor : Editor
{
    private Vector2 scrollPosition;
    private int selectedChannelIndexForRaisingEvent = 0;
    private string eventNameToRaise = "";
    private int channelFilterMask = 0;
    private List<string> channelNames = new List<string>();
    private bool showEventHistory = true;
    private bool showDebugSection = true;

    private List<bool> channelToggles = new List<bool>();

    private void OnEnable()
    {
        RefreshChannelNames();
        InitializeChannelToggles();
    }

    private void InitializeChannelToggles()
    {
        channelToggles = new List<bool>(new bool[channelNames.Count]);
    }

    private void DrawEventFilteringSection()
    {
        EditorGUILayout.Space();
        GUILayout.Label("Filter Channels", EditorStyles.boldLabel);

        // Adding the master toggle for select all or deselect all
        bool allSelected = !channelToggles.Contains(false); // Check if all toggles are true
        bool newAllSelected = EditorGUILayout.ToggleLeft("Select All / Deselect All", allSelected);
        if (newAllSelected != allSelected)
        {
            for (int i = 0; i < channelToggles.Count; i++)
            {
                channelToggles[i] = newAllSelected;
            }
        }

        int numColumns = 2; // Number of columns you want
        int numRows = (int)Math.Ceiling((double)channelNames.Count / numColumns);

        for (int row = 0; row < numRows; row++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int col = 0; col < numColumns; col++)
            {
                int index = row * numColumns + col;
                if (index < channelNames.Count)
                {
                    bool previousValue = channelToggles[index];
                    channelToggles[index] = EditorGUILayout.Toggle(channelNames[index], channelToggles[index], GUILayout.ExpandWidth(true));
                    if (channelToggles[index] != previousValue) // If toggle changes, update the master toggle
                    {
                        allSelected = !channelToggles.Contains(false);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }



    public override void OnInspectorGUI()
    {
        DrawHeader();
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        //if (GUILayout.Button("Refresh Channel List", GUILayout.Height(30)))
        //{
        //    RefreshChannelNames();
        //}

        if (channelNames.Count > 0)
        {
            DrawDebugTools();
        }
        else
        {
            EditorGUILayout.HelpBox("No channels are currently available. Please ensure channels are properly initialized in the LocalEventHandler.", MessageType.Warning);
        }
    }

    private void DrawHeader()
    {
        GUIStyle headerStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 16,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.UpperCenter,
            margin = new RectOffset(0, 0, 10, 10)
        };

        EditorGUILayout.LabelField("Local Event Handler", headerStyle);
    }

    private void DrawDebugTools()
    {
        showDebugSection = EditorGUILayout.Foldout(showDebugSection, "Event Handling Tools", true);
        if (showDebugSection)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            DrawEventRaisingSection();
            DrawEventFilteringSection();
            DrawEventHistorySection();
            EditorGUILayout.EndVertical();
        }
    }

    private void DrawEventRaisingSection()
    {
        EditorGUILayout.LabelField("Raise Events", EditorStyles.boldLabel);
        selectedChannelIndexForRaisingEvent = EditorGUILayout.Popup("Select Channel", selectedChannelIndexForRaisingEvent, channelNames.ToArray());
        eventNameToRaise = EditorGUILayout.TextField("Event Name", eventNameToRaise);

        if (GUILayout.Button("Raise Event"))
        {
            if (selectedChannelIndexForRaisingEvent >= 0 && selectedChannelIndexForRaisingEvent < channelNames.Count)
            {
                LocalEventHandler handler = (LocalEventHandler)target;
                handler.Publish((LocalEventChannel)Enum.Parse(typeof(LocalEventChannel), channelNames[selectedChannelIndexForRaisingEvent]), eventNameToRaise, "Raised from Editor");
                Debug.Log($"Raised event '{eventNameToRaise}' on channel '{channelNames[selectedChannelIndexForRaisingEvent]}'");
                eventNameToRaise = ""; // Clear the event name after publishing
                GUI.FocusControl(null); // Remove focus from text field
                Debug.Log("Event name cleared"); // Confirm that the line is executed
            }
            else
            {
                Debug.LogError("Invalid channel index selected for event raising.");
            }
        }
        // Display the current value of eventNameToRaise to debug
        EditorGUILayout.LabelField("Current Event Name: " + eventNameToRaise);
    }




    private void DrawEventHistorySection()
    {
        LocalEventHandler handler = target as LocalEventHandler;
        if (handler == null)
        {
            EditorGUILayout.HelpBox("Event handler target is not available.", MessageType.Error);
            return;
        }

        EditorGUILayout.Space();
        showEventHistory = EditorGUILayout.Foldout(showEventHistory, "View Event History");
        if (showEventHistory)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(150));
            foreach (var history in handler.GetEventHistory())
            {
                LocalEventChannel channel = (LocalEventChannel)Enum.Parse(typeof(LocalEventChannel), history.ChannelName);
                int channelIndex = Array.IndexOf(Enum.GetValues(typeof(LocalEventChannel)), channel);
                if (channelIndex < channelToggles.Count && channelToggles[channelIndex])
                {
                    string timeOnly = history.Timestamp.ToString("HH:mm:ss");
                    EditorGUILayout.LabelField($"{timeOnly}, {history.ChannelName}, {history.EventName}, Sender: {history.SenderName}");
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }


    private void RefreshChannelNames()
    {
        channelNames = Enum.GetValues(typeof(LocalEventChannel))
            .Cast<LocalEventChannel>()
            .Select(c => c.ToString())
            .ToList();
        channelFilterMask = (1 << channelNames.Count) - 1; // Reset the mask to include all channels
    }


}
