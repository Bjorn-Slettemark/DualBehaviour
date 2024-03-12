using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(EventChannelManager))]
public class EventChannelManagerEditor : Editor
{
    private Vector2 scrollPosition;
    private int selectedChannelIndexForRaisingEvent = 0;
    private string eventNameToRaise = "";
    private int channelMask = -1;
    private List<string> channelNames = new List<string>();
    private bool showEventHistory = true;
    private bool showDebugSection = false;

    private Texture2D iconTexture;
    private Texture2D debugIconTexture;

    private void OnEnable()
    {
        iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Icons/EventChannelIcon.png");
        debugIconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Icons/DebugIcon.png");
    }

    public override void OnInspectorGUI()
    {
        GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 18,
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white },
            fixedHeight = EditorGUIUtility.singleLineHeight * 2,
            padding = new RectOffset(0, 0, (int)(EditorGUIUtility.singleLineHeight * 0.5f), 0)
        };

        EditorGUILayout.BeginVertical(GUI.skin.box);
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(iconTexture, GUILayout.Width(40), GUILayout.Height(40));
        GUILayout.Label("Event Channel Manager", titleStyle);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();

        base.OnInspectorGUI();

        EventChannelManager manager = (EventChannelManager)target;

        if (manager.EventChannels != null)
        {
            channelNames = manager.EventChannels.Select(channel => channel.name).ToList();
        }

        if (channelNames.Any())
        {
            showDebugSection = EditorGUILayout.Foldout(showDebugSection, new GUIContent(" Debug", debugIconTexture), true);
            if (showDebugSection)
            {
                EditorGUILayout.Space();
            EditorGUILayout.LabelField("Raise Event", EditorStyles.boldLabel);
            selectedChannelIndexForRaisingEvent = EditorGUILayout.Popup("Event Channel:", selectedChannelIndexForRaisingEvent, channelNames.ToArray());
            eventNameToRaise = EditorGUILayout.TextField("Event Name:", eventNameToRaise);

            if (GUILayout.Button("Raise Event", GUILayout.ExpandWidth(true)))
            {
                if (!string.IsNullOrEmpty(eventNameToRaise) && selectedChannelIndexForRaisingEvent >= 0 && selectedChannelIndexForRaisingEvent < channelNames.Count)
                {
                    string channelName = channelNames[selectedChannelIndexForRaisingEvent];
                    manager.RaiseEvent(channelName, eventNameToRaise);
                    eventNameToRaise = "";
                }
            }

            EditorGUILayout.Space();


                channelMask = EditorGUILayout.MaskField("Filter Channels:", channelMask, channelNames.ToArray());

                EditorGUILayout.Space();

                showEventHistory = EditorGUILayout.Foldout(showEventHistory, "Event History", true);
                if (showEventHistory)
                {
                    DrawEventHistory(manager);
                }

                if (GUILayout.Button("Clear Event History"))
                {
                    manager.ClearEventHistory();
                }
            }
        }
    }

    private void DrawEventHistory(EventChannelManager manager)
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
        foreach (var history in manager.eventHistory)
        {
            int channelIndex = channelNames.IndexOf(history.ChannelName);
            if (channelIndex != -1)
            {
                bool isChannelSelected = (channelMask == -1) || ((channelMask & (1 << channelIndex)) != 0);
                if (isChannelSelected)
                {
                    string timeOnly = history.Timestamp.ToString("HH:mm:ss");
                    EditorGUILayout.LabelField($"Time: {timeOnly}, Channel: {history.ChannelName}, Event: {history.EventName}");
                }
            }
        }
        EditorGUILayout.EndScrollView();
    }
}
