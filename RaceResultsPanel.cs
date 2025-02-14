using UnityEngine;
using System.Collections;

namespace RGSK
{
    public class RaceResultsPanel : RaceEntry
    {
        public void UpdateRaceResults()
        {
			if (RaceManager.instance == null)
                return;

			for (int i = 0; i < RaceManager.instance.racerList.Count; i++)
            {
                if (i > raceEntry.Count - 1)
                    break;

                //Racer position text
                if (raceEntry[i].position != null)
                {
                    raceEntry[i].position.text = RaceManager.instance.racerList[i].Position.ToString();
                }

                //Racer name text
                if (raceEntry[i].name != null)
                {
                    raceEntry[i].name.text = RaceManager.instance.racerList[i].GetName();
                }

                //Vehicle name text
                if (raceEntry[i].vehicle != null)
                {
                    raceEntry[i].vehicle.text = RaceManager.instance.racerList[i].GetVehicle();
                }

                //Racer nationality
                if (raceEntry[i].nationality != null)
                {
                    raceEntry[i].nationality.sprite = Helper.GetCountryFlag(RaceManager.instance.racerList[i].GetNationality());

                    if (raceEntry[i].nationality.sprite != null)
                        raceEntry[i].nationality.enabled = true;
                }

                //Best lap text
                if (raceEntry[i].bestLap != null)
                {
                    raceEntry[i].bestLap.text = RaceManager.instance.racerList[i].bestLapTime > 0 ?
                        Helper.FormatTime(RaceManager.instance.racerList[i].bestLapTime) : "--:--.---";
                }

                //Total time text
                if (raceEntry[i].totalTime != null)
                {
                    //Show the racers total time
                    raceEntry[i].totalTime.text = RaceManager.instance.racerList[i].finished ?
                    Helper.FormatTime(RaceManager.instance.racerList[i].totalRaceTime) : "--:--.---";               

                    //"DNF" if the racer did not finish
                    if (RaceManager.instance.racerList[i].disqualified)
                    {
                        raceEntry[i].totalTime.text = "DNF";
                    }
                }

                //Gap from first
                if (raceEntry[i].gap != null)
                {
                    raceEntry[i].gap.text = RaceManager.instance.racerList[i].finished ?
                       "+ " + Helper.FormatTime(RaceManager.instance.racerList[i].totalRaceTime - RaceManager.instance.racerList[0].totalRaceTime, TimeFormat.SecMs) : "--:--:---";
                }

                //Speedtrap total speed
                if (RaceManager.instance.raceType == RaceType.SpeedTrap)
                {
                    if (raceEntry[i].speedtrapSpeed != null)
                    {
                        raceEntry[i].speedtrapSpeed.text = RaceManager.instance.racerList[i].totalSpeed.ToString("F1") + RaceManager.instance.speedUnit.ToString();
                    }
                }
            }
        }
    }
}