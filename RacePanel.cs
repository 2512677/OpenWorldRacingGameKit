using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using I2.Loc;

namespace RGSK
{
    public class RacePanel : MonoBehaviour
    {
        public static RacePanel instance;
        public UIPositionDisplayMode positionDisplayMode;
        public UILapDisplayMode lapDisplayMode;

        [Header("Самое необходимое")]
        public Text position;
        public Text lap;
        public Text lapTime;
        public Text lastLapTime;
        public Text bestLapTime;
        public Text totalTime;
        public Text raceInfoText;
        public Image raceInfoImage;
        public Text sectorTime;
        public Text vehicleAhead;
        public Text vehicleBehind;
        public Text raceProgress;
        public Text distanceDriven;
        public Text personalBestLapTime;
        public Text raceLimitTimer;
        public Text raceOverText;
        public Text raceEndTimer;
        public GameObject rearViewMirror;
        public GameObject wrongwayPanel;

        [Header("Обратный отсчет")]
        public Text countdownText;
        [SerializeField] private string startTextKey = "RacePanel/StartText"; // Ключ локализации

        public Image countdownImage;
        public CountdownImage[] countdownSprites;

        [Header("Разное")]
        public float raceInfoDuration = 2;
        public Color validLaptimeColor = Color.white;
        public Color invalidLaptimeColor = Color.red;

        [Header("Localization Keys")]
        [SerializeField] private string positionPrefixKey = "RacePanel/PositionPrefix";
        [SerializeField] private string lapPrefixKey = "RacePanel/LapPrefix";
        [SerializeField] private string lapTimePrefixKey = "RacePanel/LapTimePrefix";
        [SerializeField] private string lastLapTimePrefixKey = "RacePanel/LastLapTimePrefix";
        [SerializeField] private string bestTimePrefixKey = "RacePanel/BestTimePrefix";
        [SerializeField] private string totalTimePrefixKey = "RacePanel/TotalTimePrefix";
        [SerializeField] private string vehicleAheadPrefixKey = "RacePanel/VehicleAheadPrefix";
        [SerializeField] private string vehicleBehindPrefixKey = "RacePanel/VehicleBehindPrefix";
        [SerializeField] private string lapProgressPrefixKey = "RacePanel/LapProgressPrefix";
        [SerializeField] private string distanceDrivenPrefixKey = "RacePanel/DistanceDrivenPrefix";
        [SerializeField] private string personalBestPrefixKey = "RacePanel/PersonalBestPrefix";

        
        public string positionPrefix => LocalizationManager.GetTranslation("RacePanel/PositionPrefix");
        public string lapPrefix => LocalizationManager.GetTranslation("RacePanel/LapPrefix");
        public string lapTimePrefix => LocalizationManager.GetTranslation("RacePanel/LapTimePrefix");
        public string lastLapTimePrefix => LocalizationManager.GetTranslation("RacePanel/LastLapTimePrefix");
        public string bestTimePrefix => LocalizationManager.GetTranslation("RacePanel/BestTimePrefix");
        public string totalTimePrefix => LocalizationManager.GetTranslation("RacePanel/TotalTimePrefix");
        public string vehicleAheadPrefix => LocalizationManager.GetTranslation("RacePanel/VehicleAheadPrefix");
        public string vehicleBehindPrefix => LocalizationManager.GetTranslation("RacePanel/VehicleBehindPrefix");
        public string lapProgressPrefix => LocalizationManager.GetTranslation("RacePanel/LapProgressPrefix");
        public string distanceDrivenPrefix => LocalizationManager.GetTranslation("RacePanel/DistanceDrivenPrefix");
        public string personalBestPrefix => LocalizationManager.GetTranslation("RacePanel/PersonalBestPrefix");

        void Awake()
        {
            instance = this;
        }


        void Start()
        {
            //Clear UI elements
            if (raceInfoText != null)
            {
                raceInfoText.text = string.Empty;
            }

            if (raceInfoImage != null)
            {
                raceInfoImage.enabled = false;
            }

            if (raceOverText != null)
            {
                raceOverText.text = string.Empty;
            }

            if (sectorTime != null)
            {
                sectorTime.text = string.Empty;
            }

            if (vehicleAhead != null)
            {
                vehicleAhead.text = string.Empty;
            }

            if (vehicleBehind != null)
            {
                vehicleBehind.text = string.Empty;
            }

            if (raceLimitTimer != null)
            {
                raceLimitTimer.text = string.Empty;
            }

            if (raceEndTimer != null)
            {
                raceEndTimer.text = string.Empty;
            }

            if (countdownText != null)
            {
                countdownText.text = string.Empty;
            }

            if (countdownImage != null)
            {
                countdownImage.enabled = false;
            }
        }


        void Update()
        {
			if (RaceManager.instance != null)
            {
                UpdateLapTimers();
                UpdateWrongwayUI();
                UpdateProgressText();
                UpdateDistanceDrivenText();
            }
        }


      


        void UpdateLapTimers()
        {
            // Проверяем, есть ли данные о гонщике
            if (RaceManager.instance.playerStatistics == null)
                return;

            // Если игрок ещё не завершил гонку
            if (RaceManager.instance.playerStatistics != null && RaceManager.instance.playerStatistics.finished == false)
            {
                // Обновление текста времени круга
                if (lapTime != null)
                {
                    // Обновляем текст времени круга в зависимости от типа гонки
                    lapTime.text = lapTimePrefix + " " + Helper.FormatTime(RaceManager.instance.playerStatistics.lapTime);
                }

                // Обновление общего времени гонки
                if (totalTime != null)
                {
                    totalTime.text = totalTimePrefix + " " + Helper.FormatTime(RaceManager.instance.playerStatistics.totalRaceTime);
                }

                // Обновление текста таймера ограничения времени
                if (raceLimitTimer != null)
                {
                    if (RaceManager.instance.raceType == RaceType.Checkpoint)
                    {
                        // Для гонок типа Checkpoint
                        raceLimitTimer.text = Helper.FormatTime(RaceManager.instance.playerStatistics.checkpointTimer, TimeFormat.MinSec);
                    }
                    else
                    {
                        // Для других типов гонок
                        if (RaceManager.instance.raceLimitTimer > 0)
                        {
                            raceLimitTimer.text = Helper.FormatTime(RaceManager.instance.raceLimitTimer, TimeFormat.MinSec);
                        }
                    }
                }
            }

            // Обновление текста таймера завершения гонки
            // Внутри метода UpdateLapTimers() ищем место, где выводится "Race End:"
            if (raceEndTimer != null)
            {
                if (RaceManager.instance.endRaceTimerStarted)
                {
                    // Получаем переведённую строку из I2 Localization
                    string localizedRaceEnd = LocalizationManager.GetTranslation("RaceMessages/RaceEndTimer");

                    // Если ключ не найден или пуст, используем "Race End: " по умолчанию
                    if (string.IsNullOrEmpty(localizedRaceEnd))
                    {
                        localizedRaceEnd = "Race End: ";
                    }

                    // Отображаем таймер с локализованным текстом
                    raceEndTimer.text = localizedRaceEnd + (int)RaceManager.instance.raceEndTimer;
                }
                else
                {
                    raceEndTimer.text = string.Empty;
                }
            }

        }
    



        public void UpdatePositionText()
        {
			if (RaceManager.instance == null)
                return;

			if (RaceManager.instance.playerStatistics == null)
                return;

            if (position != null)
            {
				string position_string = positionDisplayMode == UIPositionDisplayMode.Default ? 
					RaceManager.instance.playerStatistics.Position + "/" + RaceManager.instance.activeRacerCount
					: positionDisplayMode == UIPositionDisplayMode.OrdinalPosition ? Helper.AddOrdinal(RaceManager.instance.playerStatistics.Position)
					: RaceManager.instance.playerStatistics.Position.ToString();

                position.text = positionPrefix + " " + position_string;
            }
        }


        public void UpdateLapText()
        {
			if (RaceManager.instance == null) 
				return;

			if (RaceManager.instance.playerStatistics == null)
				return;

            if (lap != null)
            {
				string totalLaps = (!RaceManager.instance.infiniteLaps) ? RaceManager.instance.lapCount.ToString() : "-";

                lap.text = lapDisplayMode == UILapDisplayMode.Default ? 
                    lapPrefix + " " + RaceManager.instance.playerStatistics.lap + "/" + totalLaps
                    : lapPrefix + " " + RaceManager.instance.playerStatistics.lap;
            }

            if (lastLapTime != null)
            {
				lastLapTime.text = lastLapTimePrefix + " " + (RaceManager.instance.playerStatistics.lastLapTime != 0 ? 
					Helper.FormatTime(RaceManager.instance.playerStatistics.lastLapTime) : "--:--.---");
            }

            if (bestLapTime != null)
            {
				bestLapTime.text = bestTimePrefix + " " + (RaceManager.instance.playerStatistics.bestLapTime != 0 ? 
					Helper.FormatTime(RaceManager.instance.playerStatistics.bestLapTime) : "--:--.---");
            }

            if (personalBestLapTime != null)
            {
                float best = RaceManager.instance.LoadOverallBestTimeForScene();
                personalBestLapTime.text = personalBestPrefix + " " + (best != 0 ? Helper.FormatTime(best) : "--:--.---");
            }

            SetInvalidLap(false);
        }


        void UpdateProgressText()
        {
            if (raceProgress == null)
                return;

            if (RaceManager.instance == null || RaceManager.instance.playerStatistics == null)
                return;

            // Определяем, что обновляем прогресс только в гонке Sprint
            bool isSprint = (RaceManager.instance.raceType == RaceType.Sprint);

            // Включаем/выключаем объект UI в зависимости от режима
            raceProgress.gameObject.SetActive(isSprint);

            // Если это не Sprint, не заполняем текст
            if (!isSprint)
                return;

            // Если гонка Sprint, берём процент прохождения гонки
            raceProgress.text = lapProgressPrefix + " "
                + (int)RaceManager.instance.playerStatistics.raceCompletionPercentage + "%";
        }



        void UpdateDistanceDrivenText()
        {
            if (distanceDriven == null)
                return;

            if (RaceManager.instance == null || RaceManager.instance.playerStatistics == null)
                return;

            // Проверяем, является ли текущая гонка Endurance
            bool isEndurance = (RaceManager.instance.raceType == RaceType.Endurance);

            // Включаем/выключаем объект Text в зависимости от режима
            distanceDriven.gameObject.SetActive(isEndurance);

            // Если это не Endurance, выходим, чтобы не обновлять текст
            if (!isEndurance)
                return;

            // Если Endurance, обновляем текст пройденного расстояния
            distanceDriven.text = distanceDrivenPrefix + " " + (int)RaceManager.instance.playerStatistics.totalDistance + "m";
        }



        void UpdateWrongwayUI()
        {
            if (wrongwayPanel == null)
                return;

			if (RaceManager.instance.playerStatistics == null)
                return;

			if (RaceManager.instance.playerStatistics.wrongway)
            {
                if(!wrongwayPanel.activeSelf)
                {
                    wrongwayPanel.SetActive(true);
                }
            }
            else
            {
                if (wrongwayPanel.activeSelf)
                {
                    wrongwayPanel.SetActive(false);
                }
            }
        }


        public void ShowRearViewMirror(bool show)
        {
            if(rearViewMirror != null)
            {
                rearViewMirror.SetActive(show);
            }
        }


        public void ShowRaceInfoMessage(string message)
        {
            if (raceInfoText == null)
                return;

            CancelInvoke("ClearRaceInfoMessage");
            raceInfoText.text = message;
            Invoke("ClearRaceInfoMessage", raceInfoDuration);
        }


        public void ShowRaceInfoImage(Sprite image)
        {
            if (raceInfoImage == null)
                return;

            CancelInvoke("ClearRaceInfoImage");
            raceInfoImage.sprite = image;
            raceInfoImage.enabled = true;
            Invoke("ClearRaceInfoImage", raceInfoDuration);
        }


        public void ShowSectorTime(float time)
        {
            if (sectorTime == null)
                return;

            sectorTime.color = time < 0 ? Color.green : Color.red;

            sectorTime.text = (time > 0 ? 
                "+" + Helper.FormatTime(Mathf.Abs(time))
                : "-" + Helper.FormatTime(Mathf.Abs(time)));

            Invoke("ClearSectorTime", 2);
        }


        public void ShowVehicleAhead(float time)
        {
            if (vehicleAhead == null)
                return;

            vehicleAhead.text = vehicleAheadPrefix + " -" + Helper.FormatTime(time);
            Invoke("ClearVehicleAhead", 3);
        }


        public void ShowVehicleBehind(float time)
        {
            if (vehicleBehind == null)
                return;

            vehicleBehind.text = vehicleBehindPrefix + " +" + Helper.FormatTime(time);
            Invoke("ClearVehidleBehind", 3);
        }


        public void ShowRaceEndMessage(string message)
        {
            //Deactivate all gameObjects in this panel
            Transform[] rectTransforms = GetComponentsInChildren<Transform>();
            foreach (Transform rect in rectTransforms)
            {
                if (rect != transform)
                    rect.gameObject.SetActive(false);
            }

            //Activate the race finished text with the message
            if (raceOverText != null)
            {
                raceOverText.gameObject.SetActive(true);
                raceOverText.text = message;
            }
        }


        void Countdown(int number)
        {
            //Countdown Text
            if (countdownText != null)
            {
                if (number > 0)
                    countdownText.text = number.ToString();
                else if (number == 0)
                    countdownText.text = LocalizationManager.GetTranslation(startTextKey);
                else
                    countdownText.text = string.Empty;
            }

            //Countdown Image
            if (countdownImage != null)
            {
                countdownImage.enabled = true;

                for (int i = 0; i < countdownSprites.Length; i++)
                {
                    if (number == countdownSprites[i].countdownNumber)
                    {
                        countdownImage.sprite = countdownSprites[i].countdownSprite;
                    }

                    if (number == -1)
                    {
                        countdownImage.enabled = false;
                    }
                }
            }

            if (number == 0)
            {
                Invoke("ClearCountdown", 1);
            }
        }


        public void SetInvalidLap(bool invalid)
        {
            if (lapTime == null)
                return;

            lapTime.color = invalid ? invalidLaptimeColor : validLaptimeColor;
        }


        void ClearRaceInfoMessage()
        {
            raceInfoText.text = string.Empty;
        }

        void ClearRaceInfoImage()
        {
            raceInfoImage.enabled = false;
        }


        void ClearSectorTime()
        {
            sectorTime.text = string.Empty;
        }


        void ClearVehicleAhead()
        {
            vehicleAhead.text = string.Empty;
        }


        void ClearVehidleBehind()
        {
            vehicleBehind.text = string.Empty;
        }


        void ClearCountdown()
        {
            Countdown(-1);
        }


        void OnEnable()
        {
            RaceManager.OnRaceCountdown += Countdown;
        }


        void OnDisable()
        {
            RaceManager.OnRaceCountdown -= Countdown;
        }
    }


    [System.Serializable]
    public class CountdownImage
    {
        public int countdownNumber;
        public Sprite countdownSprite;
    }
}