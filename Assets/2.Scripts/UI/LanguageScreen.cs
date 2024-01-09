using System.Collections;
using System.Collections.Generic;
using TMPro;
using MenuUI;
using UnityEngine;

/// <summary>
/// 언어 옵션을 설정하기 위한 메뉴 화면을 처리하는 클래스입니다.
/// </summary>
public class LanguageScreen : MonoBehaviour
{
    public GameObject optionsMenuScreen;    // 옵션 메뉴 화면
    public TextMeshProUGUI languageText;    // 옵션 명칭 텍스트
    public TextMeshProUGUI manualText;      // 메뉴얼 텍스트
    public Menu[] menu; // 메뉴 배열

    bool _rightInput, _leftInput;   // 왼쪽, 오른쪽 입력 여부

    void Awake()
    {
        if (manualText == null)
        {
            manualText = GameObject.Find("ManualText").GetComponent<TextMeshProUGUI>();
        }
    }

    void OnEnable()
    {
        LanguageOptionsRefresh();
        MenuUIController.SetMenualText(menu[0], manualText);
    }

    void Update()
    {
        // 입력 받기
        _rightInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Right);
        _leftInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Left);
        bool backInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Cancle);

        if (_rightInput || _leftInput)
        {
            // 왼쪽이나 오른쪽 입력시 메뉴 선택 이벤트 실행(접근성 옵션 설정)
            menu[0].menuSelectEvent.Invoke();
            MenuUIController.SetMenualText(menu[0], manualText);
        }

        if (backInput)
        {
            // 뒤로 가는 버튼 입력시 언어 옵션을 종료하고 옵션 메뉴로 돌아감
            ReturnToOptionsMenuScreen();
        }
    }

    /// <summary>
    /// 언어를 설정하는 메소드입니다.
    /// </summary>
    public void SetLanguage()
    {
        bool right = _rightInput ? false : true;
        LanguageManager.SetLanguage(right);
        LanguageOptionsRefresh();
    }

    /// <summary>
    /// 언어 옵션 메뉴의 텍스트들의 내용을 설정에서 가져와 새로고침하는 메소드입니다.
    /// </summary>
    void LanguageOptionsRefresh()
    {
        languageText.text = LanguageManager.GetText("Language");
        menu[0].text[0].text = LanguageManager.GetCurrentLanguageToText();
    }

    /// <summary>
    /// 언어 옵션을 종료하고 옵션 메뉴로 돌아가는 메소드입니다.
    /// </summary>
    void ReturnToOptionsMenuScreen()
    {
        this.gameObject.SetActive(false);
        optionsMenuScreen.SetActive(true);
    }
}