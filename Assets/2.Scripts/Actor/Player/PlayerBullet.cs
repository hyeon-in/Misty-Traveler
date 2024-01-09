using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 원거리 투사체(총알)를 처리하기 위한 클래스입니다.
/// </summary>
/// <remarks>해당 클래스를 사용하면 자동으로 PlayerAttack클래스가 inspector에 추가됩니다.</remarks>
[RequireComponent(typeof(PlayerAttack))]
public class PlayerBullet : MonoBehaviour
{
    [SerializeField] int damage = 3;                // 위력
    [SerializeField] float _bulletSpeed = 10f;      // 총알 속도
    [SerializeField] float bulletDuration = 2.0f;   // 총알 지속 시간
    [SerializeField] float knockBackForce = 8.0f;   // 총알 넉백량
    [SerializeField] AudioClip hitSound;            // 명중 시 재생되는 사운드

    float _angle;
    float _duration;
    bool _isHit;

    Vector3 _bulletDirection;
    KnockBack _knockBack = new KnockBack();

    Animator _anim;
    Transform _bulletTransform;
    PlayerAttack _attack;

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _attack = GetComponent<PlayerAttack>();
        _bulletTransform = GetComponent<Transform>();

        _knockBack.force = knockBackForce;
    }

    void OnEnable()
    {
        // 총알이 생성될 때 rotation 값을 이용해 날아갈 방향을 결정
        // 이후 총알의 rotation은 초기값으로 변경
        var rotation = _bulletTransform.rotation;
        _bulletDirection = rotation * Vector3.left;
        _angle = rotation.eulerAngles.z;
        _bulletTransform.rotation = Quaternion.identity;

        // 총알이 날아가는 방향에 따라 넉백 방향이 달라짐
        _knockBack.direction = _angle > 90 && _angle < 270 ? 1 : -1;
    }

    void Update()
    {
        // 이미 적중된 상태이면 실행하지 않음
        if(_isHit) return;

        _bulletTransform.Translate(_bulletDirection * _bulletSpeed * Time.deltaTime);

        // 총알이 적이나 사물에 닿은 경우 적중 된 상태로 변경하고 효과음을 재생
        if(_attack.IsAttacking(damage, _knockBack, false) || _attack.GroundHit())
        {
            _isHit = true;
            _anim.SetTrigger("Hit");
            SoundManager.instance.SoundEffectPlay(hitSound);
        }

        // 총알이 일정 시간을 넘을 경우 풀로 반환
        _duration += Time.deltaTime;
        if(_duration >= bulletDuration)
        {
            ObjectPoolManager.instance.ReturnPoolObject(gameObject);
        }
    }

    void OnDisable()
    {
        // 반환할 때 상태 초기화
        _isHit = false;
        _duration = 0;
    }
}
