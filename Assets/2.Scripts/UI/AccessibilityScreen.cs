using System.Collections;
using System.Collections.Generic;
using TMPro;
using MenuUI;
using UnityEngine;

/// <summary>
/// 접근성 옵션을 설정하기 위한 메뉴 화면을 처리하는 클래스입니다.
/// </summary>
public class AccessibilityScreen : MonoBehaviour
{
    public GameObject optionsMenuScreen;        // 옵션 메뉴 화면
    public TextMeshProUGUI accessibilityText;   // 옵션 명칭 텍스트
    public TextMeshProUGUI manualText;          // 메뉴얼 텍스트
    public Menu[] menu;         // 메뉴 배열
    int _currentMenuIndex;      // 현재 선택된 메뉴 인덱스

    bool _rightInput;   // 오른쪽 입력 여부
    bool _leftInput;    // 왼쪽 입력 여부

    void OnEnable()
    {
        // 메뉴가 활성화 될 때 UI 새로 고침
        AccessibilityOptionsRefresh();
        MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
    }

    void Update()
    {
        // 입력 받아오기
        bool upInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Up);
        bool downInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Down);
        _rightInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Right);
        _leftInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Left);
        bool backInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Cancle);

        if (upInput)
        {
            // 위 입력시 인덱스 감소(선택 메뉴를 위로 이동)
            _currentMenuIndex--;
            MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
        }
        else if (downInput)
        {
            // 아래 입력시 인덱스 증가(선택 메뉴를 아래로 이동)
            _currentMenuIndex++;
            MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
        }

        if (_rightInput || _leftInput)
        {
            // 왼쪽이나 오른쪽 입력시 메뉴 선택 이벤트 실행(접근성 옵션 설정)
            menu[_currentMenuIndex].menuSelectEvent.Invoke();
        }

        if (backInput)
        {
            // 뒤로 가는 버튼 입력시 접근성 옵션을 종료하고 옵션 메뉴로 돌아감
            _currentMenuIndex = 0;
            MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
            ReturnToOptionsMenuScreen();
        }
    }

    /// <summary>
    /// 게임패드의 진동 사용 여부를 설정하는 메소드입니다.
    /// </summary>
    public void SetGamepadVibration()
    {
        bool increase = _leftInput ? false : true;
        AccessibilitySettingsManager.SetGamepadVibration(increase);
        AccessibilityOptionsRefresh();
    }

    /// <summary>
    /// 화면의 흔들림 여부를 설정하는 메소드입니다.
    /// </summary>
    public void SetScreenShake()
    {
        AccessibilitySettingsManager.SetScreenShake();
        AccessibilityOptionsRefresh();
    }

    /// <summary>
    /// 화면의 번쩍임 여부를 설정하는 메소드입니다.
    /// </summary>
    public void SetScreenFlashes()
    {
        AccessibilitySettingsManager.SetScreenFlashes();
        AccessibilityOptionsRefresh();
    }

    /// <summary>
    /// 접근성 옵션 메뉴의 텍스트들의 내용을 언어 데이터에서 가져와 새로고침하는 메소드입니다.
    /// </summary>
    void AccessibilityOptionsRefresh()
    {
        accessibilityText.text = LanguageManager.GetText("Accessibility");
        for (int i = 0; i < menu.Length; i++)
        {
            switch (menu[i].text[0].name)
            {
                case "ControllerVibrationText":
                    menu[i].text[0].text = LanguageManager.GetText("ControllerVibration");
                    menu[i].text[1].text = AccessibilitySettingsManager.GetGamepadVibrationToUI();
                    break;
                case "ScreenShakeText":
                    menu[i].text[0].text = LanguageManager.GetText("ScreenShake");
                    menu[i].text[1].text = AccessibilitySettingsManager.GetScreenShakeToggle();
                    break;
                case "ScreenFlashesText":
                    menu[i].text[0].text = LanguageManager.GetText("ScreenFlashes");
                    menu[i].text[1].text = AccessibilitySettingsManager.GetScreenFlashesToggle();
                    break;
            }
        }
    }

    /// <summary>
    /// 접근성 옵션을 종료하고 옵션 메뉴로 돌아가는 메소드입니다.
    /// </summary>
    void ReturnToOptionsMenuScreen()
    {
        gameObject.SetActive(false);
        optionsMenuScreen.SetActive(true);
    }

}
