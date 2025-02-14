using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using I2.Loc;

namespace RGSK
{
    public class DriftPanel : MonoBehaviour
    {
        private DriftPointsManager driftPointController;
        public Text currentDriftPoints; //Text that shows the players current drift points (while drifting)
        public Text totalDriftPoints; //Text that shows the players total drift points
        public Text driftMultiplier; //Text that shows the player's point multiplier value
        public Text driftInfo; //Text that shows Completed & Failed drift information


        void Start()
        {
            // Clear the assigned texts
            if (totalDriftPoints != null)
            {
                totalDriftPoints.text = LocalizationManager.GetTranslation("DriftPanel/Points");
            }

            if (currentDriftPoints != null)
            {
                currentDriftPoints.text = string.Empty;
            }

            if (driftMultiplier != null)
            {
                driftMultiplier.text = string.Empty;
            }

            if (driftInfo != null)
            {
                driftInfo.text = string.Empty;
            }

            FindDriftPointController();
            ClearDriftInfoLocalized();  
        }


        void Update()
        {
            if (driftPointController == null)
            {
                FindDriftPointController();
            }

            UpdateCurrentDriftPoints();
        }


        void UpdateCurrentDriftPoints()
        {
            if (driftPointController == null)
                return;

            //If the vehicle is drifting, update the CurrentDriftPoints text to display how many
            //points the player has accumulated
            if (driftPointController.drifting)
            {
                if (currentDriftPoints != null && driftPointController.currentDriftPoints > 0)
                {
                    currentDriftPoints.text = "+ " + driftPointController.currentDriftPoints.ToString("N0");
                }
            }
        }


        public void UpdateTotalDriftPoints()
        {
            if (driftPointController == null)
                return;

            if (totalDriftPoints)
                totalDriftPoints.text = $"{LocalizationManager.GetTranslation("DriftPanel/Points")}: {driftPointController.totalDriftPoints:N0}";

            if (currentDriftPoints)
                currentDriftPoints.text = string.Empty;

            if (driftMultiplier)
                driftMultiplier.text = string.Empty;

            if (driftInfo)
            {
                driftInfo.color = Color.green;
                driftInfo.text = string.Format(
                    LocalizationManager.GetTranslation("DriftPanel/DriftComplete"),
                    driftPointController.currentDriftPoints.ToString("N0")
                );
                Invoke("ClearDriftInfo", 2);
            }
        }


        public void UpdateDriftMultipier()
        {
            if (driftPointController == null)
                return;

            if (driftMultiplier)
                driftMultiplier.text = string.Format(
                    LocalizationManager.GetTranslation("DriftPanel/Multiplier"),
                    driftPointController.driftMultiplier
                );
        }


        public void UpdateDriftFailInfo()
        {
            if (driftInfo)
            {
                driftInfo.color = Color.red;
                driftInfo.text = LocalizationManager.GetTranslation("DriftPanel/DriftFailed");
                Invoke("ClearDriftInfo", 2);
            }

            if (currentDriftPoints != null)
            {
                currentDriftPoints.text = string.Empty;
            }

            if (driftMultiplier != null)
            {
                driftMultiplier.text = string.Empty;
            }
        }




        void ClearDriftInfoLocalized()
        {
            if (driftInfo != null)
            {
                driftInfo.text = string.Empty;
            }
        }

        void FindDriftPointController()
        {
            if (driftPointController == null)
            {
                driftPointController = FindObjectOfType<DriftPointsManager>();
            }

            if (driftPointController == null)
            {
                Debug.LogWarning("DriftPointsManager не найден в сцене.");
            }
        }



        void OnEnable()
        {
            RaceManager.OnVehicleSpawn += FindDriftPointController;
        }


        void Disable()
        {
            RaceManager.OnVehicleSpawn -= FindDriftPointController;
        }
    }
}