using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 이펙트가 끝났을 때 오브젝트 풀에 해당 오브젝트를 반환하기 위한 클래스입니다.
/// </summary>
public class ReturnSpriteEffect : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ObjectPoolManager.instance.ReturnPoolObject(animator.gameObject);
    }
}
