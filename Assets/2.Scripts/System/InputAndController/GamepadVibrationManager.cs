using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 게임패드의 진동을 관리하는 싱글톤 클래스입니다.
/// </summary>
public class GamepadVibrationManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GamepadVibrationManager instance = null;
    Coroutine _gamepadRumble = null;

    void Awake()
    {
        // 하나의 매니저 인스턴스만 존재하도록 함(싱글톤)
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 왼쪽과 오른쪽 모터에 대한 강도를 지정하여 게임패드 진동을 시작하는 메소드입니다.
    /// </summary>
    /// <param name="left">왼쪽 모터 진동 강도</param>
    /// <param name="right">오른쪽 모터 진동 강도</param>
    /// <param name="duration">진동 지속 시간(초)</param>
    public void GamepadRumbleStart(float left, float right, float duration)
    {
        // 컨트롤러를 사용하고 있지 않으면 중단
        if(!GameInputManager.usingController) return;

        // 진동하고 있을 경우 기존 진동을 중단
        if (_gamepadRumble != null)
        {
            StopCoroutine(_gamepadRumble);
            _gamepadRumble = null;
        }

        // 접근성 설정에 따라 진동 강도를 조정한 뒤 진동 코루틴 시작
        left = left * AccessibilitySettingsManager.gamepadVibration;
        right = right * AccessibilitySettingsManager.gamepadVibration;
        _gamepadRumble = StartCoroutine(GamepadRumble(left, right, duration));
    }

    /// <summary>
    /// 강도를 지정하여 게임패드 진동을 시작하는 메소드입니다.
    /// </summary>
    /// <param name="intensity">모터 진동 강도(왼쪽과 오른쪽 모두 같은 진동 적용)</param>
    /// <param name="duration">진동 지속 시간(초)</param>
    public void GamepadRumbleStart(float intensity, float duration)
    {
        // 컨트롤러를 사용하고 있지 않으면 중단
        if (!GameInputManager.usingController) return;

        // 진동하고 있을 경우 기존 진동을 중단
        if (_gamepadRumble != null)
        {
            StopCoroutine(_gamepadRumble);
            _gamepadRumble = null;
        }

        // 접근성 설정에 따라 진동 강도를 조정한 뒤 진동 코루틴 시작
        intensity = intensity * AccessibilitySettingsManager.gamepadVibration;
        _gamepadRumble = StartCoroutine(GamepadRumble(intensity * 0.3f, intensity, duration));
    }

    /// <summary>
    /// 게임패드의 진동을 중단하는 메소드입니다.
    /// </summary>
    public void GamepadRumbleStop()
    {
        // 컨트롤러를 사용하고 있지 않으면 중단
        if (!GameInputManager.usingController) return;

        // 진동 중지 및 햅틱 리셋
        if (_gamepadRumble != null)
        {
            StopCoroutine(_gamepadRumble);
            InputSystem.ResetHaptics();
            _gamepadRumble = null;
        }
    }

    /// <summary>
    /// 지정된 시간 동안 게임패드의 진동을 처리하는 코루틴입니다.
    /// </summary>
    /// <param name="left">왼쪽 모터 진동 강도</param>
    /// <param name="right">오른쪽 모터 진동 강도</param>
    /// <param name="duration">진동 지속 시간(초)</param>
    IEnumerator GamepadRumble(float left, float right, float duration)
    {
        // 게임패드의 진동 설정
        Gamepad.current.SetMotorSpeeds(left, right);

        // 지정된 시간동안 대기
        yield return YieldInstructionCache.WaitForSecondsRealtime(duration);

        // 시간이 지난 후 햅틱을 리셋하고 코루틴 참조를 해제
        InputSystem.ResetHaptics();
        _gamepadRumble = null;
    }
}
