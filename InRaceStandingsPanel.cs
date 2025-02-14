using UnityEngine;
using System.Collections.Generic;

namespace RGSK
{
    public class InRaceStandingsPanel : RaceEntry
    {
        public UIPositionDisplayMode positionDisplay;
        public UIGapDisplayMode gapDisplay;
        public Color playerColor = Color.green;
        public Color opponentColor = Color.white;     
        private float lastUpdate;
        private float gap;

        void Start()
        {
            //Deactivate all entries on Start
            for (int i = 0; i < Entries.Length; i++)
            {
                Entries[i].SetActive(false);
            }
        }


        void Update()
        {
			if (RaceManager.instance == null)
                return;

            if (Time.time > lastUpdate)
            {
                lastUpdate = Time.time + 0.25f;

				for (int i = 0; i < RaceManager.instance.racerList.Count; i++)
                {
                    if (i > raceEntry.Count - 1) break;

                    if (raceEntry[i].gap != null)
                    {
						if (RaceManager.instance.racerList[i] != RaceManager.instance.playerStatistics)
                        {
							gap = (gapDisplay == UIGapDisplayMode.Time) ? RaceManager.instance.GetTimeGapBetween(RaceManager.instance.racerList[i]) :
								Mathf.RoundToInt(RaceManager.instance.GetTrackDistanceBetween(RaceManager.instance.racerList[i]));

                            raceEntry[i].gap.text = gapDisplay == UIGapDisplayMode.Time ? gap > 0 ? "-" + gap.ToString("F1") : Mathf.Abs(gap).ToString("F1")
                            : (gap > 0 ? "-" + Mathf.Abs(Mathf.RoundToInt(gap)) : Mathf.Abs(Mathf.RoundToInt(gap)).ToString()) + "m";
                        }
                        else
                        {
                            raceEntry[i].gap.text = "";
                        }
                    }
                }
            }
        }

        
        public void UpdateStandings()
        {
			if (RaceManager.instance == null)
                return;

            //Update the In Race Standings
			for (int i = 0; i < RaceManager.instance.racerList.Count; i++)
            {
                if (i > raceEntry.Count - 1)
                    break;

                //Display the racer's position
                if (raceEntry[i].position != null)
                {
					raceEntry[i].position.text = positionDisplay == UIPositionDisplayMode.Default | positionDisplay == UIPositionDisplayMode.PositionOnly ? 
                        RaceManager.instance.racerList[i].Position.ToString() :
						Helper.AddOrdinal(RaceManager.instance.racerList[i].Position);
                }

                //Display the racer's name
                if (raceEntry[i].name != null)
                {
					raceEntry[i].name.text = RaceManager.instance.racerList[i].racerInformation.racerName;

                    //Chnange color based on whether this is the player or opponent
					raceEntry[i].name.color = RaceManager.instance.racerList[i].isPlayer ? playerColor : opponentColor;

                    //If the racer is knocked out, display "DNF" after the name
					if (RaceManager.instance.racerList[i].disqualified)
                    {
                        raceEntry[i].name.text += " (DNF)";
                    }
                }

                //Display the racer's total speed for speedtrap races
				if(RaceManager.instance.raceType == RaceType.SpeedTrap)
                {
                    if(raceEntry[i].speedtrapSpeed != null)
                    {
						raceEntry[i].speedtrapSpeed.text = RaceManager.instance.racerList[i].totalSpeed.ToString("F1") + RaceManager.instance.speedUnit.ToString();
                    }
                }

                //Activate the entry if disabled
				if (Entries[i].activeSelf == false && RaceManager.instance.racerList.Count > 1)
                {
                    Entries[i].SetActive(true);
                }
            }
        }
    }
}