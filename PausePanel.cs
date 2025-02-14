using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace RGSK
{
    public class PausePanel : MonoBehaviour
    {
        [Header("Кнопки")]
        public Button resumeButton;
        public Button restartButton;
        public Button watchReplayButton;
        public Button exitButton;

        void Start()
        {
            AddButtonListeners();
        }


        void AddButtonListeners()
        {
            if (resumeButton != null)
            {
                if (RaceManager.instance)
                    resumeButton.onClick.AddListener(delegate { RaceManager.instance.UnPause(); });
            }

            if (restartButton != null)
            {
                if (SceneController.instance)
                    restartButton.onClick.AddListener(delegate { SceneController.instance.ReloadScene(); });
            }

            if (watchReplayButton != null)
            {
                if (ReplayManager.instance != null)
                    watchReplayButton.onClick.AddListener(delegate { ReplayManager.instance.WatchReplay(); });
            }

            if (exitButton != null)
            {
                if (SceneController.instance)
                    exitButton.onClick.AddListener(delegate { SceneController.instance.ExitToMenu(); });
            }
        }
    }
}
