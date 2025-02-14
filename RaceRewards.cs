using RGSK;
using UnityEngine;

/// <summary>
/// Объединённый скрипт RaceRewards, который:
/// 1) Хранит текущие награды (currentRewards).
/// 2) Проверяет дисквалификацию игрока (DNF) и не выдаёт наград, если "awardDNF" = false.
/// 3) Слушает событие завершения гонки и автоматически вызывает GiveRewards().
/// 4) Выдаёт валюту, опыт, буст скорости, предметы и т.д.
/// </summary>
public class RaceRewards : MonoBehaviour
{
    public static RaceRewards Instance;

    // Проверять, выдавать ли награду при DNF (Disqualified/Did Not Finish)
    public bool awardDNF = false;

    // Поля для отображения выданных наград (для отладки или UI)
    public float awardedCurrency { get; private set; }
    public float awardedXP { get; private set; }
    public int awardedSpeedBoost { get; private set; }

    // Текущий массив наград (для разных мест: 1-го, 2-го и т.д.)
    public Rewards[] currentRewards;

    [System.Serializable]
    public class Rewards
    {
        public int currency;
        public int xp;
        public int speedBoost;
        public string[] items;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Пробуем загрузить pendingRewards из CareerData -> иначе из ChampionshipData
        if (CareerData.pendingRewards != null && CareerData.pendingRewards.Length > 0)
        {
            currentRewards = CareerData.pendingRewards;
            Debug.Log("Загружены награды за карьеру.");
        }
        else if (ChampionshipData.pendingRewards != null && ChampionshipData.pendingRewards.Length > 0)
        {
            currentRewards = ChampionshipData.pendingRewards;
            Debug.Log("Загружены награды за чемпионат.");
        }
        else
        {
            currentRewards = null;
            Debug.LogWarning("Нет доступных наград.");
        }
    }

    /// <summary>
    /// Основной метод для выдачи наград игроку,
    /// вызывается при финише гонки (OnPlayerFinish).
    /// </summary>
    public void GiveRewards()
    {
        // 1. Определяем место игрока
        int position = RaceManager.instance.playerStatistics.Position;

        // 2. Учитываем другие типы гонок (TimeAttack, Drift и т.д.)
        if (RaceManager.instance.raceType == RaceType.TimeAttack)
        {
            position = RaceManager.instance.GetTimeAttackPosition();
        }
        else if (RaceManager.instance.raceType == RaceType.Drift)
        {
            position = RaceManager.instance.GetDriftRacePosition();
        }

        // 3. Проверяем дисквалификацию.
        //    Если игрок дисквалифицирован и awardDNF = false, то награды не выдаём.
        if (RaceManager.instance.playerStatistics.disqualified && !awardDNF)
        {
            Debug.Log("Игрок дисквалифицирован (DNF). Награды не выдаются.");
            return;
        }

        // ------------------------- НОВАЯ ПРОВЕРКА ДЛЯ TIME ATTACK -------------------------
        // Если это режим Time Attack, и позиция выше 3-го места => награду не даём
        if (RaceManager.instance.raceType == RaceType.TimeAttack && position > 3)
        {
            Debug.Log("Time Attack: игрок занял место 4 или хуже, награда = 0");
            return;
        }
        // ----------------------------------------------------------------------------------

        // 4. Проверяем, что массив наград не пуст и позиция игрока попадает в его диапазон
        if (currentRewards == null || currentRewards.Length == 0)
        {
            Debug.LogWarning("currentRewards пуст — наград нет.");
            return;
        }

        // Индекс в массиве: (position - 1)
        if (position - 1 < 0 || position - 1 >= currentRewards.Length)
        {
            Debug.LogWarning($"Для позиции {position} нет награды в currentRewards.");
            return;
        }

        // 5. Получаем из массива нужную награду
        Rewards reward = currentRewards[position - 1];

        // 6. Запоминаем, сколько выдали (для UI или отладки)
        awardedCurrency = reward.currency;
        awardedXP = reward.xp;
        awardedSpeedBoost = reward.speedBoost;

        // 7. Применяем к PlayerData (при условии, что PlayerData.instance существует)
        if (PlayerData.instance != null)
        {
            // Валюта
            PlayerData.instance.AddPlayerCurrecny(awardedCurrency);

            // Опыт
            PlayerData.instance.AddPlayerXP(awardedXP);

            // Буст скорости (если у вас есть соответствующий метод в PlayerData)
            // PlayerData.instance.AddSpeedBoost(awardedSpeedBoost);

            // Если есть items, вызываем PlayerData.instance.UnlockItem(item)
            foreach (string item in reward.items)
            {
                PlayerData.instance.UnlockItem(item);
            }
        }
        else
        {
            Debug.LogWarning("PlayerData.instance не найден: награду некуда добавить!");
        }
    }


    // Подписываемся на событие завершения гонки
    private void OnEnable()
    {
        RaceManager.OnPlayerFinish += GiveRewards;
    }

    // Не забудьте отписываться, если скрипт может отключаться/уничтожаться
    private void OnDisable()
    {
        RaceManager.OnPlayerFinish -= GiveRewards;
    }

    // Методы для получения/установки currentRewards, если нужно извне
    public Rewards[] GetRewards()
    {
        return currentRewards;
    }

    public void SetRewards(Rewards[] rewards)
    {
        currentRewards = rewards;
    }
}
