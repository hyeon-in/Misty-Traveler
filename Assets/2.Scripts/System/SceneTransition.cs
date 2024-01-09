using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���̵��� �� ���̵�ƿ� ȿ���� ����Ͽ� ��� ��ȯ�� �����ϴ� �̱��� Ŭ�����Դϴ�.
/// </summary>
public class SceneTransition : MonoBehaviour
{
    // �̱��� ������ ���� �ν��Ͻ� ����
    public static SceneTransition instance = null;

    // ���̵� ȿ���� �ӵ�
    const float FadeSpeed = 0.08f;

    bool isFadingIn; // ���̵��� ������ ���θ� ��Ÿ���� ����

    // ���̵� ȿ���� ���� Image �� ���� ����
    Image _fadeEffectImage;
    Color _fadeEffectColor;

    void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        instance = this;

        // �ʱ�ȭ
        _fadeEffectImage = GetComponent<Image>();
        _fadeEffectColor = _fadeEffectImage.color;

        // ���� ���� �� ���̵� �� �ڷ�ƾ ����
        StartCoroutine(FadeIn());
    }

    /// <summary>
    /// ������ ���� �ε��ϴ� �޼ҵ��Դϴ�.
    /// ���̵� ����Ʈ�� �����ϰ� ���� ���� �ε��ϴ� �ڷ�ƾ ����
    /// </summary>
    /// <param name="nextScene">�ҷ������� ���� �̸�</param>
    public void LoadScene(string nextScene)
    {
        StartCoroutine(FadeEffect(nextScene));
    }

    /// <summary>
    /// ���̵� ����Ʈ�� �����ϰ� ���� ���� �ε��ϴ� �ڷ�ƾ�Դϴ�.
    /// </summary>
    /// <param name="nextScene">�ҷ������� ���� �̸�</param>
    IEnumerator FadeEffect(string nextScene)
    {
        yield return StartCoroutine(FadeOut());
        LoadingSceneManager.LoadScene(nextScene);
    }

    /// <summary>
    /// ���̵� �� ����Ʈ�� �����ϴ� �ڷ�ƾ�Դϴ�.
    /// </summary>
    IEnumerator FadeIn()
    {

        isFadingIn = true; // ���̵� �� ������ ����

        float r = _fadeEffectColor.r;
        float g = _fadeEffectColor.g;
        float b = _fadeEffectColor.b;

        float alpha = 1;
        _fadeEffectImage.color = new Color(r, g, b, alpha);

        yield return null;

        // ���� ���� 0�� �� ������ ���̵� �ƿ� ����
        while(alpha > 0f)
        {
            alpha -= FadeSpeed;
            _fadeEffectImage.color = new Color(r, g, b, alpha);
            yield return YieldInstructionCache.WaitForSecondsRealtime(0.01f);
        }

        isFadingIn = false; // ���̵� �� ����
    }

    /// <summary>
    /// ���̵� �ƿ� ����Ʈ�� �����ϴ� �ڷ�ƾ�Դϴ�.
    /// </summary>
    IEnumerator FadeOut()
    {
        float r = _fadeEffectColor.r;
        float g = _fadeEffectColor.g;
        float b = _fadeEffectColor.b;

        // ���̵� �� ���̶�� ���
        while(isFadingIn)
        {
            yield return null;
        }

        float alpha = 0;
        // ���� ���� 1�� �� ������ ���̵� �� ����
        while (alpha < 1f)
        {
            alpha += FadeSpeed;
            _fadeEffectImage.color = new Color(r, g, b, alpha);

            yield return YieldInstructionCache.WaitForSecondsRealtime(0.01f);
        }

        // 0.07�ʰ� ����� �� ���̵� �ƿ� ����
        yield return YieldInstructionCache.WaitForSecondsRealtime(0.07f);
    }
}
