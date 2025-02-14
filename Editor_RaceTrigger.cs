using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using RGSK;

[CustomEditor(typeof(RaceTrigger))]
public class Editor_RaceTrigger : Editor 
{
    RaceTrigger _target;

    SerializedProperty triggerType;
    SerializedProperty nearestTrackNode;
    SerializedProperty index;
    SerializedProperty addedTime;

    void OnEnable()
    {
        _target = (RaceTrigger)target;

        triggerType = serializedObject.FindProperty("triggerType");
        nearestTrackNode = serializedObject.FindProperty("nearestTrackNode");
        index = serializedObject.FindProperty("index");
        addedTime = serializedObject.FindProperty("addedTime");
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(triggerType);

        if (_target.triggerType != RaceTriggerType.FinishLine)
        {
            if (_target.nearestTrackNode == null)
            {
                EditorGUILayout.HelpBox("A nearest node has not been assigned! This will allow the trigger to be passed backwards.", MessageType.Warning);
            }

            EditorGUILayout.HelpBox("The nearest node to this trigger. The node must be behind this trigger.", MessageType.Info);
            EditorGUILayout.PropertyField(nearestTrackNode);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            EditorGUILayout.HelpBox("This number should be unique for each trigger type.", MessageType.Info);
            EditorGUILayout.PropertyField(index);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        if (_target.triggerType == RaceTriggerType.Checkpoint)
        {
            EditorGUILayout.HelpBox("The amount of time added when a racer passes this checkpoint.", MessageType.Info);
            EditorGUILayout.PropertyField(addedTime);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
