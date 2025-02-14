using RGSK;
using UnityEngine;
using UnityEngine.UI;

public class CompliteText : MonoBehaviour
{
    [Header("Настройки гонки")]
    public string raceID; // ID гонки (нужно задать в инспекторе)

    [Header("UI")]
    public GameObject completedText; // Ссылка на объект CompletedText

    void Start()
    {
        CheckCompletionStatus();
    }

    public void CheckCompletionStatus()
    {
        if (completedText == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Не найден CompletedText! Проверь привязку в инспекторе.");
            return;
        }

        if (string.IsNullOrEmpty(raceID))
        {
            Debug.LogWarning($"[{gameObject.name}] raceID не задан! Проверь настройки в инспекторе.");
            return;
        }

        // Проверяем, завершена ли гонка с первым местом
        bool isCompleted = PlayerData.instance.playerData.completedRaces.Contains(raceID);

        // Включаем/выключаем "ПРОЙДЕНО"
        completedText.SetActive(isCompleted);
    }
}
