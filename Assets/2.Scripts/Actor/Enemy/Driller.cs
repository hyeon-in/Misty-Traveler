using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �帱����� ���� �����Ӱ� �ൿ�� ó���ϴ� Ŭ�����Դϴ�.
/// �ش� ���� ���� ���� �÷��̾ �߰��ϸ� �÷��̾ �����ϸ鼭 �������� �����ϴ� ������ �մϴ�.
/// </summary>
public class Driller : Enemy
{
    [SerializeField] float _timeItTakesToFlip = 0.3f;

    [SerializeField] Transform _detector;           // �÷��̾ Ž���ϴ� ������ ������ ���� Ʈ������
    [SerializeField] Transform _frontCliffChecker;  // ������ ������ �����ϴ� Ʈ������
    [SerializeField] Transform _backCliffChecker;   // ������ ������ �����ϴ� Ʈ������
    [SerializeField] AudioClip _drillSound;     // ���� �� ����
    
    bool _isAttacking;  // �����ϰ� �ִ��� üũ
    bool _isChasing;    // �÷��̾ �����ϰ� �ִ��� üũ

    float _timeRemainingToAttack;   // ���� ���ݱ��� ���� �ð�
    float _timeRemainingToFlip;     // ���ƺ� �� ���� ���� �ð�
    
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
        // �̹� ���� ���¸� ó������ ����
        if (isDead) return;

        deltaTime = Time.deltaTime;

        // �̲������� ���� �� ������ ����ġ�� �̲������� �ߴ��ϰ� �ӵ��� 0���� ����
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
            // ����

            // �̵� �ӵ��� ���� �ӵ��� ����
            controller.VelocityX = enemyData.patrolSpeed * actorTransform.localScale.x;

            // �����̳� ���� ����ġ�� �ڵ��ƺ�
            if (WallChecked() || FrontCliffChecked())
            {
                Flip();
            }

            // �÷��̾ �߰��ϸ� ���� ���·� ����
            if (IsPlayerDetected())
            {
                _isChasing = true;
                animator.SetFloat(GetAnimationHash("Speed"), 2f);
            }
        }
        else
        {
            // �÷��̾� ����
            if (!_isAttacking)
            {
                // ���� ���°� �ƴ� ���

                // �̵� �ӵ��� ���� �ӵ��� ����
                controller.VelocityX = enemyData.followSpeed * actorTransform.localScale.x;

                // �����̳� ���� ����ġ�� �ڵ��ƺ�
                if (WallChecked() || FrontCliffChecked())
                {
                    Flip();
                }

                // ���� ���ݱ��� �ð��� �������� ��� �ǽð� ����
                if (_timeRemainingToAttack > 0)
                {
                    _timeRemainingToAttack -= deltaTime;
                }

                // ���� �ٶ󺸴� ����� �÷��̾��� ��ġ�� ���� ���� ���� ����
                bool enemyFacingRight = actorTransform.localScale.x == 1;
                bool playerIsRight = actorTransform.position.x < _playerTransform.position.x;
                if (enemyFacingRight != playerIsRight)
                {
                    // �ٶ󺸴� ����� �÷��̾��� ��ġ�� �ٸ��� �ణ�� �ð� �� �ڵ��ƺ�
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
                    // �ٶ󺸴� ����� �÷��̾��� ��ġ�� ���� ���� ���ݱ��� ���� �ð��� 0 ���ϸ� ���� ����
                    _isAttacking = true;
                    _timeRemainingToAttack = enemyData.attackDelay;
                    StartCoroutine(Attack());
                }

                // �÷��̾ ���ƴٸ� �ٽ� ���� ���� ��ȯ
                if (!IsPlayerDetected())
                {
                    _isChasing = false;
                    animator.SetFloat(GetAnimationHash("Speed"), 1f);
                }
            }
        }
    }

    /// <summary>
    /// �帱���� ������ ó���ϴ� �ڷ�ƾ�Դϴ�.
    /// </summary>
    IEnumerator Attack()
    {
        // ���� �ִϸ��̼� ����
        animator.ResetTrigger(GetAnimationHash("AttackEnd"));
        animator.SetTrigger(GetAnimationHash("Attack"));

        bool isSlided = false;

        // 1������ ���
        yield return null;

        // ���� �ִϸ��̼��� ����� �� ���� ����
        while (!IsAnimationEnded())
        {
            // �̲������� ���°� �ƴϰ� �ִϸ��̼��� ����ȭ �ð��� 0.53�� ~ 0.6���̸� �̲������� ���� ����
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

        // ������ �����ϰ� �̲������� ����
        animator.SetTrigger(GetAnimationHash("AttackEnd"));
        controller.SlideCancle();
        _isAttacking = false;
    }

    /// <summary>
    /// �帱���� �÷��̾ �����ߴ��� Ȯ������ �޼ҵ��Դϴ�.
    /// </summary>
    bool IsPlayerDetected()
    {
        // �÷��̾���� �Ÿ��� Ž�� ���� �̳��� ��� 
        float playerDistance = Vector2.Distance(actorTransform.position, _playerTransform.position);
        if (playerDistance <= enemyData.detectRange)
        {
            // �÷��̾� ���� ���ϱ�
            Vector2 playerDirection = (_playerTransform.position - actorTransform.position).normalized;
            // �÷��̾�� �� ���̿� Ground�� �������� �ʾƾ� �߰��� ������ ó��
            var playerDetected = !Physics2D.Raycast(actorTransform.position, playerDirection, playerDistance, LayerMask.GetMask("Ground"));
#if UNITY_EDITOR
            Debug.DrawRay(actorTransform.position, playerDirection * playerDistance, Color.yellow);
#endif
            return playerDetected;
        }
        return false;
    }

    /// <summary>
    /// �帱�� �տ� ������ �ִ��� üũ�ϱ� ���� �޼ҵ��Դϴ�.
    /// </summary>
    bool FrontCliffChecked()
    {
        var cliffed = Physics2D.Raycast(_frontCliffChecker.position, Vector2.down, 1.0f, _groundLayer);

        return !cliffed;
    }

    /// <summary>
    /// �帱�� �ڿ� ������ �ִ��� üũ�ϱ� ���� �޼ҵ��Դϴ�.
    /// </summary>
    bool BackCliffChecked()
    {
        var cliffed = Physics2D.Raycast(_backCliffChecker.position, Vector2.down, 1.0f, _groundLayer);

        return !cliffed;
    }

    /// <summary>
    /// �帱���� ���� �����ߴ��� üũ�ϱ� ���� �޼ҵ��Դϴ�.
    /// </summary>
    bool WallChecked()
    {
         bool isWalled = actorTransform.localScale.x == 1 ? controller.IsRightWalled : controller.IsLeftWalled;
         return isWalled;
    }
}