using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Text;

namespace MenuUI
{
    /// <summary>
    /// �޴����� Ư�� �޴� ��� ���� � �Է��� �ؾ� �ϴ��� ������ �� ����� �޴��� �����͸� ���� Ŭ�����Դϴ�.
    /// </summary>
    [System.Serializable]
    public class Manual
    {
        public string key;                                  // �Ϸ��� �ϴ� �޴� ������ key(��� �����͸� ������ �� ���)
        public GameInputManager.MenuControl menuControl;    // �ش� �޴� ��� �����ϴ� �޴� ��Ʈ�� Ű Ȥ�� ��ư
    }

    /// <summary>
    /// �޴� UI���� ������ �޴� �ؽ�Ʈ, Ư�� �޴��� ���� � �Է��� �ؾ� �ϴ����� ���� ����, ,
    /// </summary>
    [System.Serializable]
    public class Menu
    {
        public TextMeshProUGUI[] text;      // �ش� �޴��� ���Ե� �ؽ�Ʈ �迭
        public Manual[] manual;             // �޴��� �迭
        public UnityEvent menuSelectEvent;  // �޴��� ���������� ȣ���ϴ� �̺�Ʈ
    }

    /// <summary>
    /// �޴� UI���� ����Ϸ��� ��ɵ��� ��Ƴ��� Ŭ�����Դϴ�.
    /// </summary>
    public static class MenuUIController
    {
        public static readonly Color selectColor = new Color32(255, 236, 214, 255);     // ���õ� �޴��� ����
        public static readonly Color notSelectColor = new Color32(255, 170, 94, 255);   // ���õ��� ���� �޴��� ����

        /// <summary>
        /// �޴� UI�� �ؽ�Ʈ�� ���ΰ�ħ�ϴ� �޼ҵ��Դϴ�.
        /// ���� ������ �޴��� ǥ���� �� ����մϴ�.
        /// </summary>
        /// <param name="menu">�޴� �迭</param>
        /// <param name="currentMenuIndex">���� �޴� �ε���</param>
        /// <param name="manualText">�޴��� �ؽ�Ʈ</param>
        public static void MenuRefresh(Menu[] menu, ref int currentMenuIndex, TextMeshProUGUI manualText = null)
        {
            if (currentMenuIndex >= menu.Length)
            {
                currentMenuIndex = 0;
            }
            else if (currentMenuIndex < 0)
            {
                currentMenuIndex = menu.Length - 1;
            }

            // ��� �޴��� �ؽ�Ʈ �÷��� �ʱ�ȭ �� �� ������ �޴��� �ؽ�Ʈ �÷��� SelectColor�� ����
            AllMenuTextColorInit(menu);
            SelectMenuTextColorChange(menu[currentMenuIndex]);

            // �ش� �޴��� �޴��� ǥ��
            if (manualText != null)
            {
                SetMenualText(menu[currentMenuIndex], manualText);
            }
        }

        /// <summary>
        /// �޴� UI�� �ؽ�Ʈ�� ���ΰ�ħ�ϴ� �޼ҵ��Դϴ�.
        /// ���� ������ �޴��� ǥ���� �� ����մϴ�.
        /// </summary>
        /// <param name="menu">�޴� ����Ʈ</param>
        /// <param name="currentMenuIndex">���� �޴� �ε���</param>
        /// <param name="manualText">�޴��� �ؽ�Ʈ</param>
        public static void MenuRefresh(List<Menu> menu, ref int currentMenuIndex, TextMeshProUGUI manualText = null)
        {
            if (currentMenuIndex >= menu.Count)
            {
                currentMenuIndex = 0;
            }
            else if (currentMenuIndex < 0)
            {
                currentMenuIndex = menu.Count - 1;
            }

            // ��� �޴��� �ؽ�Ʈ �÷��� �ʱ�ȭ �� �� ������ �޴��� �ؽ�Ʈ �÷��� SelectColor�� ����
            AllMenuTextColorInit(menu);
            SelectMenuTextColorChange(menu[currentMenuIndex]);

            // �ش� �޴��� �޴��� ǥ��
            if (manualText != null)
            {
                SetMenualText(menu[currentMenuIndex], manualText);
            }
        }

        /// <summary>
        /// ��� �޴��� �ؽ�Ʈ�� �÷��� �ʱ�ȭ�ϴ� �޼ҵ��Դϴ�. (nonSelectColor�� ����)
        /// </summary>
        /// <param name="menu">�޴� �迭</param>
        public static void AllMenuTextColorInit(Menu[] menu)
        {
            for (int i = 0; i < menu.Length; i++)
            {
                for(int j = 0; j < menu[i].text.Length; j++)
                {
                    menu[i].text[j].color = notSelectColor;
                }
            }
        }

        /// <summary>
        /// ��� �޴��� �ؽ�Ʈ�� �÷��� �ʱ�ȭ�ϴ� �޼ҵ��Դϴ�. (nonSelectColor�� ����)
        /// </summary>
        /// <param name="menu">�޴� ����Ʈ</param>
        public static void AllMenuTextColorInit(List<Menu> menu)
        {
            for (int i = 0; i < menu.Count; i++)
            {
                for (int j = 0; j < menu[i].text.Length; j++)
                {
                    menu[i].text[j].color = notSelectColor;
                }
            }
        }

        /// <summary>
        /// ��� �޴��� �ؽ�Ʈ�� ����� �޼ҵ��Դϴ�.
        /// </summary>
        /// <param name="menu">�޴� �迭</param>
        public static void AllMenuTextHide(Menu[] menu)
        {
            for (int i = 0; i < menu.Length; i++)
            {
                for (int j = 0; j < menu[i].text.Length; j++)
                {
                    menu[i].text[j].color = new Color32(0, 0, 0, 0);
                }
            }
        }

        /// <summary>
        /// ��� �޴��� �ؽ�Ʈ�� ����� �޼ҵ��Դϴ�.
        /// </summary>
        /// <param name="menu">�޴� ����Ʈ</param>
        public static void AllMenuTextHide(List<Menu> menu)
        {
            for (int i = 0; i < menu.Count; i++)
            {
                for (int j = 0; j < menu[i].text.Length; j++)
                {
                    menu[i].text[j].color = new Color32(0, 0, 0, 0);
                }
            }
        }
        
        /// <summary>
        /// ������ �޴��� �ؽ�Ʈ�� selectColor�� �����ϴ� �޼ҵ��Դϴ�.
        /// </summary>
        /// <param name="menu">������ �޴�</param>
        public static void SelectMenuTextColorChange(Menu menu)
        {
            for (int j = 0; j < menu.text.Length; j++)
            {
                menu.text[j].color = selectColor;
            }
        }

        /// <summary>
        /// ������ �޴��� �ؽ�Ʈ�� ����� �޼ҵ��Դϴ�.
        /// </summary>
        /// <param name="menu">������ �޴�</param>
        public static void SelectMenuTextHide(Menu menu)
        {
            for (int j = 0; j < menu.text.Length; j++)
            {
                menu.text[j].color = new Color(0, 0, 0, 0);
            }
        }

        /// <summary>
        /// Ư�� �޴��� �޴����� ǥ���ϱ� ���� �޼ҵ��Դϴ�.
        /// </summary>
        /// <param name="menu">������ �޴�</param>
        /// <param name="manualText">�޴����� ǥ���Ϸ��� �ؽ�Ʈ</param>
        public static void SetMenualText(Menu menu, TextMeshProUGUI manualText)
        {
            // 
            StringBuilder manual = new StringBuilder();
            for (int i = 0; i < menu.manual.Length; i++)
            {
                string key = LanguageManager.GetText(menu.manual[i].key);
                string input = GameInputManager.usingController ? GameInputManager.MenuControlToButtonText(menu.manual[i].menuControl)
                                                                : GameInputManager.MenuControlToKeyText(menu.manual[i].menuControl);
                manual.AppendFormat("{0} [ <color=#ffaa5e>{1}</color> ]", key, input);
                if (i < menu.manual.Length - 1)
                {
                    manual.Append(" ");
                }
            }
            manualText.text = manual.ToString();
        }
    }
}