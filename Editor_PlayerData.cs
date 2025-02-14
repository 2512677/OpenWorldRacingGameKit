using UnityEngine;
using System.Collections;
using UnityEditor;
using RGSK;

[CustomEditor(typeof(PlayerData))]
public class Editor_PlayerData : Editor
{
    PlayerData _target;

    void OnEnable()
    {
        _target = (PlayerData)target;    
    }


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        if(GUILayout.Button("Reset Data"))
        {
            _target.ResetData();
        }

        //if (GUILayout.Button("Delete Data"))
        //{
        //    _target.DeleteSaveFile();
        //}
    }
}
