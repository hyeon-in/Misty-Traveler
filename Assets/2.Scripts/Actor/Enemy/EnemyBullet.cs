using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ����ϴ� ���Ÿ� ����ü�� ����� ó���ϱ� ���� Ŭ�����Դϴ�. 
/// </summary>
public class EnemyBullet : MonoBehaviour
{
    [SerializeField] float _bulletSpeed = 6f;       // ���ư��� �ӵ�
    [SerializeField] float _hitRange = 1.0f;         // �浹 ����(radius)
    [SerializeField] Vector2 _offset = Vector2.zero; // ������
    [SerializeField] int _damage = 1;               // ����
    [SerializeField] float _knockBackForce = 8.0f;  // �˹鷮
    [SerializeField] float _duration = 3.0f;    // �Ѿ��� ���ӵǴ� �ð�

    float _currentDuration;                 // ���� �Ѿ��� ���� ���� �ð�

    LayerMask _playerLayer, _groundLayer;   // ���̾��ũ
    KnockBack _knockBack;                   // �˹� ����ü

    Animator _anim;             // �ִϸ�����
    PlayerDamage _playerDamage; // �÷��̾� ����� ó�� Ŭ����

    protected bool isHit;   // �浹 ����
    protected float angle;  // ����ü ����

    protected Vector3 bulletDirection;    // ���ư��� ����
    protected Transform _bulletTransform; // �ش� ������Ʈ�� Ʈ������

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
        // �Ѿ� ������Ʈ�� rotation���� �������� ��ȯ
        // angle�� rotation�� eulerAngles.z���� ������ ��, rotation�� �ʱ�ȭ
        var rotation = _bulletTransform.rotation;
        bulletDirection = rotation * Vector3.left;
        angle = rotation.eulerAngles.z;
        _bulletTransform.rotation = Quaternion.identity;
    }

    protected virtual void Update()
    {
        // ���� �浹���� �ʾ��� ��� ����ü ó��
        if (!isHit)
        {
            // �Ѿ��� �����̰� ��
            _bulletTransform.Translate(bulletDirection * _bulletSpeed * Time.deltaTime);

            // �߽��� ���ϱ�
            var center = (Vector2)_bulletTransform.position + _offset;
            // ���� �浹ü ����
            var hit = Physics2D.OverlapCircle(center, _hitRange, _playerLayer + _groundLayer);
            if (hit)
            {
                // �浹 ó��
                if (hit.CompareTag("Player"))
                {
                    // �÷��̾� �浹�� �÷��̾��� ����� ó��
                    if (!_playerDamage.IsDodged)
                    {
                        _knockBack.direction = angle > 90 && angle < 270 ? 1 : -1;
                        _playerDamage.TakeDamage(_damage, _knockBack);
                    }
                }
                isHit = true;
                _anim.SetTrigger("Hit");
            }

            // ���� ���� �ð��� �������� ������ �ڵ����� ��ȯ
            _currentDuration += Time.deltaTime;
            if (_currentDuration >= _duration)
            {
                ObjectPoolManager.instance.ReturnPoolObject(gameObject);
            }
        }
    }

    void OnDisable()
    {
        // ������Ʈ�� ��ȯ�� �� �ڵ����� �ʱ�ȭ
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
