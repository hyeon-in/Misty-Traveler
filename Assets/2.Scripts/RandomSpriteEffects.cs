using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 이펙트를 무작위로 실행하기 위한 클래스입니다.
/// </summary>
public class RandomSpriteEffects : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        animator.SetFloat("RandomEffects", Random.Range(0f,1f));
    }
}
