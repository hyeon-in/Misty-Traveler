using System.Collections;
using System.Collections.Generic;
using TMPro;
using MenuUI;
using UnityEngine;

/// <summary>
/// ���� �ɼ��� �����ϱ� ���� �޴� ȭ���� ó���ϴ� Ŭ�����Դϴ�.
/// </summary>
public class VideoScreen : MonoBehaviour
{
    public GameObject optionsMenuScreen;    // �ɼ� �޴�
    public TextMeshProUGUI videoText;       // �ɼ� ��Ī �ؽ�Ʈ
    public TextMeshProUGUI manualText;      // �޴��� �ؽ�Ʈ
    public Menu[] menu;     // �޴� �迭
    int _currentMenuIndex;  // ���� ������ �޴��� �ε���

    bool _rightInput, _leftInput, _selectInput; // �Է� ����
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
        // �Է� ����
        bool upInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Up);
        bool downInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Down);
        _rightInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Left);
        _leftInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Right);
        _selectInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Select);
        bool backInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Cancle);

        if (upInput)
        {
            // �� �Է½� �ε��� ����(���� �޴��� ���� �̵�)
            _currentMenuIndex--;
            VideoSettingsManager.ResolutionIndexReturn();
            VideoOptionsRefresh();
            MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
        }
        else if (downInput)
        {
            // �Ʒ� �Է½� �ε��� ����(���� �޴��� �Ʒ��� �̵�)
            _currentMenuIndex++;
            VideoSettingsManager.ResolutionIndexReturn();
            VideoOptionsRefresh();
            MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
        }
        else if (_rightInput || _leftInput)
        {
            // �����̳� ������ �Է½� �޴� ���� �̺�Ʈ ����(���ټ� �ɼ� ����)
            _increase = _leftInput;
            menu[_currentMenuIndex].menuSelectEvent.Invoke();
        }
        else if (_selectInput)
        {
            if (menu[_currentMenuIndex].text[0].name == "ResolutionText")
            {
                // �ػ� �޴����� ���� �Է½� ������ �ػ� �޴� ���� ����
                VideoSettingsManager.NewResolutionAccept();
                MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
            }
        }
        else if (backInput)
        {
            // �ڷ� ���� ��ư �Է½� ���� �ɼ��� �����ϰ� �ɼ� �޴��� ���ư�
            _currentMenuIndex = 0;
            VideoSettingsManager.ResolutionIndexReturn();
            VideoOptionsRefresh();
            MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
            ReturnToOptionsMenuScreen();
        }
    }

    /// <summary>
    /// ��ü ȭ�� ��� ���θ� �����ϴ� �޼ҵ��Դϴ�.
    /// </summary>
    public void SetFullScreen()
    {
        VideoSettingsManager.SetFullScreen();
        VideoOptionsRefresh();
    }

    /// <summary>
    /// �ػ󵵸� �����ϴ� �޼ҵ��Դϴ�.
    /// </summary>
    public void SetResolution()
    {
        VideoSettingsManager.SetResolution(_increase);
        menu[_currentMenuIndex].text[1].color = new Color32(141, 105, 122, 255); // �����Ϸ��� �ϴ� �����̸� �ؽ�Ʈ ���� ����

        VideoOptionsRefresh();
    }

    /// <summary>
    /// VSync ��� ���θ� �����ϴ� �޼ҵ��Դϴ�.
    /// </summary>
    public void SetVSync()
    {
        VideoSettingsManager.SetVSync();
        VideoOptionsRefresh();
    }

    /// <summary>
    /// ���� �ɼ� �޴��� �ؽ�Ʈ���� ������ ��� �����Ϳ��� ������ ���ΰ�ħ�ϴ� �޼ҵ��Դϴ�.
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
    /// ���� �ɼ��� �����ϰ� �ɼ� �޴��� ���ư��� �޼ҵ��Դϴ�.
    /// </summary>
    void ReturnToOptionsMenuScreen()
    {
        this.gameObject.SetActive(false);
        optionsMenuScreen.SetActive(true);
    }
}
