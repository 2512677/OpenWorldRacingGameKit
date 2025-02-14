using I2.Loc;

public static class LanguageManager
{
    /// <summary>
    /// �������� �������������� ������ ��� ���� �����.
    /// </summary>
    public static string GetLocalizedRaceType(RaceType raceType)
    {
        return LocalizationManager.GetTranslation($"RaceType/{raceType}");
    }

    /// <summary>
    /// �������� �������������� ������ ��� ������ ���������.
    /// </summary>
    public static string GetLocalizedDifficulty(AIDifficultyLevel difficultyLevel)
    {
        return LocalizationManager.GetTranslation($"DifficultyLevel/{difficultyLevel}");
    }

    /// <summary>
    /// ������������� ������� ����.
    /// </summary>
    public static void SetLanguage(string languageCode)
    {
        LocalizationManager.CurrentLanguage = languageCode;
    }
}
