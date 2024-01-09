using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 내 캐릭터들의 기본 클래스로, 오브젝트 풀링과 애니메이션, 좌우 뒤집기와 같은 공통 기능을 제공합니다.
/// </summary>
public abstract class Actor : MonoBehaviour
{
    // 풀링 오브젝트 리스트
    [SerializeField] private List<PoolObjectData> _poolObjectDataList;

    protected float deltaTime;      // Time.deltaTime을 캐싱하기 위한 float 변수
    protected bool isDead;          // 캐릭터가 죽은 상태인지 체크

    protected Transform actorTransform;
    protected ActorController controller = null;

    protected Animator animator;
    protected Dictionary<string, int> animationHash = new Dictionary<string, int>();

    protected bool FacingRight { get; private set; }

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        actorTransform = GetComponent<Transform>();

        if (TryGetComponent(out ActorController controller))
        {
            this.controller = controller;
        }

        // 오브젝트 풀 초기화
        foreach (var poolObject in _poolObjectDataList)
        {
            ObjectPoolManager.instance.CreatePool(poolObject);
        }

        // 초기 방향 설정
        FacingRight = actorTransform.localScale.x > 0;
    }

    #region Animator

    /// <summary>
    /// 지정한 애니메이션의 이름의 해시 코드를 가져오고 캐시하는 메소드입니다.
    /// </summary>
    /// <param name="name">애니메이션 이름</param>
    /// <returns>애니메이션의 해시</returns>
    protected int GetAnimationHash(string name)
    {
        // 지정한 애니메이션의 hash가 없을 경우 새로 생성한 뒤 반환
        if (!animationHash.TryGetValue(name, out int hash))
        {
            animationHash.Add(name, Animator.StringToHash(name));
            hash = Animator.StringToHash(name);
        }

        return hash;
    }

    /// <summary>
    /// 현재 애니메이션의 정규화된 시간을 가져오는 메소드입니다.
    /// </summary>
    /// <returns>정규화 된 현재 애니메이션 시간</returns>
    protected float GetAnimatorNormalizedTime() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

    /// <summary>
    /// 정규화된 시간을 이용하여 현재 애니메이션이 종료되었는지 확인하는 메소드입니다.
    /// </summary>
    /// <returns>애니메이션 종료 여부</returns>
    protected bool IsAnimationEnded() => GetAnimatorNormalizedTime() >= 0.99f;

    /// <summary>
    /// 현재 애니메이션의 정규화된 시간이 지정 범위 내에 있는지 확인하는 메소드입니다.
    /// </summary>
    /// <param name="minTime">최소 시간(정규화 기준)</param>
    /// <param name="maxTime">최대 시간(정규화 기준)</param>
    /// <returns>정규화된 애니메이션 시간이 지정된 범위 내에 있는지 여부</returns>
    protected bool IsAnimatorNormalizedTimeInBetween(float minTime, float maxTime)
    {
        float normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        return normalizedTime >= minTime && normalizedTime <= maxTime;
    }

    #endregion

    #region Actor Flip

    /// <summary>
    /// 캐릭터를 좌우로 뒤집어 방향을 변경하는 메소드입니다.
    /// </summary>
    protected void Flip()
    {
        FacingRight = !FacingRight;
 
        int newScaleX = FacingRight ? 1 : -1;
        actorTransform.localScale = new Vector2(newScaleX, 1);
    }

    /// <summary>
    /// 값에 따라 캐릭터의 방향을 설정하는 메소드입니다. 값이 0보다 낮을 경우 왼쪽, 0보다 클 경우 오른쪽을 바라봅니다.
    /// </summary>
    /// <param name="direction">설정하려는 방향</param>
    protected void SetFacingDirection(float direction)
    {
        // 값이 0이면 방향을 바꾸지 않음
        if (direction != 0)
        {
            actorTransform.localScale = new Vector2(Mathf.Sign(direction), 1);
            FacingRight = actorTransform.localScale.x > 0;
        }
    }

    #endregion
}
