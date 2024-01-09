using System.Collections;
using System.Collections.Generic;
using TMPro;
using MenuUI;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

/// <summary>
/// 플레이어 조작과 관련된 옵션을 설정하기 위한 메뉴 화면을 처리하는 클래스입니다.
/// </summary>
public class ControlsScreen : MonoBehaviour
{
    public GameObject optionsMenuScreen;    // 옵션 메뉴 화면
    public TextMeshProUGUI controlsText;    // 옵션 명칭 텍스트
    public TextMeshProUGUI actionsText;     // 플레이어의 행동들을 나타내고 있음을 표시 텍스트
    public TextMeshProUGUI keyboardText;    // 키보드 입력과 관련된 옵션임을 나타내는 표시 텍스트
    public TextMeshProUGUI controllerText;  // 컨트롤러 입력과 관련된 옵션임을 나타내는 표시 텍스트
    public TextMeshProUGUI manualText;      // 메뉴얼 텍스트
    public Menu[] menu; // 메뉴 배열

    int _currentMenuIndex;  // 현재 메뉴의 인덱스
    bool _isMapping;        // 맵핑이 진행되고 있음을 체크 

    void Awake()
    {
        if (manualText == null)
        {
            manualText = GameObject.Find("ManualText").GetComponent<TextMeshProUGUI>();
        }
        // 새로 고침
        KeyRefresh();
        ButtonRefresh();
    }

    void OnEnable()
    {
        ControlsOptionsRefresh();
        MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
        SetNonSelectedDeviceTextColor();
    }

    void Update()
    {
        // 매핑 중이면 중단
        if (_isMapping) return;

        // 입력 받기
        bool upInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Up);
        bool downInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Down);
        bool selectInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Select);
        bool backInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Cancle);

        if (upInput)
        {
            // 위 입력시 인덱스 감소(선택 메뉴를 위로 이동)
            _currentMenuIndex--;
            MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
            SetNonSelectedDeviceTextColor();
        }
        else if (downInput)
        {
            // 아래 입력시 인덱스 증가(선택 메뉴를 아래로 이동)
            _currentMenuIndex++;
            MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
            SetNonSelectedDeviceTextColor();
        }

        if (selectInput)
        {
            // 선택 입력시 메뉴 선택 이벤트 실행
            menu[_currentMenuIndex].menuSelectEvent.Invoke();
            SetNonSelectedDeviceTextColor();
        }

        if (backInput)
        {
            // 뒤로 가는 버튼을 입력시 조작 설정 옵션을 종료하고 옵션 메뉴로 돌아감
            _currentMenuIndex = 0;
            MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
            ReturnToOptionsMenuScreen();
        }
    }

    /// <summary>
    /// 특정 플레이어 행동에 대한 버튼 매핑 코루틴을 시작하는 메소드입니다.
    /// </summary>
    /// <param name="action">입력 설정을 변경하려는 플레이어의 행동</param>
    public void SetButtonMapping(string action)
    {
        var stringToAction = (GameInputManager.PlayerActions)Enum.Parse(typeof(GameInputManager.PlayerActions), action); // string을 PlayerActions로 변환
        StartCoroutine(ChooseButtonToMap(stringToAction));
    }

    /// <summary>
    /// 버튼 매핑을 진행하는 코루틴입니다.
    /// </summary>
    /// <param name="action">입력 설정을 변경하려는 플레이어의 행동</param>
    IEnumerator ChooseButtonToMap(GameInputManager.PlayerActions action)
    {
        _isMapping = true;  // 매핑을 하고 있는 상태로 변경

        // 설정하려는 버튼(혹은 키)의 텍스트의 색 변경
        TextMeshProUGUI buttonText;
        if(GameInputManager.usingController)
        {
            buttonText = menu[_currentMenuIndex].text[2];
        }
        else
        {
            buttonText = menu[_currentMenuIndex].text[1];
        }
        buttonText.color = new Color32(141, 105, 122, 255);
        manualText.text = LanguageManager.GetText("Empty"); // 설명 텍스트를 비워놓음

        yield return null;

        // 버튼 매핑이 완료될 때 까지 실행
        while(true)
        {
            if (GameInputManager.usingController)
            {
                // 입력한 컨트롤러 버튼으로 변경
                GamepadButton inputButton = GameInputManager.GetCurrentInputButton();

                if (inputButton != GamepadButton.Start)
                {
                    GameInputManager.GamepadMapping(action, inputButton);
                    ButtonRefresh();
                    break;
                }
            }
            else
            {
                // 입력한 키로 변경
                Key inputKey = GameInputManager.GetCurrentInputKey();

                if(inputKey != Key.None)
                {
                    GameInputManager.KeyboardMapping(action, inputKey);
                    KeyRefresh();
                    break;
                }
            }
            yield return null;
        }

        // 색을 원레대로 돌리고 매핑을 하고 있지 않는 상태로 변경
        MenuUIController.SelectMenuTextColorChange(menu[_currentMenuIndex]);
        _isMapping = false;

        // 메뉴 새로고침 및 
        MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
        SetNonSelectedDeviceTextColor();
    }

    /// <summary>
    /// 입력 설정을 기본 값으로 변경하는 메소드 입니다.
    /// 컨트롤러를 사용하고 있을 경우 컨트롤러 입력 버튼을 기본 값으로 설정하며, 키보드를 사용하고 있을 경우 키보드 입력 키를 기본 값으로 설정합니다.
    /// </summary>
    public void ReturnToDefault()
    {
        if (GameInputManager.usingController)
        {
            GameInputManager.ControllerInputSetDefaults();
            ButtonRefresh();
        }
        else
        {
            GameInputManager.KeyboardInputSetDefaults();
            KeyRefresh();
        }
    }

    /// <summary>
    /// 현재 선택된 메뉴의 텍스트 중 현재 사용 중이지 않은 장치 쪽의 색상을 변경하여 선택되지 않았음을 나타내는 메소드입니다.
    /// </summary>
    void SetNonSelectedDeviceTextColor()
    {
        if (menu[_currentMenuIndex].text.Length <= 1) return;

        if (GameInputManager.usingController)
        {
            menu[_currentMenuIndex].text[1].color = MenuUIController.notSelectColor;
        }
        else
        {
            menu[_currentMenuIndex].text[2].color = MenuUIController.notSelectColor;
        }
    }

    /// <summary>
    /// 컨트롤 옵션 텍스트들을 언어 설정에 맞춰 표시하고 새로 고침하는 메소드입니다.
    /// </summary>
    void ControlsOptionsRefresh()
    {
        controlsText.text = LanguageManager.GetText("Controls");
        actionsText.text = LanguageManager.GetText("Actions");
        keyboardText.text = LanguageManager.GetText("Keyboard");
        controllerText.text = LanguageManager.GetText("Controller");
        for (int i = 0; i < menu.Length; i++)
        {
            switch (menu[i].text[0].name)
            {
                case "MoveLeftText":
                    menu[i].text[0].text = LanguageManager.GetText("MoveLeft");
                    break;
                case "MoveRightText":
                    menu[i].text[0].text = LanguageManager.GetText("MoveRight");
                    break;
                case "MoveUpText":
                    menu[i].text[0].text = LanguageManager.GetText("MoveUp");
                    break;
                case "MoveDownText":
                    menu[i].text[0].text = LanguageManager.GetText("MoveDown");
                    break;
                case "JumpText":
                    menu[i].text[0].text = LanguageManager.GetText("Jump");
                    break;
                case "AttackText":
                    menu[i].text[0].text = LanguageManager.GetText("Attack");
                    break;
                case "SpecialAttackText":
                    menu[i].text[0].text = LanguageManager.GetText("SpecialAttack");
                    break;
                case "DodgeText":
                    menu[i].text[0].text = LanguageManager.GetText("Dodge");
                    break;
                case "ResetToDefaultText":
                    menu[i].text[0].text = LanguageManager.GetText("ResetToDefault");
                    break;
            }
        }
    }

    /// <summary>
    /// 현재 키보드의 키 입력 설정을 텍스트로 표시하고 새로고침하는 메소드입니다.
    /// </summary>
    void KeyRefresh()
    {
        for (int i = 0; i < menu.Length; i++)
        {
            switch (menu[i].text[0].name)
            {
                case "MoveLeftText":
                    menu[i].text[1].text = GameInputManager.ActionToKeyText(GameInputManager.PlayerActions.MoveLeft);
                    break;
                case "MoveRightText":
                    menu[i].text[1].text = GameInputManager.ActionToKeyText(GameInputManager.PlayerActions.MoveRight);
                    break;
                case "MoveUpText":
                    menu[i].text[1].text = GameInputManager.ActionToKeyText(GameInputManager.PlayerActions.MoveUp);
                    break;
                case "MoveDownText":
                    menu[i].text[1].text = GameInputManager.ActionToKeyText(GameInputManager.PlayerActions.MoveDown);
                    break;
                case "JumpText":
                    menu[i].text[1].text = GameInputManager.ActionToKeyText(GameInputManager.PlayerActions.Jump);
                    break;
                case "AttackText":
                    menu[i].text[1].text = GameInputManager.ActionToKeyText(GameInputManager.PlayerActions.Attack);
                    break;
                case "SpecialAttackText":
                    menu[i].text[1].text = GameInputManager.ActionToKeyText(GameInputManager.PlayerActions.SpecialAttack);
                    break;
                case "DodgeText":
                    menu[i].text[1].text = GameInputManager.ActionToKeyText(GameInputManager.PlayerActions.Dodge);
                    break;
#if UNITY_EDITOR
                case "ResetToDefaultText":
                    break;
                default:
                    Debug.LogError(menu[i].text[0].name + "은 잘못된 이름입니다");
                    break;
#endif
            }
        }
    }

    /// <summary>
    /// 현재 컨트롤러의 버튼 입력 설정을 텍스트로 표시하고 새로고침하는 메소드입니다.
    /// </summary>
    void ButtonRefresh()
    {
        for (int i = 0; i < menu.Length; i++)
        {
            switch (menu[i].text[0].name)
            {
                case "MoveLeftText":
                    menu[i].text[2].text = GameInputManager.ActionToButtonText(GameInputManager.PlayerActions.MoveLeft);
                    break;
                case "MoveRightText":
                    menu[i].text[2].text = GameInputManager.ActionToButtonText(GameInputManager.PlayerActions.MoveRight);
                    break;
                case "MoveUpText":
                    menu[i].text[2].text = GameInputManager.ActionToButtonText(GameInputManager.PlayerActions.MoveUp);
                    break;
                case "MoveDownText":
                    menu[i].text[2].text = GameInputManager.ActionToButtonText(GameInputManager.PlayerActions.MoveDown);
                    break;
                case "JumpText":
                    menu[i].text[2].text = GameInputManager.ActionToButtonText(GameInputManager.PlayerActions.Jump);
                    break;
                case "AttackText":
                    menu[i].text[2].text = GameInputManager.ActionToButtonText(GameInputManager.PlayerActions.Attack);
                    break;
                case "SpecialAttackText":
                    menu[i].text[2].text = GameInputManager.ActionToButtonText(GameInputManager.PlayerActions.SpecialAttack);
                    break;
                case "DodgeText":
                    menu[i].text[2].text = GameInputManager.ActionToButtonText(GameInputManager.PlayerActions.Dodge);
                    break;
#if UNITY_EDITOR
                case "ResetToDefaultText":
                    break;
                default:
                    Debug.LogError(menu[i].text[0].name + "은 잘못된 이름입니다");
                    break;
#endif
            }
        }
    }


    /// <summary>
    /// 조작 버튼 설정 옵션을 종료하고 옵션 메뉴로 돌아가는 메소드입니다.
    /// </summary>
    void ReturnToOptionsMenuScreen()
    {
        gameObject.SetActive(false);
        optionsMenuScreen.SetActive(true);
    }
}
