using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarClass : MonoBehaviour
{
    // ������������ ��� ������� �����
    public enum VehicleClass
    {
        None,       // �������� ����� "None" ��� ���������� ��������
        Stockvaz21012107,
        Stockvaz21082114,
        Sport,
        Turbo,
        
    
    }

    [Header("����� ������")]
    public VehicleClass carClass; // ������� ����� ������ � ����������

    // ����� � ������� ��� ������������
    private void Awake()
    {
        Debug.Log($"������ {gameObject.name} ��������� � ������ {carClass}");
    }
}
