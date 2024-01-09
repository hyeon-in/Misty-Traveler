using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어의 원동력을 처리하는 클래스
/// </summary>
public class PlayerDrivingForce : MonoBehaviour
{
    readonly int hashFill = Animator.StringToHash("Fill");
    readonly int hashFilled = Animator.StringToHash("Filled");
    readonly int hashUse = Animator.StringToHash("Use");

    public int maxDrivingForce = 6; // 최대 원동력
    int _currentDrivingForce;       // 현재 원동력

    /// <summary>
    /// 플레이어 캐릭터의 원동력 UI 정보를 담는 구조체입니다.
    /// </summary> 
    [System.Serializable]
    struct DrivingForceUI
    {
        [HideInInspector] public Image image;
        [HideInInspector] public Animator animator;
    }
    DrivingForceUI[] _drivingForceUI;

    /// <summary>
    /// 플레이어의 현재 원동력에 대한 프로퍼티입니다.
    /// </summary>
    public int CurrentDrivingForce
    {
        get => _currentDrivingForce;
        set
        {
            // 초기화 할 때만 사용
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

        // 원동력 UI 초기화
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
    /// 플레이어 캐릭터의 원동력을 증가시키는 메소드입니다.
    /// </summary>
    public void IncreaseDrivingForce()
    {
        // 현재 원동력이 최대 원동력과 동일하면 실행하지 않음
        if(_currentDrivingForce == maxDrivingForce) return;

        // 원동력을 1 증가시키고 UI에 반영
        _currentDrivingForce++;
        _drivingForceUI[_currentDrivingForce - 1].animator.SetTrigger(hashFilled);

        // 게임 매니저에 반영
        GameManager.instance.playerCurrentDrivingForce = _currentDrivingForce;
    }

    /// <summary>
    /// 플레이어 캐릭터의 원동력을 소모시키는 메소드입니다.
    /// 현재 원동력이 소모되는 원동력보다 적으면 실행되지 않고 false를 반환합니다.
    /// </summary>
    /// <param name="drivingForce">소모되는 원동력의 값</param>
    public bool TryConsumeDrivingForce(int drivingForce)
    {
        // 현재 원동력이 소모되는 원동력보다 적으면 false 반환
        if(_currentDrivingForce < drivingForce) return false;

        // 원동력 소모
        int prevDrivingForce = _currentDrivingForce;
        _currentDrivingForce -= drivingForce;

        // UI에 원동력 소모 반영
        for(int i = prevDrivingForce - 1; i >= prevDrivingForce - drivingForce; i--)
        {
            _drivingForceUI[i].animator.SetTrigger(hashUse);
        }

        // 게임 매니저 업데이트
        GameManager.instance.playerCurrentDrivingForce = _currentDrivingForce;

        return true;
    }
}