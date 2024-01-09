using System.Collections;
using System.Collections.Generic;
using TMPro;
using MenuUI;
using UnityEngine;

/// <summary>
/// 옵션 메뉴 동작을 처리하는 클래스입니다.
/// </summary>
public class OptionsScreen : MonoBehaviour
{
    // 이전 화면으로 돌아갈 때 호출할 델리게이트(타이틀 화면에서만 호출됨)
    public delegate void PrevScreenReturnEventHandler();
    public PrevScreenReturnEventHandler PrevScreenReturn;

    public GameObject optionsMenuScreen;    // 옵션 메뉴 화면
    public TextMeshProUGUI optionsText;     // 명칭 텍스트(OPTIONS)
    public TextMeshProUGUI manualText;      // 설명 텍스트
    public List<Menu> optionMenu;           // 옵션 메뉴 리스트
    int _currentMenuIndex;                  // 현재 선택한 메뉴의 인덱스

    void Awake()
    {
        if (manualText == null)
        {
            manualText = GameObject.Find("ManualText").GetComponent<TextMeshProUGUI>();
        }

        // 해당 옵션 스크린을 실행한 곳이 타이틀 화면일 경우
        // 타이틀 화면으로 돌아가는 메뉴와 게임 종료 메뉴 제거
        if (GameManager.instance.currentGameState == GameManager.GameState.Title)
        {
            List<int> menuToRemove = new List<int>();
            for (int i = optionMenu.Count - 1; i >= 0; i--)
            {
                if (optionMenu[i].text[0].name.Equals("ReturnToTitleScreenText") || optionMenu[i].text[0].name.Equals("QuitToDesktopText"))
                {
                    menuToRemove.Add(i);
                    if (menuToRemove.Count >= 2) break;
                }
            }
            for (int i = 0; i < menuToRemove.Count; i++)
            {
                MenuUIController.SelectMenuTextHide(optionMenu[menuToRemove[i]]);
                optionMenu.RemoveAt(menuToRemove[i]);
            }
        }
    }

    private void OnEnable()
    {
        // 새로고침
        OptionTextRefresh();
        MenuUIController.MenuRefresh(optionMenu, ref _currentMenuIndex, manualText);
    }

    void Update()
    {
        // 옵션 메뉴 화면이 비활성화 상태이면 중단
        if (!optionsMenuScreen.activeSelf) return;

        OptionTextRefresh();
        MenuUIController.MenuRefresh(optionMenu, ref _currentMenuIndex, manualText);

        // 입력 받기
        bool upInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Up);
        bool downInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Down);
        bool backInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Cancle);
        bool selectInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Select);

        if (upInput)
        {
            // 위 입력시 인덱스 감소(선택 메뉴를 위로 이동)
            _currentMenuIndex--;
            MenuUIController.MenuRefresh(optionMenu, ref _currentMenuIndex, manualText);
        }
        else if (downInput)
        {
            // 아래 입력시 인덱스 증가(선택 메뉴를 아래로 이동)
            _currentMenuIndex++;
            MenuUIController.MenuRefresh(optionMenu, ref _currentMenuIndex, manualText);
        }
        if (selectInput)
        {
            // 선택 입력시 메뉴 선택 이벤트 실행
            optionMenu[_currentMenuIndex].menuSelectEvent.Invoke();
        }

        if (GameManager.instance.currentGameState == GameManager.GameState.Title)
        {
            if (backInput)
            {
                _currentMenuIndex = 0;
                gameObject.SetActive(false);
                PrevScreenReturn?.Invoke();
                OptionsData.OptionsSave();
                MenuUIController.MenuRefresh(optionMenu, ref _currentMenuIndex, manualText);
            }
        }
    }

    /// <summary>
    /// 타이틀 화면으로 돌아가는 메소드 입니다.
    /// 게임 저장, 데이터 초기화 등을 동시에 실행합니다.
    /// </summary>
    public void ReturnToTitleScreen()
    {
        GameManager.instance.GameSave();
        SceneTransition.instance.LoadScene("Title");
        DeadEnemyManager.ClearDeadBosses();
        DeadEnemyManager.ClearDeadEnemies();
        PlayerLearnedSkills.hasLearnedClimbingWall = false;
        PlayerLearnedSkills.hasLearnedDoubleJump = false;
        TutorialManager.SeenTutorialClear();
        MapManager.ClearDiscoveredMaps();
        GameManager.instance.SetGameState(GameManager.GameState.Title);
    }

    /// <summary>
    /// 게임 플레이로 돌아가는 메소드입니다.
    /// </summary>
    void ReturnToGamePlay()
    {
        _currentMenuIndex = 0;
        GameManager.instance.SetGameState(GameManager.GameState.Play);
        MenuUIController.MenuRefresh(optionMenu, ref _currentMenuIndex, manualText);
        gameObject.SetActive(false);
        OptionsData.OptionsSave();
        PrevScreenReturn?.Invoke();
    }

    /// <summary>
    /// 특정 화면(씬 아님!)으로 넘어가는 메소드입니다.
    /// 특정 옵션 화면을 실행시킬 때 사용합니다.
    /// </summary>
    /// <param name="nextScreen">넘어가려는 화면(옵션 화면)</param>
    public void GoToNextScreen(GameObject nextScreen)
    {
        nextScreen.SetActive(true);
        optionsMenuScreen.SetActive(false);
    }

    /// <summary>
    /// 게임을 종료하는 메소드입니다.
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    /// <summary>
    /// 옵션의 모든 텍스트들을 언어 설정에 맞춰 새로고침하는 메소드입니다.
    /// </summary>
    void OptionTextRefresh()
    {
        optionsText.text = LanguageManager.GetText("Options");
        for (int i = 0; i < optionMenu.Count; i++)
        {
            switch (optionMenu[i].text[0].name)
            {
                case "VideoText":
                    optionMenu[i].text[0].text = LanguageManager.GetText("Video");
                    break;
                case "SoundText":
                    optionMenu[i].text[0].text = LanguageManager.GetText("Sound");
                    break;
                case "ControlsText":
                    optionMenu[i].text[0].text = LanguageManager.GetText("Controls");
                    break;
                case "AccessibilityText":
                    optionMenu[i].text[0].text = LanguageManager.GetText("Accessibility");
                    break;
                case "LanguageText":
                    optionMenu[i].text[0].text = LanguageManager.GetText("Language");
                    break;
                case "ReturnToTitleScreenText":
                    optionMenu[i].text[0].text = LanguageManager.GetText("ReturnToTitleScreen");
                    break;
                case "QuitToDesktopText":
                    optionMenu[i].text[0].text = LanguageManager.GetText("QuitToDesktop");
                    break;
            }
        }
    }
}
