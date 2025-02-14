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

        public Text BrandName; // ����� ����������

        public Text ModelName; // ������ ����������

        public Text year; // ��� �������

        public Text price; // ���� ����������

        public Text TopSpeed; // ������������ ��������

        public Text Accel; // ���������

        public Text BHP; // ��������� ����

        public Text Mass; // ����� ����������

        public Text EngineType; // ��� ���������

        public Text DriveTrain; // ��� �������

        public Text CarClassNames; // ����� ����

        

        public Button nextVehicle;
        public Button previousVehicle;
        public Button selectVehicle;
        public Button customizeVehicle;
        public GameObject colorPanel;
        public Button buyVehicle; // ������ ������� ������

        [Header("Back")]
        public Button backButton;
        public GameObject previousPanel;

        [Header("Lock Icon")]
        public GameObject lockIcon; // ������ �����, ���� ������ �������������

        [Header("UI Elements")]
        public Text playerCurrencyText; // ����������� ���������� ������ ������
        public GameObject insufficientFundsPanel; // ������ � ���������� � ������������� ���������� �������

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
            // ��������� �������� ������ ����������
            if (ModelName != null)
            {
                ModelName.text = vehicleInstantiator.GetVehicleData().ModelName;
            }

            if (!vehicleInstantiator.HasVehicleDatabase())
                return;

            // �������� ������ � ������� ������������ ��������
            VehicleDatabase.VehicleData currentVehicle = vehicleInstantiator.GetVehicleData();

            // ��������� �������� ����� ����������
            if (BrandName != null)
            {
                BrandName.text = currentVehicle.BrandName;
            }

           

            // ��������� ��� ������� ����������
            if (year != null)
            {
                year.text = currentVehicle.year;
            }

            // ��������� ���� ����������
            if (price != null)
            {
                price.text = currentVehicle.price;
            }

            // ��������� ������������ �������� ����������
            if (TopSpeed != null)
            {
                TopSpeed.text = currentVehicle.TopSpeed;
            }

            // ��������� ��������� ����������
            if (Accel != null)
            {
                Accel.text = currentVehicle.Accel;
            }

            // ��������� �������� ����������
            if (BHP != null)
            {
                BHP.text = currentVehicle.BHP;
            }

            // ��������� ����� ����������
            if (Mass != null)
            {
                Mass.text = currentVehicle.Mass;
            }

            // ��������� ��� ��������� ����������
            if (EngineType != null)
            {
                EngineType.text = currentVehicle.EngineType;
            }

            // // ����� ����
            if (CarClassNames != null)
            {
                CarClassNames.text = currentVehicle.CarClassNames; 
            }

            

            // ��������� ��� ������� ����������
            if (DriveTrain != null)
            {
                DriveTrain.text = currentVehicle.DriveTrain;
            }

            // ����������/�������� ������ ����� � ����������� �� ����, ������������ �� ����������
            if (lockIcon != null)
            {
                lockIcon.SetActive(currentVehicle.isLocked);
            }

            // ����������/�������� ������ �������, ���� ���������� ������������
            if (buyVehicle != null)
            {
                buyVehicle.gameObject.SetActive(currentVehicle.isLocked);
                buyVehicle.GetComponentInChildren<Text>().text = "" + currentVehicle.unlockCost + " CR";
            }

            // ����������/�������� ������ ������, ���� ���������� �������������
            if (selectVehicle != null)
            {
                selectVehicle.gameObject.SetActive(!currentVehicle.isLocked);
            }

            // ����������/�������� ������ ��������� �������, ���� ���������� �������������
            if (customizeVehicle != null)
            {
                customizeVehicle.gameObject.SetActive(!currentVehicle.isLocked);
            }

            // ����������/�������� ������ ��������� �������, ���� ���������� �������������
            if (colorPanel != null)
            {
                colorPanel.gameObject.SetActive(!currentVehicle.isLocked);
            }

            // ��������� ����������� ����� ������������������ ����������
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

            // ����������/�������� ������ "��������� ����������", ���� ��� ��������� ���������� � ������
            if (nextVehicle != null)
            {
                nextVehicle.gameObject.SetActive(!vehicleInstantiator.IsLastVehicleInList());
            }

            // ����������/�������� ������ "���������� ����������", ���� ��� ������ ���������� � ������
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
                    // �������� ��������� ����������
                    PlayerData.instance.AddPlayerCurrecny(-currentVehicle.unlockCost);

                    // ������������� ���������� � ������
                    currentVehicle.isLocked = false;

                    // ���������� ����������������� ����������
                    PlayerData.instance.UnlockItem(currentVehicle.uniqueID);

                    // ���������� ����������
                    UpdatePlayerCurrencyUI();
                    UpdateVehicleInformation();

                    Debug.Log("������ " + currentVehicle.ModelName + " ������� �������.");
                }
                else
                {
                    // �������� ������ � ���������� �������
                    if (insufficientFundsPanel != null)
                    {
                        insufficientFundsPanel.SetActive(true);
                        StartCoroutine(HideInsufficientFundsPanel());
                    }
                    Debug.Log("������������ ������� ��� ������� " + currentVehicle.ModelName);
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
