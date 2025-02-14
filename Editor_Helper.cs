using UnityEngine;
using UnityEditor;
using System.IO;
using RGSK;

public class Editor_Helper : MonoBehaviour 
{
    public static string version = "2.0.0 beta";
    public static string forumURL = "https://forum.unity3d.com/threads/racing-game-starter-kit-easily-create-racing-games.337366/";
    public static string youtubeURL = "https://www.youtube.com/playlist?list=PLdNzy1P_hi4SQ9qg9Lv1CNC9wa6zs3Va3";
    public static string onlineDocumentationURL = "https://www.dropbox.com/s/oe98mz89m0msf8y/ReadMe.pdf?dl=0";

    //---Asset Logo---
    public static Texture2D Logo()
    {
        return (Texture2D)Resources.Load("RGSK/Logo/RGSKLogo");
    }


    //--Update Project Settings--
    public static void UpdateProjectSettings()
    {
        string projectSettingsFolder = Application.dataPath.Replace("Assets", "ProjectSettings");
        string assetsFolder = Application.dataPath + "/Racing Game Starter Kit 2.0/Other/ProjectSettings";

        if (Directory.Exists(projectSettingsFolder))
        {
            //--FileUtil.ReplaceFile doesn't seem to work so I opted to Delete then Copy the .asset--

            //Update the InputManager
            if (File.Exists(assetsFolder + "/InputManager.asset"))
            {
                FileUtil.DeleteFileOrDirectory(projectSettingsFolder + "/InputManager.asset");
                FileUtil.CopyFileOrDirectory(assetsFolder + "/InputManager.asset", projectSettingsFolder + "/InputManager.asset");
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogWarning("Input settings could not be updated! Esnure the directory exists: " + assetsFolder + "/InputManager.asset");
                return;
            }

            //Update the Tags&Layers
            if (File.Exists(assetsFolder + "/TagManager.asset"))
            {
                FileUtil.DeleteFileOrDirectory(projectSettingsFolder + "/TagManager.asset");
                FileUtil.CopyFileOrDirectory(assetsFolder + "/TagManager.asset", projectSettingsFolder + "/TagManager.asset");
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogWarning("Tags & Layers could not be updated! Esnure the directory exists: " + assetsFolder + "/TagManager.asset");
                return;
            }

            //Update the Physics (Dynamic Manager)
            if (File.Exists(assetsFolder + "/DynamicsManager.asset"))
            {
                FileUtil.DeleteFileOrDirectory(projectSettingsFolder + "/DynamicsManager.asset");
                FileUtil.CopyFileOrDirectory(assetsFolder + "/DynamicsManager.asset", projectSettingsFolder + "/DynamicsManager.asset");
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogWarning("Physics settings could not be updated! Esnure the directory exists: " + assetsFolder + "/DynamicsManager.asset");
                return;
            }

            Debug.Log("Project Settings were successfully updated!");
        }
    }
}
