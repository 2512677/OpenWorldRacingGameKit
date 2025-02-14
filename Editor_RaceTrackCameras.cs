using UnityEngine;
using System.Collections;
using UnityEditor;
using RGSK;

[CustomEditor(typeof(RaceTrackCameras))]
public class Editor_RaceTrackCameras : Editor
{
    RaceTrackCameras _target;

    SerializedProperty offset;
    SerializedProperty gizmoColor;
    SerializedProperty visible;


    void OnEnable()
    {
        _target = (RaceTrackCameras)target;

        offset = serializedObject.FindProperty("offset");
        gizmoColor = serializedObject.FindProperty("gizmoColor");
        visible = serializedObject.FindProperty("visible");
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.HelpBox("Use 'Shift + Left Mouse Button' to place track cameras", MessageType.Info);

        EditorGUILayout.PropertyField(visible);
        GUILayout.Space(10);

        EditorGUILayout.PropertyField(offset);
        EditorGUILayout.PropertyField(gizmoColor);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Total Cameras: " + _target.transform.childCount);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        if (GUILayout.Button("Delete All"))
        {
            foreach (Transform child in _target.transform.GetComponentsInChildren<Transform>())
            {
                if (child != _target.transform)
                {
                    Undo.DestroyObjectImmediate(child.gameObject);
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
            //Make sure we cant click anything else while in layout mode
            GUIUtility.hotControl = 1;

            //Cast a ray to check where was clicked
            Ray sceneRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(sceneRay, out hit))
            {
                if (!hit.collider.isTrigger)
                {
                    GameObject newTrackCam = new GameObject("Track Camera ");
                    Undo.RegisterCreatedObjectUndo(newTrackCam, "Created Track Camera");

                    newTrackCam.transform.position = hit.point + new Vector3(0, _target.offset, 0);
                    newTrackCam.transform.parent = _target.transform;
                    newTrackCam.name += _target.transform.childCount;

                    newTrackCam.AddComponent<TrackCamera>();
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
