using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 체력 회복 조각 오브젝트의 기능을 처리하는 클래스입니다.
/// </summary>
/// <remarks>적을 처치하면 일정 확률로 등장하는 오브젝트로, 플레이어와 접촉시 체력을 회복하는 기능을 가지고 있는 오브젝트입니다.</remarks>
public class HealingPiece : MonoBehaviour
{
    readonly int hashBroken = Animator.StringToHash("Broken");
    readonly int hashInit = Animator.StringToHash("Init");

    [SerializeField] float _collisionRange = 0.5f;  // 충돌 범위(radius)
    [SerializeField] bool _shouldDefinitelyDrop;    // 반드시 드롭되는 체력 회복 오브젝트인지 체크
    [SerializeField] AudioClip _pickUpSound;        // 플레이어가 해당 오브젝트와 접촉하여 체력을 회복했을때 재생되는 사운드

    LayerMask _playerLayer;

    Animator _anim;
    Transform _transform;
    PlayerDamage _playerDamage;
    ActorController _controller;

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _transform = GetComponent<Transform>();
        _controller = GetComponent<ActorController>();
        _playerDamage = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDamage>();

        _playerLayer = LayerMask.GetMask("Player");
    }

    void OnEnable()
    {
        if (ShouldDropBasedOnProbability())
        {
            // 드롭 확률을 넘었을 경우 활성화
            _controller.UseGravity = true;
            StartCoroutine(PlayerHealing());
        }
        else
        {
            // 드롭 확률을 넘지 않았다면 바로 반환
            ObjectPoolManager.instance.ReturnPoolObject(gameObject);
        }
    }

    /// <summary>
    /// 확률에 따라 해당 오브젝트를 드롭할지 결정하는 메소드입니다.
    /// 확률은 플레이어의 현재 체력 퍼센트에 따라 달라지며, 무작위 값이 드롭률보다 낮으면 true를 반환합니다.
    /// </summary>
    /// <param name="healthPercent">플레이어의 체력 퍼센트</param>
    bool ShouldDropBasedOnProbability()
    {
        if(_shouldDefinitelyDrop) return true;

        float healthPercent = _playerDamage.GetHealthPercent(); // 플레이어의 체력 퍼센트
        float dropRate;

        // 체력이 75% 초과일 경우 15% 확률
        // 체력이 50% 초과일 경우 37.5% 확률
        // 체력이 25% 초과일 경우 60% 확률
        // 체력이 25% 이하일 경우 85% 확률
        if (healthPercent > 0.75f)
        {
            dropRate = 0.15f;
        }
        else if (healthPercent > 0.5f)
        {
            dropRate = 0.375f;
        }
        else if (healthPercent > 0.25f)
        {
            dropRate = 0.6f;
        }
        else
        {
            dropRate = 0.85f;
        }

        return Random.value <= dropRate;
    }

    /// <summary>
    /// 체력 회복 오브젝트 기능을 처리하기 위한 코루틴입니다.
    /// </summary>
    IEnumerator PlayerHealing()
    {
        // 초기화
        _anim.SetTrigger(hashInit);
        float time = 0;
        float moveX = Random.Range(6.0f, 18.0f);
        float direction = Mathf.Sign(Random.Range(-1f, 1f));
        float velocityX = moveX * direction;
        _controller.VelocityY = Random.Range(6f, 14f);
        bool isGrounded = false;

        // 시간이 15초를 넘기 전에는 기능 수행
        while(time < 15.0f)
        {
            time += Time.deltaTime;
            if (!isGrounded)
            {
                // 바닥에 닿을 때 까지 미끄러짐
                _controller.VelocityX = velocityX;
                if (_controller.IsGrounded)
                {
                    isGrounded = true;
                    _controller.VelocityX = 0f;
                    _controller.SlideMove(moveX, direction);
                }
            }

            // 벽에 닿을 경우 튕겨져 나옴
            bool isLeftWalled = _controller.VelocityX < 0 && _controller.IsLeftWalled;
            bool isRightWalled = _controller.VelocityX > 0 && _controller.IsRightWalled;
            if (isLeftWalled || isRightWalled)
            {
                direction = -direction;
                _controller.VelocityX = moveX * direction;
            }

            // 플레이어와 접촉시 플레이어의 체력 회복
            if(Physics2D.OverlapCircle(_transform.position, _collisionRange, _playerLayer))
            {
                _playerDamage.HealthRecovery(1);
                _anim.SetTrigger(hashBroken);
                _controller.UseGravity = false;
                SoundManager.instance.SoundEffectPlay(_pickUpSound);

                yield return YieldInstructionCache.WaitForSeconds(1.0f);
                break;
            }

            yield return null;
        }
        // 반환
        ObjectPoolManager.instance.ReturnPoolObject(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _collisionRange);
    }
}
