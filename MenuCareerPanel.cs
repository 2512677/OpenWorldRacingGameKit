using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace RGSK
{
    public class MenuCareerPanel : MonoBehaviour
    {
        public static MenuCareerPanel Instance; // �������� ��� �������

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void LoadCareerEvents(CareerData careerData)
        {
            if (careerData == null)
            {
                Debug.LogError("MenuCareerPanel: CareerData �� ���������!");
                return;
            }

            foreach (var round in careerData.careerRounds)
            {
                Debug.Log($"�������� �������: {round.raceType} - {round.laps} ������");
            }
        }

        private void StartRace(CareerData.CareerRound round)
        {
            if (round == null)
            {
                Debug.LogError("MenuCareerPanel: Round �� ��������!");
                return;
            }

            PlayerPrefs.SetInt("RaceType", (int)round.raceType);
            PlayerPrefs.SetInt("LapCount", round.laps);
            PlayerPrefs.SetInt("OpponentCount", round.opponentCount);

            if (SceneController.instance != null)
            {
                SceneController.instance.LoadScene(round.trackData.trackName);
            }
            else
            {
                Debug.LogWarning("SceneController �� ������.");
            }
        }
    }
}
