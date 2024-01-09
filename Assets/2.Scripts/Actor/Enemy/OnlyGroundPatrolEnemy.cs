using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오로지 땅 위를 순찰하기만 하는 적을 처리하는 클래스입니다.
/// </summary>
public class OnlyGroundPatrolEnemy : Enemy
{
    [SerializeField] Transform _frontCliffChecker;
    [SerializeField] Transform _backCliffChecker;

    protected override void Awake()
    {
        base.Awake();

        if (_frontCliffChecker == null)
        {
            _frontCliffChecker = transform.Find("FrontCliffChecker").GetComponent<Transform>();
        }
        if (_backCliffChecker == null)
        {
            _backCliffChecker = transform.Find("BackCliffChecker").GetComponent<Transform>();
        }
    }

    void Update()
    {
        if (isDead) return;

        // 미끄러지는 상태(넉백 상태)가 아닐 경우 순찰을 함
        if (!controller.IsSliding)
        {
            // 현재 속도를 순찰 속도로 설정
            controller.VelocityX = enemyData.patrolSpeed * actorTransform.localScale.x;
            
            // 벽과 부딪치거나 절벽과 마주쳤을 경우 플립
            bool isWalled = actorTransform.localScale.x == 1 ? controller.IsRightWalled : controller.IsLeftWalled;
            if (FrontCliffChecked() || isWalled)
            {
                Flip();
            }
        }
        else
        {
            // 미끄러지는 상태(공격 받아서 넉백중인 상태)일 경우를 처리
            // 절벽과 마주치면 속도를 0으로 맞추고 미끄러짐을 중단함
            if (FrontCliffChecked() || BackCliffChecked())
            {
                controller.VelocityX = 0;
                controller.SlideCancle();
            }
        }
    }
    
    /// <summary>
    /// 앞에 바닥이 감지되지 않을 경우 절벽으로 판단하는 메소드
    /// </summary>
    bool FrontCliffChecked() => !Physics2D.Raycast(_frontCliffChecker.position, Vector2.down, 1.0f, LayerMask.GetMask("Ground"));
    /// <summary>
    /// 뒤에 바닥이 감지되지 않을 경우 절벽으로 판단하는 메소드
    /// </summary>
    bool BackCliffChecked() => !Physics2D.Raycast(_backCliffChecker.position, Vector2.down, 1.0f, LayerMask.GetMask("Ground"));
}
