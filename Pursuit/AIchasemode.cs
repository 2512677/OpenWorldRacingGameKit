using RGSK;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AIchasemode : MonoBehaviour
{
    public int healthbar = 4; // �������� (�����)
    public GameObject[] destoryeffects; // ������� ��� �����������
    public GameObject[] hbpart; // ���������� �����
    public Rigidbody Rb; // Rigidbody ������
    private List<GameObject> vehicles = new List<GameObject>();

    private bool isStopped = false; // ���� ���������

    void Start()
    {
        vehicles = FindObjectsOfType<RCC_CarControllerV3>()
                    .Where(c => !c.CompareTag("Player")) // ��������� ������ ������
                    .Select(c => c.gameObject)
                    .ToList();

        Debug.Log($"������� {vehicles.Count} ����� ��� ���������.");
    }

    private void Update()
    {
        // ��������� ������� ������� L
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("������ ������� L. ������������� �������� �� 0.");
            healthbar = 0;
            TestStopAllVehicles(); // ������������� ��� ������
        }
    }

    private void TestStopAllVehicles()
    {
        foreach (GameObject vehicle in vehicles)
        {
            if (vehicle != null)
            {
                Debug.Log($"��������� ��������� ��� ������ {vehicle.name}");
                StopVehicle(vehicle);
            }
            else
            {
                Debug.LogError("���� �� ����������� � ������ �������� null!");
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"������������ � �������! ������� ��������: {healthbar}");

            // ��������� ��������
            healthbar--;

            Debug.Log($"�������� ����� ����������: {healthbar}");

            // ��������� ���������� ����������� ��������
            UpdateHealthbarVisual();

            // ���� �������� �������� 0, ��������� ��������� ������
            if (healthbar <= 0)
            {
                Debug.Log("�������� �����������. ������������� ������������� ������!");

                RCC_CarControllerV3 carController = GetComponent<RCC_CarControllerV3>();
                if (carController != null)
                {
                    Debug.Log("RCC_CarControllerV3 ������. ���������� �����.");
                    carController.gasInput = 0f;
                    carController.brakeInput = 1f;
                    carController.steerInput = 0f;
                    carController.handbrakeInput = 1f;
                    carController.engineRunning = false;

                    Debug.Log($"������ {gameObject.name} ������� ����������� ����� RCC_CarControllerV3.");
                }
                else
                {
                    Debug.LogError("RCC_CarControllerV3 �� ������! ��������� ���������� �������.");
                }

                // �������������� �������� ����������
                RCCAIInput aiInput = GetComponent<RCCAIInput>();
                if (aiInput != null)
                {
                    Debug.Log("RCCAIInput ������. �������.");
                    Destroy(aiInput);
                }
                else
                {
                    Debug.LogWarning("RCCAIInput �� ������!");
                }

                AiLogic aiLogic = GetComponent<AiLogic>();
                if (aiLogic != null)
                {
                    Debug.Log("AiLogic ������. �������.");
                    Destroy(aiLogic);
                }
                else
                {
                    Debug.LogWarning("AiLogic �� ������!");
                }

                // �������������� ��������� Rigidbody
                Rigidbody rb = GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Debug.Log("Rigidbody ������. ������������� �������������.");
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = true; // �������� ��������� ������
                    rb.isKinematic = false; // ���������� ��� ���������� ��������������
                }
                else
                {
                    Debug.LogWarning("Rigidbody �� ������!");
                }

                // ���������� ������� �����������
                Debug.Log("���������� ������� �����������.");
                ActivateEffects();

                // ����������� ������� ������� ������
                Debug.Log($"��������� ������ ��� {gameObject.name}:");
                Debug.Log($"gasInput: {carController?.gasInput}, brakeInput: {carController?.brakeInput}, steerInput: {carController?.steerInput}");
                Debug.Log($"velocity: {rb?.velocity}, angularVelocity: {rb?.angularVelocity}");
            }
        }
    }









    private void StopVehicle(GameObject vehicle)
    {
        if (vehicle == null)
        {
            Debug.LogError("���������� ������ � StopVehicle �������� null!");
            return;
        }

        RCC_CarControllerV3 carController = vehicle.GetComponent<RCC_CarControllerV3>();
        if (carController != null)
        {
            // ���������� ����� ����������
            carController.gasInput = 0f;
            carController.brakeInput = 1f; // ������ ����������
            carController.steerInput = 0f;
            carController.handbrakeInput = 1f; // ������ ������

            // ������������� ���������
            carController.engineRunning = false;

            Debug.Log($"������ {vehicle.name} ������� �����������.");
        }

        // ���������� AI Logic
        AiLogic aiLogic = vehicle.GetComponent<AiLogic>();
        if (aiLogic != null)
        {
            Destroy(aiLogic);
            Debug.Log($"AI Logic �������� ��� {vehicle.name}");
        }

        // �������� AI Input
        RCCAIInput aiInput = vehicle.GetComponent<RCCAIInput>();
        if (aiInput != null)
        {
            Destroy(aiInput);
            Debug.Log($"AI ���������� ������� ��� {vehicle.name}");
        }

        // �������������� ��������� Rigidbody
        Rigidbody rb = vehicle.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;       // ����� �������� ��������
            rb.angularVelocity = Vector3.zero; // ����� ������� ��������
            rb.isKinematic = true;           // ������ ��������� ������
            rb.isKinematic = false;          // ������ �������� ��� ���������� ��������

            Debug.Log($"Rigidbody ���������� ��� {vehicle.name}");
        }
    }




    // ��������� ���������� ����������� �������� (����)
    private void UpdateHealthbarVisual()
    {
        for (int i = 0; i < hbpart.Length; i++)
        {
            if (i < healthbar)
            {
                hbpart[i].SetActive(true); // �������� �����, ��������������� �������� ��������
            }
            else
            {
                hbpart[i].SetActive(false); // ��������� �����, ������� ������ ���������
            }
        }
    }

    private void ActivateEffects()
    {
        Debug.Log("���������� ������� �����������.");
        foreach (var effect in destoryeffects)
        {
            if (effect != null)
                effect.SetActive(true); // �������� �������
        }
    }
}