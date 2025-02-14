using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RGSK
{
    public class MenuMainPanel : MonoBehaviour
    {
        public UnityAction VehicleSelectedEvent; // Событие для уведомления о выборе автомобиля

        public QuickRacePanel quickRacePanel;
        public DailyPanel dailePanel;
        public EventPanel eventPanel;
        public MenuChampionshipPanel championshipPanel;
        public VehicleSelectionPanel vehicleSeletPanel;
        public CarPaintPanel carTuningPanel;             
        public MenuOptionsPanel optionsPanel;
        public MenuCareerPanel menuCareerPanel;
        public MenuIAPPanel iapPanel;
        public QuitPanel quitPanel;
        public PlayerSettings playerPanel;

        public Button quickRaceButton;
        public Button dailyButton;
        public Button eventButton;
        public Button championshipButton;
        public Button vehicleSelectButton;
        public Button carTuningButton;
        public Button optionsButton;
        public Button careerButton;
        public Button iapButton;
        public Button quitButton;
        public Button playerButton;

        private bool hasSelectedVehicle = false;

        void Start()
        {
            // Add button listeners
            if (dailyButton != null)
                dailyButton.onClick.AddListener(delegate { ShowPanel("Daily"); });

            if (eventButton != null)
                eventButton.onClick.AddListener(delegate { ShowPanel("Event"); });

            if (quickRaceButton != null)
                quickRaceButton.onClick.AddListener(delegate { ShowPanel("QuickRace"); });

            if (championshipButton != null)
                championshipButton.onClick.AddListener(delegate { ShowPanel("Championship"); });

            if (vehicleSelectButton != null)
                vehicleSelectButton.onClick.AddListener(delegate { ShowPanel("VehicleSelect"); });

            if (carTuningButton != null)
                carTuningButton.onClick.AddListener(delegate { ShowPanel("Car Tuning"); });

            if (optionsButton != null)
                optionsButton.onClick.AddListener(delegate { ShowPanel("Options"); });

            if (careerButton != null)
                careerButton.onClick.AddListener(OnCareerButtonClicked);

            if (iapButton != null)
                iapButton.onClick.AddListener(delegate { ShowPanel("IAP"); });

            if (playerButton != null)
                playerButton.onClick.AddListener(delegate { ShowPanel("Player"); });

            if (quitButton != null)
                quitButton.onClick.AddListener(Quit);
        }

        // Пример вызова события, когда автомобиль выбран
        public void SelectVehicle(string vehicleID)
        {
            // Логика выбора автомобиля
            Debug.Log($"Vehicle {vehicleID} selected.");
            PlayerData.instance.SaveSelectedVehicle(vehicleID);

            // Вызываем событие
            VehicleSelectedEvent?.Invoke();
        }

        public void  Telegram()
        {
            Application.OpenURL("https://t.me/rru_racing");
        }

        public void privacy()
        {
            Application.OpenURL("https://sevenwolf.uz/Privacy.html");
        }


        private void OnCareerButtonClicked()
        {
            // Проверяем, выбрана ли машина у игрока
            if (string.IsNullOrEmpty(PlayerData.instance.playerData.vehicleID))
            {
                // Если машина не выбрана, переходим в панель выбора машины
                ShowPanel("VehicleSelect");
                Debug.Log("No vehicle selected. Redirecting to Vehicle Selection.");
            }
            else
            {
                // Если машина выбрана, переходим в меню карьеры
                ShowPanel("CareerStage");
                Debug.Log($"Selected Vehicle: {PlayerData.instance.playerData.vehicleID}. Redirecting to Career Menu.");
            }
        }

        private void HandleVehicleSelected()
        {
            // Логика при выборе автомобиля
            hasSelectedVehicle = true;

            // Отписываемся от события, чтобы избежать дублирования
            vehicleSeletPanel.VehicleSelectedEvent -= HandleVehicleSelected;

            // Переход в панель карьеры
            ShowPanel("CareerStage");
        }



        void ShowPanel(string panel)
        {
            gameObject.SetActive(false);

            switch (panel)
            {
                case "QuickRace":
                    quickRacePanel?.gameObject.SetActive(true);
                    break;
                case "Daily":
                    dailePanel?.gameObject.SetActive(true);
                    break;
                case "Event":
                    eventPanel?.gameObject.SetActive(true);
                    break;
                case "Championship":
                    championshipPanel?.gameObject.SetActive(true);
                    break;
                case "VehicleSelect":
                    vehicleSeletPanel?.gameObject.SetActive(true);
                    break;
                case "Car Tuning":
                    carTuningPanel?.gameObject.SetActive(true);
                    break;
                case "Options":
                    optionsPanel?.gameObject.SetActive(true);
                    break;
                case "CareerStage":
                    menuCareerPanel?.gameObject.SetActive(true);
                    break;
                case "IAP":
                    iapPanel?.gameObject.SetActive(true);
                    break;
                case "Player":
                    playerPanel?.gameObject.SetActive(true);
                    break;
            }
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
