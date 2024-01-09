using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �� ĳ������ �����͸� ��Ÿ���� ScriptableObject�Դϴ�.
/// </summary>
public class EnemyData : ScriptableObject
{
    public string enemyName = "NAME";   // ���� �̸�
    public float patrolSpeed = 1.5f;    // ���� �ӵ�
    public float followSpeed = 0f;      // �÷��̾� ���� �ӵ�

    public bool isBodyTackled = true;   // ���� ���� ����
    public int bodyTackleDamage = 1;    // ���� ���� �����

    public float detectRange = 3.0f;    // �÷��̾� ���� ����
    public float attackDelay = 1.5f;    // ������ �� ���� ���� ���ݱ����� ������
    public int attackDamage = 1;        // ���� �����

    public int health;                  // ü��
    public bool superArmor;             // ���� �Ƹ� ����

    public int money;                   // ���� óġ�� �÷��̾ ȹ���ϴ� ���� ��(�ش� ������Ʈ������ ������ �ʴ� �����Դϴ�.)

    public Material blinkMaterial;      // ���� ���� �޾��� �� ��������Ʈ�� �����̴� ����Ʈ�� ����ϱ� ���� ���׸����Դϴ�.
}