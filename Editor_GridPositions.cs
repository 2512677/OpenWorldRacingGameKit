using UnityEngine;
using System.Collections;
using UnityEditor;
using RGSK;

[CustomEditor(typeof(GridPositions))]
public class Editor_GridPositions : Editor 
{
    GridPositions _target;

    SerializedProperty offset;
    SerializedProperty gizmoColor;
    SerializedProperty visible;

    void OnEnable()
    {
        _target = (GridPositions)target;

        offset = serializedObject.FindProperty("offset");
        gizmoColor = serializedObject.FindProperty("gizmoColor");
        visible = serializedObject.FindProperty("visible");
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.HelpBox("Use 'Shift + Left Mouse Button' to place new grid positions", MessageType.Info);

        EditorGUILayout.PropertyField(visible);
        GUILayout.Space(10);

        EditorGUILayout.PropertyField(offset);
        EditorGUILayout.PropertyField(gizmoColor);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Grid Positions: " + _target.transform.childCount);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        if (GUILayout.Button("Delete All"))
        {
            foreach (Transform sp in _target.transform.GetComponentsInChildren<Transform>())
            {
                if (sp != _target.transform)
                {
                    Undo.DestroyObjectImmediate(sp.gameObject);
                }
            }
        }

         serializedObject.ApplyModifiedProperties();
    }


    void OnSceneGUI()
    {
        SceneViewRaycast();
    }


    void SceneViewRaycast()
    {
        Event e = Event.current;

        if (e.button == 0 && e.type == EventType.MouseDown && e.shift)
        {
            //Make sure we cant click anything else
            GUIUtility.hotControl = 1;

            //Cast a ray to check where was clicked
            Ray sceneRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(sceneRay, out hit))
            {
                if (!hit.collider.isTrigger)
                {
                    //Create a new spawnpoint at the clicked postition
                    GameObject newSpawnpoint = new GameObject("P");
                    Undo.RegisterCreatedObjectUndo(newSpawnpoint, "Created Grid Position");

                    newSpawnpoint.transform.position = hit.point + new Vector3(0, _target.offset, 0);
                    newSpawnpoint.transform.eulerAngles = _target.defaultRotation;
                    newSpawnpoint.transform.parent = _target.transform;
                    newSpawnpoint.name += _target.transform.childCount;
                }
            }
        }

        if (e.button == 0 && e.type == EventType.MouseUp)
        {
            //Reset hot control on mouse up
            GUIUtility.hotControl = 0;
        }
    }
}
