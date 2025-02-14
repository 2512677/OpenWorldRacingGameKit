using UnityEngine;
using UnityEditor;

public class Window_About : EditorWindow
{
    GUIStyle centerLabelStyle;

    void OnEnable()
    {
        centerLabelStyle = new GUIStyle();
        centerLabelStyle.fontStyle = FontStyle.Normal;
        centerLabelStyle.normal.textColor = Color.white;
        centerLabelStyle.alignment = TextAnchor.MiddleCenter;
    }


    void OnGUI()
    {
        //-- Asset Logo --
        if (Editor_Helper.Logo())
        {
            GUILayout.Label(Editor_Helper.Logo(), GUILayout.Height(50));
        }

        EditorGUILayout.Space();

        //-- Version Number --
        GUI.Label(new Rect(180, 0, 50, 20), "v" + Editor_Helper.version, centerLabelStyle);

        EditorGUILayout.Space();

        //-- Forum thread --
        if (GUILayout.Button("Forum Thread"))
        {
            Application.OpenURL(Editor_Helper.forumURL);
        }

        //-- YT tutorials --
        if (GUILayout.Button("Video Tutorials"))
        {
            Application.OpenURL(Editor_Helper.youtubeURL);
        }

        //-- Online docs --
        if (GUILayout.Button("Online Documentation"))
        {
            Application.OpenURL(Editor_Helper.onlineDocumentationURL);
        }
    }
}
