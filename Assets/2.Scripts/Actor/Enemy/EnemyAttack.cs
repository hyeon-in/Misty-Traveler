using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적의 공격을 처리하기 위한 클래스입니다.
/// </summary>
public class EnemyAttack : MonoBehaviour
{
    public Transform attackPoint;   // 공격 좌표 트랜스폼
    public Vector2 attackRange;     // 공격 범위(사각형)

    LayerMask _playerLayer;

    PlayerDamage _playerDamage;

    void Awake()
    {
        _playerLayer = LayerMask.GetMask("Player");
        _playerDamage = GameObject.Find("Player").GetComponent<PlayerDamage>();
    }

    /// <summary>
    /// 플레이어에 대한 적의 공격을 처리하고 공격이 적중했는지 체크하기 위한 메소드입니다.
    /// </summary>
    /// <param name="damage">피해량</param>
    /// <param name="knockBack">넉백 구조체</param>
    /// <returns>공격 적중 여부</returns>
    public bool IsAttacking(int damage, KnockBack knockBack)
    {
        // 공격 범위가 0이면 false 반환
        if(attackRange == Vector2.zero) return false;

        // 적의 공격이 플레이어에게 닿으면 대미지 처리를 실행합니다.
        if(Physics2D.OverlapBox(attackPoint.position, attackRange, 0, _playerLayer))
        {
            _playerDamage.TakeDamage(damage, knockBack);
            return true;
        }

        // 적중하지 않았으면 false 반환
        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, attackRange);
    }
}