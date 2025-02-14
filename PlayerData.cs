using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.Events;
using System;


namespace RGSK
{
    public class PlayerData : MonoBehaviour
    {
        public static PlayerData instance; // Ссылка на текущий экземпляр PlayerData
        public static UnityAction OnPlayerLevelUp; // Событие при повышении уровня игрока
        public static PlayerData Instance { get; private set; }
        public int currentDailyDay = 1; // Текущий день дейликов
        public string lastDailyRaceTime; // Время последнего прохождения гонки



        [Header("Default Values")]
        public DataContainer defaultValues = new DataContainer(); // Начальные значения данных игрока

        [Header("Player Data")]
        public DataContainer playerData = new DataContainer(); // Текущие данные игрока





        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject); // Не уничтожать объект при смене сцен
            }
            else if (instance != this)
            {
                Destroy(gameObject); // Уничтожаем дублирующий объект
            }

            LoadData(); // Загрузка данных из файла
        }

        public void SaveDailyProgress()
        {
            PlayerPrefs.SetInt("CurrentDailyDay", currentDailyDay);
            PlayerPrefs.SetString("LastDailyRaceTime", lastDailyRaceTime);
            PlayerPrefs.Save();
        }

        public void LoadDailyProgress()
        {
            currentDailyDay = PlayerPrefs.GetInt("CurrentDailyDay", 1);
            lastDailyRaceTime = PlayerPrefs.GetString("LastDailyRaceTime", "");
        }





        public void SaveRaceResult(string raceID)
        {
            Debug.Log($"SaveRaceResult called for raceID: {raceID}");
            if (!playerData.completedRaces.Contains(raceID))
            {
                playerData.completedRaces.Add(raceID);
                SaveData();
                Debug.Log($"Гонка '{raceID}' сохранена. Текущий список: {string.Join(", ", playerData.completedRaces)}");
            }
            else
            {
                Debug.Log($"Гонка '{raceID}' уже была завершена.");
            }
        }

        // Сохранить попытку (win/lose) и время (UTC)
        public void SaveDailyRaceAttempt(int dayIndex, bool isWin)
        {
            string timeKey = $"DailyRace_{dayIndex}_LastTime";
            string timeValue = DateTime.UtcNow.ToString("o");
            PlayerPrefs.SetString(timeKey, timeValue);

            string winKey = $"DailyRace_{dayIndex}_IsWin";
            int winVal = isWin ? 1 : 0;
            PlayerPrefs.SetInt(winKey, winVal);

            PlayerPrefs.Save();
        }

        // Получить время последней попытки (UTC)
        public DateTime GetLastAttemptTime(int dayIndex)
        {
            string key = $"DailyRace_{dayIndex}_LastTime";
            string saved = PlayerPrefs.GetString(key, "");
            if (string.IsNullOrEmpty(saved))
                return DateTime.MinValue;

            if (DateTime.TryParse(saved, out DateTime parsed))
                return parsed;

            return DateTime.MinValue;
        }

        // Узнать, выигран ли день
        
        public bool IsDailyRaceWin(int dayIndex)
        {
            string winKey = $"DailyRace_{dayIndex}_IsWin";
            int val = PlayerPrefs.GetInt(winKey, -1);
            // -1 => не играли вообще, 0 => проигрыш, 1 => победа

            if (val == 1)
                return true; // Победа
            else
                return false;
        }




        public void AddCurrency(float amount)
        {
            //Debug.Log($"AddCurrency called: oldBalance={playerData.playerCurrency}, plus={amount}");
            playerData.playerCurrency += amount;
            SaveData();
        }

        public bool SpendCurrency(float amount)
        {
            if (playerData.playerCurrency >= amount)
            {
                playerData.playerCurrency -= amount;
                SaveData();
                return true; // Покупка успешна
            }
            return false; // Недостаточно средств
        }

        // Метод для добавления очков опыта
        public void AddXP(float amount)
        {
            playerData.totalPlayerXP += amount;
            // Debug.Log($"PlayerData: Добавлено {amount} XP. Общий XP: {playerData.totalPlayerXP}");
            SaveData();
        }



        private void Start()
        {
            ApplySavedCarColor();
            LoadData(); // Загружаем данные игрока из файла
            FindObjectOfType<PlayerSettings>()?.UpdateUIToMatchSettings(); // Обновляем интерфейс после загрузки
            // Загружаем сохранённые данные при старте
            LoadData();
        }


        // Метод для повышения уровня
        private void LevelUp()
        {
            playerData.playerXPLevel++; // Обращаемся через playerData
            playerData.totalPlayerXP = 0; // Сбрасываем текущий опыт
            Debug.Log($"PlayerData: Уровень повышен! Новый уровень: {playerData.playerXPLevel}");
        }




        private float GetXPForNextLevel()
        {
            return playerData.playerXPLevel * 100; // Всё обращение через playerData
        }


        // Добавление валюты игроку
        public void AddPlayerCurrecny(float amount)
        {
            playerData.playerCurrency += amount;
            SaveData();

            Debug.Log($"Player currency updated to: {playerData.playerCurrency}");

            // Обновляем интерфейс валюты
            var playerSettings = FindObjectOfType<PlayerSettings>();
            if (playerSettings != null)
            {
                playerSettings.UpdateUIToMatchSettings();
                Debug.Log("PlayerSettings UI updated.");
            }

            // Обновляем интерфейс событий
            var careerManager = FindObjectOfType<CareerManager>();
            if (careerManager != null)
            {
                careerManager.UpdateEventUI();
                Debug.Log("Event UI updated.");
            }
        }

        // Добавление очков опыта игроку
        public void AddPlayerXP(float points)
        {
            playerData.totalPlayerXP += points; // Увеличиваем общий опыт
            playerData.currentLevelXP += points; // Увеличиваем опыт на текущем уровне

            if (playerData.currentLevelXP >= playerData.nextLevelXP) // Проверка на повышение уровня
            {
                AddPlayerXPLevel();
            }

            SaveData(); // Сохранение данных
        }

        // Повышение уровня игрока
        void AddPlayerXPLevel()
        {
            playerData.playerXPLevel++; // Увеличение уровня
            playerData.currentLevelXP = 0; // Сброс опыта текущего уровня
            playerData.nextLevelXP *= playerData.nextLevelXpMultiplier; // Увеличение требуемого опыта для следующего уровня
        }

        // Разблокировка предмета по его идентификатору
        public void UnlockItem(string itemID)
        {
            // Если уже в списке, выходим
            if (playerData.items.Contains(itemID))
                return;

            // Добавляем
            playerData.items.Add(itemID);

            // Разблокируем в VehicleDatabase
            VehicleDatabase.Instance?.UnlockVehicle(itemID);

            // Сохраняем
            SaveData();
        }




        // Сохранение имени игрока
        public void SavePlayerName(string newName)
        {
            playerData.playerName = newName; // Обновление имени игрока
            SaveData(); // Сохранение данных
        }

        // Сохранение национальности игрока
        public void SavePlayerNationality(Nationality newNationality)
        {
            playerData.playerNationality = newNationality; // Обновление национальности
            SaveData(); // Сохранение данных
        }

        // Сохранение выбранного транспортного средства
        public void SaveSelectedVehicle(string id)
        {
            playerData.vehicleID = id; // Сохранение ID транспортного средства
            SaveData(); // Сохранение данных
        }

        // Проверка, разблокирован ли предмет по его идентификатору
        public bool IsItemUnlocked(string id)
        {
            return playerData.items != null && playerData.items.Contains(id);
        }

        // Метод для добавления SPB
        public void AddSPB(int amount)
        {
            Debug.Log($"Добавление SpeedBoost: {amount}");
            playerData.speedBoost += amount;
            SaveData();
            Debug.Log($"Новый баланс SpeedBoost: {playerData.speedBoost}");
        }

        // Метод для получения текущего SPB
        public int GetSPB()
        {
            return (int)playerData.speedBoost; // Убедитесь, что значение всегда возвращается
        }


        // Разблокировка предмета
        public void AddItem(string item)
        {
            if (playerData.items == null)
            {
                playerData.items = new List<string>(); // Инициализация списка, если он отсутствует
            }

            if (!playerData.items.Contains(item))
            {
                playerData.items.Add(item); // Добавляем предмет, если он не был добавлен ранее
                SaveData(); // Сохраняем изменения
                Debug.Log($"PlayerData: Предмет '{item}' добавлен в список разблокированных.");
            }
            else
            {
                Debug.Log($"PlayerData: Предмет '{item}' уже существует в списке.");
            }
        }


        public void UnlockStage(string stageName)
        {
            if (!playerData.unlockedStages.Contains(stageName))
            {
                playerData.unlockedStages.Add(stageName);
                SaveData();
                Debug.Log($"PlayerData: Stage {stageName} сохранён как разблокированный. Все разблокированные уровни: {string.Join(", ", playerData.unlockedStages)}");
            }
            else
            {
                Debug.Log($"PlayerData: Stage {stageName} уже разблокирован.");
            }
        }


        // Сохранение данных игрока в файл
        public void SaveData()
        {
            try
            {
                Debug.Log("SaveData: Сохраняем данные игрока...");
                Debug.Log($"Unlocked Stages Before Save: {string.Join(", ", playerData.unlockedStages)}");

                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Create(Application.persistentDataPath + "/savefile.dat");

                DataContainer data = new DataContainer
                {
                    playerName = playerData.playerName,
                    playerNationality = playerData.playerNationality,
                    playerCurrency = playerData.playerCurrency,
                    playerXPLevel = playerData.playerXPLevel,
                    totalPlayerXP = playerData.totalPlayerXP,
                    currentLevelXP = playerData.currentLevelXP,
                    nextLevelXP = playerData.nextLevelXP,
                    items = new List<string>(playerData.items),
                    vehicleID = playerData.vehicleID,
                    unlockedStages = new List<string>(playerData.unlockedStages),
                    speedBoost = playerData.speedBoost,
                    completedRaces = new List<string>(playerData.completedRaces)
                };

                bf.Serialize(file, data);
                file.Close();

                Debug.Log($"PlayerData успешно сохранён. Unlocked Stages After Save: {string.Join(", ", data.unlockedStages)}");
            }
            catch (Exception e)
            {
                Debug.LogError("Error saving data: " + e.Message);
            }
        }


        public void LoadData()
        {
            if (!File.Exists(Application.persistentDataPath + "/savefile.dat"))
            {
                Debug.LogWarning("Save file not found, resetting data to defaults.");
                ResetData();
                return;
            }

            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/savefile.dat", FileMode.Open);

                DataContainer data = (DataContainer)bf.Deserialize(file);
                file.Close();

                playerData.playerName = data.playerName;
                playerData.playerNationality = data.playerNationality;
                playerData.playerCurrency = data.playerCurrency;
                playerData.playerXPLevel = data.playerXPLevel;
                playerData.totalPlayerXP = data.totalPlayerXP;
                playerData.currentLevelXP = data.currentLevelXP;
                playerData.nextLevelXP = data.nextLevelXP;
                playerData.items = new List<string>(data.items);
                playerData.vehicleID = data.vehicleID;
                playerData.unlockedStages = data.unlockedStages;
                playerData.speedBoost = data.speedBoost;
                playerData.completedRaces = new List<string>(data.completedRaces);

                Debug.Log($"PlayerData успешно загружен. Unlocked Stages After Load: {string.Join(", ", playerData.unlockedStages)}");
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading data: " + e.Message);
            }
        }


        public void ResetData()
        {
            playerData.playerName = defaultValues.playerName; // Установка имени по умолчанию
            playerData.playerNationality = Nationality.Other; // Установка национальности по умолчанию
            playerData.playerCurrency = defaultValues.playerCurrency; // Установка валюты по умолчанию
            playerData.totalPlayerXP = defaultValues.totalPlayerXP; // Установка общего опыта по умолчанию
            playerData.playerXPLevel = defaultValues.playerXPLevel; // Установка уровня опыта по умолчанию
            playerData.currentLevelXP = defaultValues.currentLevelXP; // Установка текущего опыта по умолчанию
            playerData.nextLevelXP = defaultValues.nextLevelXP; // Установка опыта для следующего уровня по умолчанию
            playerData.nextLevelXpMultiplier = defaultValues.nextLevelXpMultiplier; // Установка множителя для следующего уровня
            playerData.speedBoost = defaultValues.speedBoost; // Сбрасываем speedBoost
            playerData.completedRaces = new List<string>(defaultValues.completedRaces); // Восстанавливаем завершенные гонки

            // Полностью очищаем список разблокированных предметов
            playerData.items = new List<string>();

            // Уникальный идентификатор автомобиля, который разблокирован по умолчанию
            string defaultVehicleID = "vaz2101"; // Замените на ваш уникальный ID

            // Добавляем уникальный идентификатор первой машины в список разблокированных
            if (!playerData.items.Contains(defaultVehicleID))
            {
                playerData.items.Add(defaultVehicleID);
                Debug.Log($"Машина с уникальным ID '{defaultVehicleID}' разблокирована по умолчанию.");
            }

            if (!playerData.unlockedStages.Contains("Sezon1"))
                playerData.unlockedStages.Add("Sezon1");

            // Если есть дополнительные стартовые предметы, добавляем их
            foreach (var item in defaultValues.items)
            {
                if (!playerData.items.Contains(item))
                {
                    playerData.items.Add(item); // Добавляем только стартовые разблокированные предметы
                }
            }

            playerData.vehicleID = defaultValues.vehicleID; // Установка ID транспортного средства по умолчанию

            // Сохраняем сброшенные данные
            SaveData();

            // Синхронизация с базой данных автомобилей
            if (GlobalSettings.Instance.vehicleDatabase != null)
            {
                GlobalSettings.Instance.vehicleDatabase.SyncVehicleData(this);
            }

            Debug.Log("Player data and vehicle data have been reset!"); // Вывод сообщения в консоль
        }


        public void SaveCarColor(Color color)
        {
            playerData.selectedCarColor = ColorUtility.ToHtmlStringRGB(color); // Конвертируем в формат HEX
            SaveData(); // Сохраняем изменения
            Debug.Log($"Цвет автомобиля сохранён: {playerData.selectedCarColor}");
        }

        public Color LoadCarColor()
        {
            if (ColorUtility.TryParseHtmlString("#" + playerData.selectedCarColor, out Color color))
            {
                Debug.Log($"Загруженный цвет автомобиля: {color}");
                return color;
            }
            else
            {
                Debug.LogWarning("Цвет автомобиля не найден. Используется белый.");
                return Color.white; // Цвет по умолчанию
            }
        }

        void ApplySavedCarColor()
        {
            if (PlayerData.instance != null)
            {
                Color savedColor = PlayerData.instance.LoadCarColor();
                Renderer carRenderer = GetComponent<Renderer>();
                if (carRenderer != null)
                {
                    carRenderer.material.color = savedColor;
                }
            }
        }





        // Удаление файла с сохраненными данными
        public void DeleteSaveFile()
        {
            if (File.Exists(Application.persistentDataPath + "/savefile.dat"))
            {
                File.Delete(Application.persistentDataPath + "/savefile.dat"); // Удаление файла, если он существует
            }
        }
    }

    [Serializable]
    public class DataContainer
    {
        public string playerName; // Имя игрока
        public Nationality playerNationality; // Национальность игрока
        public List<string> unlockedStages = new List<string>(); // Разблокированные уровни
        //public List<string> unlockedItems = new List<string>();


        public float playerCurrency; // Валюта
        public float totalPlayerXP; // Общий опыт
        public int playerXPLevel; // Уровень игрока
        public float currentLevelXP; // Текущий опыт
        public float nextLevelXP; // Опыт для следующего уровня
        public float nextLevelXpMultiplier; // Множитель опыта для следующего уровня
        public List<string> items = new List<string>(); // Храним все разблокированные машины

        public string vehicleID; // ID текущего транспорта
        public int speedBoost; // Speed Boost
        public List<string> completedRaces = new List<string>(); // Завершенные гонки
        public string selectedCarColor; // Цвет кузова автомобиля в формате HEX



    }
}
