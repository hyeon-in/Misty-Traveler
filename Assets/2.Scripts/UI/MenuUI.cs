using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Text;

namespace MenuUI
{
    /// <summary>
    /// 메뉴에서 특정 메뉴 제어에 대해 어떤 입력을 해야 하는지 설명할 때 사용할 메뉴얼 데이터를 담은 클래스입니다.
    /// </summary>
    [System.Serializable]
    public class Manual
    {
        public string key;                                  // 하려고 하는 메뉴 제어의 key(언어 데이터를 가져올 때 사용)
        public GameInputManager.MenuControl menuControl;    // 해당 메뉴 제어에 대응하는 메뉴 컨트롤 키 혹은 버튼
    }

    /// <summary>
    /// 메뉴 UI에서 각각의 메뉴 텍스트, 특정 메뉴에 대해 어떤 입력을 해야 하는지에 대한 설명, ,
    /// </summary>
    [System.Serializable]
    public class Menu
    {
        public TextMeshProUGUI[] text;      // 해당 메뉴에 포함된 텍스트 배열
        public Manual[] manual;             // 메뉴얼 배열
        public UnityEvent menuSelectEvent;  // 메뉴를 선택했을때 호출하는 이벤트
    }

    /// <summary>
    /// 메뉴 UI에서 사용하려는 기능들을 모아놓은 클래스입니다.
    /// </summary>
    public static class MenuUIController
    {
        public static readonly Color selectColor = new Color32(255, 236, 214, 255);     // 선택된 메뉴의 색상
        public static readonly Color notSelectColor = new Color32(255, 170, 94, 255);   // 선택되지 않은 메뉴의 색상

        /// <summary>
        /// 메뉴 UI의 텍스트를 새로고침하는 메소드입니다.
        /// 현재 선택한 메뉴를 표시할 때 사용합니다.
        /// </summary>
        /// <param name="menu">메뉴 배열</param>
        /// <param name="currentMenuIndex">현재 메뉴 인덱스</param>
        /// <param name="manualText">메뉴얼 텍스트</param>
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

            // 모든 메뉴의 텍스트 컬러를 초기화 한 뒤 선택한 메뉴의 텍스트 컬러를 SelectColor로 변경
            AllMenuTextColorInit(menu);
            SelectMenuTextColorChange(menu[currentMenuIndex]);

            // 해당 메뉴의 메뉴얼 표시
            if (manualText != null)
            {
                SetMenualText(menu[currentMenuIndex], manualText);
            }
        }

        /// <summary>
        /// 메뉴 UI의 텍스트를 새로고침하는 메소드입니다.
        /// 현재 선택한 메뉴를 표시할 때 사용합니다.
        /// </summary>
        /// <param name="menu">메뉴 리스트</param>
        /// <param name="currentMenuIndex">현재 메뉴 인덱스</param>
        /// <param name="manualText">메뉴얼 텍스트</param>
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

            // 모든 메뉴의 텍스트 컬러를 초기화 한 뒤 선택한 메뉴의 텍스트 컬러를 SelectColor로 변경
            AllMenuTextColorInit(menu);
            SelectMenuTextColorChange(menu[currentMenuIndex]);

            // 해당 메뉴의 메뉴얼 표시
            if (manualText != null)
            {
                SetMenualText(menu[currentMenuIndex], manualText);
            }
        }

        /// <summary>
        /// 모든 메뉴의 텍스트의 컬러를 초기화하는 메소드입니다. (nonSelectColor로 설정)
        /// </summary>
        /// <param name="menu">메뉴 배열</param>
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
        /// 모든 메뉴의 텍스트의 컬러를 초기화하는 메소드입니다. (nonSelectColor로 설정)
        /// </summary>
        /// <param name="menu">메뉴 리스트</param>
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
        /// 모든 메뉴의 텍스트를 숨기는 메소드입니다.
        /// </summary>
        /// <param name="menu">메뉴 배열</param>
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
        /// 모든 메뉴의 텍스트를 숨기는 메소드입니다.
        /// </summary>
        /// <param name="menu">메뉴 리스트</param>
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
        /// 선택한 메뉴의 텍스트를 selectColor로 변경하는 메소드입니다.
        /// </summary>
        /// <param name="menu">선택한 메뉴</param>
        public static void SelectMenuTextColorChange(Menu menu)
        {
            for (int j = 0; j < menu.text.Length; j++)
            {
                menu.text[j].color = selectColor;
            }
        }

        /// <summary>
        /// 선택한 메뉴의 텍스트를 숨기는 메소드입니다.
        /// </summary>
        /// <param name="menu">선택한 메뉴</param>
        public static void SelectMenuTextHide(Menu menu)
        {
            for (int j = 0; j < menu.text.Length; j++)
            {
                menu.text[j].color = new Color(0, 0, 0, 0);
            }
        }

        /// <summary>
        /// 특정 메뉴의 메뉴얼을 표시하기 위한 메소드입니다.
        /// </summary>
        /// <param name="menu">선택한 메뉴</param>
        /// <param name="manualText">메뉴얼을 표시하려는 텍스트</param>
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