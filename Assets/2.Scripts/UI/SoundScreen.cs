using System.Collections;
using System.Collections.Generic;
using TMPro;
using MenuUI;
using UnityEngine;

/// <summary>
/// ���� �ɼ��� �����ϱ� ���� �޴� ȭ���� ó���ϴ� Ŭ�����Դϴ�.
/// </summary>
public class SoundScreen : MonoBehaviour
{
    public GameObject optionsMenuScreen;    // �ɼ� �޴� ȭ��
    public TextMeshProUGUI soundText;       // �ɼ� ��Ī �ؽ�Ʈ
    public TextMeshProUGUI manualText;      // �޴��� �ؽ�Ʈ
    public Menu[] menu;     // �޴� �迭
    int _currentMenuIndex;  // ���� ������ �޴� �ε���

    bool _rightInput;   // ������ �Է�
    bool _leftInput;    // ���� �Է�
    void OnEnable()
    {
        SoundOptionsRefresh();
        MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
    }

    void Update()
    {
        // �Է� �ޱ�
        bool upInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Up);
        bool downInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Down);
        _rightInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Right);
        _leftInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Left);
        bool backInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Cancle);

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

        if (_rightInput || _leftInput)
        {
            // �����̳� ������ �Է½� �޴� ���� �̺�Ʈ ����(���ټ� �ɼ� ����)
            menu[_currentMenuIndex].menuSelectEvent.Invoke();
        }

        if (backInput)
        {
            // �ڷ� ���� ��ư �Է½� ���ټ� �ɼ��� �����ϰ� �ɼ� �޴��� ���ư�
            _currentMenuIndex = 0;
            MenuUIController.MenuRefresh(menu, ref _currentMenuIndex, manualText);
            ReturnToOptionsMenuScreen();
        }
    }

    /// <summary>
    /// ������ ������ �����ϴ� �޼ҵ��Դϴ�.
    /// </summary>
    public void SetMasterVolume()
    {
        bool increase = _leftInput ? false : true;
        SoundSettingsManager.SetMasterVolume(increase);
        SoundOptionsRefresh();
    }

    /// <summary>
    /// ���� ������ �����ϴ� �޼ҵ��Դϴ�.
    /// </summary>
    public void SetMusicVolume()
    {
        bool increase = _leftInput ? false : true;
        SoundSettingsManager.SetMusicVolume(increase);
        SoundManager.instance.MusicVolumeRefresh();
        SoundOptionsRefresh();
    }

    /// <summary>
    /// ȿ���� ������ �����ϴ� �޼ҵ��Դϴ�.
    /// </summary>
    public void SetEffectsVolume()
    {
        bool increase = _leftInput ? false : true;
        SoundSettingsManager.SetEffectsVolume(increase);
        SoundOptionsRefresh();
    }

    /// <summary>
    /// ���� �ɼ� �޴��� �ؽ�Ʈ���� ������ ��� �����Ϳ��� ������ ���ΰ�ħ�ϴ� �޼ҵ��Դϴ�.
    /// </summary>
    void SoundOptionsRefresh()
    {
        soundText.text = LanguageManager.GetText("Sound");
        for (int i = 0; i < menu.Length; i++)
        {
            switch (menu[i].text[0].name)
            {
                case "MasterVolumeText":
                    menu[i].text[0].text = LanguageManager.GetText("MasterVolume");
                    menu[i].text[1].text = SoundSettingsManager.GetMasterVolumeToTextUI();
                    break;
                case "MusicVolumeText":
                    menu[i].text[0].text = LanguageManager.GetText("MusicVolume");
                    menu[i].text[1].text = SoundSettingsManager.GetMusicVolumeToTextUI();
                    break;
                case "EffectsVolumeText":
                    menu[i].text[0].text = LanguageManager.GetText("EffectsVolume");
                    menu[i].text[1].text = SoundSettingsManager.GetEffectsVolumeToTextUI();
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
