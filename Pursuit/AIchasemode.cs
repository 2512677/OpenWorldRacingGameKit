using RGSK;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AIchasemode : MonoBehaviour
{
    public int healthbar = 4; // Здоровье (звёзды)
    public GameObject[] destoryeffects; // Эффекты при уничтожении
    public GameObject[] hbpart; // Визуальные звёзды
    public Rigidbody Rb; // Rigidbody машины
    private List<GameObject> vehicles = new List<GameObject>();

    private bool isStopped = false; // Флаг остановки

    void Start()
    {
        vehicles = FindObjectsOfType<RCC_CarControllerV3>()
                    .Where(c => !c.CompareTag("Player")) // Исключаем машину игрока
                    .Select(c => c.gameObject)
                    .ToList();

        Debug.Log($"Найдено {vehicles.Count} машин для обработки.");
    }

    private void Update()
    {
        // Проверяем нажатие клавиши L
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Нажата клавиша L. Устанавливаем здоровье на 0.");
            healthbar = 0;
            TestStopAllVehicles(); // Останавливаем все машины
        }
    }

    private void TestStopAllVehicles()
    {
        foreach (GameObject vehicle in vehicles)
        {
            if (vehicle != null)
            {
                Debug.Log($"Проверяем остановку для машины {vehicle.name}");
                StopVehicle(vehicle);
            }
            else
            {
                Debug.LogError("Один из автомобилей в списке оказался null!");
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Столкновение с игроком! Текущее здоровье: {healthbar}");

            // Уменьшаем здоровье
            healthbar--;

            Debug.Log($"Здоровье после уменьшения: {healthbar}");

            // Обновляем визуальное отображение здоровья
            UpdateHealthbarVisual();

            // Если здоровье достигло 0, выполняем остановку машины
            if (healthbar <= 0)
            {
                Debug.Log("Здоровье закончилось. Принудительно останавливаем машину!");

                RCC_CarControllerV3 carController = GetComponent<RCC_CarControllerV3>();
                if (carController != null)
                {
                    Debug.Log("RCC_CarControllerV3 найден. Сбрасываем входы.");
                    carController.gasInput = 0f;
                    carController.brakeInput = 1f;
                    carController.steerInput = 0f;
                    carController.handbrakeInput = 1f;
                    carController.engineRunning = false;

                    Debug.Log($"Машина {gameObject.name} успешно остановлена через RCC_CarControllerV3.");
                }
                else
                {
                    Debug.LogError("RCC_CarControllerV3 не найден! Проверьте компоненты объекта.");
                }

                // Принудительное удаление управления
                RCCAIInput aiInput = GetComponent<RCCAIInput>();
                if (aiInput != null)
                {
                    Debug.Log("RCCAIInput найден. Удаляем.");
                    Destroy(aiInput);
                }
                else
                {
                    Debug.LogWarning("RCCAIInput не найден!");
                }

                AiLogic aiLogic = GetComponent<AiLogic>();
                if (aiLogic != null)
                {
                    Debug.Log("AiLogic найден. Удаляем.");
                    Destroy(aiLogic);
                }
                else
                {
                    Debug.LogWarning("AiLogic не найден!");
                }

                // Принудительная остановка Rigidbody
                Rigidbody rb = GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Debug.Log("Rigidbody найден. Принудительно останавливаем.");
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = true; // Временно отключаем физику
                    rb.isKinematic = false; // Возвращаем для дальнейших взаимодействий
                }
                else
                {
                    Debug.LogWarning("Rigidbody не найден!");
                }

                // Активируем эффекты уничтожения
                Debug.Log("Активируем эффекты уничтожения.");
                ActivateEffects();

                // Отображение полного статуса машины
                Debug.Log($"Финальный статус для {gameObject.name}:");
                Debug.Log($"gasInput: {carController?.gasInput}, brakeInput: {carController?.brakeInput}, steerInput: {carController?.steerInput}");
                Debug.Log($"velocity: {rb?.velocity}, angularVelocity: {rb?.angularVelocity}");
            }
        }
    }









    private void StopVehicle(GameObject vehicle)
    {
        if (vehicle == null)
        {
            Debug.LogError("Переданный объект в StopVehicle оказался null!");
            return;
        }

        RCC_CarControllerV3 carController = vehicle.GetComponent<RCC_CarControllerV3>();
        if (carController != null)
        {
            // Сбрасываем входы управления
            carController.gasInput = 0f;
            carController.brakeInput = 1f; // Полное торможение
            carController.steerInput = 0f;
            carController.handbrakeInput = 1f; // Ручной тормоз

            // Останавливаем двигатель
            carController.engineRunning = false;

            Debug.Log($"Машина {vehicle.name} успешно остановлена.");
        }

        // Отключение AI Logic
        AiLogic aiLogic = vehicle.GetComponent<AiLogic>();
        if (aiLogic != null)
        {
            Destroy(aiLogic);
            Debug.Log($"AI Logic отключён для {vehicle.name}");
        }

        // Удаление AI Input
        RCCAIInput aiInput = vehicle.GetComponent<RCCAIInput>();
        if (aiInput != null)
        {
            Destroy(aiInput);
            Debug.Log($"AI управление удалено для {vehicle.name}");
        }

        // Принудительная остановка Rigidbody
        Rigidbody rb = vehicle.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;       // Сброс линейной скорости
            rb.angularVelocity = Vector3.zero; // Сброс угловой скорости
            rb.isKinematic = true;           // Полная остановка физики
            rb.isKinematic = false;          // Снятие фиксации для дальнейших действий

            Debug.Log($"Rigidbody остановлен для {vehicle.name}");
        }
    }




    // Обновляем визуальное отображение здоровья (звёзд)
    private void UpdateHealthbarVisual()
    {
        for (int i = 0; i < hbpart.Length; i++)
        {
            if (i < healthbar)
            {
                hbpart[i].SetActive(true); // Включаем звёзды, соответствующие текущему здоровью
            }
            else
            {
                hbpart[i].SetActive(false); // Выключаем звёзды, которые больше неактивны
            }
        }
    }

    private void ActivateEffects()
    {
        Debug.Log("Активируем эффекты уничтожения.");
        foreach (var effect in destoryeffects)
        {
            if (effect != null)
                effect.SetActive(true); // Включаем эффекты
        }
    }
}