using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 플레이어가 스킬을 학습했을때 해당 스킬이 어떤 스킬인지 설명하는 UI를 표시하기 위한 클래스입니다.
/// </summary>
public class SkillLearnEffect : MonoBehaviour
{
    public RectTransform window;    // UI 창
    public TextMeshProUGUI skillName;   // 스킬 명칭 텍스트

    /// <summary>
    /// 스킬을 학습했다는 UI를 표시하는 코루틴을 실행하는 메소드입니다.
    /// </summary>
    /// <param name="skillNameKey">스킬 이름 키</param>
    public void SkillLearnEffectStart(string skillNameKey)
    {
        StartCoroutine(SkillLearnEffectCoroutine(skillNameKey));
    }

    IEnumerator SkillLearnEffectCoroutine(string skillNameKey)
    {
        // 시간 정지 및 게임 상태를 메뉴 오픈 상태로 전환
        ScreenEffect.instance.TimeStopStart();
        GameManager.instance.currentGameState = GameManager.GameState.MenuOpen;

        // 창의 sizeDelta.y가 50까지 서서히 커짐
        while(window.sizeDelta.y < 50)
        {
            float x = window.sizeDelta.x;
            float y = window.sizeDelta.y + 2;
            window.sizeDelta = new Vector2(x, y);
            yield return YieldInstructionCache.WaitForSecondsRealtime(0.015f);
        }

        // 스킬 명칭 표시 후 2초 대기
        skillName.text = LanguageManager.GetText(skillNameKey);
        yield return YieldInstructionCache.WaitForSecondsRealtime(2.0f);

        // 스킬 명칭을 숨긴 뒤 창의 sizeDelta.y가 0이 될 때까지 서서히 작아짐
        skillName.text = "";
        while (window.sizeDelta.y > 0)
        {
            float x = window.sizeDelta.x;
            float y = window.sizeDelta.y - 2;
            window.sizeDelta = new Vector2(x, y);
            yield return YieldInstructionCache.WaitForSecondsRealtime(0.015f);
        }
        // 게임 플레이를 재개하고 시간 정지 취소
        GameManager.instance.currentGameState = GameManager.GameState.Play;
        ScreenEffect.instance.TimeStopCancle();
    }
}
