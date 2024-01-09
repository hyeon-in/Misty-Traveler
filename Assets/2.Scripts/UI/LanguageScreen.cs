using System.Collections;
using System.Collections.Generic;
using TMPro;
using MenuUI;
using UnityEngine;

/// <summary>
/// ��� �ɼ��� �����ϱ� ���� �޴� ȭ���� ó���ϴ� Ŭ�����Դϴ�.
/// </summary>
public class LanguageScreen : MonoBehaviour
{
    public GameObject optionsMenuScreen;    // �ɼ� �޴� ȭ��
    public TextMeshProUGUI languageText;    // �ɼ� ��Ī �ؽ�Ʈ
    public TextMeshProUGUI manualText;      // �޴��� �ؽ�Ʈ
    public Menu[] menu; // �޴� �迭

    bool _rightInput, _leftInput;   // ����, ������ �Է� ����

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
        // �Է� �ޱ�
        _rightInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Right);
        _leftInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Left);
        bool backInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Cancle);

        if (_rightInput || _leftInput)
        {
            // �����̳� ������ �Է½� �޴� ���� �̺�Ʈ ����(���ټ� �ɼ� ����)
            menu[0].menuSelectEvent.Invoke();
            MenuUIController.SetMenualText(menu[0], manualText);
        }

        if (backInput)
        {
            // �ڷ� ���� ��ư �Է½� ��� �ɼ��� �����ϰ� �ɼ� �޴��� ���ư�
            ReturnToOptionsMenuScreen();
        }
    }

    /// <summary>
    /// �� �����ϴ� �޼ҵ��Դϴ�.
    /// </summary>
    public void SetLanguage()
    {
        bool right = _rightInput ? false : true;
        LanguageManager.SetLanguage(right);
        LanguageOptionsRefresh();
    }

    /// <summary>
    /// ��� �ɼ� �޴��� �ؽ�Ʈ���� ������ �������� ������ ���ΰ�ħ�ϴ� �޼ҵ��Դϴ�.
    /// </summary>
    void LanguageOptionsRefresh()
    {
        languageText.text = LanguageManager.GetText("Language");
        menu[0].text[0].text = LanguageManager.GetCurrentLanguageToText();
    }

    /// <summary>
    /// ��� �ɼ��� �����ϰ� �ɼ� �޴��� ���ư��� �޼ҵ��Դϴ�.
    /// </summary>
    void ReturnToOptionsMenuScreen()
    {
        this.gameObject.SetActive(false);
        optionsMenuScreen.SetActive(true);
    }
}