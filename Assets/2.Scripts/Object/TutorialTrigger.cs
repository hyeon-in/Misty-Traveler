using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾ ���˽� Ʃ�丮���� �����ϴ� ����� ó���ϴ� Ŭ�����Դϴ�.
/// </summary>
public class TutorialTrigger : MonoBehaviour
{
    public TutorialManager.Tutorial tutorial; // � Ʃ�丮������ Ȯ��
    public TutorialScreen tutorialScreen;     // Ʃ�丮�� ȭ�� UI
    public Vector2 size;    // �浹 ������(box)
    LayerMask _playerLayer;

    void Awake()
    {
        // �̹� �� ���� �ִ� Ʃ�丮���̸� ����
        if(TutorialManager.HasSeenTutorial(tutorial))
        {
            Destroy(gameObject);
            return;
        }
        _playerLayer = LayerMask.GetMask("Player");
    }

    void Update()
    {
        // �÷��̾ ���˽� Ʃ�丮�� ����
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