using UnityEngine;
using System.Collections;
using UnityEditor;
using RGSK;

[CustomEditor(typeof(TrackLayout))]
public class Editor_TrackBoundary : Editor
{
    TrackLayout _target;
    GUIStyle centerLabelStyle;

    SerializedProperty defaultTrackWidth;
    SerializedProperty loop;
    SerializedProperty showSegments;
    SerializedProperty showNodeIndexes;
    SerializedProperty visible;
    SerializedProperty color;

    //AI Target Projection
    SerializedProperty minTargetDistance;
    SerializedProperty maxTargetDistance;

    //AI Speed Calulation
    SerializedProperty minSpeed;
    SerializedProperty maxSpeed;
    SerializedProperty cautionAngle;


    void OnEnable()
    {
        _target = (TrackLayout)target;

        centerLabelStyle = new GUIStyle();
        centerLabelStyle.fontStyle = FontStyle.Normal;
        centerLabelStyle.normal.textColor = Color.white;
        centerLabelStyle.alignment = TextAnchor.MiddleCenter;

        //The track boundary shouldnt be smooth route
        _target.smoothRoute = false;

        defaultTrackWidth = serializedObject.FindProperty("defaultTrackWidth");
        loop = serializedObject.FindProperty("loop");
        showSegments = serializedObject.FindProperty("showSegments");
        showNodeIndexes = serializedObject.FindProperty("showNodeIndexes");
        visible = serializedObject.FindProperty("visible");
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

        EditorGUILayout.HelpBox("Используйте «Shift + Левая кнопка мыши» для размещения узлов пути.", MessageType.Info);

        EditorGUILayout.PropertyField(visible);
        GUILayout.Space(10);
        EditorGUILayout.PropertyField(defaultTrackWidth);
        EditorGUILayout.PropertyField(loop);
        EditorGUILayout.PropertyField(showSegments);
        EditorGUILayout.PropertyField(showNodeIndexes);
        EditorGUILayout.PropertyField(color);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Длина пути: " + Helper.MeterToKm(_target.length).ToString("F2") + " Км / " + Helper.MeterToMiles(_target.length).ToString("F2") + " Миля");

        //Target Projection
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Проекция цели ИИ", centerLabelStyle);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(minTargetDistance);
        EditorGUILayout.PropertyField(maxTargetDistance);

        //Node Speed Calculation
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Расчет скорости узла ИИ", centerLabelStyle);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(minSpeed);
        EditorGUILayout.PropertyField(maxSpeed);
        EditorGUILayout.PropertyField(cautionAngle);

        if (GUILayout.Button("Рассчитать скорость узла"))
        {
            _target.CalculateNodeSpeeds();
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Разное", centerLabelStyle);

        if (GUILayout.Button("Добавить дочерние узлы"))
        {
            _target.GetChildNodes();
            for (int i = 0; i < _target.nodes.Count; i++)
            {
                _target.nodes[i].gameObject.AddComponent<TrackNode>();
            }
        }

        if (GUILayout.Button("Отрегулируйте вращение узла"))
        {
            _target.AdjustNodeRotation();
        }

        if (GUILayout.Button("Удалить все узлы"))
        {
            if (_target.transform.childCount == 0) return;

            foreach (Transform node in _target.nodes.ToArray())
            {
                Undo.RecordObject(_target, "Removed Track Node");
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
                string info = i + " (" + (int)_target.transform.GetChild(i).GetComponent<RacingLineNode>().targetSpeed + " KPH)";
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
                    TrackNode newNode = new GameObject("Node ").AddComponent<TrackNode>();
                    newNode.gameObject.AddComponent<RacingLineNode>();
                    Undo.RegisterCreatedObjectUndo(newNode.gameObject, "Созданный узел трека");

                    newNode.transform.position = hit.point + new Vector3(0, 0.25f, 0);
                    newNode.transform.parent = _target.transform;
                    newNode.name += _target.transform.childCount;
                    newNode.leftWidth = _target.defaultTrackWidth / 2;
                    newNode.rightWidth = _target.defaultTrackWidth / 2;

                    Undo.RecordObject(_target, "Добавлен узел трека");

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
