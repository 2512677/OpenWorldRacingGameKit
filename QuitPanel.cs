using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitPanel : MonoBehaviour
{
    public void QuitGame()
    {
        // ��� � ������� ��� �������� � ���������
        Debug.Log("���� �������.");

        // ������� ����������
        Application.Quit();

        // ��� ��������� Unity (������ � ������ ����������)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
