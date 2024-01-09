using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 접촉시 튜토리얼을 실행하는 기능을 처리하는 클래스입니다.
/// </summary>
public class TutorialTrigger : MonoBehaviour
{
    public TutorialManager.Tutorial tutorial; // 어떤 튜토리얼인지 확인
    public TutorialScreen tutorialScreen;     // 튜토리얼 화면 UI
    public Vector2 size;    // 충돌 사이즈(box)
    LayerMask _playerLayer;

    void Awake()
    {
        // 이미 본 적이 있는 튜토리얼이면 삭제
        if(TutorialManager.HasSeenTutorial(tutorial))
        {
            Destroy(gameObject);
            return;
        }
        _playerLayer = LayerMask.GetMask("Player");
    }

    void Update()
    {
        // 플레이어가 접촉시 튜토리얼 실행
        Vector2 pos = transform.position;
        if(Physics2D.OverlapBox(pos, size, 0, _playerLayer))
        {
            TutorialManager.AddSeenTutorial(tutorial);
            tutorialScreen.TotorialStart(tutorial);
            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector2 pos = transform.position;
        Gizmos.DrawWireCube(pos, size);
    }
}