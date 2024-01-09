using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 보스가 사용하는 드론의 정보를 담고있는 클래스입니다.
/// </summary>
[System.Serializable]
public class DelegateDrone
{
    public GameObject gameObject;
    public SpriteRenderer spriteRenderer;
}

/// <summary>
/// 보스를 제어하고 처리하는 클래스입니다.
/// </summary>
[RequireComponent(typeof(BossEnemyDamage))]
public class BossEnemy : Enemy
{
    public GameObject bossHealthBarUI;                      // 보스의 체력 UI
    public GameObject[] invisibleWall;                      // 보스전을 시작할 때 활성화하여 플레이어의 맵 탈출을 방지하기 위한 보이지 않는 벽
    public List<string> attackName = new List<string>();    // 공격 이름 리스트 
    public DelegateDrone[] delegateDrones;                  // 보스가 공격할 때 다루는 드론의 정보를 담은 클래스
    public Sprite[] droneSprite;                            // 드론이 사용하는 공격을 구분하기 위해 나눠놓은 스프라이트
    public SpriteRenderer eyeSprite;        // 플레이어를 바라보는 효과를 구현하기 위한 스프라이트 렌더러

    // 보스가 전투 중 이동할 때 사용하는 트랜스폼 객체들
    public Transform topLeftMovePos;
    public Transform topRightMovePos;
    public Transform centerMovePos;
    public Transform bottomLeftMovePos;
    public Transform bottomRightMovePos;

    public AudioClip battleMusic;           // 전투 중 재생되는 음악
    public AudioClip targetMissileSound;    // 보스가 플레이어 방향으로 탄을 발사할 때 재생되는 사운드
    public AudioClip radiationBulletSound;  // 보스가 사방으로 탄을 발사할 때 재생되는 사운드

    int _phase = 1;             // 현재 전투가 몇 페이즈인지를 담은 변수
    int _currentAttackIndex;    // 현재 하고있는 공격의 인덱스
    float _attackDelay;         // 공격 딜레이

    bool _isDetected;       // 플레이어의 감지 여부
    bool _isAttacking;      // 보스가 공격을 하고 있는지 여부
    bool _isBattleStarted;  // 보스와의 전투가 시작됐는지 여부

    Transform _playerTransform;
    Transform _currentMovePos;
    List<Transform> movePos = new List<Transform>();

    Coroutine _attackCoroutine = null;
    AudioClip _prevMusic;
    SpriteRenderer _sprite;
    BossEnemyDamage _bossEnemyDamage;

    protected override void Awake()
    {
        // 이미 죽은 보스이면 제거
        if (DeadEnemyManager.IsDeadBoss(keyName))
        {
            isDead = true;
            Destroy(gameObject);
            return;
        }

        base.Awake();

        // 움직일 좌표를 한 꺼번에 리스트에 추가
        var movePosList = new List<Transform>() { topRightMovePos, topRightMovePos, centerMovePos, bottomLeftMovePos, bottomRightMovePos };
        movePos.AddRange(movePosList);

        // 처음 움직일 좌표를 중앙으로 설정
        _currentMovePos = centerMovePos;

        // 스프라이트 초기화
        _sprite = transform.GetComponent<SpriteRenderer>();
        _sprite.color = new Color32(255, 255, 255, 0);

        // 공격 딜레이 초기화
        _attackDelay = enemyData.attackDelay;

        // 보스 대미지 초기화
        _bossEnemyDamage = (BossEnemyDamage)enemyDamage;
        _bossEnemyDamage.IsInvincibled = true;
        _bossEnemyDamage.KnockBack = null;
        _bossEnemyDamage.Died = null;
        _bossEnemyDamage.phaseChangedEvent += OnPhaseChanged;
        _bossEnemyDamage.KnockBack += OnKnockedBack;
        _bossEnemyDamage.Died += OnDied;

        _playerTransform = GameObject.Find("Player").GetComponent<Transform>();
    }

    void Update()
    {
        // 사망한 상태일 경우 실행하지 않음
        if (isDead) return;

        // Time.deltaTime 캐시
        deltaTime = Time.deltaTime;

        if (!_isDetected)
        {
            // 아직 플레이어를 발견하지 않은 상태일 경우 플레이어가 일정범위 안으로 들어오기 전 까지 대기
            // 감지 범위 안으로 들어오면 전투 시작
            float distance = (_playerTransform.position - actorTransform.position).sqrMagnitude;
            if (distance < Mathf.Pow(enemyData.detectRange, 2))
            {
                StartCoroutine(BattleStart());
            }
        }
        else if (_isBattleStarted)
        {
            // 전투가 시작된 상태일 경우 실시간으로 공격 처리
            if (!_isAttacking)
            {
                _attackDelay -= deltaTime;

                if (_attackDelay <= 0)
                {
                    StartAttack();
                }
            }
        }
    }

    /// <summary>
    /// 보스 전투를 시작하기 위한 코루틴입니다.
    /// </summary>
    IEnumerator BattleStart()
    {
        _isDetected = true;

        // 보스 체력바 활성화
        bossHealthBarUI.SetActive(true);

        // 투명 벽 활성화
        foreach (var obj in invisibleWall)
        {
            obj.SetActive(true);
        }

        // 현재 음악을 이전 음악에 담은 뒤 전투 음악 재생 
        _prevMusic = SoundManager.instance.GetCurrentMusic();
        SoundManager.instance.MusicPlay(battleMusic);

        // 보스가 중심 좌표에 올 때까지 대기, 이후 0.5초 대기
        while (actorTransform.position != centerMovePos.position)
        {
            actorTransform.position = Vector3.MoveTowards(actorTransform.position, centerMovePos.position, 4f * deltaTime);
            yield return null;
        }
        yield return YieldInstructionCache.WaitForSeconds(0.5f);

        // 등장 애니메이션 재생
        _sprite.color = new Color32(255, 255, 255, 255);
        animator.SetTrigger(GetAnimationHash("Reappear"));

        yield return null;
        float time = animator.GetCurrentAnimatorClipInfo(0).Length;
        yield return YieldInstructionCache.WaitForSeconds(time);

        _isBattleStarted = true;
        _bossEnemyDamage.IsInvincibled = false;
    }


    /// <summary>
    /// 보스의 공격을 실행하는 메소드
    /// 매번 다른 공격을 하기 위해 현재 공격의 index값을 증가시킴
    /// </summary>
    void StartAttack()
    {
        _currentAttackIndex++;
        if (_currentAttackIndex == attackName.Count)
        {
            _currentAttackIndex = 0;
        }
        string nextAttack = attackName[_currentAttackIndex];
        if (nextAttack != null)
        {
            _isAttacking = true;
            _attackCoroutine = StartCoroutine(nextAttack);
        }
    }

    /// <summary>
    /// 보스가 지정한 좌표로 이동하는 메소드
    /// </summary>
    IEnumerator Move()
    {
        // 무적 상태로 전환
        _bossEnemyDamage.IsInvincibled = true;

        // 보스가 뒤쪽으로 사라지는 애니메이션 실행 후 1프레임 대기
        animator.SetTrigger(GetAnimationHash("Disappear"));
        yield return null;

        // 애니메이션의 재생 시간만큼 대기
        float time = animator.GetCurrentAnimatorClipInfo(0).Length;
        yield return YieldInstructionCache.WaitForSeconds(time);

        // 이동할 좌표 리스트를 가져온 뒤, 현재 보스 위치를 제외한 무작위 좌표 선정
        List<Transform> nextMovePos = movePos.ToList();
        nextMovePos.Remove(_currentMovePos);
        int movePosIndex = Random.Range(0, nextMovePos.Count);
        
        // 목표 좌표로 이동
        while(actorTransform.position != nextMovePos[movePosIndex].position)
        {
            actorTransform.position = Vector3.MoveTowards(actorTransform.position, nextMovePos[movePosIndex].position, enemyData.patrolSpeed * deltaTime);
            yield return null;
        }
        _currentMovePos = nextMovePos[movePosIndex];

        // 약간의 대기 이후 다시 나타나는 애니메이션 실행
        yield return YieldInstructionCache.WaitForSeconds(0.05f);
        animator.SetTrigger(GetAnimationHash("Reappear"));
        yield return null;
        time = animator.GetCurrentAnimatorClipInfo(0).Length;
        yield return YieldInstructionCache.WaitForSeconds(time);
        
        // 보스 무적 상태 해제 및 공격하지 않는 상태로 변경
        _bossEnemyDamage.IsInvincibled = false;
        _isAttacking = false;
        _attackDelay = enemyData.attackDelay;
    }

    /// <summary>
    /// 드론이 공격을 실행하게 하는 코루틴
    /// </summary>
    IEnumerator DroneAttack()
    {
        // 얼만큼 공격하고 자리를 옮길지 무작위로 선정(최소 2 ~ 최대 3)
        int repeatCount = Random.Range(2, 4);

        for (int i = 0; i < repeatCount; i++)
        {
            // 현재 페이즈 설정(_phase를 그대로 사용하면 중간에 페이즈가 변할 때 인덱스 범위가 벗어나는 에러 발생)
            int currentPhase = _phase;
            // 현재 페이즈 만큼 드론 어택 실행
            Coroutine[] droneAttackCoroutine = new Coroutine[currentPhase];

            // 드론을 설치한 후 0.5초 대기
            yield return StartCoroutine(DroneSetup(currentPhase));
            yield return YieldInstructionCache.WaitForSeconds(0.5f);

            // 드론의 스프라이트에 따라 드론의 공격 방식이 달라짐
            for (int j = 0; j < currentPhase; j++)
            {
                if (delegateDrones[j].spriteRenderer.sprite == droneSprite[0])
                {
                    droneAttackCoroutine[j] = StartCoroutine(TargetMissile(delegateDrones[j].gameObject.transform));
                }
                else if (delegateDrones[j].spriteRenderer.sprite == droneSprite[1])
                {
                    droneAttackCoroutine[j] = StartCoroutine(RadiationBulletFast(delegateDrones[j].gameObject.transform));
                }
                else
                {
                    droneAttackCoroutine[j] = StartCoroutine(RadiationBulletSlow(delegateDrones[j].gameObject.transform));
                }
            }

            // 2초 대기 후 드론 공격 중단
            yield return YieldInstructionCache.WaitForSeconds(2f);
            for (int j = 0; j < droneAttackCoroutine.Length; j++)
            {
                StopCoroutine(droneAttackCoroutine[j]);
            }

            // 0.5초 대기 후 드론이 보스의 위치로 되돌아감
            yield return YieldInstructionCache.WaitForSeconds(0.5f);
            yield return StartCoroutine(DroneReturn(currentPhase));

            // 1프레임 대기 후 드론의 위치를 화면 밖으로 옮김
            yield return null;
            for (int j = 0; j < delegateDrones.Length; j++)
            {
                delegateDrones[j].gameObject.transform.position = new Vector3(-1000, -1000, 0);
            }
        }

        // 공격 종료
        _attackDelay = enemyData.attackDelay;
        _isAttacking = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="phase"></param>
    /// <returns></returns>
    IEnumerator DroneSetup(int phase)
    {
        // 드론이 설치될 최소 X좌표, 최대 X좌표
        const float minX = 5;
        const float maxX = 12;

        List<Transform> dronsTransform = new List<Transform>();
        List<Vector3> movePos = new List<Vector3>();

        // 현재 페이즈 만큼의 드론 설치
        for (int i = 0; i < phase; i++)
        {
            // 드론을 보스 좌표에 옮겨온 뒤, 스프라이트로 어떤 공격을 하는 드론인지 선정 
            Vector3 pos = actorTransform.position;
            pos.z += 1;
            delegateDrones[i].gameObject.transform.position = pos;
            delegateDrones[i].spriteRenderer.sprite = droneSprite[Random.Range(0, droneSprite.Length)];
            dronsTransform.Add(delegateDrones[i].gameObject.transform);

            // 드론 좌표 무작위 선정(보스의 위치에 따라 생성 위치 달라짐)
            Vector3 randomPos;
            if (_currentMovePos == topLeftMovePos || _currentMovePos == bottomLeftMovePos)
            {
                randomPos.x = Random.Range(actorTransform.position.x + minX, actorTransform.position.x + maxX);
            }
            else if (_currentMovePos == topRightMovePos || _currentMovePos == bottomRightMovePos)
            {
                randomPos.x = Random.Range(actorTransform.position.x - minX, actorTransform.position.x - maxX);
            }
            else
            {
                randomPos.x = Random.Range(actorTransform.position.x - maxX,actorTransform.position.x + maxX);
                if (randomPos.x < actorTransform.position.x && randomPos.x > actorTransform.position.x - minX)
                {
                    randomPos.x = actorTransform.position.x - minX;
                }
                else if (randomPos.x >= actorTransform.position.x && randomPos.x < actorTransform.position.x + minX)
                {
                    randomPos.x = actorTransform.position.x + minX;
                }
            }
            randomPos.y = Random.Range(actorTransform.position.y - 1.5f,
                                       actorTransform.position.y + 4);
            // 드론이 보스와 가깝게 배치되지 않게 조정
            if (randomPos.y < actorTransform.position.y && randomPos.y > actorTransform.position.y - 2)
            {
                randomPos.y = actorTransform.position.y - 2;
            }
            else if (randomPos.y >= actorTransform.position.y && randomPos.y < actorTransform.position.y + 2)
            {
                randomPos.y = actorTransform.position.y + 2;
            }
            randomPos.z = actorTransform.position.z + 1;

            // 두번째 드론이 첫번째 드론과 비슷한 위치에 있지 않게 조정
            if(i > 0)
            {
                if(randomPos.x < movePos[0].x && randomPos.x > movePos[0].x - 3)
                {
                    randomPos.x = movePos[0].x - 3;
                }
                else if (randomPos.x >= movePos[0].x && randomPos.x < movePos[0].x + 3)
                {
                    randomPos.x = movePos[0].x + 3;
                }

                if (randomPos.y < movePos[0].y && randomPos.y > movePos[0].y - 3)
                {
                    randomPos.y = movePos[0].y - 3;
                }
                else if (randomPos.y >= movePos[0].y && randomPos.y < movePos[0].y + 3)
                {
                    randomPos.y = movePos[0].y + 3;
                }
            }

            movePos.Add(randomPos);
        }

        // 모든 드론이 목적지에 도달할 때 까지 대기
        bool[] droneArrival = new bool[_phase];
        while(true)
        {
            for(int i = 0; i < dronsTransform.Count; i++)
            {
                if(dronsTransform[i].position != movePos[i])
                {
                    dronsTransform[i].position = Vector3.MoveTowards(dronsTransform[i].position, 
                                                            movePos[i], 
                                                            19.0f * deltaTime);
                }
                else droneArrival[i] = true;
            }

            if(droneArrival.Length == 1)
            {
                if (droneArrival[0] == true) yield break;
            }
            else
            {
                if (droneArrival[0] == true || droneArrival[1] == true) 
                {
                    yield break;
                }
            }

            yield return null;
        }
    }

    /// <summary>
    /// 드론이 플레이어 방향으로 탄을 발사하는 공격을 하는 코루틴
    /// </summary>
    /// <param name="droneTransform">공격하려는 드론</param>
    IEnumerator TargetMissile(Transform droneTransform)
    {
        while (!isDead)
        {
            // 공격 방향을 플레이어 위치로 선정
            Vector2 firePos = droneTransform.position;
            float x = firePos.x - _playerTransform.position.x;
            float y = firePos.y - _playerTransform.position.y;
            float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

            ObjectPoolManager.instance.GetPoolObject("TargetMissile", droneTransform.position, 1, angle);
            SoundManager.instance.SoundEffectPlay(targetMissileSound);

            // 0.375초 마다 공격
            yield return YieldInstructionCache.WaitForSeconds(0.375f);
        }
    }

    /// <summary>
    /// 드론이 사방으로 빠르게 탄을 흩부리는 공격을 하는 코루틴
    /// </summary>
    /// <param name="droneTransform">공격하려는 드론</param>
    IEnumerator RadiationBulletFast(Transform droneTransform)
    {
        // 첫 공격 방향을 플레이어의 위치로 선정
        Vector2 firePos = droneTransform.position;
        float x = firePos.x - _playerTransform.position.x;
        float y = firePos.y - _playerTransform.position.y;
        float currentAngle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

        float time = 0;
        const float Delay = 0.175f;
        while (!isDead)
        {
            // 4 방향으로 탄환 4개 발사
            for (int i = 0; i < 4; i++)
            {
                float bulletAngle = currentAngle + (90 * i);
                ObjectPoolManager.instance.GetPoolObject("RadiationBulletFast", droneTransform.position, 1, bulletAngle);
            }
            // 사운드 재생
            SoundManager.instance.SoundEffectPlay(radiationBulletSound);
            
            // 0.6초 이후부터 발사 각도가 10도씩 돌아감
            if (time >= 0.6f)
            {
                currentAngle += 10f;
            }
            else time += Delay;

            // 0.175초마다 공격
            yield return YieldInstructionCache.WaitForSeconds(Delay);
        }
    }

    /// <summary>
    /// 드론이 사방으로 느리게 탄을 흩부리는 공격을 하는 코루틴
    /// </summary>
    /// <param name="droneTransform">공격하려는 드론</param>
    IEnumerator RadiationBulletSlow(Transform droneTransform)
    {
        // 첫 공격 방향을 플레이어의 위치로 선정
        Vector2 firePos = droneTransform.position;
        float x = firePos.x - _playerTransform.position.x;
        float y = firePos.y - _playerTransform.position.y;
        float currentAngle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

        while (!isDead)
        {
            // 4 방향으로 탄환 4개 발사
            for (int i = 0; i < 4; i++)
            {
                float bulletAngle = currentAngle + (90 * i);
                ObjectPoolManager.instance.GetPoolObject("RadiationBulletSlow", droneTransform.position, 1, bulletAngle);
            }
            // 사운드 재생
            SoundManager.instance.SoundEffectPlay(radiationBulletSound);
            // 공격을 할 때마다 발사 각도가 30도씩 돌아감
            currentAngle += 30f;
            // 0.3초마다 공격
            yield return YieldInstructionCache.WaitForSeconds(0.3f);
        }
    }

    /// <summary>
    /// 공격하던 드론을 다시 회수하는 코루틴
    /// </summary>
    IEnumerator DroneReturn(int phase)
    {
        List<Transform> dronsTransform = new List<Transform>();
        for (int i = 0; i < phase; i++)
        {
            dronsTransform.Add(delegateDrones[i].gameObject.transform);
        }

        // 돌아가려는 좌표를 보스 위치로 선정
        Vector3 returnTarget = actorTransform.position;
        returnTarget.z = dronsTransform[0].position.z;

        // 모든 드론이 목적지에 도달할 때 까지 대기
        bool[] droneArrival = new bool[phase];
        while (true)
        {
            for (int i = 0; i < dronsTransform.Count; i++)
            {
                if (dronsTransform[i].position != returnTarget)
                {
                    dronsTransform[i].position = Vector3.MoveTowards(dronsTransform[i].position, returnTarget, 24f * deltaTime);
                }
                else
                {
                    droneArrival[i] = true;
                }
            }

            if (droneArrival.Length == 1)
            {
                if (droneArrival[0] == true) yield break;
            }
            else
            {
                if (droneArrival[0] == true || droneArrival[1] == true)
                {
                    yield break;
                }
            }

            yield return null;
        } 
    }

    /// <summary>
    /// 보스의 사망을 처리하는 코루틴입니다.
    /// </summary>
    protected override IEnumerator OnDied()
    {
        isDead = true;

        // 공격 코루틴을 중단하고 Idle 상태로 변경
        StopCoroutine(_attackCoroutine);
        animator.SetTrigger(GetAnimationHash("Idle"));

        // 화면 흔들기 및 불릿타임 실행
        ScreenEffect.instance.ShakeEffectStart(0.15f, 0.5f);
        ScreenEffect.instance.BulletTimeStart(0f, 1.0f);

        // 호출한 드론을 전부 화면 밖으로 이동
        foreach(var delegateDrone in delegateDrones)
        {
            delegateDrone.gameObject.transform.position = new Vector2(-1000, -1000);
        }

        // 1프레임 대기
        yield return null;

        // 회복 아이템 2개 드롭
        Vector2 position = new Vector2(transform.position.x, transform.position.y + 2);
        for (int i = 0; i < 2; i++)
        {
            ObjectPoolManager.instance.GetPoolObject("HealingPieceDefinitelyDrop", position);
        }

        // 죽은 보스 목록에 추가
        DeadEnemyManager.AddDeadBoss(keyName);

        // 체력바 비활성화
        bossHealthBarUI.SetActive(false);

        // 보스 음악을 중단하고 다시 이전에 재생하던 음악으로 변경
        SoundManager.instance.MusicPlay(_prevMusic);

        // 보스의 눈을 서서히 안 보이게 함
        while (eyeSprite.color.a > 0)
        {
            eyeSprite.color = new Color(255, 255, 255, eyeSprite.color.a - 0.1f);
            yield return YieldInstructionCache.WaitForSeconds(0.1f);
        }

        // 1초 대기
        yield return YieldInstructionCache.WaitForSeconds(1.0f);

        // 보스가 서서히 내려가면서 화면 밑으로 사라짐
        float fallingDuration = 3.0f;
        float fallSpeed = 2.0f;
        while (fallingDuration > 0)
        {
            actorTransform.Translate(fallSpeed * Vector2.down * deltaTime);
            fallSpeed += deltaTime * 7;
            fallingDuration -= deltaTime;
            yield return null;
        }

        // 벽 비활성화
        foreach (var obj in invisibleWall)
        {
            obj.SetActive(false);
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// 보스의 페이즈가 변경될 때 호출하는 메소드입니다.
    /// </summary>
    void OnPhaseChanged()
    {
        // 다음 페이즈로 변경
        _phase++;

        // 플레이어의 체력에 따라 회복 아이템 드롭
        float playerDamage = GameObject.Find("Player").GetComponent<PlayerDamage>().GetHealthPercent();
        int healingPieceCount = playerDamage <= 0.5f ? 2 :
                                playerDamage < 1f ? 1 : 0;
        for (int i = 0; i < healingPieceCount; i++)
        {
            Vector2 position = new Vector2(transform.position.x, transform.position.y + 2);
            ObjectPoolManager.instance.GetPoolObject("HealingPieceDefinitelyDrop", position);
        }
    }
}