using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EnemyBullet�� ���� Ŭ������, ����ü�� �÷��̾ �����ϴ� ����� �����մϴ�.
/// </summary>
public class GuidedMissile : EnemyBullet
{
    Transform _playerTransform;

    protected override void Awake()
    {
        base.Awake();
        _playerTransform = GameObject.Find("Player").GetComponent<Transform>();
    }

    protected override void Update()
    {
        if (isHit) return;

        float x = _bulletTransform.position.x - _playerTransform.position.x;
        float y = _bulletTransform.position.y - _playerTransform.position.y;
        float targetAngle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

        var rotation = Quaternion.Euler(0, 0, targetAngle);
        bulletDirection = rotation * Vector3.left;

        base.Update();
    }
}
