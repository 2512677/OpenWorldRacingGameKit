using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // На случай, если SceneController отсутствует

namespace RGSK
{
    [CreateAssetMenu(fileName = "Новая Карьера", menuName = "MRFE/New Career Data", order = 1)]
    public class CareerData : ScriptableObject
    {
        // -----------------------------
        // ВАЖНО: добавляем статическое поле для наград
        // -----------------------------
        public static RaceRewards.Rewards[] pendingRewards;

        public string careerName;
        public string stageName;
        [Header("Optional Championship Link")]
        public ChampionshipData championshipData; // <-- Доп. поле, если нужно привязать ChampionshipData

        [SerializeField]
        public List<CareerRound> careerRounds = new List<CareerRound>();

        [System.Serializable]
        public class CareerRound
        {
            

            public TrackData trackData;
            public RaceType raceType;
            public int laps;
            public int opponentCount;
            public string raceID; // Уникальный идентификатор гонки

            [Header("Настройки Гонки")]
            public bool nonCollisionRace;
            public bool autoDriveTimeTrial;
            public bool useTimeLimit;
            public bool finishEnduranceImmediately;

            [Header("Настройки догоняющего")]

            [Header("включить догонялки")] 

            public bool enableCatchup;

            [Header("Догоняющая сила")]
           
            [Range(0.1f, 1)] public float catchupStrength = 0.1f;

            [Header("Мин. диапазон догонялок")]

            public float minCatchupRange = 10;
            [Header("Максимальный диапазон догонялок")]

            public float maxCatchupRange = 100;

            [Header("Настройка Slipstream")]
            public SlipstreamSettings slipstreamSettings;

            [Header("Настройки возрождения")]

            //Настройки возрождения
            public RespawnSettings respawnSettings;

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

            [Header("Награды")]
            public List<RaceRewards.Rewards> raceRewards;
        }

        public void GenerateRaceIDs(string stageName)
        {
            for (int i = 0; i < careerRounds.Count; i++)
            {
                careerRounds[i].raceID = $"{stageName}_Round_{i}";
            }
        }

        /// <summary>
        /// Запускаем гонку. 
        ///  1) Сохраняем награды (pendingRewards)
        ///  2) Устанавливаем PlayerPrefs (тип гонки, круги, соперники)
        ///  3) Загружаем сцену гонки (через SceneController или напрямую)
        /// </summary>
        public void StartRace(int roundIndex)
        {
            Debug.Log($"CareerData: StartRace({roundIndex}) called.");

            // Проверка наличия roundIndex в пределах списка
            if (careerRounds == null || roundIndex < 0 || roundIndex >= careerRounds.Count)
            {
                Debug.LogError("CareerData: Invalid roundIndex or careerRounds is null/empty!");
                return;
            }

            var round = careerRounds[roundIndex];
            if (round.trackData == null)
            {
                Debug.LogError($"CareerData: round.trackData is null for roundIndex={roundIndex}!");
                return;
            }

            // Сохраняем награды
            if (round.raceRewards != null && round.raceRewards.Count > 0)
            {
                pendingRewards = round.raceRewards.ToArray();
                Debug.Log($"CareerData: Stored {pendingRewards.Length} rewards for roundIndex={roundIndex}.");
            }
            else
            {
                pendingRewards = null;
                Debug.LogWarning($"CareerData: No rewards found for roundIndex={roundIndex}!");
            }

            // Запись настроек гонки
            PlayerPrefs.SetInt("RaceType", (int)round.raceType);
            PlayerPrefs.SetInt("LapCount", round.laps);
            PlayerPrefs.SetInt("OpponentCount", round.opponentCount);

            PlayerPrefs.SetString("AiDifficulty", round.aidifficult.ToString());
            if (round.easyAiDifficulty != null)
                PlayerPrefs.SetString("EasyAiDifficulty", round.easyAiDifficulty.name);
            if (round.mediumAiDifficulty != null)
                PlayerPrefs.SetString("MediumAiDifficulty", round.mediumAiDifficulty.name);
            if (round.hardAiDifficulty != null)
                PlayerPrefs.SetString("HardAiDifficulty", round.hardAiDifficulty.name);

            PlayerPrefs.SetFloat("EliminationTime", round.eliminationTimeStart);
            PlayerPrefs.SetFloat("CheckpointTime", round.checkpointTimeStart);
            PlayerPrefs.SetFloat("EnduranceTime", round.enduranceTimeStart);
            PlayerPrefs.SetFloat("DriftTime", round.driftTimeStart);

            PlayerPrefs.SetInt("EnableCatchup", round.enableCatchup ? 1 : 0);
            PlayerPrefs.SetFloat("CatchupStrength", round.catchupStrength);
            PlayerPrefs.SetFloat("MinCatchupRange", round.minCatchupRange);
            PlayerPrefs.SetFloat("MaxCatchupRange", round.maxCatchupRange);

            if (round.slipstreamSettings != null)
            {
                PlayerPrefs.SetInt("EnableSlipstream", round.slipstreamSettings.enableSlipstream ? 1 : 0);
                PlayerPrefs.SetFloat("SlipstreamStrength", round.slipstreamSettings.slipstreamStrength);
                PlayerPrefs.SetFloat("MinSlipstreamSpeed", round.slipstreamSettings.minSlipstreamSpeed);
            }

            PlayerPrefs.SetInt("EnableOfftrackPenalty", round.enableOfftrackPenalty ? 1 : 0);
            PlayerPrefs.SetInt("MinWheelCountForOfftrack", round.minWheelCountForOfftrack);
            PlayerPrefs.SetInt("ForceWrongwayRespawn", round.forceWrongwayRespawn ? 1 : 0);
            PlayerPrefs.SetFloat("WrongwayRespawnTime", round.wrongwayRespawnTime);

            PlayerPrefs.SetInt("NonCollisionRace", round.nonCollisionRace ? 1 : 0);
            PlayerPrefs.SetInt("AutoDriveTimeTrial", round.autoDriveTimeTrial ? 1 : 0);
            PlayerPrefs.SetInt("UseTimeLimit", round.useTimeLimit ? 1 : 0);
            PlayerPrefs.SetInt("FinishEnduranceImmediately", round.finishEnduranceImmediately ? 1 : 0);

            PlayerPrefs.SetFloat("TargetTimeGold", round.targetTimeGold);
            PlayerPrefs.SetFloat("TargetTimeSilver", round.targetTimeSilver);
            PlayerPrefs.SetFloat("TargetTimeBronze", round.targetTimeBronze);

            PlayerPrefs.SetFloat("TargetScoreGold", round.targetScoreGold);
            PlayerPrefs.SetFloat("TargetScoreSilver", round.targetScoreSilver);
            PlayerPrefs.SetFloat("TargetScoreBronze", round.targetScoreBronze);

            // ВАЖНО: сохраняем raceID, чтобы RaceManager мог считать именно его
            PlayerPrefs.SetString("CurrentRaceID", round.raceID);
            PlayerPrefs.Save();

            // Пробуем загрузить сцену
            if (SceneController.instance != null)
            {
                Debug.Log($"CareerData: Using SceneController to load scene '{round.trackData.trackName}'");
                SceneController.instance.LoadScene(round.trackData.trackName);
            }
            else
            {
                Debug.Log($"CareerData: SceneController not found, using SceneManager to load '{round.trackData.trackName}'");
                SceneManager.LoadScene(round.trackData.trackName);
            }
        }
    }
}
