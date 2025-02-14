using UnityEngine;
using System.Collections;

namespace RGSK
{
    public class RespawnTrigger : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            RacerStatistics stats = other.GetComponentInParent<RacerStatistics>();

            if(stats != null)
            {
                RaceManager.instance.RespawnVehicle(stats.transform);
            }
        }
    }
}
