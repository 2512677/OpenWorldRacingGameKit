using UnityEngine;
using UnityEditor;

public class Window_Welcome : Editor 
{
	[InitializeOnLoad]
    public class RGSKWelcomeWindow
    {
        static RGSKWelcomeWindow()
        {
            string version = Editor_Helper.version;

            if(!EditorPrefs.HasKey("RGSK_" + version))
            {
                EditorPrefs.SetInt("RGSK_" + version, 1);

                string message = version.Contains("beta") ? 
                    "Thank you for taking part in beta!\n\nPlease note that the beta version is not complete and many things will change in the final product.\n\nPlease report any bugs and suggestions directly to me or in the forum thread.\n\nStart by updating your project settings from the 'Racing Game Starter Kit/Integrations/Other/Project Settings' folder" :
                                        
                    "Thank you for purchasing the Racing Game Starter Kit. Please read the documentation to get started.";

                EditorUtility.DisplayDialog("Racing Game Starter Kit (v" + version.ToLower() +")", message, "OK");
            }
        }
    }
}
