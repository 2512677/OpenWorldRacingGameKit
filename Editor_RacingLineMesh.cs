using UnityEngine;
using System.Collections;
using UnityEditor;
using RGSK;

[CustomEditor(typeof(RacingLineMesh))]
public class Editor_RacingLineMesh : Editor
{
    RacingLineMesh _target;

    void OnEnable()
    {
        _target = (RacingLineMesh)target;
    }


    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("Please note that the Racing Line Mesh v" + Editor_Helper.version + " is not optimized.", MessageType.Warning);

        DrawDefaultInspector();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        if (GUILayout.Button("Generate Race Line Mesh"))
        {
            _target.GenerateRaceLine();
        }

        //if (GUILayout.Button("Combine Race Line Mesh"))
        //{
            //_target.CombineMeshes();
        //}

        if (GUILayout.Button("Delete Race Line Mesh"))
        {
            _target.DeleteRaceLine();
        }
    }
}
