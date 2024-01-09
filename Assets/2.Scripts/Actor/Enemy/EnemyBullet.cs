using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적이 사용하는 원거리 투사체의 기능을 처리하기 위한 클래스입니다. 
/// </summary>
public class EnemyBullet : MonoBehaviour
{
    [SerializeField] float _bulletSpeed = 6f;       // 날아가는 속도
    [SerializeField] float _hitRange = 1.0f;         // 충돌 범위(radius)
    [SerializeField] Vector2 _offset = Vector2.zero; // 오프셋
    [SerializeField] int _damage = 1;               // 위력
    [SerializeField] float _knockBackForce = 8.0f;  // 넉백량
    [SerializeField] float _duration = 3.0f;    // 총알이 지속되는 시간

    float _currentDuration;                 // 현재 총알의 남은 지속 시간

    LayerMask _playerLayer, _groundLayer;   // 레이어마스크
    KnockBack _knockBack;                   // 넉백 구조체

    Animator _anim;             // 애니메이터
    PlayerDamage _playerDamage; // 플레이어 대미지 처리 클래스

    protected bool isHit;   // 충돌 여부
    protected float angle;  // 투사체 각도

    protected Vector3 bulletDirection;    // 날아가는 방향
    protected Transform _bulletTransform; // 해당 오브젝트의 트랜스폼

    protected virtual void Awake()
    {
        _anim = GetComponent<Animator>();
        _bulletTransform = GetComponent<Transform>();
        _playerDamage = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDamage>();

        _playerLayer = LayerMask.GetMask("Player");
        _groundLayer = LayerMask.GetMask("Ground");

        _knockBack.force = _knockBackForce;
    }

    protected virtual void OnEnable()
    {
        // 총알 오브젝트의 rotation값을 방향으로 변환
        // angle에 rotation의 eulerAngles.z값을 대입한 뒤, rotation값 초기화
        var rotation = _bulletTransform.rotation;
        bulletDirection = rotation * Vector3.left;
        angle = rotation.eulerAngles.z;
        _bulletTransform.rotation = Quaternion.identity;
    }

    protected virtual void Update()
    {
        // 아직 충돌하지 않았을 경우 투사체 처리
        if (!isHit)
        {
            // 총알을 움직이게 함
            _bulletTransform.Translate(bulletDirection * _bulletSpeed * Time.deltaTime);

            // 중심점 구하기
            var center = (Vector2)_bulletTransform.position + _offset;
            // 원형 충돌체 생성
            var hit = Physics2D.OverlapCircle(center, _hitRange, _playerLayer + _groundLayer);
            if (hit)
            {
                // 충돌 처리
                if (hit.CompareTag("Player"))
                {
                    // 플레이어 충돌시 플레이어의 대미지 처리
                    if (!_playerDamage.IsDodged)
                    {
                        _knockBack.direction = angle > 90 && angle < 270 ? 1 : -1;
                        _playerDamage.TakeDamage(_damage, _knockBack);
                    }
                }
                isHit = true;
                _anim.SetTrigger("Hit");
            }

            // 현재 지속 시간이 설정값을 넘으면 자동으로 반환
            _currentDuration += Time.deltaTime;
            if (_currentDuration >= _duration)
            {
                ObjectPoolManager.instance.ReturnPoolObject(gameObject);
            }
        }
    }

    void OnDisable()
    {
        // 오브젝트가 반환될 때 자동으로 초기화
        isHit = false;
        _currentDuration = 0;
    }

    void OnDrawGizmos()
    {
        var center = (Vector2)transform.position + _offset;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, _hitRange);
    }
}
