using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적 캐릭터의 데이터를 나타내는 ScriptableObject입니다.
/// </summary>
public class EnemyData : ScriptableObject
{
    public string enemyName = "NAME";   // 적의 이름
    public float patrolSpeed = 1.5f;    // 순찰 속도
    public float followSpeed = 0f;      // 플레이어 추적 속도

    public bool isBodyTackled = true;   // 접촉 공격 여부
    public int bodyTackleDamage = 1;    // 접촉 공격 대미지

    public float detectRange = 3.0f;    // 플레이어 감지 범위
    public float attackDelay = 1.5f;    // 공격을 한 이후 다음 공격까지의 딜레이
    public int attackDamage = 1;        // 공격 대미지

    public int health;                  // 체력
    public bool superArmor;             // 슈퍼 아머 여부

    public int money;                   // 적을 처치시 플레이어가 획득하는 돈의 량(해당 프로젝트에서는 사용되지 않는 더미입니다.)

    public Material blinkMaterial;      // 적이 공격 받았을 때 스프라이트가 깜빡이는 이펙트를 출력하기 위한 머테리얼입니다.
}