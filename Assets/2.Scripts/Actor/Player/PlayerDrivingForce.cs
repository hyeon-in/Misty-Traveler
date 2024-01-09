using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �÷��̾��� �������� ó���ϴ� Ŭ����
/// </summary>
public class PlayerDrivingForce : MonoBehaviour
{
    readonly int hashFill = Animator.StringToHash("Fill");
    readonly int hashFilled = Animator.StringToHash("Filled");
    readonly int hashUse = Animator.StringToHash("Use");

    public int maxDrivingForce = 6; // �ִ� ������
    int _currentDrivingForce;       // ���� ������

    /// <summary>
    /// �÷��̾� ĳ������ ������ UI ������ ��� ����ü�Դϴ�.
    /// </summary> 
    [System.Serializable]
    struct DrivingForceUI
    {
        [HideInInspector] public Image image;
        [HideInInspector] public Animator animator;
    }
    DrivingForceUI[] _drivingForceUI;

    /// <summary>
    /// �÷��̾��� ���� �����¿� ���� ������Ƽ�Դϴ�.
    /// </summary>
    public int CurrentDrivingForce
    {
        get => _currentDrivingForce;
        set
        {
            // �ʱ�ȭ �� ���� ���
            _currentDrivingForce = value;
            GameManager.instance.playerCurrentDrivingForce = value;
            for (int i = 0; i < _currentDrivingForce; i++)
            {
                _drivingForceUI[i].animator.SetTrigger(hashFill);
            }
        }
    }

    void Awake()
    {
        _currentDrivingForce = 0;

        // ������ UI �ʱ�ȭ
        GameObject drivingForceUI = GameObject.Find("DrivingForce");
        int drivingForceCount = drivingForceUI.transform.childCount;
        _drivingForceUI = new DrivingForceUI[drivingForceCount];
        for (int i = 0; i < _drivingForceUI.Length; i++)
        {
            Transform drivingForce = drivingForceUI.transform.GetChild(i);

            _drivingForceUI[i].image = drivingForce.GetComponent<Image>();
            _drivingForceUI[i].animator = drivingForce.GetComponent<Animator>();
        }
    }

    /// <summary>
    /// �÷��̾� ĳ������ �������� ������Ű�� �޼ҵ��Դϴ�.
    /// </summary>
    public void IncreaseDrivingForce()
    {
        // ���� �������� �ִ� �����°� �����ϸ� �������� ����
        if(_currentDrivingForce == maxDrivingForce) return;

        // �������� 1 ������Ű�� UI�� �ݿ�
        _currentDrivingForce++;
        _drivingForceUI[_currentDrivingForce - 1].animator.SetTrigger(hashFilled);

        // ���� �Ŵ����� �ݿ�
        GameManager.instance.playerCurrentDrivingForce = _currentDrivingForce;
    }

    /// <summary>
    /// �÷��̾� ĳ������ �������� �Ҹ��Ű�� �޼ҵ��Դϴ�.
    /// ���� �������� �Ҹ�Ǵ� �����º��� ������ ������� �ʰ� false�� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="drivingForce">�Ҹ�Ǵ� �������� ��</param>
    public bool TryConsumeDrivingForce(int drivingForce)
    {
        // ���� �������� �Ҹ�Ǵ� �����º��� ������ false ��ȯ
        if(_currentDrivingForce < drivingForce) return false;

        // ������ �Ҹ�
        int prevDrivingForce = _currentDrivingForce;
        _currentDrivingForce -= drivingForce;

        // UI�� ������ �Ҹ� �ݿ�
        for(int i = prevDrivingForce - 1; i >= prevDrivingForce - drivingForce; i--)
        {
            _drivingForceUI[i].animator.SetTrigger(hashUse);
        }

        // ���� �Ŵ��� ������Ʈ
        GameManager.instance.playerCurrentDrivingForce = _currentDrivingForce;

        return true;
    }
}