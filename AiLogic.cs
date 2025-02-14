using UnityEngine;
using System.Collections;

namespace RGSK
{
    public class AiLogic : MonoBehaviour
    {
        [Header("Behaviour")]
        [Range(0, 1)] public float throttleSensitivity = 0.8f;
        [Range(0, 1)] public float brakeSensitivity = 0.25f;
        [Range(0, 1)] public float steerSensitivity = 0.5f;
        [Range(0.8f, 1)] public float speedModifier = 1;
        public bool revOnRaceCountdown;

        // Навигация
        private TrackLayout racingLine;
        public Transform racingLineTarget { get; private set; }
        public TrackSpline.RoutePoint racingLinePoint { get; private set; }
        private float racingLineDistance;
        private float currentSpeed;
        private float targetSpeed;
        private int racingLineNodeIndex;
        private float targetDistanceAhead;

        // Обгон
        public float cautionDistance = 10;
        private float travelOffset;
        private float newTravelOffset;
        private Collider closestThreat;
        private bool slowDownThreat;
        private float traveOffsetResetTimer;

        // Восстановление
        private float recoverTimer;
        private float reverseTimer;
        private bool reversing;
        public float respawnWait = 10;

        [Header("Сенсор")]
        public bool visualizeSensors = true;
        [Space(10)]
        // Передний датчик
        public float frontSensorDistance = 100;
        public float frontSensorWidth = 2;
        public float frontSensorHeight = 0.5f;
        public Sensor frontSensor { get; private set; }
        [Space(5)]
        // Левый датчик
        public float leftSensorDistance = 5;
        public float leftSensorWidth = 2;
        public float leftSensorHeight = 0.5f;
        public float leftSensorOffset = 1;
        public Sensor leftSensor { get; private set; }
        [Space(5)]
        // Правый датчик
        public float rightSensorDistance = 5;
        public float rightSensorWidth = 2;
        public float rightSensorHeight = 0.5f;
        public float rightSensorOffset = 1;
        public Sensor rightSensor { get; private set; }

        // Ссылки
        private IAiInput aiInput;
        private Rigidbody rigid;
        private RacerStatistics racerStatistics;

        // Входные значения
        public float throttleInput { get; private set; }
        public float steerInput { get; private set; }
        public float brakeInput { get; private set; }
        public float handbrakeInput { get; private set; }


        void Awake()
        {
            racingLine = FindObjectOfType<TrackLayout>();
            if (racingLine != null)
            {
                //Создайте цель рулевого управления гоночной траектории
                racingLineTarget = new GameObject("RacingLineTarget:" + gameObject.name).transform;
                racingLineTarget.transform.parent = GameObject.Find("RGSK_Trackers")
                    ? GameObject.Find("RGSK_Trackers").transform : null;
            }
        }


        void Start()
        {
            //Получить ссылки на компоненты
            aiInput = GetComponent<IAiInput>();
            rigid = GetComponent<Rigidbody>();
            racerStatistics = GetComponent<RacerStatistics>();

            //Настройка датчиков ИИ
            SetupSensors();
        }


        void SetupSensors()
        {
            Vector3 center = new Vector3();
            Vector3 size = new Vector3();

            //Создать передний датчик
            center = new Vector3(0, frontSensorHeight, (frontSensorDistance / 2) + 0.5f);
            size = new Vector3(frontSensorWidth, 1, frontSensorDistance);
            BoxCollider front = new GameObject("FrontSensor").AddComponent<BoxCollider>();
            front.gameObject.layer = LayerMask.NameToLayer("AISensor");
            front.transform.SetParent(transform);
            front.transform.localRotation = Quaternion.Euler(Vector3.zero);
            front.transform.localPosition = Vector3.zero;
            front.center = center;
            front.size = size;
            front.isTrigger = true;
            frontSensor = front.gameObject.AddComponent<Sensor>();
            frontSensor.AddLayer(LayerMask.NameToLayer("Vehicle"));

            //Создаем левый датчик
            center = new Vector3(-leftSensorOffset, leftSensorHeight, 0);
            size = new Vector3(leftSensorWidth, 1, leftSensorDistance);
            BoxCollider left = new GameObject("LeftSensor").AddComponent<BoxCollider>();
            left.gameObject.layer = LayerMask.NameToLayer("AISensor");
            left.transform.SetParent(transform);
            left.transform.localRotation = Quaternion.Euler(Vector3.zero);
            left.transform.localPosition = Vector3.zero;
            left.center = center;
            left.size = size;
            left.isTrigger = true;
            leftSensor = left.gameObject.AddComponent<Sensor>();
            leftSensor.AddLayer(LayerMask.NameToLayer("Vehicle"));

            //Создаем правый датчик
            center = new Vector3(rightSensorOffset, rightSensorHeight, 0);
            size = new Vector3(rightSensorWidth, 1, rightSensorDistance);
            BoxCollider right = new GameObject("RightSensor").AddComponent<BoxCollider>();
            right.gameObject.layer = LayerMask.NameToLayer("AISensor");
            right.transform.SetParent(transform);
            right.transform.localRotation = Quaternion.Euler(Vector3.zero);
            right.transform.localPosition = Vector3.zero;
            right.center = center;
            right.size = size;
            right.isTrigger = true;
            rightSensor = right.gameObject.AddComponent<Sensor>();
            rightSensor.AddLayer(LayerMask.NameToLayer("Vehicle"));
        }


        void FixedUpdate()
        {
            UpdateTargetPosition();
            CalculateSpeedValues();
            Navigate();
            CheckFrontThreats();
            Recover();
        }


        void Navigate()
        {
            //Рассчитать вектор рулевого управления гоночной траектории
            Vector3 offset = racingLineTarget.position;
            offset += racingLineTarget.right * travelOffset;
            Vector3 local = transform.InverseTransformPoint(offset);
            float steerAngle = Mathf.Atan2(local.x, local.z) * Mathf.Rad2Deg;
            float steerValue = Mathf.Clamp(steerAngle * (steerSensitivity / 10), -1, 1) * Mathf.Sign(currentSpeed);

            //Рассчитать вектор рулевого управления гоночной траектории
            if (travelOffset != 0)
            {
                traveOffsetResetTimer += Time.deltaTime;

                if (traveOffsetResetTimer >= 5)
                {
                    travelOffset = Mathf.MoveTowards(travelOffset, 0, Time.deltaTime * 2);
                }
            }
            else
            {
                traveOffsetResetTimer = 0;
            }

            //Использовать разную чувствительность при ускорении или торможении
            float sensitivity = (currentSpeed > targetSpeed) ?
                brakeSensitivity / 10 : throttleSensitivity / 10;

            float throttleBrakeRatio = Mathf.Clamp((targetSpeed - currentSpeed) * sensitivity, -1, 1);

            //Установите входные значения          
            throttleInput = !reversing ? Mathf.Clamp(throttleBrakeRatio, 0, 1) : 0;
            brakeInput = !reversing ? (-1 * Mathf.Clamp(throttleBrakeRatio, -1, 0)) : 1;
            steerInput = !reversing ? steerValue : -steerValue;

            //Обработка отправки входных данных на контроллер транспортного средства
            if (aiInput != null)
            {
                if (RaceManager.instance != null)
                {
                    //Гонка началась, поэтому устанавливаем рассчитанные входные значения
                    if (RaceManager.instance.raceStarted)
                    {
                        aiInput.SetInputValues(throttleInput, brakeInput, steerInput, 0);
                    }
                    else
                    {
                        //Гонка еще не началась, так что либо нажимайте на обратный отсчет, либо оставайтесь в режиме ожидания
                        float revInput = 0;
                        if (revOnRaceCountdown && RaceManager.instance.isCountdownStarted)
                        {
                            revInput = Mathf.Repeat(Time.time * 2f, 1);
                        }

                        aiInput.SetInputValues(revInput, 0, 0, 1);
                    }
                }
            }
        }


        void CheckFrontThreats()
        {
            if (!IsThreatFront())
            {
                slowDownThreat = false;
                // // Debug.Log("CheckFrontThreats: Угрозы впереди не обнаружено.");
                return;
            }

            // Найти ближайшую угрозу
            closestThreat = GetClosestThreat(frontSensor.collidersInRange.ToArray());
            if (closestThreat == null)
            {
                // // Debug.LogWarning("CheckFrontThreats: Ближайшая угроза не найдена.");
                return;
            }

            // Определите расстояние до угрозы
            float distanceToThreat = Vector3.Distance(closestThreat.transform.position, transform.position);
            //Debug.Log($"CheckFrontThreats: Угроза обнаружена. Расстояние до угрозы: {distanceToThreat:F2} м.");

            // Получить информацию о границах
            Bounds bounds = new Bounds(closestThreat.bounds.center, closestThreat.bounds.size);
            float threatWidth = bounds.size.x;
            //  // Debug.Log($"CheckFrontThreats: Ширина угрозы: {threatWidth:F2} м.");

            // Проверяем, есть ли у нас преимущество в скорости
            float threatSpeed = 0;
            Rigidbody rigid = closestThreat.GetComponentInParent<Rigidbody>();
            if (rigid != null)
            {
                threatSpeed = rigid.velocity.magnitude * 3.6f; // Преобразуем в км/ч
                                                               // Debug.Log($"CheckFrontThreats: Скорость угрозы: {threatSpeed:F2} км/ч.");
            }
            else
            {
                //// Debug.LogWarning("CheckFrontThreats: Угроза не имеет компонента Rigidbody.");
            }

            // Если у нас есть преимущество в скорости, двигаемся так, чтобы избежать столкновения с угрозой
            if (currentSpeed > threatSpeed * 1.05f)
            {
                // Рассчитать кратчайший путь обхода угрозы
                var threatLocalDelta = transform.InverseTransformPoint(closestThreat.transform.position);
                float threatAngle = Mathf.Atan2(threatLocalDelta.x, threatLocalDelta.z);
                float bestDirection = -Mathf.Sign(threatAngle);

                // Вычислить позицию угрозы на трассе
                float threatTrackPosition = racingLineTarget.InverseTransformPoint(closestThreat.transform.position).x;

                if (bestDirection == 1)
                {
                    newTravelOffset = threatTrackPosition + threatWidth;
                    // Debug.Log("CheckFrontThreats: Обход угрозы вправо.");
                }

                if (bestDirection == -1)
                {
                    newTravelOffset = threatTrackPosition - threatWidth;
                    // Debug.Log("CheckFrontThreats: Обход угрозы влево.");
                }
            }

            // Сбавьте скорость, если мы подъедем слишком близко
            slowDownThreat = distanceToThreat < cautionDistance;
            if (slowDownThreat)
            {
                targetSpeed = threatSpeed;
                //Debug.Log($"CheckFrontThreats: Сбавляем скорость. Целевая скорость: {targetSpeed:F2} км/ч.");
            }

            // Lerp для нового смещения перемещения
            travelOffset = Mathf.Lerp(travelOffset, newTravelOffset, Time.deltaTime * 2);

            // Соблюдайте ограничения по трассе
            travelOffset = Mathf.Clamp(travelOffset, -racingLine.GetLeftWidth(racingLineNodeIndex), racingLine.GetRightWidth(racingLineNodeIndex));
            //// Debug.Log($"CheckFrontThreats: Новое смещение: {travelOffset:F2} м.");
        }



        void CalculateSpeedValues()
        {
            //Рассчитать целевую скорость
            if (racingLine != null)
            {
                racingLineNodeIndex = racingLine.GetNodeIndexAtDistance(racingLineDistance);

                if (!slowDownThreat)
                {
                    targetSpeed = racingLine.GetSpeedAtNode(racingLineNodeIndex);
                    targetSpeed *= speedModifier;
                }
            }

            //Рассчитать текущую скорость
            if (rigid != null)
            {
                currentSpeed = rigid.velocity.magnitude * 3.6f;
            }

            //Установите целевую скорость на скорость начала движения во время начала движения
            if (RaceManager.instance != null && RaceManager.instance.isRollingStart)
            {
                targetSpeed = RaceManager.instance.rollingStartSpeed;
            }

            //Установите целевую скорость на значение скорости после гонки, когда ИИ финишировал/дисквалифицировался
            if (racerStatistics != null)
            {
                if (racerStatistics.finished || racerStatistics.disqualified)
                {
                    if (RaceManager.instance != null)
                    {
                        targetSpeed *= RaceManager.instance.postRaceSpeedMultiplier;
                    }
                }
            }
        }


        void UpdateTargetPosition()
        {
            if (racingLine == null)
                return;

            //Обновить целевую линию гонки
            racingLineTarget.position = racingLine.GetRoutePoint(racingLineDistance + targetDistanceAhead).position;
            racingLineTarget.rotation = Quaternion.LookRotation(racingLine.GetRoutePoint(racingLineDistance + targetDistanceAhead).direction);
            racingLinePoint = racingLine.GetRoutePoint(racingLineDistance);
            Vector3 progressDelta = racingLinePoint.position - transform.position;

            if (Vector3.Dot(progressDelta, racingLinePoint.direction) < 0)
            {
                racingLineDistance += progressDelta.magnitude * 0.5f;
            }
            else if (Vector3.Dot(progressDelta, racingLinePoint.direction) > 2)
            {
                racingLineDistance -= progressDelta.magnitude * 0.5f;
            }

            //Сбрасывать расстояние в конце каждого круга
            if (racingLineDistance >= racingLine.distances[racingLine.distances.Length - 1])
            {
                racingLineDistance = 0;
            }

            //Рассчитать расстояние до цели на основе текущей скорости
            targetDistanceAhead = Mathf.Clamp((currentSpeed * 0.2f), racingLine.minTargetDistance, racingLine.maxTargetDistance);
        }


        void Recover()
        {
            if (!racerStatistics.started)
                return;

            if (currentSpeed <= 2)
            {
                recoverTimer += Time.deltaTime;
            }
            else
            {
                recoverTimer = 0;
                reverseTimer = 0;
                reversing = false;
            }

            if (recoverTimer >= 2)
            {
                //Предположим, что ИИ застрял, поэтому обратный ход
                reversing = true;

                //Попытка дать задний ход через некоторое время
                reverseTimer += Time.deltaTime;
                if (reverseTimer >= Random.Range(1f, 1.5f))
                {
                    reverseTimer = 0;
                    reversing = false;
                }
            }

            //Восстанавливаем транспортное средство, если оно застряло на определенное время
            if (recoverTimer > respawnWait)
            {
                RaceManager.instance.RespawnVehicle(transform);
            }
        }


        void OnCollisionEnter(Collision col)
        {
            var collisionLocalDelta = transform.InverseTransformPoint(col.transform.position);
            float angle = Mathf.Atan2(collisionLocalDelta.x, collisionLocalDelta.z);
            travelOffset = 1.5f * -Mathf.Sign(angle);
            travelOffset = Mathf.Clamp(travelOffset, -racingLine.GetLeftWidth(racingLineNodeIndex), racingLine.GetRightWidth(racingLineNodeIndex));
        }


        public bool IsThreatFront()
        {
            return frontSensor.collidersInRange.Count > 0;
        }



        public Collider GetClosestThreat(Collider[] colliders)
        {
            float closestDistanceSqr = Mathf.Infinity;
            Collider closest = colliders[0];

            if (colliders.Length == 1)
            {
                return colliders[0];
            }

            foreach (Collider c in colliders)
            {
                float distanceToTarget = (c.transform.position - transform.position).sqrMagnitude;

                if (distanceToTarget < closestDistanceSqr)
                {
                    closest = c;
                    closestDistanceSqr = distanceToTarget;
                }
            }

            return closest;
        }


        public void SetDifficulty(AiDifficulty difficulty)
        {
            throttleSensitivity = difficulty.throttleSensitivity;
            brakeSensitivity = difficulty.brakeSensitivity;
            steerSensitivity = difficulty.steerSensitivity;
            speedModifier = difficulty.speedModifier;
        }


        void OnDrawGizmos()
        {
            if (visualizeSensors)
            {
                Gizmos.matrix = transform.localToWorldMatrix;

                Vector3 center = new Vector3(0, frontSensorHeight, (frontSensorDistance / 2) + 0.5f);
                Vector3 size = new Vector3(frontSensorWidth, 1, frontSensorDistance);
                Gizmos.DrawWireCube(center, size);

                Vector3 left_center = new Vector3(-leftSensorOffset, leftSensorHeight, 0);
                Vector3 left_size = new Vector3(leftSensorWidth, 1, leftSensorDistance);
                Gizmos.DrawWireCube(left_center, left_size);

                Vector3 right_center = new Vector3(rightSensorOffset, rightSensorHeight, 0);
                Vector3 right_size = new Vector3(rightSensorWidth, 1, rightSensorDistance);
                Gizmos.DrawWireCube(right_center, right_size);
            }
        }
    }
}
