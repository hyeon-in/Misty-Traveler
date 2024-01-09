using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적의 체력과, 대미지 처리, 대미지를 입었을 때 이펙트를 처리하는 클래스입니다.
/// </summary>
public class EnemyDamage : MonoBehaviour, IActorDamage
{
    // 적 넉백 이벤트 핸들러
    public IActorDamage.KnockbackEventHandler KnockBack = null;
    // 적 사망 이벤트 핸들러
    public IActorDamage.DiedEventHandler Died;

    SpriteRenderer _spriteRenderer; // 스프라이트 렌더러
    Material _defalutMaterial;      // 기본 머테리얼

    public int CurrentHealth { get; set; }      // 현재 체력
    public int MaxHealth { get; set; }          // 최대 체력
    public bool SuperArmor { get; set; }        // 슈퍼 아머
    public bool IsDead { get; protected set; }   // 사망한 상태인지 체크
    public bool IsInvincibled { get; set; }     // 무적 상태인지 체크
    public Material BlinkMaterial { get; set; } // 블링크 이펙트 머테리얼

    /// <summary>
    /// 현재 체력을 퍼센트로 가져온다.
    /// </summary>
    public float GetHealthPercent() =>  CurrentHealth / (float)MaxHealth;

    protected virtual void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _defalutMaterial = _spriteRenderer.material;

        // 넉백 이벤트가 null이면 자동으로 슈퍼아머로 처리
        if (KnockBack == null)
        {
            SuperArmor = true;
        }

        CurrentHealth = MaxHealth;
    }

    /// <summary>
    /// 플레이어가 적에게 대미지를 입히고 넉백을 처리하기 위한 메소드입니다.
    /// </summary>
    /// <param name="damage">피해량</param>
    /// <param name="knockBack">넉백 구조체</param>
    public virtual void TakeDamage(int damage, KnockBack knockBack)
    {
        // 이미 사망한 상태이면 실행하지 않음
        if(IsDead) return;

        // 슈퍼 아머가 아닐 경우 넉백 처리
        if(!SuperArmor)
        {
            KnockBack(knockBack);
        }

        // 체력을 감소한 후, 체력이 0 이하이면 사망 처리
        CurrentHealth -= damage;
        if(CurrentHealth <= 0)
        {
            StartCoroutine(Died());
            IsDead = true;
            return;
        }

        // 스프라이트가 깜빡이는 이펙트 처리
        StartCoroutine(BlinkEffect());
    }

    /// <summary>
    /// 대미지를 입었을 때 스프라이트가 깜빡거리는 이펙트를 처리하기 위한 코루틴입니다.
    /// </summary>
    IEnumerator BlinkEffect()
    {
        _spriteRenderer.material = BlinkMaterial;

        yield return YieldInstructionCache.WaitForSeconds(0.1f);

        _spriteRenderer.material = _defalutMaterial;
    }
}
