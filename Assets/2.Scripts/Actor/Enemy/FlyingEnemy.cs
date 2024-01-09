using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공중의 일정 영역을 돌아다니는 적의 기능을 처리하는 클래스입니다.
/// </summary>
public class FlyingEnemy : Enemy
{
    [SerializeField] float _movingRange = 0.6f;                 // 이동 가능한 범위(radius)
    [SerializeField] float _moveDirectionResetTimeMin = 0.5f;   // 다음 이동 방향을 결정할 최소 시간
    [SerializeField] float _moveDirectionResetTimeMax = 1.25f;  // 다음 이동 방향을 결정할 최대 시간

    [SerializeField] Transform _attackPos;      // 투사체를 생성하는 위치를 표시하는 Transform 객체
    [SerializeField] AudioClip _attackSound;    // 공격 사운드

    float _timeToChangeDirection;   // 다음 이동 방향을 결정하기까지 남은 시간
    float _timeRemainingToAttack;   // 다음 공격을 실행할 때 까지 남은 시간

    bool _isAttacking;   // 현재 공격중인 상태인지 확인

    Vector3 _moveDirection;         // 이동하는 방향
    Vector3 _originPos = Vector3.zero;  // 처음 적의 위치

    Transform _playerTransform; // 플레이어의 Transform 객체

    protected override void Awake()
    {
        base.Awake();
        _playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        _originPos = actorTransform.position;
    }
    void Update()
    {
        // Time.deltaTime 캐싱
        deltaTime = Time.deltaTime;

        // 이동 범위를 벗어나면 움직이는 방향을 원점 방향으로 설정
        Vector3 newMoveDirection;
        float movingRangeSqr = _movingRange * _movingRange;
        if(movingRangeSqr <= (_originPos - actorTransform.position).sqrMagnitude)
        {
            newMoveDirection = (_originPos - actorTransform.position).normalized;
            _moveDirection = newMoveDirection;
            _timeToChangeDirection = Random.Range(_moveDirectionResetTimeMin, _moveDirectionResetTimeMax);  // 방향 전환 시간 리셋
        }

        _timeToChangeDirection -= deltaTime;
        if (_timeToChangeDirection <= 0)
        {
            // 일정 시간마다 움직일 방향 무작위로 전환
            float x = Random.Range(-1f, 1f);
            float y = Random.Range(-1f, 1f);
            newMoveDirection = new Vector2(x, y).normalized;
            _moveDirection = newMoveDirection;

            _timeToChangeDirection = Random.Range(_moveDirectionResetTimeMin, _moveDirectionResetTimeMax); // 방향 전환 시간 리셋
        }

        // 움직이는 방향으로 속도 적용
        controller.VelocityX = enemyData.patrolSpeed * _moveDirection.x;
        controller.VelocityY = enemyData.patrolSpeed * _moveDirection.y;

        if (!_isAttacking)
        {
            // 공격을 하고 있지 않을 경우
            // 다음 공격까지 시간이 남아있을 경우 실시간 시간 감소
            // 공격 준비가 됐고, 플레이어가 감지되면 공격 실행
            _timeRemainingToAttack -= deltaTime;
            if (_timeRemainingToAttack <= 0)
            {
                if (IsPlayerDetected())
                {
                    // 공격할 때 적이 플레이어를 바라봄
                    bool enemyFacingRight = actorTransform.localScale.x == 1;
                    bool playerIsRight = actorTransform.position.x < _playerTransform.position.x;
                    if(enemyFacingRight != playerIsRight)
                    {
                        Flip();
                    }
                    _isAttacking = true;    // 공격중인 상태로 변경
                    _timeRemainingToAttack = enemyData.attackDelay; // 공격 준비시간 초기화

                    // 공격 코루틴 실행
                    StartCoroutine(Attack());
                }
            }
        }
    }

    /// <summary>
    /// 적이 플레이어를 감지했을 경우 true를 반환하는 메소드입니다.
    /// </summary>
    bool IsPlayerDetected()
    {
        // 플레이어와 적과의 거리를 계산하고 감지 범위 내로 들어오면 true 반환
        float playerDistance = Vector2.Distance(actorTransform.position, _playerTransform.position);
        if(playerDistance <= enemyData.detectRange)
        {
            Vector2 playerDirection = (_playerTransform.position - _attackPos.position).normalized;
#if UNITY_EDITOR
            Debug.DrawRay(_attackPos.position, playerDirection * playerDistance, Color.yellow);
#endif
            // 적이 플레이어의 위치로 ray를 발사한 뒤 ray 사이에 ground가 감지되면 발견하지 못한것으로 판단
            return !Physics2D.Raycast(_attackPos.position, playerDirection, playerDistance, LayerMask.GetMask("Ground"));
        }
        return false;
    }

    /// <summary>
    /// 해당 적의 공격 기능을 처리하는 코루틴
    /// </summary>
    IEnumerator Attack()
    {
        // 공격 애니메이션으로 전환 후 1프레임 대기
        bool fire = false;
        animator.SetTrigger("GuidedMissile");
        yield return null;

        // 애니메이션이 끝날때까지 반복
        while(!IsAnimationEnded())
        {
            // 애니메이션의 정규화 시간이 0.75초 ~ 0.8초 사이이면 투사체 생성
            if (IsAnimatorNormalizedTimeInBetween(0.75f, 0.8f))
            {
                if(!fire)
                {
                    // 좌표 설정 및 날아가는 각도 방향 설정
                    Vector2 firePos = _attackPos.position;
                    float x = firePos.x - _playerTransform.position.x;
                    float y = firePos.y - _playerTransform.position.y;
                    float guidedMissileAngle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

                    // 오브젝트 풀에서 플레이어를 추적하는 미사일 투사체를 가져와서 활성화하고 공격 사운드 재생
                    ObjectPoolManager.instance.GetPoolObject("GuidedMissile", firePos, actorTransform.localScale.x, guidedMissileAngle);
                    SoundManager.instance.SoundEffectPlay(_attackSound);
                    fire = true;
                }
            }
            yield return null;
        }

        // 공격이 끝나면 공격 상태 해제
        _isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        if(_originPos == Vector3.zero)
        {
            Gizmos.DrawWireSphere(transform.position, _movingRange);
        }
        else
        {
            Gizmos.DrawWireSphere(_originPos, _movingRange);
        }

        if(enemyData == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemyData.detectRange);
    }
}
