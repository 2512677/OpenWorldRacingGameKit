using UnityEngine;

namespace RGSK
{
    public class CareerStageManager : MonoBehaviour
    {
        public CareerData careerStage; // �������� � CareerStage �� CareerData

        void Start()
        {
            if (careerStage != null)
            {
               // Debug.Log("������ ������ �������: " + careerStage.levelName);
            }
            else
            {
                Debug.LogWarning("������� ������� �� �����!");
            }
        }
    }
}
    