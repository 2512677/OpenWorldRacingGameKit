using UnityEngine;
using System.Collections;
using UnityEditor;
using RGSK;

[CustomEditor(typeof(RacingLine))]
public class Editor_RacingLine : Editor
{
    RacingLine _target;
    GUIStyle centerLabelStyle;

    SerializedProperty loop;
    SerializedProperty smoothRoute;
    SerializedProperty showNodeIndexes;
    SerializedProperty visible;
    SerializedProperty smoothness;
    SerializedProperty color;

    //Target Projection
    SerializedProperty minTargetDistance;
    SerializedProperty maxTargetDistance;

    //Speed Calulation
    SerializedProperty minSpeed;
    SerializedProperty maxSpeed;
    SerializedProperty cautionAngle;


    void OnEnable()
    {
        _target = (RacingLine)target;

        centerLabelStyle = new GUIStyle();
        centerLabelStyle.fontStyle = FontStyle.Normal;
        centerLabelStyle.normal.textColor = Color.white;

        centerLabelStyle.alignment = TextAnchor.MiddleCenter;
        loop = serializedObject.FindProperty("loop");
        smoothRoute = serializedObject.FindProperty("smoothRoute");
        visible = serializedObject.FindProperty("visible");
        showNodeIndexes = serializedObject.FindProperty("showNodeIndexes");
        smoothness = serializedObject.FindProperty("smoothness");
        color = serializedObject.FindProperty("color");

        minTargetDistance = serializedObject.FindProperty("minTargetDistance");
        maxTargetDistance = serializedObject.FindProperty("maxTargetDistance");

        minSpeed = serializedObject.FindProperty("minSpeed");
        maxSpeed = serializedObject.FindProperty("maxSpeed");
        cautionAngle = serializedObject.FindProperty("cautionAngle");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.HelpBox("Use 'Shift + Left Mouse Button' to place racing line nodes", MessageType.Info);

        EditorGUILayout.PropertyField(visible);
        GUILayout.Space(10);
        EditorGUILayout.PropertyField(loop);
        EditorGUILayout.PropertyField(smoothRoute);
        EditorGUILayout.PropertyField(showNodeIndexes);
        EditorGUILayout.PropertyField(smoothness);
        EditorGUILayout.PropertyField(color);

        //Target Projection
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Target Projection", centerLabelStyle);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(minTargetDistance);
        EditorGUILayout.PropertyField(maxTargetDistance);

        //Node Speed Calculation
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Node Speed Calculation", centerLabelStyle);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(minSpeed);
        EditorGUILayout.PropertyField(maxSpeed);
        EditorGUILayout.PropertyField(cautionAngle);

        if (GUILayout.Button("Calculate Node Speeds"))
        {
            _target.CalculateNodeSpeeds();
        }

        //Other
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Space(5);

        if (GUILayout.Button("Add Child Nodes"))
        {
            _target.GetChildNodes();
            for (int i = 0; i < _target.nodes.Count; i++)
            {
                _target.nodes[i].gameObject.AddComponent<RacingLineNode>();
            }
        }

        if (GUILayout.Button("Delete All Nodes"))
        {
            if (_target.transform.childCount == 0) return;

            foreach (Transform node in _target.nodes.ToArray())
            {
                Undo.RecordObject(_target, "Removed RacingLine Node");
                _target.nodes.Remove(node);
                Undo.DestroyObjectImmediate(node.gameObject);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }


    void OnSceneGUI()
    {
        SceneViewRaycast();

        if (_target.showNodeIndexes)
        {
            for (int i = 0; i < _target.transform.childCount; i++)
            {
                string info = i + " (" + (int)_target.transform.GetChild(i).GetComponent<RacingLineNode>().targetSpeed +" KPH)";
                Handles.Label(_target.nodes[i].position, info);
            }
        }
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
                    //Create a new node at the clicked postition
                    RacingLineNode newNode = new GameObject("Node ").AddComponent<RacingLineNode>();
                    Undo.RegisterCreatedObjectUndo(newNode.gameObject, "Created RacingLine Node");

                    newNode.transform.position = hit.point + new Vector3(0, 0.25f, 0);
                    newNode.transform.parent = _target.transform;
                    newNode.name += _target.transform.childCount;

                    Undo.RecordObject(_target, "Added RacingLine Node");

                    _target.nodes.Add(newNode.transform);
                    _target.AdjustNodeRotation();
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
