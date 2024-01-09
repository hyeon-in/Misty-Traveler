using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ �� ���� �����ϱ⸸ �ϴ� ���� ó���ϴ� Ŭ�����Դϴ�.
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

        // �̲������� ����(�˹� ����)�� �ƴ� ��� ������ ��
        if (!controller.IsSliding)
        {
            // ���� �ӵ��� ���� �ӵ��� ����
            controller.VelocityX = enemyData.patrolSpeed * actorTransform.localScale.x;
            
            // ���� �ε�ġ�ų� ������ �������� ��� �ø�
            bool isWalled = actorTransform.localScale.x == 1 ? controller.IsRightWalled : controller.IsLeftWalled;
            if (FrontCliffChecked() || isWalled)
            {
                Flip();
            }
        }
        else
        {
            // �̲������� ����(���� �޾Ƽ� �˹����� ����)�� ��츦 ó��
            // ������ ����ġ�� �ӵ��� 0���� ���߰� �̲������� �ߴ���
            if (FrontCliffChecked() || BackCliffChecked())
            {
                controller.VelocityX = 0;
                controller.SlideCancle();
            }
        }
    }
    
    /// <summary>
    /// �տ� �ٴ��� �������� ���� ��� �������� �Ǵ��ϴ� �޼ҵ�
    /// </summary>
    bool FrontCliffChecked() => !Physics2D.Raycast(_frontCliffChecker.position, Vector2.down, 1.0f, LayerMask.GetMask("Ground"));
    /// <summary>
    /// �ڿ� �ٴ��� �������� ���� ��� �������� �Ǵ��ϴ� �޼ҵ�
    /// </summary>
    bool BackCliffChecked() => !Physics2D.Raycast(_backCliffChecker.position, Vector2.down, 1.0f, LayerMask.GetMask("Ground"));
}
