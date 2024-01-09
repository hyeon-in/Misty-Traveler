using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적이 플레이어와 접촉했을 때 플레이어에게 접촉 피해를 주기 위한 클래스입니다.
/// </summary>
public class EnemyBodyTackle : MonoBehaviour
{
    [HideInInspector] public bool isBodyTackled = true; // 접촉 피해를 사용할지 여부
    [HideInInspector] public int damage = 1; // 위력
    public Vector2 size = Vector2.one;  // 접촉 공격의 충돌 판정 크기
    public Transform tacklePos;         // 접촉 공격 충돌 판정의 원점 좌표를 가진 트랜스폼

    KnockBack _knockBack;
    PlayerDamage _playerDamage;

    LayerMask _playerLayer;

    void Awake()
    {
        _playerDamage = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDamage>();
        if(tacklePos == null)
        {
            tacklePos = GetComponent<Transform>();
        }
        
        _playerLayer = LayerMask.GetMask("Player");

        _knockBack = new KnockBack();
        _knockBack.force = 18.0f;
    }

    void Update()
    {
        // 접촉 공격을 사용하지 않거나 크기가 0이면 실행하지 않음
        if(!isBodyTackled || size.x == 0 || size.y == 0)
        {
            return;
        }

        // 플레이어와의 충돌 처리
        Collider2D hit = Physics2D.OverlapBox(tacklePos.position, size, 0, _playerLayer);
        if(hit)
        {
            // 플레이어의 위치에 따라 넉백 방향이 다름
            _knockBack.direction = hit.transform.position.x < tacklePos.position.x ? -1 : 1; 
            _playerDamage.TakeDamage(damage, _knockBack);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(tacklePos.position, size);
    }
}
