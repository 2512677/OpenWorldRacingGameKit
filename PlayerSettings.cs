﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;

namespace RGSK
{
    public class PlayerSettings : MonoBehaviour
    {
        public InputField playerName;
        public Dropdown countryDropdown;
        public Text playerCurrency;
        public Text playerXP;
        public Text playerXPLevel;
        public Text playerSpeedBoost; // Отображение speedBoost

        void Start()
        {
            if (playerName != null)
            {
                playerName.onEndEdit.AddListener(delegate
                {
                    SavePlayerName();
                });
            }

            if (countryDropdown != null)
            {
                PopulateCountryDropdown();
                countryDropdown.onValueChanged.AddListener(delegate
                {
                    SavePlayerNationality();
                });
            }

            UpdateUIToMatchSettings();
        }


        public void UpdateUIToMatchSettings()
        {
            if (PlayerData.instance == null)
                return;

            if (playerCurrency != null)
            {
                playerCurrency.text = PlayerData.instance.playerData.playerCurrency.ToString("N0");
                //Debug.Log($"PlayerSettings: Player Currency Updated: {playerCurrency.text}");
            }

            if (playerXP != null)
            {
                playerXP.text = PlayerData.instance.playerData.totalPlayerXP.ToString();
                // Debug.Log($"PlayerSettings: Player XP Updated: {playerXP.text}");
            }

            if (playerSpeedBoost != null)
            {
                playerSpeedBoost.text = PlayerData.instance.playerData.speedBoost.ToString("N0");
                //  Debug.Log($"PlayerSettings: Player SpeedBoost Updated: {playerSpeedBoost.text}");
            }

            if (playerXPLevel != null)
            {
                playerXPLevel.text = PlayerData.instance.playerData.playerXPLevel.ToString();
                // Debug.Log($"PlayerSettings: Player XP Level Updated: {playerXPLevel.text}");
            }

            if (playerName != null)
            {
                playerName.text = PlayerData.instance.playerData.playerName;
            }

            if (countryDropdown != null)
            {
                for (int i = 0; i < countryDropdown.options.Count; i++)
                {
                    if (countryDropdown.options[i].text == PlayerData.instance.playerData.playerNationality.ToString())
                    {
                        countryDropdown.value = i;
                        break;
                    }
                }
            }
        }



        void PopulateCountryDropdown()
        {
            string[] countries = Enum.GetNames(typeof(Nationality));
            countryDropdown.AddOptions(countries.ToList());
            countryDropdown.onValueChanged.AddListener(delegate { SavePlayerNationality(); });
        }


        void SavePlayerName()
        {
            if (PlayerData.instance != null)
            {
                PlayerData.instance.SavePlayerName(playerName.text);
            }
        }


        void SavePlayerNationality()
        {
            if (PlayerData.instance == null)
                return;

            Nationality nat = (Nationality)Enum.Parse(typeof(Nationality),
                countryDropdown.options[countryDropdown.value].text, true);

            PlayerData.instance.SavePlayerNationality(nat);
        }
    }
}