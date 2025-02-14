﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RGSK
{
    public class ChampionshipManager : MonoBehaviour
    {
        public static ChampionshipManager instance;
        public ChampionshipData championshipData;
        public List<ChampionshipRound> championshipRounds { get { return championshipData.championshipRounds; } }
        public List<ChampionshipRacer> championshipRacers { get; private set; }
        public int roundIndex { get; private set; }
        private RaceType[] unavailableRaceTypes = { RaceType.TimeTrial, RaceType.TimeAttack };

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }


        public void Start()
        {
            //Initialize the list
            championshipRacers = new List<ChampionshipRacer>();

            //Add the AI racers
            foreach (ChampionshipRacer racer in championshipData.championshipRacers)
            {
                championshipRacers.Add(new ChampionshipRacer(racer.vehicle, racer.name, false));
            }

            //Add the Player
            if (PlayerData.instance != null)
            {
                championshipRacers.Add(new ChampionshipRacer(GlobalSettings.Instance.vehicleDatabase.GetVehicle
                    (PlayerData.instance.playerData.vehicleID).vehicle, PlayerData.instance.playerData.playerName, true));
            }
            else
            {
                Debug.LogWarning("Could not create a player vehicle for this championship beacuse PlayerData and/or GameVehiclesData could not be found!");
            }
        }


        public void SpawnVehicles()
        {
            RaceType raceType = CurrentRound().raceType;

            for (int i = 0; i < unavailableRaceTypes.Length; i++)
            {
                //If the current race type is not supported in chamionships, return
                if (raceType == unavailableRaceTypes[i])
                {
                    Debug.LogWarning("The " + raceType.ToString() + " race type is not supported in Championships!");
                    return;
                }
            }

            //If the current race type is not supported in chamionships, return
            if (raceType == RaceType.Drag && championshipRacers.Count > 2)
            {
                Debug.LogWarning("The Drag race type can only have 2 racers! Please re-create your championship with only 1 opponent.");
                return;
            }

            //Spawn the championship racers
            for (int i = 0; i < championshipRacers.Count; i++)
            {
                RaceManager.instance.SpawnChampionshipVehicle(championshipRacers[i].vehicle, i, championshipRacers[i].points,
                                championshipRacers[i].name, championshipRacers[i].nationality, championshipRacers[i].isPlayer);
            }
        }


        public void LoadNextRound()
        {
            if (championshipData.championshipRounds.Count > roundIndex + 1)
            {
                if (SceneController.instance != null)
                {
                    SceneController.instance.LoadScene(championshipData.championshipRounds[roundIndex + 1].trackData.scene);
                    roundIndex++;
                }
            }
            else
            {
                Debug.Log($"[DEBUG] Завершение чемпионата. Количество наград: {championshipData.raceRewards.Count}");
                Debug.Log("Чемпионат завершён. Начисление награды...");

                // Сохраняем награды за чемпионат
                if (championshipData.raceRewards != null && championshipData.raceRewards.Count > 0)
                {
                    ChampionshipData.pendingRewards = championshipData.raceRewards.ToArray();
                    Debug.Log($"Сохранены награды за чемпионат: {ChampionshipData.pendingRewards.Length} шт.");
                }
                else
                {
                    ChampionshipData.pendingRewards = null;
                    Debug.LogWarning("Нет наград за чемпионат.");
                }

                // Возвращаемся в меню
                if (SceneController.instance != null)
                {
                    SceneController.instance.ExitToMenu();
                }

                // Удаляем объект чемпионата
                Destroy(gameObject);
            }
        }



        public int GetPointsForPosition(int position)
        {
            return championshipData.championshipPoints[position - 1];
        }


        public int GetPointsForRacer(string name)
        {
            for (int i = 0; i < championshipRacers.Count; i++)
            {
                if (championshipRacers[i].name == name)
                {
                    return championshipRacers[i].points;
                }
            }

            return 0;
        }


        public void SetPointsForRacer(string name, int points)
        {
            for (int i = 0; i < championshipRacers.Count; i++)
            {
                if (championshipRacers[i].name == name)
                {
                    //Save the racer's last points
                    championshipRacers[i].previousPoints = championshipRacers[i].points;

                    //Save the racer's points
                    championshipRacers[i].points = points;
                }
            }

            championshipRacers = championshipRacers.OrderByDescending(x => x.points).ToList();
        }


        public void RevertChampionshipPoints()
        {
            for (int i = 0; i < championshipRacers.Count; i++)
            {
                championshipRacers[i].points = championshipRacers[i].previousPoints;
            }
        }


        public ChampionshipRound CurrentRound()
        {
            return championshipRounds[roundIndex];
        }


        public bool IsFinalRound()
        {
            return roundIndex == championshipRounds.Count - 1;
        }
    }

    [System.Serializable]
    public class ChampionshipRound
    {
        public TrackData trackData;
        public RaceType raceType;
        public int laps;
        public string championshipID; // Уникальный идентификатор гонки
        [Header("Настройки Гонки")]
        public bool nonCollisionRace; // Гонка без столкновений
        public bool autoDriveTimeTrial; // Автоматическое управление в тайм-триале
        public bool useTimeLimit; // Использование ограничения по времени
        public bool finishEnduranceImmediately; // Немедленное завершение гонки на выносливость

        [Header("Настройки догоняющего")]
        public bool enableCatchup;
        [Range(0.1f, 1)] public float catchupStrength = 0.1f;
        public float minCatchupRange = 10;
        public float maxCatchupRange = 100;

        [Header("Настройка Slipstream")]

        public SlipstreamSettings slipstreamSettings;

        [Header("Настройки штрафаm")]

        public bool enableOfftrackPenalty;
        public int minWheelCountForOfftrack = 4;
        public bool forceWrongwayRespawn;
        public float wrongwayRespawnTime = 5;


        [Header("Настройко Ботов")]

        public AIDifficultyLevel aidifficult;
        public AiDetails aiDetails;
        public AiDifficulty easyAiDifficulty;
        public AiDifficulty mediumAiDifficulty;
        public AiDifficulty hardAiDifficulty;
        private List<AIDetial> aiDetailsList = new List<AIDetial>();

        // --- новые поля ---
        public float eliminationTimeStart = 30f;
        public float checkpointTimeStart = 30f;
        public float enduranceTimeStart = 300f;
        public float driftTimeStart = 120f;

        [Header("На время")]
        // === Новые поля для Time Attack ===
        public float targetTimeGold = 130f;
        public float targetTimeSilver = 115f;
        public float targetTimeBronze = 100f;

        [Header("Дрифт")]
        // === Новые поля для Drift ===
        public float targetScoreGold = 0f;
        public float targetScoreSilver = 0f;
        public float targetScoreBronze = 0f;

       
    }

    [System.Serializable]
    public class ChampionshipRacer
    {
        public GameObject vehicle;
        public string name;
        public Nationality nationality;
        [HideInInspector] public bool isPlayer;
        public int points { get; set; }
        public int previousPoints { get; set; }

        public ChampionshipRacer(GameObject _vehicle, string _name, bool _isPlayer)
        {
            vehicle = _vehicle;
            name = _name;
            isPlayer = _isPlayer;
        }
    }
}