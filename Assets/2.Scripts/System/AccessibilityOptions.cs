using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// 접근성 옵션을 설정하고 관리하는 매니저 정적 클래스입니다.
/// </summary>
public static class AccessibilitySettingsManager
{
    public static float gamepadVibration = 1.0f;    // 게임패드의 진동 강도
    public static bool screenShake = true;          // 화면이 흔들리는지 여부
    public static bool screenFlashes = true;        // 화면이 번쩍이는지 여부

    /// <summary>
    /// 접근성 매니저를 초기화하는 정적 메소드입니다.
    /// </summary>
    public static void Init()
    {
        gamepadVibration = OptionsData.optionsSaveData.gamepadVibration;
        screenShake = OptionsData.optionsSaveData.screenShake;
        screenFlashes = OptionsData.optionsSaveData.screenFlashes;
    }

    /// <summary>
    /// 게임패드의 진동의 세기를 설정하는 정적 메소드입니다.
    /// </summary>
    /// <param name="increase">진동 세기가 증가하는지 여부</param>
    public static void SetGamepadVibration(bool increase)
    {
        // 진동 세기 설정
        gamepadVibration += increase ? 0.2f : -0.2f;
        gamepadVibration = Mathf.Clamp(gamepadVibration, 0f, 1f);

        // 게임패드에 진동을 일으켜서 현재 진동 세기를 사용자가 체감할 수 있게 함
        GamepadVibrationManager.instance.GamepadRumbleStart(gamepadVibration, 0.05f);

        // 현재 진동 세기를 옵션 데이터에 저장
        OptionsData.optionsSaveData.gamepadVibration = gamepadVibration;
    }

    /// <summary>
    /// 게임패드의 현재 진동 세기를 표시하기 위한 UI를 가져오는 정적 메소드입니다.
    /// </summary>
    /// <returns>게임패드의 진동 세기를 나타내는 UI</returns>
    public static string GetGamepadVibrationToUI()
    {
        int vibration = Mathf.RoundToInt(gamepadVibration * 5);
        StringBuilder vibrationToText = new StringBuilder();
        for (int i = 0; i < 5; i++)
        {
            if (i < vibration)
            {
                vibrationToText.Append("■");
            }
            else
            {
                vibrationToText.Append("□");
            }
        }
        return vibrationToText.ToString();
    }

    /// <summary>
    /// 화면 흔들림을 사용할 것이지 설정하는 정적 메소드입니다.
    /// </summary>
    public static void SetScreenShake()
    {
        screenShake = !screenShake;
        OptionsData.optionsSaveData.screenShake = screenShake;
    }

    /// <summary>
    /// 화면 흔들기를 사용하고 있는 상태인지 여부를 나타내는 Toggle UI를 가져오는 메소드입니다.
    /// </summary>
    /// <returns>화면 흔들기를 사용하고 있는 상태인지 나타내는 Toggle UI</returns>
    public static string GetScreenShakeToggle()
    {
        return screenShake ? LanguageManager.GetText("Enabled") : LanguageManager.GetText("Disabled");
    }

    /// <summary>
    /// 화면 번쩍임을 사용할 것이지 설정하는 정적 메소드입니다.
    /// </summary>
    public static void SetScreenFlashes()
    {
        screenFlashes = !screenFlashes;
        OptionsData.optionsSaveData.screenFlashes = screenFlashes;
    }

    /// <summary>
    /// 화면 번쩍임을 사용하고 있는 상태인지 여부를 나타내는 Toggle UI를 가져오는 메소드입니다.
    /// </summary>
    /// <returns>화면 번쩍임을 사용하고 있는 상태인지 나타내는 Toggle UI</returns>
    public static string GetScreenFlashesToggle()
    {
        return screenFlashes == true ? LanguageManager.GetText("Enabled") : LanguageManager.GetText("Disabled");
    }
}