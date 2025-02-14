using RGSK;
using UnityEngine;
using UnityEngine.UI;

public class CompliteText : MonoBehaviour
{
    [Header("��������� �����")]
    public string raceID; // ID ����� (����� ������ � ����������)

    [Header("UI")]
    public GameObject completedText; // ������ �� ������ CompletedText

    void Start()
    {
        CheckCompletionStatus();
    }

    public void CheckCompletionStatus()
    {
        if (completedText == null)
        {
            Debug.LogWarning($"[{gameObject.name}] �� ������ CompletedText! ������� �������� � ����������.");
            return;
        }

        if (string.IsNullOrEmpty(raceID))
        {
            Debug.LogWarning($"[{gameObject.name}] raceID �� �����! ������� ��������� � ����������.");
            return;
        }

        // ���������, ��������� �� ����� � ������ ������
        bool isCompleted = PlayerData.instance.playerData.completedRaces.Contains(raceID);

        // ��������/��������� "��������"
        completedText.SetActive(isCompleted);
    }
}
