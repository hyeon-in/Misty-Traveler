using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����Ʈ�� �������� �����ϱ� ���� Ŭ�����Դϴ�.
/// </summary>
public class RandomSpriteEffects : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        animator.SetFloat("RandomEffects", Random.Range(0f,1f));
    }
}
