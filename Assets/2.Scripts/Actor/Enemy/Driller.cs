using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 드릴러라는 적의 움직임과 행동을 처리하는 클래스입니다.
/// 해당 적은 순찰 도중 플레이어를 발견하면 플레이어를 추적하면서 전방으로 돌진하는 공격을 합니다.
/// </summary>
public class Driller : Enemy
{
    [SerializeField] float _timeItTakesToFlip = 0.3f;

    [SerializeField] Transform _detector;           // 플레이어를 탐지하는 레이의 원점을 가진 트랜스폼
    [SerializeField] Transform _frontCliffChecker;  // 앞쪽의 절벽을 감지하는 트랜스폼
    [SerializeField] Transform _backCliffChecker;   // 뒤쪽의 절벽을 감지하는 트랜스폼
    [SerializeField] AudioClip _drillSound;     // 공격 시 사운드
    
    bool _isAttacking;  // 공격하고 있는지 체크
    bool _isChasing;    // 플레이어를 추적하고 있는지 체크

    float _timeRemainingToAttack;   // 다음 공격까지 남은 시간
    float _timeRemainingToFlip;     // 돌아볼 때 까지 남은 시간
    
    LayerMask _groundLayer;
    Transform _playerTransform;

    protected override void Awake()
    {
        base.Awake();

        _playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _groundLayer = LayerMask.GetMask("Ground");
    }

    void Update()
    {
        // 이미 죽은 상태면 처리하지 않음
        if (isDead) return;

        deltaTime = Time.deltaTime;

        // 미끄러지고 있을 때 절벽과 마주치면 미끄러짐을 중단하고 속도를 0으로 변경
        if (controller.IsSliding)
        {
            if (FrontCliffChecked() || (BackCliffChecked()))
            {
                controller.SlideCancle();
                controller.VelocityX = 0;
            }
        }

        if (!_isChasing)
        {
            // 순찰

            // 이동 속도를 순찰 속도로 설정
            controller.VelocityX = enemyData.patrolSpeed * actorTransform.localScale.x;

            // 절벽이나 벽과 마주치면 뒤돌아봄
            if (WallChecked() || FrontCliffChecked())
            {
                Flip();
            }

            // 플레이어를 발견하면 추적 상태로 변경
            if (IsPlayerDetected())
            {
                _isChasing = true;
                animator.SetFloat(GetAnimationHash("Speed"), 2f);
            }
        }
        else
        {
            // 플레이어 추적
            if (!_isAttacking)
            {
                // 공격 상태가 아닐 경우

                // 이동 속도를 추적 속도로 설정
                controller.VelocityX = enemyData.followSpeed * actorTransform.localScale.x;

                // 절벽이나 벽과 마주치면 뒤돌아봄
                if (WallChecked() || FrontCliffChecked())
                {
                    Flip();
                }

                // 다음 공격까지 시간이 남아있을 경우 실시간 감소
                if (_timeRemainingToAttack > 0)
                {
                    _timeRemainingToAttack -= deltaTime;
                }

                // 적이 바라보는 방향과 플레이어의 위치를 통해 공격 여부 결정
                bool enemyFacingRight = actorTransform.localScale.x == 1;
                bool playerIsRight = actorTransform.position.x < _playerTransform.position.x;
                if (enemyFacingRight != playerIsRight)
                {
                    // 바라보는 방향과 플레이어의 위치가 다르면 약간의 시간 후 뒤돌아봄
                    if (_timeRemainingToFlip >= _timeItTakesToFlip)
                    {
                        Flip();
                        _timeRemainingToFlip = 0;
                    }
                    else
                    {
                        _timeRemainingToFlip += deltaTime;
                    }
                }
                else if (_timeRemainingToAttack <= 0)
                {
                    // 바라보는 방향과 플레이어의 위치가 같고 다음 공격까지 남은 시간이 0 이하면 공격 실행
                    _isAttacking = true;
                    _timeRemainingToAttack = enemyData.attackDelay;
                    StartCoroutine(Attack());
                }

                // 플레이어를 놓쳤다면 다시 순찰 모드로 전환
                if (!IsPlayerDetected())
                {
                    _isChasing = false;
                    animator.SetFloat(GetAnimationHash("Speed"), 1f);
                }
            }
        }
    }

    /// <summary>
    /// 드릴러의 공격을 처리하는 코루틴입니다.
    /// </summary>
    IEnumerator Attack()
    {
        // 공격 애니메이션 실행
        animator.ResetTrigger(GetAnimationHash("AttackEnd"));
        animator.SetTrigger(GetAnimationHash("Attack"));

        bool isSlided = false;

        // 1프레임 대기
        yield return null;

        // 현재 애니메이션이 종료될 때 까지 실행
        while (!IsAnimationEnded())
        {
            // 미끄러지는 상태가 아니고 애니메이션의 정규화 시간이 0.53초 ~ 0.6초이면 미끄러지는 동작 실행
            if(!isSlided)
            {
                if (IsAnimatorNormalizedTimeInBetween(0.53f, 0.6f))
                {
                    SoundManager.instance.SoundEffectPlay(_drillSound);
                    controller.SlideMove(60f, actorTransform.localScale.x, 220f);
                    isSlided = true;
                }
            }
            yield return null;
        }

        // 공격을 종료하고 미끄러짐을 멈춤
        animator.SetTrigger(GetAnimationHash("AttackEnd"));
        controller.SlideCancle();
        _isAttacking = false;
    }

    /// <summary>
    /// 드릴러가 플레이어를 감지했는지 확인위한 메소드입니다.
    /// </summary>
    bool IsPlayerDetected()
    {
        // 플레이어와의 거리가 탐지 범위 이내일 경우 
        float playerDistance = Vector2.Distance(actorTransform.position, _playerTransform.position);
        if (playerDistance <= enemyData.detectRange)
        {
            // 플레이어 방향 구하기
            Vector2 playerDirection = (_playerTransform.position - actorTransform.position).normalized;
            // 플레이어와 적 사이에 Ground가 감지되지 않아야 발견한 것으로 처리
            var playerDetected = !Physics2D.Raycast(actorTransform.position, playerDirection, playerDistance, LayerMask.GetMask("Ground"));
#if UNITY_EDITOR
            Debug.DrawRay(actorTransform.position, playerDirection * playerDistance, Color.yellow);
#endif
            return playerDetected;
        }
        return false;
    }

    /// <summary>
    /// 드릴러 앞에 절벽이 있는지 체크하기 위한 메소드입니다.
    /// </summary>
    bool FrontCliffChecked()
    {
        var cliffed = Physics2D.Raycast(_frontCliffChecker.position, Vector2.down, 1.0f, _groundLayer);

        return !cliffed;
    }

    /// <summary>
    /// 드릴러 뒤에 절벽이 있는지 체크하기 위한 메소드입니다.
    /// </summary>
    bool BackCliffChecked()
    {
        var cliffed = Physics2D.Raycast(_backCliffChecker.position, Vector2.down, 1.0f, _groundLayer);

        return !cliffed;
    }

    /// <summary>
    /// 드릴러가 벽을 감지했는지 체크하기 위한 메소드입니다.
    /// </summary>
    bool WallChecked()
    {
         bool isWalled = actorTransform.localScale.x == 1 ? controller.IsRightWalled : controller.IsLeftWalled;
         return isWalled;
    }
}