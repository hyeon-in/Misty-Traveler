using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 화면 흔들기, 불릿 타임, 시간 정지 등의 화면 관련 이펙트를 처리하는 싱글톤 클래스입니다.
/// </summary>
public class ScreenEffect : MonoBehaviour
{
    public static ScreenEffect instance = null; // 싱글톤

    float _magnitude;           // 화면 흔들림 강도
    float _shakeDuration;       // 화면 흔들림 지속 시간
    float _bulletTimeScale;     // 불릿 타임 TimeScale
    float _bulletTimeDuration;  // 불릿 타임 지속 시간

    bool isTimeStopping;        // 시간 정지 여부

    Vector3 _cameraOriginPos;   // 카메라 원점 좌표
    Transform _cameraTransform; // 카메라 Transform

    Coroutine _shakeEffect = null;      // 화면 흔들기 코루틴
    Coroutine _bulletTimeEffect = null; // 불릿 타임 코루틴

    /// <summary>
    /// 화면이 흔들리기 시작할 때 호출되는 델리게이트입니다.
    /// </summary>
    public delegate void StartShakeEventHandler();
    public StartShakeEventHandler StartShake;

    /// <summary>
    /// 화면 흔들기가 종료됐을 때 호출되는 델리게이트입니다.
    /// </summary>
    public delegate void EndShakeEventHandler();
    public EndShakeEventHandler EndShake;

    void Awake()
    {
        // 싱글톤 클래스로 지정
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        _cameraTransform = Camera.main.GetComponent<Transform>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 씬이 바뀌면 바뀐 씬의 카메라 Transform을 받아오고 델리게이트를 null로 변경
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _cameraTransform = Camera.main.GetComponent<Transform>();
        StartShake = null;
        EndShake = null;
    }

    /// <summary>
    /// 화면 흔들기 코루틴을 실행하는 메소드입니다.
    /// </summary>
    /// <param name="magnitude">화면 흔들기 강도</param>
    /// <param name="duration">화면 흔들기 지속시간</param>
    public void ShakeEffectStart(float magnitude, float duration)
    {
        // 접근성 옵션에서 화면 흔들기 옵션이 꺼져있으면 실행하지 않음
        if(!AccessibilitySettingsManager.screenShake) return;

        // 화면 흔들기 코루틴이 실행되고 있을 경우
        // 남은 화면 흔들기 지속 시간이 새로운 화면 흔들기의 지속시간보다 길 경우 실행 중단
        // 아니라면 기존 화면 흔들기 중단
        if(_shakeEffect != null)
        {
            if(_shakeDuration > duration) return;
            _cameraTransform.position = _cameraOriginPos;
            StopCoroutine(_shakeEffect);
            _shakeEffect = null;
        }

        // 화면 흔들기 코루틴 실행
        _magnitude = magnitude;
        _shakeDuration = duration;
        _shakeEffect = StartCoroutine(ShakeEffectCoroutine());
    }

    
    /// <summary>
    /// 화면 흔들기를 중단하는 메소드입니다.
    /// </summary>
    public void ShakeEffectStop()
    {
        if(_shakeEffect == null) return;
        _cameraTransform.position = _cameraOriginPos;
        StopCoroutine(_shakeEffect);
        EndShake();
        _shakeEffect = null;
    }

    /// <summary>
    /// 화면 흔들기 이펙트를 처리하는 코루틴입니다.
    /// </summary>
    IEnumerator ShakeEffectCoroutine()
    {
        Vector3 setPos;

        _cameraOriginPos = _cameraTransform.position;
        StartShake();   // 화면 흔들기가 시작했을 때의 델리게이트 호출

        // 화면 흔들기 처리
        while (_shakeDuration > 0)
        {
            float shakePosX = Random.Range(-1f, 1f) * _magnitude;
            float shakePosY = Random.Range(-1f, 1f) * _magnitude;

            setPos = _cameraTransform.position;
            setPos.x += shakePosX;
            setPos.y += shakePosY;
            _cameraTransform.position = setPos;

            yield return YieldInstructionCache.WaitForSecondsRealtime(0.012f);
            _shakeDuration -= 0.012f;

            _cameraTransform.position = _cameraOriginPos;

            yield return YieldInstructionCache.WaitForSecondsRealtime(0.012f);
            _shakeDuration -= 0.012f;
        }

        _cameraTransform.position = _cameraOriginPos;
        EndShake(); // 화면 흔들기가 종료됐을 때의 델리게이트 호출
        _shakeEffect = null;
    }

    /// <summary>
    /// 시간 정지를 실행하는 메소드입니다.
    /// </summary>
    public void TimeStopStart()
    {
        BulletTimeStop();
        isTimeStopping = true;
        Time.timeScale = 0;
    }

    /// <summary>
    /// 시간 정지를 중단하는 메소드입니다.
    /// </summary>
    public void TimeStopCancle()
    {
        isTimeStopping = false;
        Time.timeScale = 1;
    }

    /// <summary>
    /// 불릿 타임 이펙트 코루틴을 실행하는 메소드입니다.
    /// </summary>
    /// <param name="timeScale">설정하려는 TimeScale</param>
    /// <param name="duration">불릿 타임 지속 시간(초)</param>
    public void BulletTimeStart(float timeScale, float duration)
    {
        // 시간이 멈춘 상태이면 실행 중단
        if(isTimeStopping) return;

        // 현재 불릿 타임 코루틴 종료
        if (_bulletTimeEffect != null)
        {

            StopCoroutine(_bulletTimeEffect);
            _bulletTimeEffect = null;
        }

        // 지속 시간과 TimeScale 설정 후 코루틴 실행
        _bulletTimeDuration = duration;
        _bulletTimeScale = timeScale;
        _bulletTimeEffect = StartCoroutine(BulletTimeEffect());
    }

    /// <summary>
    /// 불릿 타임을 중단하는 메소드입니다.
    /// </summary>
    public void BulletTimeStop()
    {
        if(_bulletTimeEffect == null) return;

        StopCoroutine(_bulletTimeEffect);
        _bulletTimeEffect = null;

        Time.timeScale = 1f;
    }

    /// <summary>
    /// 시간 정지 이펙트 코루틴입니다.
    /// </summary>
    IEnumerator BulletTimeEffect()
    {
        Time.timeScale = _bulletTimeScale;
        yield return YieldInstructionCache.WaitForSecondsRealtime(_bulletTimeDuration);
        Time.timeScale = 1f;
        _bulletTimeEffect = null;
    }
}
