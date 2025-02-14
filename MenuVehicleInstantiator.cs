using System;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class MenuVehicleInstantiator : MonoBehaviour
    {
        [Serializable]
        public class MenuVehicle
        {
            public GameObject vehicle;

            public VehicleDatabase.VehicleData vehicleData;

            public MenuVehicle(GameObject _vehicle, VehicleDatabase.VehicleData _vehicleData)
            {
                vehicle = _vehicle;
                vehicleData = _vehicleData;
            }
        }

        [HideInInspector]
        public List<MenuVehicle> menuVehicles;

        private int vehicleIndex;

        public VehicleDatabase vehicleDatabase => GlobalSettings.Instance.vehicleDatabase;

        private void Start()
        {
            InstantiateVehicles();
            LoadPlayerVehicle();
        }

        private void InstantiateVehicles()
        {
            if (vehicleDatabase == null)
            {
                Debug.LogError("Vehicle database is not assigned.");
                return;
            }

            foreach (var vehicleData in vehicleDatabase.vehicles)
            {
                GameObject vehicleObject = Instantiate(vehicleData.menuVehicle, transform.position, transform.rotation, transform);
                menuVehicles.Add(new MenuVehicle(vehicleObject, vehicleData));
                vehicleObject.SetActive(false);

                // Обновляем статус блокировки
                if (PlayerData.instance != null)
                {
                    vehicleData.isLocked = !PlayerData.instance.IsItemUnlocked(vehicleData.uniqueID);
                }
            }
        }

        private void LoadPlayerVehicle()
        {
            if (PlayerData.instance == null)
            {
                if (menuVehicles.Count > 0)
                {
                    menuVehicles[0].vehicle.SetActive(true);
                    vehicleIndex = 0; // Установить индекс на первый автомобиль
                }
                return;
            }

            bool vehicleFound = false;

            for (int i = 0; i < menuVehicles.Count; i++)
            {
                string vehicleID = menuVehicles[i].vehicleData.uniqueID;

                // Проверяем, разблокирована ли машина
                if (PlayerData.instance.IsItemUnlocked(vehicleID))
                {
                    menuVehicles[i].vehicleData.isLocked = false;
                }

                // Устанавливаем активное транспортное средство
                if (vehicleID == PlayerData.instance.playerData.vehicleID)
                {
                    vehicleIndex = i;
                    menuVehicles[i].vehicle.SetActive(true);
                    vehicleFound = true;

                    // Загрузка сохранённого цвета
                    if (menuVehicles[i].vehicleData.bodyMaterials != null)
                    {
                        foreach (var material in menuVehicles[i].vehicleData.bodyMaterials)
                        {
                            material.color = LoadVehicleColor(vehicleID, material.color);
                        }
                    }
                }
                else
                {
                    menuVehicles[i].vehicle.SetActive(false);
                }
            }

            // Если ни одно транспортное средство не выбрано, активируем первый
            if (!vehicleFound && menuVehicles.Count > 0)
            {
                menuVehicles[0].vehicle.SetActive(true);
                vehicleIndex = 0;

                // Загрузка цвета для первого автомобиля (если не выбран другой)
                if (menuVehicles[0].vehicleData.bodyMaterials != null)
                {
                    foreach (var material in menuVehicles[0].vehicleData.bodyMaterials)
                    {
                        material.color = LoadVehicleColor(menuVehicles[0].vehicleData.uniqueID, material.color);
                    }
                }
            }
        }


        public void CycleVehicles(int direction)
        {
            menuVehicles[vehicleIndex].vehicle.SetActive(false);
            vehicleIndex += direction;
            vehicleIndex = Mathf.Clamp(vehicleIndex, 0, menuVehicles.Count - 1);
            menuVehicles[vehicleIndex].vehicle.SetActive(true);
        }

        public void SetSelectedVehicle()
        {
            if (PlayerData.instance != null)
            {
                PlayerData.instance.SaveSelectedVehicle(GetVehicleData().uniqueID);
            }
        }

        public void SetSelectedVehicleColor(Color color)
        {
            var vehicleData = GetVehicleData();
            if (vehicleData.bodyMaterials != null && vehicleData.bodyMaterials.Length > 0)
            {
                foreach (var material in vehicleData.bodyMaterials)
                {
                    material.color = color;
                }
                SaveSelectedVehicleColor(color);
            }
        }

        public void SaveSelectedVehicleColor(Color color)
        {
            PlayerPrefs.SetFloat(GetVehicleData().uniqueID + "_colorR", color.r);
            PlayerPrefs.SetFloat(GetVehicleData().uniqueID + "_colorG", color.g);
            PlayerPrefs.SetFloat(GetVehicleData().uniqueID + "_colorB", color.b);
            PlayerPrefs.Save();
        }

        public Color LoadVehicleColor(string id, Color defaultColor)
        {
            float r = PlayerPrefs.GetFloat(id + "_colorR", defaultColor.r);
            float g = PlayerPrefs.GetFloat(id + "_colorG", defaultColor.g);
            float b = PlayerPrefs.GetFloat(id + "_colorB", defaultColor.b);
            return new Color(r, g, b);
        }


        public void RevertPlayerVehicle()
        {
            for (int i = 0; i < menuVehicles.Count; i++)
            {
                if (menuVehicles[i].vehicleData.uniqueID == PlayerData.instance.playerData.vehicleID)
                {
                    vehicleIndex = i;
                    menuVehicles[i].vehicle.SetActive(true);
                }
                else
                {
                    menuVehicles[i].vehicle.SetActive(false);
                }
            }
        }

        public VehicleDatabase.VehicleData GetVehicleData()
        {
            if (menuVehicles.Count == 0)
            {
                return null;
            }
            return menuVehicles[vehicleIndex].vehicleData;
        }

        public bool IsLastVehicleInList()
        {
            return vehicleIndex == menuVehicles.Count - 1;
        }

        public bool IsFirstVehicleInList()
        {
            return vehicleIndex == 0;
        }

        public bool HasVehicleDatabase()
        {
            return vehicleDatabase != null;
        }
    }
}
