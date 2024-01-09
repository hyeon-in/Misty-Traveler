using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 어떤 버튼을 입력해야 하는지 화면에 표시하기 위한 UI 클래스입니다.
/// </summary>
public class HUDManual : MonoBehaviour
{
    Transform _transform;
    TextMeshProUGUI _manual;
    readonly Color32 textColor = new Color32(3,6,26,255);
    
    Coroutine showManualCoroutine = null;
    Coroutine closeManualCoroutine = null;

    void Awake()
    {
        _manual = GetComponent<TextMeshProUGUI>();
        _transform = GetComponent<Transform>();

        byte r = textColor.r;
        byte g = textColor.g;
        byte b = textColor.b;

        _manual.color = new Color32(r,g,b,0);
    }

    /// <summary>
    /// 메뉴얼을 표시하는 메소드입니다.
    /// </summary>
    /// <param name="key">특정 행동의 이름을 담은 Key</param>
    /// <param name="action">플레이어가 하려는 행동</param>
    /// <param name="targetPos">메뉴얼이 표시 될 타겟의 좌표</param>
    public void DisplayManual(string key, GameInputManager.PlayerActions action, Vector3 targetPos)
    {
        // 메뉴얼이 이미 보여지고 있을 경우 실행 중단
        if(showManualCoroutine != null) return;

        // 메뉴얼 종료 코루틴 종료
        if(closeManualCoroutine != null)
        {
            StopCoroutine(closeManualCoroutine);
            closeManualCoroutine = null;
        }

        // 메뉴얼의 텍스트를 받아온 key와 action에 맞춰 설정
        string keyToText = LanguageManager.GetText(key);
        string input = !GameInputManager.usingController ? GameInputManager.ActionToKeyText(action) : GameInputManager.ActionToButtonText(action);
        _manual.text = keyToText + " [ <color=#ffaa5e>" + input + "</color> ]";

        // 메뉴얼이 위치할 좌표 설정
        targetPos.y += 4.0f;
        Vector3 screenPos = targetPos;
        screenPos.z = _transform.position.z;
        _transform.position = screenPos;

        // 메뉴얼을 천천히 보여주기 위한 코루틴 실행
        showManualCoroutine = StartCoroutine(ManualFadeIn());
    }

    /// <summary>
    /// 메뉴얼이 천천히 화면에 나타나는 코루틴입니다.
    /// </summary>
    IEnumerator ManualFadeIn()
    {
        byte r = textColor.r;
        byte g = textColor.g;
        byte b = textColor.b;
        byte a = Convert.ToByte(_manual.color.a * 255f);

        while(a < 255)
        {
            if(a + 20 < 255)
            {
                a += 20;
            }
            else a = 255;

            _manual.color = new Color32(r,g,b,a);

            yield return YieldInstructionCache.WaitForSeconds(0.02f);
        }

        showManualCoroutine = null;
    }

    /// <summary>
    /// 메뉴얼을 숨기는 메소드입니다.
    /// </summary>
    public void HideManual()
    {
        // 메뉴얼이 이미 종료되고 있을 경우 실행 중단
        if (closeManualCoroutine != null) return;

        // 메뉴얼 실행 코루틴 종료
        if (showManualCoroutine != null)
        {
            StopCoroutine(showManualCoroutine);
            showManualCoroutine = null;
        }

        closeManualCoroutine = StartCoroutine(ManualFadeOut());
    }

    /// <summary>
    /// 메뉴얼이 천천히 화면에서 사라지는 코루틴입니다.
    /// </summary>
    /// <returns></returns>
    IEnumerator ManualFadeOut()
    {
        byte r = textColor.r;
        byte g = textColor.g;
        byte b = textColor.b;
        byte a = Convert.ToByte(_manual.color.a * 255f);

        while (a > 0)
        {
            if (a - 20 > 0)
            {
                a -= 20;
            }
            else a = 0;

            _manual.color = new Color32(r, g, b, a);

            yield return YieldInstructionCache.WaitForSeconds(0.02f);
        }

        closeManualCoroutine = null;
    }
}
