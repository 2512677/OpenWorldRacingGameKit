using UnityEngine;
using System.Collections.Generic;

public class VehicleLayerRenderer : MonoBehaviour
{
    public GameObject vehicleSelectMenu; // ����, ������� ���������� ��� ������������ ������
    private List<GameObject> cars = new List<GameObject>(); // ������ ��� �������� �������� CarMenu

    void Start()
    {
        // ������� � ��������� ��� ������� CarMenu � ������
        GameObject[] carObjects = GameObject.FindGameObjectsWithTag("CarMenu");
        foreach (GameObject car in carObjects)
        {
            cars.Add(car);
            car.SetActive(false); // ������������ ��� ������ ��� ������
        }
    }

    void Update()
    {
        // ���������� ������, ������ ���� vehicleSelectMenu �������
        if (vehicleSelectMenu != null && vehicleSelectMenu.activeSelf)
        {
            SetCarsActive(true);
        }
        else
        {
            SetCarsActive(false);
        }
    }

    // ����� ��� ��������� ��� ����������� ���� ���������� �������� CarMenu
    private void SetCarsActive(bool isActive)
    {
        foreach (GameObject car in cars)
        {
            if (car != null)
            {
                car.SetActive(isActive);
            }
        }
    }
}
