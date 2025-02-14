using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace RGSK
{
    public class VehicleSelectionPanel : MonoBehaviour
    {
        private MenuVehicleInstantiator vehicleInstantiator;
        private IEnumerator moveBars;

        [Header("Tech Specs")]
        public Image topSpeedBar;
        public Image accelerationBar;
        public Image handlingBar;
        public Image brakingBar;

        public Text BrandName; // Марка автомобиля

        public Text ModelName; // Модель автомобиля

        public Text year; // Год выпуска

        public Text price; // Цена автомобиля

        public Text TopSpeed; // Максимальная скорость

        public Text Accel; // Ускорение

        public Text BHP; // Лошадиные силы

        public Text Mass; // Масса автомобиля

        public Text EngineType; // Тип двигателя

        public Text DriveTrain; // Тип привода

        public Text CarClassNames; // Класс Авто

        

        public Button nextVehicle;
        public Button previousVehicle;
        public Button selectVehicle;
        public Button customizeVehicle;
        public GameObject colorPanel;
        public Button buyVehicle; // Кнопка покупки машины

        [Header("Back")]
        public Button backButton;
        public GameObject previousPanel;

        [Header("Lock Icon")]
        public GameObject lockIcon; // Иконка замка, если машина заблокирована

        [Header("UI Elements")]
        public Text playerCurrencyText; // Отображение количества валюты игрока
        public GameObject insufficientFundsPanel; // Панель с сообщением о недостаточном количестве средств

        public Action VehicleSelectedEvent { get; internal set; }

        void Start()
        {
            vehicleInstantiator = FindObjectOfType<MenuVehicleInstantiator>();
            if (vehicleInstantiator == null)
                return;

            //Add button listeners
            if (nextVehicle != null)
            {
                nextVehicle.onClick.AddListener(delegate { vehicleInstantiator.CycleVehicles(1); });
                nextVehicle.onClick.AddListener(delegate { UpdateVehicleInformation(); });
            }

            if (previousVehicle != null)
            {
                previousVehicle.onClick.AddListener(delegate { vehicleInstantiator.CycleVehicles(-1); });
                previousVehicle.onClick.AddListener(delegate { UpdateVehicleInformation(); });
            }

            if (selectVehicle != null)
            {
                selectVehicle.onClick.AddListener(delegate { vehicleInstantiator.SetSelectedVehicle(); });
            }

            if (buyVehicle != null)
            {
                buyVehicle.onClick.AddListener(delegate { BuyVehicle(); });
            }

            if (backButton != null)
            {
                backButton.onClick.AddListener(delegate { Back(); });
            }

            UpdateVehicleInformation();
            UpdatePlayerCurrencyUI();
        }

        public void UpdateVehicleInformation()
        {
            if (vehicleInstantiator == null)
                return;
            // Обновляем название модели автомобиля
            if (ModelName != null)
            {
                ModelName.text = vehicleInstantiator.GetVehicleData().ModelName;
            }

            if (!vehicleInstantiator.HasVehicleDatabase())
                return;

            // Получаем данные о текущем транспортном средстве
            VehicleDatabase.VehicleData currentVehicle = vehicleInstantiator.GetVehicleData();

            // Обновляем название марки автомобиля
            if (BrandName != null)
            {
                BrandName.text = currentVehicle.BrandName;
            }

           

            // Обновляем год выпуска автомобиля
            if (year != null)
            {
                year.text = currentVehicle.year;
            }

            // Обновляем цену автомобиля
            if (price != null)
            {
                price.text = currentVehicle.price;
            }

            // Обновляем максимальную скорость автомобиля
            if (TopSpeed != null)
            {
                TopSpeed.text = currentVehicle.TopSpeed;
            }

            // Обновляем ускорение автомобиля
            if (Accel != null)
            {
                Accel.text = currentVehicle.Accel;
            }

            // Обновляем мощность автомобиля
            if (BHP != null)
            {
                BHP.text = currentVehicle.BHP;
            }

            // Обновляем массу автомобиля
            if (Mass != null)
            {
                Mass.text = currentVehicle.Mass;
            }

            // Обновляем тип двигателя автомобиля
            if (EngineType != null)
            {
                EngineType.text = currentVehicle.EngineType;
            }

            // // Класс Авто
            if (CarClassNames != null)
            {
                CarClassNames.text = currentVehicle.CarClassNames; 
            }

            

            // Обновляем тип привода автомобиля
            if (DriveTrain != null)
            {
                DriveTrain.text = currentVehicle.DriveTrain;
            }

            // Показываем/скрываем иконку замка в зависимости от того, заблокирован ли автомобиль
            if (lockIcon != null)
            {
                lockIcon.SetActive(currentVehicle.isLocked);
            }

            // Показываем/скрываем кнопку покупки, если автомобиль заблокирован
            if (buyVehicle != null)
            {
                buyVehicle.gameObject.SetActive(currentVehicle.isLocked);
                buyVehicle.GetComponentInChildren<Text>().text = "" + currentVehicle.unlockCost + " CR";
            }

            // Показываем/скрываем кнопку выбора, если автомобиль разблокирован
            if (selectVehicle != null)
            {
                selectVehicle.gameObject.SetActive(!currentVehicle.isLocked);
            }

            // Показываем/скрываем кнопку изменения окраски, если автомобиль разблокирован
            if (customizeVehicle != null)
            {
                customizeVehicle.gameObject.SetActive(!currentVehicle.isLocked);
            }

            // Показываем/скрываем кнопку изменения окраски, если автомобиль разблокирован
            if (colorPanel != null)
            {
                colorPanel.gameObject.SetActive(!currentVehicle.isLocked);
            }

            // Обновляем отображение полос производительности автомобиля
            if (moveBars != null)
            {
                StopCoroutine(moveBars);
            }

            moveBars = MovePerformanceBars(
                currentVehicle.topSpeed,
                currentVehicle.acceleration,
                currentVehicle.handling,
                currentVehicle.braking);

            StartCoroutine(moveBars);

            // Показываем/скрываем кнопку "следующий автомобиль", если это последний автомобиль в списке
            if (nextVehicle != null)
            {
                nextVehicle.gameObject.SetActive(!vehicleInstantiator.IsLastVehicleInList());
            }

            // Показываем/скрываем кнопку "предыдущий автомобиль", если это первый автомобиль в списке
            if (previousVehicle != null)
            {
                previousVehicle.gameObject.SetActive(!vehicleInstantiator.IsFirstVehicleInList());
            }
        }



        public void BuyVehicle()
        {
            if (vehicleInstantiator == null)
                return;

            if (!vehicleInstantiator.HasVehicleDatabase())
                return;

            VehicleDatabase.VehicleData currentVehicle = vehicleInstantiator.GetVehicleData();

            if (PlayerData.instance != null && currentVehicle.isLocked)
            {
                if (PlayerData.instance.playerData.playerCurrency >= currentVehicle.unlockCost)
                {
                    // Списание стоимости автомобиля
                    PlayerData.instance.AddPlayerCurrecny(-currentVehicle.unlockCost);

                    // Разблокировка автомобиля в памяти
                    currentVehicle.isLocked = false;

                    // Сохранение разблокированного автомобиля
                    PlayerData.instance.UnlockItem(currentVehicle.uniqueID);

                    // Обновление интерфейса
                    UpdatePlayerCurrencyUI();
                    UpdateVehicleInformation();

                    Debug.Log("Машина " + currentVehicle.ModelName + " успешно куплена.");
                }
                else
                {
                    // Показать панель о недостатке средств
                    if (insufficientFundsPanel != null)
                    {
                        insufficientFundsPanel.SetActive(true);
                        StartCoroutine(HideInsufficientFundsPanel());
                    }
                    Debug.Log("Недостаточно средств для покупки " + currentVehicle.ModelName);
                }
            }
        }


        private void UpdatePlayerCurrencyUI()
        {
            if (PlayerData.instance != null && playerCurrencyText != null)
            {
                playerCurrencyText.text = "$" + PlayerData.instance.playerData.playerCurrency;
            }
        }

        private IEnumerator HideInsufficientFundsPanel()
        {
            yield return new WaitForSeconds(2f);
            if (insufficientFundsPanel != null)
            {
                insufficientFundsPanel.SetActive(false);
            }
        }

        IEnumerator MovePerformanceBars(float speed, float accel, float handle, float brake)
        {
            float timer = 0;
            float lerpSpeed = 1;

            while (timer < lerpSpeed)
            {
                timer += Time.deltaTime;

                if (topSpeedBar != null)
                {
                    topSpeedBar.fillAmount = Mathf.Lerp(topSpeedBar.fillAmount, speed, timer / lerpSpeed);
                }

                if (accelerationBar != null)
                {
                    accelerationBar.fillAmount = Mathf.Lerp(accelerationBar.fillAmount, accel, timer / lerpSpeed);
                }

                if (handlingBar != null)
                {
                    handlingBar.fillAmount = Mathf.Lerp(handlingBar.fillAmount, handle, timer / lerpSpeed);
                }

                if (brakingBar != null)
                {
                    brakingBar.fillAmount = Mathf.Lerp(brakingBar.fillAmount, brake, timer / lerpSpeed);
                }

                yield return null;
            }
        }

        void RevertPlayerVehicle()
        {
            if (vehicleInstantiator == null)
                return;

            if (!vehicleInstantiator.HasVehicleDatabase())
                return;

            //Revert the player vehicle if the selected vehicle does not match the saved vehicle
            if (PlayerData.instance.playerData.vehicleID != vehicleInstantiator.GetVehicleData().uniqueID)
                vehicleInstantiator.RevertPlayerVehicle();
        }

        public void Back()
        {
            RevertPlayerVehicle();

            if (previousPanel != null)
            {
                gameObject.SetActive(false);
                previousPanel.SetActive(true);
            }
        }

        void OnEnable()
        {
            UpdateVehicleInformation();
            UpdatePlayerCurrencyUI();
        }
    }
}
