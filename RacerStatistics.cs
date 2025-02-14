using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using I2.Loc;

namespace RGSK
{
    public class RacerStatistics : MonoBehaviour
    {
        private int position;
        private int prevPosition;
        public int finalPosition { get; private set; }
        public int Position
        {
            get
            {
                return position;
            }
            set
            {
                prevPosition = position;
                position = value;

                if (position < prevPosition)
                {
                    if (isPlayer && started)
                    {
                        //Запустить событие, когда игрок получает позицию
                        if (OnPositionGained != null)
                            OnPositionGained.Invoke();
                    }
                }
                else if (position > prevPosition && started)
                {
                    if (isPlayer)
                    {
                        //Вызвать событие, когда игрок теряет позицию
                        if (OnPositionLost != null)
                            OnPositionLost.Invoke();
                    }
                }
            }
        }

        public int lap { get; private set; }
        public int championshipPosition { get; set; }
        public int championshipPoints { get; set; }
        public float raceCompletionPercentage { get; private set; }
        public float racePositionScore { get; set; }
        public float totalDistance { get; private set; }
        public float lapDistance { get; private set; }
        public float raceDistance { get; private set; }
        public float totalSpeed { get; private set; }
        public float lapTime { get; private set; }
        public float lastLapTime { get; private set; }
        public float bestLapTime { get; private set; }
        public float totalRaceTime { get; private set; }
        public float checkpointTimer { get; set; }
        public float scorePoints { get; set; }
        public bool started { get; set; }
        public bool finished { get; set; }
        public bool disqualified { get; set; }
        public bool lapCompleted { get; set; }
        public bool lapInvalidated { get; set; }
        public bool wrongway { get; private set; }
        public bool offTrack { get; set; }
        public bool isPlayer { get; set; }
        public float startingPositionOffset { get; private set; }
        private float wrongwayTimer;
        private float lastTotalDistance;
        private float lastLapDistance;

        private List<LapData> lapData = new List<LapData>();
        private List<Sector> sectors = new List<Sector>();
        private List<int> passedCheckpoints = new List<int>();
        private List<int> passedSpeedtraps = new List<int>();
        public RacerInformation racerInformation = new RacerInformation();

        private TrackLayout trackLayout;
        public TrackSpline.RoutePoint progressPoint { get; private set; }
        private Transform raceDistanceTracker;
        private int trackNodeIndex;
        private int lastTrackNodeIndex;

        private RaceManager raceManager;

        // События
        public static UnityAction OnEnterNewLap;
        public static UnityAction OnPositionGained;
        public static UnityAction OnPositionLost;
        public int lastValidCheckpointIndex = -1;
        public float FinishTime { get; set; }


        void Awake()
        {
            //Найти схему трассы
            trackLayout = FindObjectOfType<TrackLayout>();
            if (trackLayout != null)
            {
                raceDistanceTracker = new GameObject(gameObject.name + "_tracker").transform;
                raceDistanceTracker.transform.parent = GameObject.Find("RGSK_Trackers") ? GameObject.Find("RGSK_Trackers").transform : null;
            }

            //Проверяем, является ли гонщик игроком
            isPlayer = gameObject.CompareTag("Player");
        }


        void Start()
        {
            //Получить ссылку на экземпляр RaceManager
            raceManager = RaceManager.instance;

            //Найти все сектора пути
            FindTrackSectors();

            //Сбрасываем значения
            Reset();
        }


        void Update()
        {
            //Если менеджера гонки нет, вернуть
            if (!enabled || raceManager == null)
                return;

            UpdateRaceTimers();
            UpdateRaceProgress();
            WrongwayChecker();
        }


        public Transform GetLastCheckpointNode()
        {
            // Если последний валидный чекпоинт установлен, возвращаем соответствующий узел,
            // иначе возвращаем текущий узел.
            if (lastValidCheckpointIndex >= 0 && lastValidCheckpointIndex < trackLayout.nodes.Count)
                return trackLayout.nodes[lastValidCheckpointIndex].transform;
            else
                return trackLayout.nodes[trackNodeIndex].transform;
        }

        public Transform GetSafeRespawnNode()
        {
            // Получаем узел из последнего чекпоинта (либо текущего, если чекпоинт не установлен)
            Transform node = GetLastCheckpointNode();
            float dist = Vector3.Distance(node.position, transform.position);
           // Debug.Log("[GetSafeRespawnNode] Last checkpoint node position: " + node.position +
                    //  ", Car position: " + transform.position + ", Distance: " + dist);

            // Если расстояние между текущей позицией и выбранным узлом меньше 1 метра,
            // выбираем следующий узел по трассе
            if (dist < 1.0f)
            {
                int fallbackIndex = (lastTrackNodeIndex + 1) % trackLayout.nodes.Count;
              //  Debug.Log("[GetSafeRespawnNode] Distance too small (" + dist + "). Using fallback index: " + fallbackIndex);
                node = trackLayout.nodes[fallbackIndex].transform;
            }
           // Debug.Log("[GetSafeRespawnNode] Final respawn node position: " + node.position);
            return node;
        }




        public void UpdateRaceTimers()
        {
            if (!started || finished || disqualified)
                return;

            //Время круга
            lapTime += Time.deltaTime;

            //Общее время
            totalRaceTime += Time.deltaTime;

            if (raceManager.raceType == RaceType.Checkpoint)
            {
                //Используется в гонках с контрольными точками для отслеживания времени гонщика.
                checkpointTimer -= Time.deltaTime;

                //Выбить гонщика, если таймер достигнет 0
                if (checkpointTimer <= 0)
                    raceManager.DisqualifyRacer(this);
            }
        }


        void UpdateRaceProgress()
        {
            if (raceDistanceTracker == null)
                return;

            //Обновляем позицию трекера
            raceDistanceTracker.position =
                trackLayout.GetRoutePoint(totalDistance)
                       .position;

            //Обновить вращение трекера
            raceDistanceTracker.rotation =
                Quaternion.LookRotation(
                    trackLayout.GetRoutePoint(totalDistance)
                           .direction);


            progressPoint = trackLayout.GetRoutePoint(totalDistance);
            Vector3 progressDelta = progressPoint.position - transform.position;

            //Двигаемся вперед
            if (Vector3.Dot(progressDelta, progressPoint.direction) < 0)
            {
                totalDistance += progressDelta.magnitude * 0.5f;
                lapDistance += progressDelta.magnitude * 0.5f;
            }

            //Двигаемся назад
            if (Vector3.Dot(progressDelta, progressPoint.direction) > 2)
            {
                totalDistance -= progressDelta.magnitude * 0.5f;
                lapDistance -= progressDelta.magnitude * 0.5f;
            }

            //Получаем индекс узла из расстояния
            trackNodeIndex = trackLayout.GetNodeIndexAtDistance(lapDistance);
            if (trackNodeIndex != lastTrackNodeIndex)
            {
                //Store distances at this index
                lastTrackNodeIndex = trackNodeIndex;
                lastTotalDistance = totalDistance;
                lastLapDistance = lapDistance;
            }

            //Обновлять счет позиции до тех пор, пока гонщик не закончит гонку
            if (!finished)
            {
                racePositionScore = Mathf.Lerp(racePositionScore, totalDistance, 0.1f);
                raceCompletionPercentage = ((totalDistance / trackLayout.length) / raceManager.lapCount) * 100;
            }
        }


        void WrongwayChecker()
        {
            if (raceDistanceTracker == null)
                return;

            //Рассчитать угол от текущего узла, чтобы проверить, смотрит ли транспортное средство в неправильном направлении
            float angle = Mathf.DeltaAngle(raceDistanceTracker.eulerAngles.y, transform.eulerAngles.y);
            wrongway = Mathf.Abs(angle) > 120f; // Порог можно откорректировать при необходимости


            //Если гонщик едет не в ту сторону, проверьте, нужен ли принудительный респаун
            if (raceManager.forceWrongwayRespawn)
            {
                if (wrongway)
                {
                    //Добавить к таймеру
                    wrongwayTimer += Time.deltaTime;

                    //Если таймер превысит лимит, возродить гонщика
                    if (wrongwayTimer > raceManager.wrongwayRespawnTime)
                    {
                        raceManager.RespawnVehicle(transform);
                    }
                }
                else
                {
                    //Сбросить таймер
                    wrongwayTimer = 0;
                }
            }
        }


        public void FinishRace()
        {
            if (finished || disqualified)
                return;

            //Умножить очки позиции при пересечении финишной черты
            racePositionScore *= 100;

            //Умножаем результат позиции на итоговую позицию
            racePositionScore *= (raceManager.activeRacerCount + 1) - Position;

            finished = true;

            //Регистрация этого гонщика завершена
            raceManager.RegisterRacerFinish(this);
        }


        void OnTriggerEnter(Collider col)
        {
            if (col.GetComponent<RaceTrigger>())
            {
                //Регистрация того, что этот гонщик прошел триггер гонки
                if (started && !finished && !disqualified)
                {
                    RegisterRaceTrigger(col.GetComponent<RaceTrigger>());
                }
            }
        }


        void RegisterRaceTrigger(RaceTrigger trigger)
        {
            if (!enabled || raceManager == null)
                return;

            switch (trigger.triggerType)
            {
                case RaceTriggerType.FinishLine:
                    RegisterNewLap();
                    break;

                case RaceTriggerType.Checkpoint:
                    RegisterCheckpoint(trigger.index, trigger.addedTime, trigger.GetValidDistance());
                    break;

                case RaceTriggerType.SpeedTrap:
                    RegisterSpeedtrap(trigger.index, trigger.GetValidDistance());
                    break;

                case RaceTriggerType.Sector:
                    RegisterSector(trigger.index, trigger.GetValidDistance());
                    break;
            }
        }


        void RegisterNewLap()
        {
            // Проверить, завершён ли круг
            lapCompleted = lapDistance >= trackLayout.distances[trackLayout.distances.Length - 2];

            // Если круг не завершён, выйти
            if (!lapCompleted)
                return;

            // Добавить время круга в список данных кругов
            lapData.Add(new LapData(lap, lapTime, lapInvalidated));

            // Увеличить количество кругов
            lap++;

            // Получить разницу от лучшего времени круга
            float difference = 0;
            if (bestLapTime > 0)
            {
                difference = lapTime - bestLapTime;
            }

            // Сохранить время последнего круга гонщика
            lastLapTime = lapTime;

            // Проверить, есть ли у гонщика новое лучшее время за сессию, если круг валиден
            if (!lapInvalidated)
            {
                if (bestLapTime == 0 || lapTime < bestLapTime)
                {
                    bestLapTime = lapTime;

                    if (isPlayer)
                    {
                        // Проверить наличие лучшего времени круга только если круг валиден                   
                        raceManager.CheckForBestLapTime(lapTime);
                    }
                }

                // Показать разделение времени круга
                if (isPlayer && lap > 2 && difference != 0)
                {
                    // Показать разделение времени круга                       
                    if (RacePanel.instance != null)
                    {
                        RacePanel.instance.ShowSectorTime(difference);
                    }
                }
            }

            if (!raceManager.infiniteLaps)
            {
                // Финальный круг
                if (lap == raceManager.lapCount)
                {
                    if (isPlayer)
                    {
                        if (RacePanel.instance != null)
                        {
                            RacePanel.instance.ShowRaceInfoMessage(raceManager.finalLapInfo);
                        }
                    }
                }

                // Завершить гонку
                if (lap > raceManager.lapCount)
                {
                    FinishRace();
                }
            }

            // Проверить наличие существующего привидения
            GhostVehicleManager ghost = FindObjectOfType<GhostVehicleManager>();
            if (ghost != null && !finished)
            {
                // Всегда создавать привидение после первого круга
                if (lap == 2)
                {
                    ghost.CacheValues();
                }
                else
                {
                    // Кэшировать лучшие значения, когда лучшее время круга побито
                    if (lastLapTime <= bestLapTime)
                    {
                        ghost.CacheValues();
                    }
                }

                // Запустить привидение
                ghost.StartGhost();
            }

            // Функции, специфичные для типа гонки
            switch (raceManager.raceType)
            {
                case RaceType.LapKnockout:

                    // Исключить гонщика на последнем месте
                    if (position == raceManager.activeRacerCount - 1)
                    {
                        raceManager.DisqualifyRacer(raceManager.GetRacerInLastPlace());
                    }

                    break;

                case RaceType.Endurance:

                    if (raceManager.raceLimitTimer <= 0)
                    {
                        if (lap > raceManager.GetRacerInFirstPlace().lap)
                        {
                            FinishRace();
                        }
                    }

                    break;
            }

            // Вызвать событие нового круга для игрока
            if (isPlayer)
            {
                OnEnterNewLap.Invoke();
            }

            // Проверить разрыв с транспортными средствами впереди и позади
            CheckForVehicleAheadAndBehind();

            // Сбросить значения
            lapCompleted = false;
            lapInvalidated = false;
            lapTime = 0;
            lapDistance = 0;
            passedCheckpoints.Clear();
            passedSpeedtraps.Clear();
            for (int i = 0; i < sectors.Count; i++)
            {
                sectors[i].passed = false;
                sectors[i].previousBestTime = sectors[i].bestTime;
            }
        }



        void RegisterCheckpoint(int index, float time, float dist)
        {
            if (lapDistance < dist)
                return;

            if (passedCheckpoints.Contains(index))
                return;

            if (raceManager.raceType != RaceType.Checkpoint)
                return;

            // Сохраняем индекс как последний валидный чекпоинт
            lastValidCheckpointIndex = index;
            Debug.Log("[RegisterCheckpoint] Updated lastValidCheckpointIndex to: " + lastValidCheckpointIndex);

            passedCheckpoints.Add(index);
            checkpointTimer += time;

            if (isPlayer)
            {
                if (RacePanel.instance != null)
                {
                    string info = "+ " + time;
                    RacePanel.instance.ShowRaceInfoMessage(info);
                }
            }
        }




        void RegisterSpeedtrap(int index, float dist)
        {
            // Проверить, что расстояние круга находится в диапазоне для прохождения этого триггера
            if (lapDistance < dist)
                return;

            // Если этот триггер уже был пройден, выйти
            if (passedSpeedtraps.Contains(index))
                return;

            // Если этот триггер не нужен для текущего типа гонки, выйти
            if (raceManager.raceType != RaceType.SpeedTrap)
                return;

            // Добавить индекс этого триггера в список, чтобы гарантировать, что мы не сможем пройти его снова
            // до следующего круга
            passedSpeedtraps.Add(index);

            Rigidbody rigid = GetComponent<Rigidbody>();
            if (rigid != null)
            {
                // Получить скорость гонщика и добавить ее к значению TotalSpeed
                SpeedUnit unit = RaceManager.instance.speedUnit;
                float speed = unit == SpeedUnit.KPH ? Helper.MpsToKph(rigid.velocity.magnitude) : Helper.MpsToMph(rigid.velocity.magnitude);
                totalSpeed += speed;

                // Принудительно обновить позиции гонки при прохождении ловушки скорости
                raceManager.UpdateRacePositions();

                if (isPlayer)
                {
                    // Показать информацию о ловушке скорости
                    if (RacePanel.instance != null)
                    {
                        string info = string.Format(LocalizationManager.GetTranslation("RacePanel/SpeedTrapMessage"), speed.ToString("F1"), unit.GetLocalizedString());
                        RacePanel.instance.ShowRaceInfoMessage(info);
                        ;
                    }
                }
            }
        }



        void RegisterSector(int index, float dist)
        {
            // Проверить, что расстояние круга находится в диапазоне для прохождения этого триггера
            if (lapDistance < dist)
                return;

            for (int i = 0; i < sectors.Count; i++)
            {
                if (index == sectors[i].index)
                {
                    if (!sectors[i].passed)
                    {
                        // Обеспечить, что мы не можем пройти этот сектор дважды, установив 'passed' в true
                        sectors[i].passed = true;

                        if (!lapInvalidated)
                        {
                            // Сохранить предыдущее лучшее время прохождения этого сектора
                            sectors[i].previousBestTime = sectors[i].bestTime;

                            // Получить разницу во времени сектора
                            float difference = 0;
                            if (sectors[i].bestTime > 0)
                            {
                                difference = lapTime - sectors[i].bestTime;
                            }

                            // Проверить наличие лучшего времени прохождения этого сектора
                            if (sectors[i].bestTime == 0 || lapTime < sectors[i].bestTime)
                            {
                                sectors[i].bestTime = lapTime;
                            }

                            if (isPlayer)
                            {
                                // Показать информацию о времени сектора
                                if (difference != 0)
                                {
                                    if (RacePanel.instance != null)
                                    {
                                        RacePanel.instance.ShowSectorTime(difference);
                                    }
                                }
                            }
                        }

                        CheckForVehicleAheadAndBehind();
                    }
                }
            }
        }


        void CheckForVehicleAheadAndBehind()
        {
            if (raceManager.activeRacerCount == 1)
                return;

            if (isPlayer)
            {
                // Показать транспортное средство впереди
                if (position > 1)
                {
                    RacerStatistics other = raceManager.GetRacerInPosition(position - 1);
                    float gap = raceManager.GetTimeGapBetween(other);

                    if (RacePanel.instance != null)
                    {
                        RacePanel.instance.ShowVehicleAhead(Mathf.Abs(gap));
                    }
                }
            }
            else
            {
                // Показать транспортное средство позади
                if (raceManager.playerStatistics != null && position == raceManager.playerStatistics.position + 1)
                {
                    float gap = raceManager.GetTimeGapBetween(this);

                    if (RacePanel.instance != null)
                    {
                        RacePanel.instance.ShowVehicleBehind(Mathf.Abs(gap));
                    }
                }
            }
        }



        void FindTrackSectors()
        {
            RaceTrigger[] raceTriggers = FindObjectsOfType<RaceTrigger>();

            foreach (RaceTrigger sector in raceTriggers)
            {
                if (sector.triggerType == RaceTriggerType.Sector)
                {
                    sectors.Add(new Sector(sector.index));
                }
            }
        }


        public void AddScorePoints(float score)
        {
            scorePoints += score;
        }


        public void SetWheelsOffTrack(int count)
        {
            if (raceManager == null)
                return;

            offTrack = count >= raceManager.minWheelCountForOfftrack;
            if (offTrack && raceManager.enableOfftrackPenalty && !lapInvalidated)
            {
                InvalidateLap();
            }
        }


        public void InvalidateLap()
        {
            if (!enabled)
                return;

            //Invalidate the lap
            lapInvalidated = true;

            //Revert sector best time for passed sectors
            for (int i = 0; i < sectors.Count; i++)
            {
                sectors[i].bestTime = sectors[i].previousBestTime;
            }

            //Show info
            if (isPlayer && RacePanel.instance != null)
            {
                RacePanel.instance.ShowRaceInfoMessage(raceManager.invalidLapInfo);
                RacePanel.instance.SetInvalidLap(true);
            }
        }


        public void Reset()
        {
            lap = 1;
            raceCompletionPercentage = 0;
            racePositionScore = 0;
            totalDistance = 0;
            lapTime = 0;
            lastLapTime = 0;
            bestLapTime = 0;
            totalRaceTime = 0;
            lapDistance = 0;
            started = false;
            finished = false;
            disqualified = false;
            lapCompleted = false;
            passedCheckpoints.Clear();
            passedSpeedtraps.Clear();
            lapData.Clear();

            for (int i = 0; i < sectors.Count; i++)
            {
                sectors[i].bestTime = 0;
                sectors[i].previousBestTime = 0;
                sectors[i].passed = false;
            }
        }


        public void RevertTotalDistance()
        {
            trackNodeIndex = lastTrackNodeIndex;
            totalDistance = lastTotalDistance;
            lapDistance = lastLapDistance;
        }


        public int GetPosition()
        {
            return position;
        }


        public int GetLap()
        {
            return lap;
        }


        public bool IsOnFinalLap()
        {
            return !raceManager.infiniteLaps && lap == raceManager.lapCount;
        }


        public string GetName()
        {
            return racerInformation.racerName;
        }


        public string GetVehicle()
        {
            return racerInformation.vehicleName;
        }


        public Nationality GetNationality()
        {
            return racerInformation.nationality;
        }


        public Transform GetCurrentNode()
        {
            return trackLayout.nodes[trackNodeIndex].transform;
        }


        public Transform GetPreviousNode()
        {
            return trackLayout.nodes[trackNodeIndex].transform;
        }


        public float GetTrackPosition()
        {
            return raceDistanceTracker.InverseTransformPoint(transform.position).x;
        }


        public bool IsPlayer()
        {
            return gameObject.CompareTag("Player");
        }


        void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                if (raceDistanceTracker == null) return;

                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, raceDistanceTracker.position);
            }
        }
    }


    [System.Serializable]
    public class LapData
    {
        public int lap;
        public float lapTime;
        public bool invalid;

        public LapData(int _lap, float _lapTime, bool _invalid)
        {
            lap = _lap;
            lapTime = _lapTime;
            invalid = _invalid;
        }
    }


    [System.Serializable]
    public class Sector
    {
        public int index;
        public float bestTime;
        public float previousBestTime;
        public bool passed;

        public Sector(int i)
        {
            index = i;
        }
    }
}
