using UnityEngine;
using UnityEditor;
using System.Collections;
using RGSK;

[CustomEditor(typeof(RaceTrackTriggers))]
public class Editor_RaceTrackTriggers : Editor
{
    RaceTrackTriggers _target;

    SerializedProperty triggerType;
    SerializedProperty autoSelectTrigger;

    void OnEnable()
    {
        _target = (RaceTrackTriggers)target;

        triggerType = serializedObject.FindProperty("triggerType");
        autoSelectTrigger = serializedObject.FindProperty("autoSelectTrigger");
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.HelpBox("Use 'Shift + Left Mouse Button' to place new race triggers", MessageType.Info);

        EditorGUILayout.PropertyField(triggerType);
        EditorGUILayout.PropertyField(autoSelectTrigger);

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
                    GameObject newObject = new GameObject(_target.triggerType.ToString());
                    Undo.RegisterCreatedObjectUndo(newObject, "Created Race Trigger");
                    newObject.AddComponent<BoxCollider>();
                    newObject.GetComponent<BoxCollider>().size = new Vector3(30, 10, 1);
                    newObject.GetComponent<BoxCollider>().isTrigger = true;
                    newObject.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                    newObject.AddComponent<RaceTrigger>();
                    newObject.GetComponent<RaceTrigger>().triggerType = _target.triggerType;
                    newObject.transform.position = hit.point + new Vector3(0, 5, 0);
                    newObject.transform.parent = _target.transform;

                    if (_target.autoSelectTrigger)
                    {
                        //Select the trigger
                        Selection.activeObject = newObject;
                        
                        //Reset hot control
                        GUIUtility.hotControl = 0;
                    }
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
