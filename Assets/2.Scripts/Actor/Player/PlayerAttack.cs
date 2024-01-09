using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾��� ������ ó���ϴ� �޼ҵ��Դϴ�.
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    public Transform attackPoint;   // ���� ���� ��ǥ�� ����ϴ� Ʈ������ ������Ʈ
    public Transform effectPoint;   // ����Ʈ�� ������ �� ����ϴ� ���� ��ǥ
    public Vector2 attackRange = Vector2.one;   // ���� ����

    public int HitCount { get; private set; }   // ������ ������ ���� ���� ǥ���ϴ� ������Ƽ

    LayerMask _enemyLayer;  // �� ���̾�
    LayerMask _groundLayer; // �׶��� ���̾�
    void Awake()
    {
        _enemyLayer = LayerMask.GetMask("Enemy");
        _groundLayer = LayerMask.GetMask("Ground");
    }

    /// <summary>
    /// �÷��̾��� ������ ������ �������� ��� ������ ���ظ� �ְ� ���� ���� ���θ� ��ȯ�ϴ� �޼ҵ��Դϴ�.
    /// </summary>
    /// <param name="damage">���� ����</param>
    /// <param name="knockBack">���� �˹鷮</param>
    /// <param name="effect">����Ʈ ���� ����</param>
    /// <returns>���� ���� ����</returns>
    public bool IsAttacking(int damage, KnockBack knockBack, bool effect = true)
    {
        HitCount = 0;   // ������ ������ ���� �� �ʱ�ȭ
        if(attackRange == Vector2.zero) return false;   // ���� ������ 0�̸� false ��ȯ

        bool isHit = false; // ���� ���� ����

        var hits = Physics2D.OverlapBoxAll(attackPoint.position, attackRange, 0, _enemyLayer);
        if(hits.Length > 0)
        {
            // ���� �����ȿ� ���� ������ ���
            for(int i = 0; i < hits.Length; i++)
            {
                EnemyDamage enemyDamage = hits[i].GetComponent<EnemyDamage>();
                if(enemyDamage.IsDead || enemyDamage.IsInvincibled) continue;   // ���� �̹� �׾��ų� ���� �����̸� �������� �Ѿ

                hits[i].GetComponent<EnemyDamage>().TakeDamage(damage, knockBack);  // ����� ó��

                // �Ҹ� Ÿ�Ӱ� ȭ�� ��鸲 ����
                ScreenEffect.instance.BulletTimeStart(0.0f, 0.05f);
                ScreenEffect.instance.ShakeEffectStart(0.2f, 0.05f);
                
                if(effect)
                {
                    // ����Ʈ�� ����ϰ� ���� ��� ���� ���� ����Ʈ ����
                    ObjectPoolManager.instance.GetPoolObject("SlashEffect", effectPoint.position, angle: Random.Range(0f,359f));
                }

                isHit = true; // ���� ���� ���θ� true�� ����
                HitCount++; // ������ ������ �� �� ����
            }
        }

        return isHit;   // ���� ���� ���� ��ȯ
    }

    /// <summary>
    /// ������ Ground�� ��Ҵ��� ���θ� ��ȯ�ϴ� �޼ҵ��Դϴ�.
    /// </summary>
    public bool GroundHit()
    {
        if(attackRange == Vector2.zero) return false;   // ���� ������ 0�̸� false ��ȯ
        return Physics2D.OverlapBox(attackPoint.position, attackRange, 0, _groundLayer);    // �浹 üũ
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, attackRange);
    }
}