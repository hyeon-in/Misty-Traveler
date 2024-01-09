using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 보스의 체력 및 대미지와 관련된 기능을 저리하기 위한 클래스입니다.
/// </summary>
public class BossEnemyDamage : EnemyDamage
{
    public Image bossHelathUI; // 보스 체력 UI
    public List<float> nextPhaseHealthPercent = new List<float>(); // 페이즈가 전환되는 체력 리스트(퍼센트)

    // 페이즈가 전환될 때 실행되는 델리게이트
    public delegate void PhaseChangedEventHandler();
    public PhaseChangedEventHandler phaseChangedEvent;

    Coroutine damageUIEffect = null;

    protected override void Start()
    {
        base.Start();
        nextPhaseHealthPercent.Sort(); // 체력 퍼센트가 정렬되어있지 않은 상황일 때를 대비해서 정렬
    }

    /// <summary>
    /// 플레이어가 적에게 대미지를 입히고 넉백을 처리하기 위한 메소드입니다.
    /// </summary>
    /// <param name="damage">피해량</param>
    /// <param name="knockBack">넉백 구조체</param>
    public override void TakeDamage(int damage, KnockBack knockBack)
    {
        base.TakeDamage(damage,knockBack);

        // 대미지 UI 이펙트 코루틴 실행
        if (damageUIEffect != null)
        {
            StopCoroutine(damageUIEffect);
            damageUIEffect = null;
        }
        damageUIEffect = StartCoroutine(DamageUIEffect());

        //체력이 다음 페이즈가 실행될 퍼센트 수치 이하면 다음 페이즈로 넘어감
        if (nextPhaseHealthPercent.Count == 0) return;
        else if(GetHealthPercent() <= nextPhaseHealthPercent[0])
        {
            nextPhaseHealthPercent.RemoveAt(0);
            phaseChangedEvent();
        }
    }

    /// <summary>
    /// 보스가 대미지를 입었을 때 UI 효과를 실행하는 코루틴입니다.
    /// </summary>
    IEnumerator DamageUIEffect()
    {
        // 현재 체력 퍼센트를 가져온 후 보스의 체력 표시가 해당 퍼센트에 도달할 때 까지 서서히 감소함
        float percent = GetHealthPercent();
        while(bossHelathUI.fillAmount < percent)
        {
            bossHelathUI.fillAmount -= 0.005f;
            yield return YieldInstructionCache.WaitForSeconds(0.02f);
        }
        bossHelathUI.fillAmount = percent;

        damageUIEffect = null;
    }
}
