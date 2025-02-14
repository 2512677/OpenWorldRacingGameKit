using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using I2.Loc;

namespace RGSK
{
    public class PostRacePanel : MonoBehaviour
    {
        private RaceResultsPanel raceResultsPanel;
        private ChampionshipResultsPanel championshipResultsPanel;
        private OtherResultsPanel otherRaceResultsPanel;
        private RaceRewardsPanel raceRewardsPanel;
        public Text raceEndTimer;
        private string currentPanel;

        [Header("Buttons")]
        public Button continueButton;
        public Button watchReplayButton;
        public Button restartRaceButton;


        void Start()
        {
            AddButtonListeners();
            FindPanels();

            //Start by showing the race results
            ShowPanel("RaceResults");

            //Clear the end race timer text
            if (raceEndTimer != null)
            {
                raceEndTimer.text = string.Empty;
            }
        }


        void FindPanels()
        {
            raceResultsPanel = GetComponentInChildren<RaceResultsPanel>(true);
            championshipResultsPanel = GetComponentInChildren<ChampionshipResultsPanel>(true);
            otherRaceResultsPanel = GetComponentInChildren<OtherResultsPanel>(true);
            raceRewardsPanel = GetComponentInChildren<RaceRewardsPanel>(true);
        }


        void Update()
        {
            if (RaceManager.instance && raceEndTimer != null)
            {
                // Если таймер окончания гонки запущен и гонка еще не завершена
                if (RaceManager.instance.endRaceTimerStarted && !RaceManager.instance.raceFinished)
                {
                    // Берём локализованную строку для "Race End:"
                    string localizedRaceEnd = LocalizationManager.GetTranslation("RaceMessages/RaceEndTimer");
                    // Если ключ не найден или пуст — используем значение "Race End: "
                    if (string.IsNullOrEmpty(localizedRaceEnd))
                    {
                        localizedRaceEnd = "Race End: ";
                    }

                    // Обновляем текст, добавляя количество оставшихся секунд
                    raceEndTimer.text = localizedRaceEnd + (int)RaceManager.instance.raceEndTimer;
                }
                else
                {
                    raceEndTimer.text = string.Empty;
                }
            }
        }


        void Continue()
        {
            switch (currentPanel)
            {
                case "RaceResults":
                    //Check if a championship is ongoing
                    if (ChampionshipManager.instance != null)
                    {
                        //Add the championship points
                        RaceManager.instance.UpdateChampionshipPositions();

                        //Show the championship results
                        ShowPanel("ChampionshipResults");
                    }
                    else
                    {
                        //Check if a race rewards panel exists
                        if (raceRewardsPanel != null)
                        {
                            ShowPanel("RaceRewards");
                        }
                        else
                        {
                            //Load the menu scene
                            LoadMenuScene();
                        }
                    }
                    break;

                case "ChampionshipResults":
                    if (!ChampionshipManager.instance.IsFinalRound())
                    {
                        //Load the next round of the championship
                        ChampionshipManager.instance.LoadNextRound();
                    }
                    else
                    {
                        //Check if a race rewards panel exists
                        if (raceRewardsPanel != null)
                        {
                            ShowPanel("RaceRewards");
                        }
                        else
                        {
                            //Load the menu scene
                            LoadMenuScene();
                        }
                    }
                    break;

                case "RaceRewards":
                    LoadMenuScene();
                    break;
            }
        }


        void ShowPanel(string panel)
        {
            currentPanel = panel;

            switch (panel)
            {
                case "RaceResults":
                    if (raceResultsPanel != null)
                    {
                        raceResultsPanel.gameObject.SetActive(RaceManager.instance.raceType != RaceType.Drift && RaceManager.instance.raceType != RaceType.TimeAttack);
                    }

                    if (otherRaceResultsPanel != null)
                    {
                        otherRaceResultsPanel.gameObject.SetActive(RaceManager.instance.raceType == RaceType.Drift || RaceManager.instance.raceType == RaceType.TimeAttack);
                    }

                    if (championshipResultsPanel != null)
                    {
                        championshipResultsPanel.gameObject.SetActive(false);
                    }

                    if (raceRewardsPanel != null)
                    {
                        raceRewardsPanel.gameObject.SetActive(false);
                    }
                    break;


                case "ChampionshipResults":
                    if (raceResultsPanel != null)
                    {
                        raceResultsPanel.gameObject.SetActive(false);
                    }

                    if (otherRaceResultsPanel != null)
                    {
                        otherRaceResultsPanel.gameObject.SetActive(false);
                    }

                    if (championshipResultsPanel != null)
                    {
                        championshipResultsPanel.gameObject.SetActive(true);
                    }

                    if (raceRewardsPanel != null)
                    {
                        raceRewardsPanel.gameObject.SetActive(false);
                    }
                    break;

                case "RaceRewards":
                    if (raceResultsPanel != null)
                    {
                        raceResultsPanel.gameObject.SetActive(false);
                    }

                    if (otherRaceResultsPanel != null)
                    {
                        otherRaceResultsPanel.gameObject.SetActive(false);
                    }

                    if (championshipResultsPanel != null)
                    {
                        championshipResultsPanel.gameObject.SetActive(false);
                    }

                    if (raceRewardsPanel != null)
                    {
                        raceRewardsPanel.gameObject.SetActive(true);

                        // Получаем позицию игрока из RaceManager
                        int playerPosition = RaceManager.instance.playerStatistics.Position;

                        // Обновляем панель наград с использованием позиции игрока (если необходимо)
                        // raceRewardsPanel.UpdateRewardsUI(playerPosition);
                    }
                    break;
            }
        }


        void LoadMenuScene()
        {
            if (SceneController.instance != null)
            {
                SceneController.instance.ExitToMenu();
            }
            else
            {
                Debug.LogWarning("The scene could not be loaded because a SceneController was not found in the scene.");
            }
        }


        void AddButtonListeners()
        {
            if (continueButton != null)
            {
                continueButton.onClick.AddListener(delegate { Continue(); });
            }

            if (watchReplayButton != null)
            {
                if (ReplayManager.instance != null)
                    watchReplayButton.onClick.AddListener(delegate { ReplayManager.instance.WatchReplay(); });
            }

            if (restartRaceButton != null)
            {
                if (SceneController.instance != null)
                    restartRaceButton.onClick.AddListener(delegate { SceneController.instance.ReloadScene(); });
            }
        }
    }
}
