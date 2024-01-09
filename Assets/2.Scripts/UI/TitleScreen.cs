using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MenuUI;
using TMPro;

/// <summary>
/// Ÿ��Ʋ ȭ��� ���õ� ��ɵ��� ó���ϴ� Ŭ�����Դϴ�.
/// </summary>
public class TitleScreen : MonoBehaviour
{
    public TextMeshProUGUI title;           // Ÿ��Ʋ ����
    public TextMeshProUGUI manualText;      // �޴��� �ؽ�Ʈ
    public TextMeshProUGUI copyrightText;   // ���۱� �ؽ�Ʈ
    public TextMeshProUGUI versionText;   // ���� ǥ�� �ؽ�Ʈ
    public GameObject optionsScreen;        // �ɼ� ȭ��
    public GameObject selectProfileScreen;  // ������ ���� ȭ��
    public Menu[] menu; // �޴� �迭

    bool _isOtherScreenOpening; // �ٸ� ȭ���� �����ִ� �������� ��Ÿ��
    int _currentMenuIndex = 0;  // ���� ������ �޴��� �ε���
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
        // �ٸ� ȭ���� ���� ���̸� �ߴ�
        if (_isOtherScreenOpening) return;

        TitleTextRefresh();
        MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);

        // �Է� �ޱ�
        bool upInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Up);
        bool downInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Down);
        bool selectInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Select);
        if (upInput)
        {
            // �� �Է½� �ε��� ����(���� �޴��� ���� �̵�)
            _currentMenuIndex--;
            MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
        }
        else if (downInput)
        {
            // �Ʒ� �Է½� �ε��� ����(���� �޴��� �Ʒ��� �̵�)
            _currentMenuIndex++;
            MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
        }
        if (selectInput)
        {
            // ���� �Է½� �޴� ���� �̺�Ʈ ����
            menu[_currentMenuIndex].menuSelectEvent.Invoke();
        }
    }

    /// <summary>
    /// �ٸ� ȭ���� ���� ���� �޼ҵ��Դϴ�.
    /// </summary>
    /// <param name="openScreen">������ �ϴ� ȭ��(���� ������Ʈ)</param>
    public void OpenOtherScreen(GameObject openScreen)
    {
        _isOtherScreenOpening = true;
        title.alpha = 0;
        versionText.alpha = copyrightText.alpha = 0;
        MenuUIController.AllMenuTextHide(menu);
        openScreen.SetActive(true);
    }

    /// <summary>
    /// �ɼ� �޴� ȭ��� ������ ���� ȭ�鿡�� �ٽ� Ÿ��Ʋ ȭ������ ���ƿ����� ȣ���ϴ� �޼ҵ��Դϴ�.
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
    /// ���� ���� �޼ҵ��Դϴ�.
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
    /// Ÿ��Ʋ ȭ���� �ؽ�Ʈ���� ������ ��� �����Ϳ��� ������ ���ΰ�ħ�ϴ� �޼ҵ��Դϴ�.
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
