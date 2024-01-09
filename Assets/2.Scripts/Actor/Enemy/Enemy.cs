using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적의 기능을 관리하고 처리하기위한 추상 클래스입니다.
/// </summary>
[RequireComponent(typeof(EnemyDamage))]
public abstract class Enemy : Actor
{
    public string keyName;  // 적 데이터를 가져오기 위해 사용하는 키, 

    protected EnemyDamage enemyDamage;              // 적 대미지 처리 클래스
    protected EnemyBodyTackle bodyTackle = null;    // 접촉 피해 처리 클래스
    protected EnemyData enemyData;                  // 적 데이터 클래스
    protected LayerMask playerLayer;

    protected override void Awake()
    {
        // 해당 적이 이미 죽은 적인지 확인하고 죽은 적일 경우 삭제
        if (DeadEnemyManager.IsDeadEnemy(gameObject.name))
        {
            isDead = true;
            Destroy(gameObject);
            return;
        }

        base.Awake();
        enemyDamage = GetComponent<EnemyDamage>();

        // key를 사용하여 적 데이터를 가져온 뒤 외부 클래스에 데이터 정보 전달
        enemyData = EnemyDataManager.GetEnemyData(keyName); 
        if (enemyData.isBodyTackled && TryGetComponent(out EnemyBodyTackle bodyTackle))
        {
            this.bodyTackle = bodyTackle;
            this.bodyTackle.damage = enemyData.bodyTackleDamage;
        }
        enemyDamage.MaxHealth = enemyData.health;
        enemyDamage.SuperArmor = enemyData.superArmor;
        enemyDamage.BlinkMaterial = enemyData.blinkMaterial;

        // 콜백 메소드 추가
        enemyDamage.KnockBack += OnKnockedBack;
        enemyDamage.Died += OnDied;

        playerLayer = LayerMask.GetMask("Player");
    }

    /// <summary>
    /// 적의 넉백 처리를 위해 호출되는 콜백 메소드입니다.
    /// </summary>
    /// <param name="knockBack">넉백 구조체</param>
    protected virtual void OnKnockedBack(KnockBack knockBack)
    {
        if(controller == null) return;
        // 미끄러짐으로 넉백 처리
        controller.SlideMove(knockBack.force, knockBack.direction);
    }

    /// <summary>
    /// 적의 사망 처리를 위해 호출되는 콜백 메소드입니다.
    /// </summary>
    protected virtual IEnumerator OnDied()
    {
        isDead = true;
        DeadEnemyManager.AddDeadEnemy(gameObject.name); // 죽은 적이 부활하기 전까지 다시 등장하지 않음

        // 접촉 공격 종료
        if(bodyTackle != null)
        {
            bodyTackle.isBodyTackled = false;
        }

        // 회복 아이템 생성
        Vector2 position = new Vector2(transform.position.x, transform.position.y + 2);
        ObjectPoolManager.instance.GetPoolObject("HealingPiece", position);
        
        yield return null;

        // 삭제
        Destroy(gameObject);
    }
}