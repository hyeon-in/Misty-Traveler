using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ����� �����ϰ� ó���ϱ����� �߻� Ŭ�����Դϴ�.
/// </summary>
[RequireComponent(typeof(EnemyDamage))]
public abstract class Enemy : Actor
{
    public string keyName;  // �� �����͸� �������� ���� ����ϴ� Ű, 

    protected EnemyDamage enemyDamage;              // �� ����� ó�� Ŭ����
    protected EnemyBodyTackle bodyTackle = null;    // ���� ���� ó�� Ŭ����
    protected EnemyData enemyData;                  // �� ������ Ŭ����
    protected LayerMask playerLayer;

    protected override void Awake()
    {
        // �ش� ���� �̹� ���� ������ Ȯ���ϰ� ���� ���� ��� ����
        if (DeadEnemyManager.IsDeadEnemy(gameObject.name))
        {
            isDead = true;
            Destroy(gameObject);
            return;
        }

        base.Awake();
        enemyDamage = GetComponent<EnemyDamage>();

        // key�� ����Ͽ� �� �����͸� ������ �� �ܺ� Ŭ������ ������ ���� ����
        enemyData = EnemyDataManager.GetEnemyData(keyName); 
        if (enemyData.isBodyTackled && TryGetComponent(out EnemyBodyTackle bodyTackle))
        {
            this.bodyTackle = bodyTackle;
            this.bodyTackle.damage = enemyData.bodyTackleDamage;
        }
        enemyDamage.MaxHealth = enemyData.health;
        enemyDamage.SuperArmor = enemyData.superArmor;
        enemyDamage.BlinkMaterial = enemyData.blinkMaterial;

        // �ݹ� �޼ҵ� �߰�
        enemyDamage.KnockBack += OnKnockedBack;
        enemyDamage.Died += OnDied;

        playerLayer = LayerMask.GetMask("Player");
    }

    /// <summary>
    /// ���� �˹� ó���� ���� ȣ��Ǵ� �ݹ� �޼ҵ��Դϴ�.
    /// </summary>
    /// <param name="knockBack">�˹� ����ü</param>
    protected virtual void OnKnockedBack(KnockBack knockBack)
    {
        if(controller == null) return;
        // �̲��������� �˹� ó��
        controller.SlideMove(knockBack.force, knockBack.direction);
    }

    /// <summary>
    /// ���� ��� ó���� ���� ȣ��Ǵ� �ݹ� �޼ҵ��Դϴ�.
    /// </summary>
    protected virtual IEnumerator OnDied()
    {
        isDead = true;
        DeadEnemyManager.AddDeadEnemy(gameObject.name); // ���� ���� ��Ȱ�ϱ� ������ �ٽ� �������� ����

        // ���� ���� ����
        if(bodyTackle != null)
        {
            bodyTackle.isBodyTackled = false;
        }

        // ȸ�� ������ ����
        Vector2 position = new Vector2(transform.position.x, transform.position.y + 2);
        ObjectPoolManager.instance.GetPoolObject("HealingPiece", position);
        
        yield return null;

        // ����
        Destroy(gameObject);
    }
}