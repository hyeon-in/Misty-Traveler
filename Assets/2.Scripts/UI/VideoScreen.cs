using System.Collections;
using System.Collections.Generic;
using TMPro;
using MenuUI;
using UnityEngine;

/// <summary>
/// 비디오 옵션을 설정하기 위한 메뉴 화면을 처리하는 클래스입니다.
/// </summary>
public class VideoScreen : MonoBehaviour
{
    public GameObject optionsMenuScreen;    // 옵션 메뉴
    public TextMeshProUGUI videoText;       // 옵션 명칭 텍스트
    public TextMeshProUGUI manualText;      // 메뉴얼 텍스트
    public Menu[] menu;     // 메뉴 배열
    int _currentMenuIndex;  // 현재 선택한 메뉴의 인덱스

    bool _rightInput, _leftInput, _selectInput; // 입력 여부
    bool _increase;

    void Awake()
    {
        if (manualText == null)
        {
            manualText = GameObject.Find("ManualText").GetComponent<TextMeshProUGUI>();
        }
    }

    void OnEnable()
    {
        VideoOptionsRefresh();
        MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
    }

    void Update()
    {
        // 입력 여부
        bool upInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Up);
        bool downInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Down);
        _rightInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Left);
        _leftInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Right);
        _selectInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Select);
        bool backInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Cancle);

        if (upInput)
        {
            // 위 입력시 인덱스 감소(선택 메뉴를 위로 이동)
            _currentMenuIndex--;
            VideoSettingsManager.ResolutionIndexReturn();
            VideoOptionsRefresh();
            MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
        }
        else if (downInput)
        {
            // 아래 입력시 인덱스 증가(선택 메뉴를 아래로 이동)
            _currentMenuIndex++;
            VideoSettingsManager.ResolutionIndexReturn();
            VideoOptionsRefresh();
            MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
        }
        else if (_rightInput || _leftInput)
        {
            // 왼쪽이나 오른쪽 입력시 메뉴 선택 이벤트 실행(접근성 옵션 설정)
            _increase = _leftInput;
            menu[_currentMenuIndex].menuSelectEvent.Invoke();
        }
        else if (_selectInput)
        {
            if (menu[_currentMenuIndex].text[0].name == "ResolutionText")
            {
                // 해상도 메뉴에서 선택 입력시 변경한 해상도 메뉴 설정 적용
                VideoSettingsManager.NewResolutionAccept();
                MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
            }
        }
        else if (backInput)
        {
            // 뒤로 가는 버튼 입력시 비디오 옵션을 종료하고 옵션 메뉴로 돌아감
            _currentMenuIndex = 0;
            VideoSettingsManager.ResolutionIndexReturn();
            VideoOptionsRefresh();
            MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
            ReturnToOptionsMenuScreen();
        }
    }

    /// <summary>
    /// 전체 화면 사용 여부를 설정하는 메소드입니다.
    /// </summary>
    public void SetFullScreen()
    {
        VideoSettingsManager.SetFullScreen();
        VideoOptionsRefresh();
    }

    /// <summary>
    /// 해상도를 설정하는 메소드입니다.
    /// </summary>
    public void SetResolution()
    {
        VideoSettingsManager.SetResolution(_increase);
        menu[_currentMenuIndex].text[1].color = new Color32(141, 105, 122, 255); // 변경하려고 하는 상태이면 텍스트 색이 변함

        VideoOptionsRefresh();
    }

    /// <summary>
    /// VSync 사용 여부를 설정하는 메소드입니다.
    /// </summary>
    public void SetVSync()
    {
        VideoSettingsManager.SetVSync();
        VideoOptionsRefresh();
    }

    /// <summary>
    /// 비디오 옵션 메뉴의 텍스트들의 내용을 언어 데이터에서 가져와 새로고침하는 메소드입니다.
    /// </summary>
    void VideoOptionsRefresh()
    {
        videoText.text = LanguageManager.GetText("Video");
        for (int i = 0; i < menu.Length; i++)
        {
            switch (menu[i].text[0].name)
            {
                case "FullScreenText":
                    menu[i].text[0].text = LanguageManager.GetText("FullScreen");
                    menu[i].text[1].text = VideoSettingsManager.GetFullScreenStatusText();
                    break;
                case "ResolutionText":
                    menu[i].text[0].text = LanguageManager.GetText("Resolution");
                    menu[i].text[1].text = VideoSettingsManager.GetCurrentResolutionText();
                    break;
                case "VSyncText":
                    menu[i].text[0].text = LanguageManager.GetText("VSync");
                    menu[i].text[1].text = VideoSettingsManager.GetVSyncStatusText();
                    break;
            }
        }
    }

    /// <summary>
    /// 비디오 옵션을 종료하고 옵션 메뉴로 돌아가는 메소드입니다.
    /// </summary>
    void ReturnToOptionsMenuScreen()
    {
        this.gameObject.SetActive(false);
        optionsMenuScreen.SetActive(true);
    }
}
