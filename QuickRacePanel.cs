using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using I2.Loc;

namespace RGSK
{
    public class QuickRacePanel : MonoBehaviour
    {
        // Данные треков
        public TrackData[] trackData;

        [Header("Выбор трассы")]
        public Image trackImage;            // Изображение трассы
        public Image trackMinimap;          // Мини-карта трассы
        public Text trackName;              // Название трассы
        public Text trackLength;            // Длина трассы
        public Text trackDescription;       // Описание трассы

        [Header("Настройки гонки")]
        public Text raceTypeText;           // Тип гонки

        [Header("Настройки параметров гонки")]
        public Text lapText;                // Количество кругов
        public Slider lapSlider;            // Слайдер для кругов
        public Text opponentText;           // Количество соперников
        public Slider opponentSlider;       // Слайдер для соперников
        public Text difficultyText;         // Сложность соперников
        public Slider difficultySlider;     // Слайдер для сложности

        [Header("Многошаговый процесс")]
        public GameObject raceModePanel;    // Панель выбора режима гонки
        public GameObject trackSelectionPanel; // Панель выбора трассы
        public GameObject raceSettingsPanel;   // Панель настроек гонки

        [Space(10)]
        public Button nextStepButton;       // Кнопка для перехода к следующему шагу
        public Button previousStepButton;   // Кнопка для возврата на предыдущий шаг

        private int currentStep = 0;        // Текущий шаг

        // Значения гонки
        private int raceTrackIndex;
        private int raceTypeIndex;
        private int laps = 3;                   // Начальное количество кругов
        private int opponentCount = 3;          // Начальное количество соперников
        private int difficultyLevel = 0;        // Начальный уровень сложности
        private string sceneToLoad;

        [Header("Прочие настройки")]
        public int maxLaps = 100;               // Максимальное количество кругов
        public int maxOpponents = 9;            // Максимальное количество соперников

        [Header("Назад")]
        public Button backButton;               // Кнопка возврата
        public GameObject previousPanel;        // Предыдущая панель

        void Start()
        {
            // Добавление слушателей для кнопок
            if (nextStepButton != null)
            {
                nextStepButton.onClick.AddListener(delegate { GoToNextStep(); });
            }
            if (previousStepButton != null)
            {
                previousStepButton.onClick.AddListener(delegate { GoToPreviousStep(); });
            }

            if (backButton != null)
            {
                backButton.onClick.AddListener(delegate { Back(); });
            }

            // Инициализация первого шага
            UpdateStepUI();
            UpdateSliders();
        }

        /// <summary>
        /// Переход к следующему шагу.
        /// </summary>
        public void GoToNextStep()
        {
            currentStep++;
            if (currentStep > 2)
                currentStep = 2;

            UpdateStepUI();
        }

        public void GoToPreviousStep()
        {
            currentStep--;
            if (currentStep < 0)
                currentStep = 0;

            UpdateStepUI();
        }





        /// <summary>
        /// Обновляет UI в зависимости от текущего шага.
        /// </summary>
        private void UpdateStepUI()
        {
            if (raceModePanel != null)
                raceModePanel.SetActive(currentStep == 0);

            if (trackSelectionPanel != null)
                trackSelectionPanel.SetActive(currentStep == 1);

            if (raceSettingsPanel != null)
                raceSettingsPanel.SetActive(currentStep == 2);
        }




        /// <summary>
        /// Обновляет значения слайдеров на панели настроек гонки.
        /// </summary>
        private void UpdateSliders()
        {
            if (lapSlider != null)
            {
                lapSlider.minValue = 1;
                lapSlider.maxValue = maxLaps;
                lapSlider.value = laps;
                lapSlider.onValueChanged.AddListener(delegate { UpdateLaps(); });
            }

            if (opponentSlider != null)
            {
                opponentSlider.minValue = 0;
                opponentSlider.maxValue = maxOpponents;
                opponentSlider.value = opponentCount;
                opponentSlider.onValueChanged.AddListener(delegate { UpdateOpponents(); });
            }

            if (difficultySlider != null)
            {
                difficultySlider.minValue = 0;
                difficultySlider.maxValue = 2; // Лёгкая, Средняя, Сложная
                difficultySlider.value = difficultyLevel;
                difficultySlider.onValueChanged.AddListener(delegate { UpdateDifficulty(); });
            }
        }

        public void SelectTrack(int trackIndex)
        {
            raceTrackIndex = Mathf.Clamp(trackIndex, 0, trackData.Length - 1);
            sceneToLoad = trackData[raceTrackIndex].scene;
        }


        public void SelectRaceType(int raceTypeIndex)
        {
            // Убедимся, что индекс в пределах допустимого диапазона
            raceTypeIndex = Mathf.Clamp(raceTypeIndex, 0, Enum.GetNames(typeof(RaceType)).Length - 1);

            // Обновляем текущий тип гонки
            this.raceTypeIndex = raceTypeIndex;

            // Обновляем UI
            if (raceTypeText != null)
            {
                RaceType raceType = (RaceType)raceTypeIndex;
                raceTypeText.text = GetRaceTypeName(raceType);
            }
        }

        private string GetRaceTypeName(RaceType raceType)
        {
            switch (raceType)
            {
                case RaceType.Circuit: return "Круговая гонка";
                case RaceType.Sprint: return "Спринт";
                case RaceType.LapKnockout: return "Гонка с выбывания";
                case RaceType.Checkpoint: return "Гонка с чекпоинтами";
                case RaceType.SpeedTrap: return "Ловушка скорости";
                case RaceType.Elimination: return "Гонка с устранением участников";
                case RaceType.TimeAttack: return "Временная атака";
                case RaceType.Endurance: return "Гонка на выносливость";
                case RaceType.Drift: return "Дрифт";
                default: return "Unknown";

            }
        }

        /// <summary>
        /// Обновляет количество кругов.
        /// </summary>
        private void UpdateLaps()
        {
            laps = (int)lapSlider.value;
            if (lapText != null)
            {
                lapText.text = laps.ToString();
            }
        }

        /// <summary>
        /// Обновляет количество соперников.
        /// </summary>
        private void UpdateOpponents()
        {
            opponentCount = (int)opponentSlider.value;
            if (opponentText != null)
            {
                opponentText.text = opponentCount.ToString();
            }
        }

        /// <summary>
        /// Обновляет уровень сложности.
        /// </summary>


        private void UpdateDifficulty()
        {
            difficultyLevel = (int)difficultySlider.value;

            if (difficultyText != null)
            {
                AIDifficultyLevel difficulty = (AIDifficultyLevel)difficultyLevel;
                difficultyText.text = difficulty.GetLocalizedString();
            }
        }





        /// <summary>
        /// Сохраняет параметры гонки и начинает гонку.
        /// </summary>
        public void StartRace()
        {
            Debug.Log($"Запуск гонки. Сцена: {sceneToLoad}, Тип гонки: {raceTypeIndex}, Круги: {laps}, Соперники: {opponentCount}, Сложность: {difficultyLevel}");

            if (string.IsNullOrEmpty(sceneToLoad))
            {
                Debug.LogWarning("Трасса не выбрана или имя сцены отсутствует!");
                return;
            }

            if (SceneController.instance != null)
            {
                PlayerPrefs.SetInt("RaceType", raceTypeIndex);
                PlayerPrefs.SetInt("LapCount", laps);
                PlayerPrefs.SetInt("OpponentCount", opponentCount);
                PlayerPrefs.SetInt("AIDifficulty", difficultyLevel);

                SceneController.instance.LoadScene(sceneToLoad);
            }
            else
            {
                Debug.LogWarning("SceneController не найден в текущей сцене!");
            }
        }



        /// <summary>
        /// Возвращается на предыдущую панель.
        /// </summary>
        public void Back()
        {
            if (previousPanel != null)
            {
                gameObject.SetActive(false);
                previousPanel.SetActive(true);
            }
        }
    }
}

