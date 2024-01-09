using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// �÷��̾ ��ų�� �н������� �ش� ��ų�� � ��ų���� �����ϴ� UI�� ǥ���ϱ� ���� Ŭ�����Դϴ�.
/// </summary>
public class SkillLearnEffect : MonoBehaviour
{
    public RectTransform window;    // UI â
    public TextMeshProUGUI skillName;   // ��ų ��Ī �ؽ�Ʈ

    /// <summary>
    /// ��ų�� �н��ߴٴ� UI�� ǥ���ϴ� �ڷ�ƾ�� �����ϴ� �޼ҵ��Դϴ�.
    /// </summary>
    /// <param name="skillNameKey">��ų �̸� Ű</param>
    public void SkillLearnEffectStart(string skillNameKey)
    {
        StartCoroutine(SkillLearnEffectCoroutine(skillNameKey));
    }

    IEnumerator SkillLearnEffectCoroutine(string skillNameKey)
    {
        // �ð� ���� �� ���� ���¸� �޴� ���� ���·� ��ȯ
        ScreenEffect.instance.TimeStopStart();
        GameManager.instance.currentGameState = GameManager.GameState.MenuOpen;

        // â�� sizeDelta.y�� 50���� ������ Ŀ��
        while(window.sizeDelta.y < 50)
        {
            float x = window.sizeDelta.x;
            float y = window.sizeDelta.y + 2;
            window.sizeDelta = new Vector2(x, y);
            yield return YieldInstructionCache.WaitForSecondsRealtime(0.015f);
        }

        // ��ų ��Ī ǥ�� �� 2�� ���
        skillName.text = LanguageManager.GetText(skillNameKey);
        yield return YieldInstructionCache.WaitForSecondsRealtime(2.0f);

        // ��ų ��Ī�� ���� �� â�� sizeDelta.y�� 0�� �� ������ ������ �۾���
        skillName.text = "";
        while (window.sizeDelta.y > 0)
        {
            float x = window.sizeDelta.x;
            float y = window.sizeDelta.y - 2;
            window.sizeDelta = new Vector2(x, y);
            yield return YieldInstructionCache.WaitForSecondsRealtime(0.015f);
        }
        // ���� �÷��̸� �簳�ϰ� �ð� ���� ���
        GameManager.instance.currentGameState = GameManager.GameState.Play;
        ScreenEffect.instance.TimeStopCancle();
    }
}
