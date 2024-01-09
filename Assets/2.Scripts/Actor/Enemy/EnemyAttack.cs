using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ������ ó���ϱ� ���� Ŭ�����Դϴ�.
/// </summary>
public class EnemyAttack : MonoBehaviour
{
    public Transform attackPoint;   // ���� ��ǥ Ʈ������
    public Vector2 attackRange;     // ���� ����(�簢��)

    LayerMask _playerLayer;

    PlayerDamage _playerDamage;

    void Awake()
    {
        _playerLayer = LayerMask.GetMask("Player");
        _playerDamage = GameObject.Find("Player").GetComponent<PlayerDamage>();
    }

    /// <summary>
    /// �÷��̾ ���� ���� ������ ó���ϰ� ������ �����ߴ��� üũ�ϱ� ���� �޼ҵ��Դϴ�.
    /// </summary>
    /// <param name="damage">���ط�</param>
    /// <param name="knockBack">�˹� ����ü</param>
    /// <returns>���� ���� ����</returns>
    public bool IsAttacking(int damage, KnockBack knockBack)
    {
        // ���� ������ 0�̸� false ��ȯ
        if(attackRange == Vector2.zero) return false;

        // ���� ������ �÷��̾�� ������ ����� ó���� �����մϴ�.
        if(Physics2D.OverlapBox(attackPoint.position, attackRange, 0, _playerLayer))
        {
            _playerDamage.TakeDamage(damage, knockBack);
            return true;
        }

        // �������� �ʾ����� false ��ȯ
        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, attackRange);
    }
}