using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MenuUI;
using TMPro;

/// <summary>
/// 타이틀 화면과 관련된 기능들을 처리하는 클래스입니다.
/// </summary>
public class TitleScreen : MonoBehaviour
{
    public TextMeshProUGUI title;           // 타이틀 제목
    public TextMeshProUGUI manualText;      // 메뉴얼 텍스트
    public TextMeshProUGUI copyrightText;   // 저작권 텍스트
    public TextMeshProUGUI versionText;   // 버전 표시 텍스트
    public GameObject optionsScreen;        // 옵션 화면
    public GameObject selectProfileScreen;  // 프로필 선택 화면
    public Menu[] menu; // 메뉴 배열

    bool _isOtherScreenOpening; // 다른 화면이 열려있는 상태임을 나타냄
    int _currentMenuIndex = 0;  // 현재 선택한 메뉴의 인덱스
    void Start()
    {
        if (manualText == null)
        {
            manualText = GameObject.Find("ManualText").GetComponent<TextMeshProUGUI>();
        }
        if (copyrightText == null)
        {
            copyrightText = GameObject.Find("CopyrightText").GetComponent<TextMeshProUGUI>();
        }
        if (versionText == null)
        {
            versionText = GameObject.Find("VersionText").GetComponent<TextMeshProUGUI>();
        }

        TitleTextRefresh();
        MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
        optionsScreen.GetComponent<OptionsScreen>().PrevScreenReturn += OnPrevScreenReturn;
        selectProfileScreen.GetComponent<SelectProfileScreen>().PrevScreenReturn += OnPrevScreenReturn;
    }

    void Update()
    {
        // 다른 화면이 실행 중이면 중단
        if (_isOtherScreenOpening) return;

        TitleTextRefresh();
        MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);

        // 입력 받기
        bool upInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Up);
        bool downInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Down);
        bool selectInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Select);
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
        if (selectInput)
        {
            // 선택 입력시 메뉴 선택 이벤트 실행
            menu[_currentMenuIndex].menuSelectEvent.Invoke();
        }
    }

    /// <summary>
    /// 다른 화면을 열기 위한 메소드입니다.
    /// </summary>
    /// <param name="openScreen">열고자 하는 화면(게임 오브젝트)</param>
    public void OpenOtherScreen(GameObject openScreen)
    {
        _isOtherScreenOpening = true;
        title.alpha = 0;
        versionText.alpha = copyrightText.alpha = 0;
        MenuUIController.AllMenuTextHide(menu);
        openScreen.SetActive(true);
    }

    /// <summary>
    /// 옵션 메뉴 화면과 프로필 선택 화면에서 다시 타이틀 화면으로 돌아왔을때 호출하는 메소드입니다.
    /// </summary>
    void OnPrevScreenReturn()
    {
        title.alpha = 1;
        versionText.alpha = copyrightText.alpha = 1;
        _isOtherScreenOpening = false;
        TitleTextRefresh();
        MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
    }

    /// <summary>
    /// 게임 종료 메소드입니다.
    /// </summary>
    public void GameExit()
    {
        OptionsData.OptionsSave();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    /// <summary>
    /// 타이틀 화면의 텍스트들의 내용을 언어 데이터에서 가져와 새로고침하는 메소드입니다.
    /// </summary>
    void TitleTextRefresh()
    {
        for(int i = 0; i < menu.Length; i++)
        {
            switch(menu[i].text[0].name)
            {
                case "StartText":
                    menu[i].text[0].text = LanguageManager.GetText("Start");
                    break;
                case "OptionsText":
                    menu[i].text[0].text = LanguageManager.GetText("Options");
                    break;
                case "ExitText":
                    menu[i].text[0].text = LanguageManager.GetText("Exit");
                    break;
            }
        }
    }
}
