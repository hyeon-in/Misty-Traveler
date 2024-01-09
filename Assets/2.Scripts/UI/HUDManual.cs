using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// � ��ư�� �Է��ؾ� �ϴ��� ȭ�鿡 ǥ���ϱ� ���� UI Ŭ�����Դϴ�.
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
    /// �޴����� ǥ���ϴ� �޼ҵ��Դϴ�.
    /// </summary>
    /// <param name="key">Ư�� �ൿ�� �̸��� ���� Key</param>
    /// <param name="action">�÷��̾ �Ϸ��� �ൿ</param>
    /// <param name="targetPos">�޴����� ǥ�� �� Ÿ���� ��ǥ</param>
    public void DisplayManual(string key, GameInputManager.PlayerActions action, Vector3 targetPos)
    {
        // �޴����� �̹� �������� ���� ��� ���� �ߴ�
        if(showManualCoroutine != null) return;

        // �޴��� ���� �ڷ�ƾ ����
        if(closeManualCoroutine != null)
        {
            StopCoroutine(closeManualCoroutine);
            closeManualCoroutine = null;
        }

        // �޴����� �ؽ�Ʈ�� �޾ƿ� key�� action�� ���� ����
        string keyToText = LanguageManager.GetText(key);
        string input = !GameInputManager.usingController ? GameInputManager.ActionToKeyText(action) : GameInputManager.ActionToButtonText(action);
        _manual.text = keyToText + " [ <color=#ffaa5e>" + input + "</color> ]";

        // �޴����� ��ġ�� ��ǥ ����
        targetPos.y += 4.0f;
        Vector3 screenPos = targetPos;
        screenPos.z = _transform.position.z;
        _transform.position = screenPos;

        // �޴����� õõ�� �����ֱ� ���� �ڷ�ƾ ����
        showManualCoroutine = StartCoroutine(ManualFadeIn());
    }

    /// <summary>
    /// �޴����� õõ�� ȭ�鿡 ��Ÿ���� �ڷ�ƾ�Դϴ�.
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
    /// �޴����� ����� �޼ҵ��Դϴ�.
    /// </summary>
    public void HideManual()
    {
        // �޴����� �̹� ����ǰ� ���� ��� ���� �ߴ�
        if (closeManualCoroutine != null) return;

        // �޴��� ���� �ڷ�ƾ ����
        if (showManualCoroutine != null)
        {
            StopCoroutine(showManualCoroutine);
            showManualCoroutine = null;
        }

        closeManualCoroutine = StartCoroutine(ManualFadeOut());
    }

    /// <summary>
    /// �޴����� õõ�� ȭ�鿡�� ������� �ڷ�ƾ�Դϴ�.
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
