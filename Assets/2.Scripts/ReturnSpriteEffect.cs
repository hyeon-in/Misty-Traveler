using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����Ʈ�� ������ �� ������Ʈ Ǯ�� �ش� ������Ʈ�� ��ȯ�ϱ� ���� Ŭ�����Դϴ�.
/// </summary>
public class ReturnSpriteEffect : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ObjectPoolManager.instance.ReturnPoolObject(animator.gameObject);
    }
}
