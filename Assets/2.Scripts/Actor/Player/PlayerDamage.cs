using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어 캐릭터의 체력 설정 및 대미지를 입는 기능, 대미지를 입었을 때의 이펙트를 처리하는 클래스입니다.
/// </summary>
public class PlayerDamage : MonoBehaviour, IActorDamage
{
    readonly int hashDamage = Animator.StringToHash("Damage");
    readonly int hashEmpty = Animator.StringToHash("Empty");
    readonly int hashRecovery = Animator.StringToHash("Recovery");

    public int maxHealth = 4;       // 플레이어 최대 체력
    public AudioClip _damageSound;  // 대미지 입었을 때 재생되는 사운드

    int _currentHealth;             // 현재 체력

    /// <summary>
    /// 플레이어 캐릭터의 체력 UI 정보를 담는 구조체입니다.
    /// </summary>
    [System.Serializable]
    struct HealthUI
    {
        [HideInInspector] public Image image;       // 체력 UI 이미지
        [HideInInspector] public Animator animator; // 체력 UI 애니메이터
    }
    HealthUI[] _healthUI;

    // 대미지 이펙트 객체
    [Header("Damage Effect")]
    [SerializeField] float _screenStopDuration = 0.15f;     // 화면 정지 시간
    [SerializeField] float _screenEffectMagnitude = 0.15f;  // 화면이 흔들리는 정도
    [SerializeField] float _screenEffectDuration = 0.25f;   // 화면이 흔들리는 시간
    [SerializeField] Material _blinkMaterial;   // 플레이어 캐릭터가 깜빡이는 이펙트를 실행하기 위한 머테리얼
    [SerializeField] float _blinkDelay = 0.2f;  // 깜빡이는 간격

    [Space(10)]

    [SerializeField] float _invincibleTime = 1.5f;  // 무적 시간

    bool _isDead;           // 죽은 상태인지 체크
    bool _isInvincibleing;  // 무적 상태인지 체크

    SpriteRenderer _spriteRenderer; // 스프라이트 렌더러
    Material _defaultMaterial;      // 기본 머테리얼

    // 적 넉백
    public IActorDamage.KnockbackEventHandler KnockBack;
    // 적 사망시 처리
    public IActorDamage.DiedEventHandler Died;

    /// <summary>
    /// 플레이어의 현재 체력에 대한 프로퍼티입니다.
    /// </summary>
    public int CurrentHealth
    {
        get => _currentHealth;
        set
        {
            // 초기화 할 때만 사용
            _currentHealth = value;
            for (int i = _currentHealth; i < maxHealth; i++)
            {
                _healthUI[i].animator.SetTrigger(hashEmpty);
            }
            GameManager.instance.playerCurrentHealth = value;
        }
    }

    /// <summary>
    /// 플레이어가 회피중인 상태인지에 대한 프로퍼티입니다.
    /// true일 경우 적의 공격에 맞지 않습니다.
    /// </summary>
    public bool IsDodged { get; set; }

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _currentHealth = maxHealth;
        _defaultMaterial = _spriteRenderer.material;

        // 체력 UI 초기화
        GameObject healthUI = GameObject.Find("Health");
        int healthCount = healthUI.transform.childCount;
        _healthUI = new HealthUI[healthCount];
        for (int i = 0; i < _healthUI.Length; i++)
        {
            Transform health = healthUI.transform.GetChild(i);

            _healthUI[i].image = health.GetComponent<Image>();
            _healthUI[i].animator = health.GetComponent<Animator>();
        }
    }

    /// <summary>
    /// 플레이어의 현재 체력의 퍼센트 수치를 반환하는 메소드입니다.
    /// </summary>
    /// <returns>플레이어의 현재 체력의 퍼센트 수치</returns>
    public float GetHealthPercent() => _currentHealth / (float)maxHealth;

    /// <summary>
    /// 플레이어의 체력을 회복하는 메소드입니다.
    /// </summary>
    /// <param name="heal">체력 회복량</param>
    public void HealthRecovery(int heal)
    {
        int prevHealth = _currentHealth;
        _currentHealth += heal;
        
        if(_currentHealth > maxHealth)
        {
            _currentHealth = maxHealth;
        }

        // UI 업데이트
        for (int i = prevHealth; i < _currentHealth; i++)
        {
            _healthUI[i].animator.SetTrigger(hashRecovery);
        }
        
        // 게임매니저에 현재 체력 업데이트
        GameManager.instance.playerCurrentHealth = _currentHealth;
    }

    /// <summary>
    /// 적이 플레이어에게 대미지를 입히고 넉백을 처리하기 위한 메소드입니다.
    /// </summary>
    /// <param name="damage">대미지</param>
    /// <param name="knockBack">넉백 구조체</param>
    public void TakeDamage(int damage, KnockBack knockBack)
    {
        // 다음 상태일 경우 실행하지 않음(무적 상태, 죽은 상태, 회피 중)
        if(_isInvincibleing || _isDead || IsDodged) return;

        // 현재 체력에 대미지 적용
        int prevHealth = _currentHealth;
        _currentHealth -= damage;
        GameManager.instance.playerCurrentHealth = _currentHealth;

        // 현재 체력이 0이하이면 사망 처리 
        if(_currentHealth <= 0)
        {
            StartCoroutine(Died());
            _isDead = true;
            _currentHealth = 0;
        }

        // 체력 UI에 대미지 적용
        for(int i = _currentHealth; i < prevHealth; i++)
        {
            _healthUI[i].animator.SetTrigger(hashDamage);
        }

        // 넉백 델리게이트 처리
        KnockBack(knockBack);

        // 사운드 재생 및 스크린 이펙트 실행
        SoundManager.instance.SoundEffectPlay(_damageSound);
        ScreenEffect.instance.BulletTimeStart(0f, _screenStopDuration);
        ScreenEffect.instance.ShakeEffectStart(_screenEffectMagnitude, _screenEffectDuration);

        // 무적 상태 실행
        _isInvincibleing = true;
        StartCoroutine(DamageEffect());
    }

    /// <summary>
    /// 대미지 이펙트를 처리하는 코루틴입니다.
    /// </summary>
    IEnumerator DamageEffect()
    {
        // 깜빡이는 이펙트 코루틴 실행
        StartCoroutine("BlinkEffect");

        // 무적 시간이 끝날 때 까지 대기
        yield return YieldInstructionCache.WaitForSeconds(_invincibleTime);

        // 깜빡이는 이펙트 코루틴 중단 후 기본 머테리얼로 변경
        StopCoroutine("BlinkEffect");
        _spriteRenderer.material = _defaultMaterial;

        // 무적 상태 종료
        _isInvincibleing = false;
    }

    /// <summary>
    /// 플레이어의 스프라이트를 깜빡거리게 하는 이펙트를 처리하는 코루틴입니다.
    /// </summary>
    IEnumerator BlinkEffect()
    {
        while(true)
        {
            _spriteRenderer.material = _blinkMaterial;
            yield return YieldInstructionCache.WaitForSeconds(_blinkDelay);

            _spriteRenderer.material = _defaultMaterial;
            yield return YieldInstructionCache.WaitForSeconds(_blinkDelay);
        }
    }
}