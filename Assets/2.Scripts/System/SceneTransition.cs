using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 페이드인 및 페이드아웃 효과를 사용하여 장면 전환을 관리하는 싱글톤 클래스입니다.
/// </summary>
public class SceneTransition : MonoBehaviour
{
    // 싱글톤 패턴을 위한 인스턴스 변수
    public static SceneTransition instance = null;

    // 페이드 효과의 속도
    const float FadeSpeed = 0.08f;

    bool isFadingIn; // 페이드인 중인지 여부를 나타내는 변수

    // 페이드 효과에 사용될 Image 및 색상 변수
    Image _fadeEffectImage;
    Color _fadeEffectColor;

    void Awake()
    {
        // 싱글톤 인스턴스 설정
        instance = this;

        // 초기화
        _fadeEffectImage = GetComponent<Image>();
        _fadeEffectColor = _fadeEffectImage.color;

        // 게임 시작 시 페이드 인 코루틴 실행
        StartCoroutine(FadeIn());
    }

    /// <summary>
    /// 변경할 씬을 로드하는 메소드입니다.
    /// 페이드 이펙트를 실행하고 다음 씬을 로드하는 코루틴 실행
    /// </summary>
    /// <param name="nextScene">불러오려는 씬의 이름</param>
    public void LoadScene(string nextScene)
    {
        StartCoroutine(FadeEffect(nextScene));
    }

    /// <summary>
    /// 페이드 이펙트를 실행하고 다음 씬을 로드하는 코루틴입니다.
    /// </summary>
    /// <param name="nextScene">불러오려는 씬의 이름</param>
    IEnumerator FadeEffect(string nextScene)
    {
        yield return StartCoroutine(FadeOut());
        LoadingSceneManager.LoadScene(nextScene);
    }

    /// <summary>
    /// 페이드 인 이펙트를 적용하는 코루틴입니다.
    /// </summary>
    IEnumerator FadeIn()
    {

        isFadingIn = true; // 페이드 인 중으로 설정

        float r = _fadeEffectColor.r;
        float g = _fadeEffectColor.g;
        float b = _fadeEffectColor.b;

        float alpha = 1;
        _fadeEffectImage.color = new Color(r, g, b, alpha);

        yield return null;

        // 알파 값이 0이 될 때까지 페이드 아웃 실행
        while(alpha > 0f)
        {
            alpha -= FadeSpeed;
            _fadeEffectImage.color = new Color(r, g, b, alpha);
            yield return YieldInstructionCache.WaitForSecondsRealtime(0.01f);
        }

        isFadingIn = false; // 페이드 인 종료
    }

    /// <summary>
    /// 페이드 아웃 이펙트를 적용하는 코루틴입니다.
    /// </summary>
    IEnumerator FadeOut()
    {
        float r = _fadeEffectColor.r;
        float g = _fadeEffectColor.g;
        float b = _fadeEffectColor.b;

        // 페이드 인 중이라면 대기
        while(isFadingIn)
        {
            yield return null;
        }

        float alpha = 0;
        // 알파 값이 1이 될 때까지 페이드 인 실행
        while (alpha < 1f)
        {
            alpha += FadeSpeed;
            _fadeEffectImage.color = new Color(r, g, b, alpha);

            yield return YieldInstructionCache.WaitForSecondsRealtime(0.01f);
        }

        // 0.07초간 대기한 후 페이드 아웃 종료
        yield return YieldInstructionCache.WaitForSecondsRealtime(0.07f);
    }
}
