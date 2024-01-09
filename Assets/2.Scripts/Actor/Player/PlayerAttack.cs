using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 공격을 처리하는 메소드입니다.
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    public Transform attackPoint;   // 공격 원점 좌표로 사용하는 트랜스폼 오브젝트
    public Transform effectPoint;   // 이펙트를 생성할 때 사용하는 원점 좌표
    public Vector2 attackRange = Vector2.one;   // 공격 범위

    public int HitCount { get; private set; }   // 공격이 적중한 적의 수를 표시하는 프로퍼티

    LayerMask _enemyLayer;  // 적 레이어
    LayerMask _groundLayer; // 그라운드 레이어
    void Awake()
    {
        _enemyLayer = LayerMask.GetMask("Enemy");
        _groundLayer = LayerMask.GetMask("Ground");
    }

    /// <summary>
    /// 플레이어의 공격이 적에게 적중했을 경우 적에게 피해를 주고 공격 적중 여부를 반환하는 메소드입니다.
    /// </summary>
    /// <param name="damage">공격 위력</param>
    /// <param name="knockBack">공격 넉백량</param>
    /// <param name="effect">이펙트 생성 여부</param>
    /// <returns>공격 적중 여부</returns>
    public bool IsAttacking(int damage, KnockBack knockBack, bool effect = true)
    {
        HitCount = 0;   // 공격이 적중한 적의 수 초기화
        if(attackRange == Vector2.zero) return false;   // 공격 범위가 0이면 false 반환

        bool isHit = false; // 공격 적중 여부

        var hits = Physics2D.OverlapBoxAll(attackPoint.position, attackRange, 0, _enemyLayer);
        if(hits.Length > 0)
        {
            // 공격 범위안에 적이 들어왔을 경우
            for(int i = 0; i < hits.Length; i++)
            {
                EnemyDamage enemyDamage = hits[i].GetComponent<EnemyDamage>();
                if(enemyDamage.IsDead || enemyDamage.IsInvincibled) continue;   // 적이 이미 죽었거나 무적 상태이면 다음으로 넘어감

                hits[i].GetComponent<EnemyDamage>().TakeDamage(damage, knockBack);  // 대미지 처리

                // 불릿 타임과 화면 흔들림 실행
                ScreenEffect.instance.BulletTimeStart(0.0f, 0.05f);
                ScreenEffect.instance.ShakeEffectStart(0.2f, 0.05f);
                
                if(effect)
                {
                    // 이펙트를 사용하고 있을 경우 공격 적중 이펙트 생성
                    ObjectPoolManager.instance.GetPoolObject("SlashEffect", effectPoint.position, angle: Random.Range(0f,359f));
                }

                isHit = true; // 공격 적중 여부를 true로 설정
                HitCount++; // 공격이 적중한 적 수 증가
            }
        }

        return isHit;   // 공격 적중 여부 반환
    }

    /// <summary>
    /// 공격이 Ground에 닿았는지 여부를 반환하는 메소드입니다.
    /// </summary>
    public bool GroundHit()
    {
        if(attackRange == Vector2.zero) return false;   // 공격 범위가 0이면 false 반환
        return Physics2D.OverlapBox(attackPoint.position, attackRange, 0, _groundLayer);    // 충돌 체크
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, attackRange);
    }
}