using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    public class RaceRewardsPanel : MonoBehaviour
    {
        [Header("UI Text fields for showing rewards")]
        public Text moneyText;
        public Text xpText;
        public Text speedBoostText;
        public Text itemsText;

        [Header("Buttons")]
        public Button doubleRewardButton;   // Кнопка "2X"
        public Button collectButton;        // Кнопка "Collect" / "ОК"

        // Переменные для расчёта награды
        private int baseReward = 0;         // Базовая награда (если нужна другая логика)
        private int currentReward = 0;      // Текущая «отображаемая» награда (для денег)
        private bool wasDoubled = false;    // Флаг, что награда удвоена

        // Сохраним базовые данные награды, полученные из RaceRewards.Instance
        private int baseCurrency;
        private int baseXp;
        private int baseSpeedBoost;
        private string[] baseItems;

        // Флаг, чтобы награда не начислилась дважды
        private bool rewardTaken = false;

        // Ссылка на контроллер рекламы
        private CSharpSampleController adController;

        private void OnEnable()
        {
            // Вычисляем базовую награду – пример: 5000 (можно заменить своей логикой)
            baseReward = CalculateRaceReward();
            currentReward = baseReward;
            wasDoubled = false;

            // Сбрасываем UI, чтобы до загрузки всё не показывалось некорректно
            moneyText.text = currentReward.ToString();
            xpText.text = "0";
            speedBoostText.text = "0";
            itemsText.text = "";

            // Ищем контроллер рекламы
            adController = FindObjectOfType<CSharpSampleController>();
            if (adController != null)
            {
                // Подписываемся на событие, когда реклама досмотрена
                adController.onUserRewardEarned += OnRewardedAdCompleted;
            }
            else
            {
                Debug.LogWarning("RaceRewardsPanel: CSharpSampleController не найден в сцене!");
            }

            // Получаем позицию игрока из RaceManager и подготавливаем UI награды
            if (RaceManager.instance != null && RaceManager.instance.playerStatistics != null)
            {
                int playerPosition = RaceManager.instance.playerStatistics.Position;
                PrepareRewardsUI(playerPosition);
            }
            else
            {
                Debug.LogWarning("RaceRewardsPanel: RaceManager или playerStatistics не инициализированы.");
                moneyText.text = "0";
                xpText.text = "0";
                speedBoostText.text = "0";
                itemsText.text = "";
            }

            // Сброс флагов
            wasDoubled = false;
            rewardTaken = false;
        }

        private void OnDisable()
        {
            // Отписываемся от события, чтобы не было утечек памяти
            if (adController != null)
                adController.onUserRewardEarned -= OnRewardedAdCompleted;
        }

        /// <summary>
        /// Метод для кнопки "2X". Вызывает показ рекламы.
        /// </summary>
        public void OnClick_DoubleReward()
        {
            if (adController == null)
            {
                Debug.LogWarning("OnClick_DoubleReward: adController не найден.");
                return;
            }

            if (wasDoubled)
            {
                Debug.Log("Награда уже удвоена, повторное удвоение не требуется.");
                return;
            }

            // Показываем rewarded-рекламу. Если пользователь досмотрит её до конца, сработает событие.
            adController.ShowRewarded();
        }

        /// <summary>
        /// Событие, вызываемое после успешного просмотра рекламы.
        /// Здесь мы только обновляем отображаемую сумму (для денег).
        /// XP и Speed Boost не удваиваем в этом примере.
        /// Панель при этом не закрывается.
        /// </summary>
        private void OnRewardedAdCompleted()
        {
            if (!wasDoubled)
            {
                wasDoubled = true;
                currentReward = baseCurrency * 2;
                moneyText.text = currentReward.ToString();
                Debug.Log($"Награда удвоена на экране: {baseCurrency} -> {currentReward}");
            }
        }

        /// <summary>
        /// Метод для кнопки "Collect" / "ОК". Начисляет награду игроку и закрывает панель.
        /// </summary>
        public void OnClick_CollectReward()
        {
            if (rewardTaken)
            {
                Debug.Log("Награда уже выдана! Повторная выдача отменена.");
                return;
            }
            rewardTaken = true;

            // ---------------------------
            // 1) Проверяем дисквалификацию
            bool disqualified = RaceManager.instance.playerStatistics.disqualified;
            bool awardDNF = RaceRewards.Instance.awardDNF;

            // Если игрок дисквалифицирован и при этом awardDNF = false,
            // то награду фактически не выдаём
            if (disqualified && !awardDNF)
            {
                Debug.Log("Игрок дисквалифицирован (DNF): не начисляем награду, закрываем панель.");
                gameObject.SetActive(false);
                return;
            }
            // ---------------------------

            // Если реклама не была досмотрена, начисляем базовую награду; если досмотрена – удвоенную.
            int finalCurrency = wasDoubled ? baseCurrency * 2 : baseCurrency;

            // Пример: деньги, XP, буст
            PlayerData.instance.AddPlayerCurrecny(finalCurrency);
            PlayerData.instance.AddXP(baseXp);
            PlayerData.instance.AddSPB(baseSpeedBoost);

            // Сохраняем изменения
            PlayerData.instance.SaveData();

            // Обновляем UI главного меню, если он есть (например, в PlayerSettings)
            var playerSettings = FindObjectOfType<PlayerSettings>();
            if (playerSettings != null)
                playerSettings.UpdateUIToMatchSettings();

            Debug.Log($"Выдали награду: {finalCurrency} денег, {baseXp} XP, +{baseSpeedBoost} SP. Удвоение: {wasDoubled}");

            // Закрываем панель
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Альтернативный вариант для выдачи базовой награды (1х).
        /// </summary>
        public void OnClick_CollectBaseReward()
        {
            if (!rewardTaken)
            {
                rewardTaken = true;
                GiveReward(baseCurrency, baseXp, baseSpeedBoost, baseItems);
                ClosePanel();
            }
        }

        /// <summary>
        /// Альтернативный вариант выдачи награды после досмотра рекламы.
        /// Здесь происходит начисление удвоенной награды и обновление UI, но панель не закрывается автоматически.
        /// </summary>
        private void OnUserRewardEarned()
        {
            if (!rewardTaken)
            {
                wasDoubled = true;
                rewardTaken = true;
                currentReward = baseCurrency * 2;
                moneyText.text = currentReward.ToString();
                Debug.Log($"Награда удвоена (OnUserRewardEarned): {currentReward} монет");
                // Здесь панель остаётся открытой – игрок должен нажать Collect
            }
        }

        /// <summary>
        /// Метод для вычисления базовой награды (можно заменить своей логикой).
        /// </summary>
        private int CalculateRaceReward()
        {
            // В данном примере просто возвращаем 5000,
            // но вы можете изменить расчёт в зависимости от позиции и пр.
            return 5000;
        }

        /// <summary>
        /// Подготавливает UI с наградой, полученной из RaceRewards.Instance, по позиции игрока.
        /// </summary>
        private void PrepareRewardsUI(int playerPosition)
        {
            // 1. Проверка дисквалификации (DNF)
            bool disqualified = RaceManager.instance.playerStatistics.disqualified;
            bool awardDNF = RaceRewards.Instance.awardDNF;

            // Если игрок дисквалифицирован и awardDNF = false, сразу обнуляем UI
            if (disqualified && !awardDNF)
            {
                Debug.Log("DNF: награда обнулена в UI!");
                baseCurrency = 0;
                baseXp = 0;
                baseSpeedBoost = 0;
                baseItems = null;

                moneyText.text = "0";
                xpText.text = "0";
                speedBoostText.text = "0";
                itemsText.text = "";
                return;
            }

            // 2. Проверка Time Attack: если позиция > 3, обнуляем награду в UI
            if (RaceManager.instance.raceType == RaceType.TimeAttack && playerPosition > 3)
            {
                Debug.Log("Time Attack: игрок занял место 4 или хуже => награда обнулена в UI!");
                baseCurrency = 0;
                baseXp = 0;
                baseSpeedBoost = 0;
                baseItems = null;

                moneyText.text = "0";
                xpText.text = "0";
                speedBoostText.text = "0";
                itemsText.text = "";
                return;
            }

            // 3. Проверка, есть ли награды в currentRewards
            var rewardsArray = RaceRewards.Instance.currentRewards;
            if (rewardsArray == null || rewardsArray.Length == 0)
            {
                Debug.LogWarning("RaceRewardsPanel: нет наград в currentRewards.");
                moneyText.text = "0";
                xpText.text = "0";
                speedBoostText.text = "0";
                itemsText.text = "";
                return;
            }

            // 4. Проверка, чтобы playerPosition не выходила за границы массива
            if (playerPosition < 1 || playerPosition > rewardsArray.Length)
            {
                Debug.LogWarning($"RaceRewardsPanel: playerPosition {playerPosition} выходит за границы наград.");
                moneyText.text = "0";
                xpText.text = "0";
                speedBoostText.text = "0";
                itemsText.text = "";
                return;
            }

            // 5. Если всё в порядке, берём награду из массива
            var reward = rewardsArray[playerPosition - 1];

            baseCurrency = reward.currency;
            baseXp = reward.xp;
            baseSpeedBoost = reward.speedBoost;
            baseItems = reward.items;

            // 6. Отображаем базовые значения в UI
            moneyText.text = baseCurrency.ToString();
            xpText.text = baseXp.ToString();
            speedBoostText.text = baseSpeedBoost.ToString();

            if (baseItems != null && baseItems.Length > 0)
                itemsText.text = string.Join(", ", baseItems);
            else
                itemsText.text = "";
        }


        /// <summary>
        /// Метод, который фактически начисляет награду в PlayerData.
        /// Изменён так, чтобы использовать методы AddPlayerCurrecny и AddSPB для обновления UI.
        /// </summary>
        private void GiveReward(int currency, int xp, int speedBoost, string[] items)
        {
            PlayerData.instance.AddPlayerCurrecny(currency);
            PlayerData.instance.AddXP(xp);
            PlayerData.instance.AddSPB(speedBoost);

            if (items != null && items.Length > 0)
            {
                foreach (var item in items)
                {
                    PlayerData.instance.AddItem(item);
                }
            }

            PlayerData.instance.SaveData();

            Debug.Log($"Выдали награду: {currency} монет, {xp} XP, +{speedBoost} SP");
        }

        private void ClosePanel()
        {
            gameObject.SetActive(false);
        }
    }
}
