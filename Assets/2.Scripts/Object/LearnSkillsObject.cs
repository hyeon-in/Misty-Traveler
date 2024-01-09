using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스킬 학습 오브젝트의 기능을 처리하는 클래스입니다.
/// </summary>
/// <remarks>플레이어가 접촉시 플레이어에게 스킬을 학습시키는 기능을 가지고 있는 오브젝트입니다.</remarks>
public class LearnSkillsObject : MonoBehaviour
{
    // 학습 하는 스킬 종류
    public enum Skills
    {
        ClimbingWall,
        DoubleJump
    }
    public SkillLearnEffect skillLearnEffect;   // 스킬을 학습했다는 UI를 표시하는 클래스
    public Skills skill;    // 스킬 종류
    public float radius;    // 충돌 크기(radius)

    Animator _anim;
    LayerMask _playerLayer;

    void Awake()
    {
        // 이미 학습한 스킬이면 오브젝트 제거 
        switch(skill)
        {
            case Skills.ClimbingWall:
                if(PlayerLearnedSkills.hasLearnedClimbingWall)
                {
                    Destroy(gameObject);
                    return;
                }
                break;
            case Skills.DoubleJump:
                if(PlayerLearnedSkills.hasLearnedDoubleJump)
                {
                    Destroy(gameObject);
                    return;
                }
                break;
        }
        _playerLayer = LayerMask.GetMask("Player");
        _anim = GetComponent<Animator>();
        _anim.SetTrigger(skill.ToString());
    }

    void Update()
    {
        Vector2 pos = transform.position;
        pos.y += 1.25f;
        // 플레이어가 오브젝트와 충돌시 스킬 학습
        if(Physics2D.OverlapCircle(pos, radius, _playerLayer))
        {
            _anim.SetTrigger("Empty");
            switch (skill)
            {
                case Skills.ClimbingWall:
                    PlayerLearnedSkills.hasLearnedClimbingWall = true;
                    skillLearnEffect.SkillLearnEffectStart("Climb");
                    break;
                case Skills.DoubleJump:
                    PlayerLearnedSkills.hasLearnedDoubleJump = true;
                    skillLearnEffect.SkillLearnEffectStart("DoubleJump");
                    break;
            }
            
            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector2 pos = transform.position;
        pos.y += 1.25f;
        Gizmos.DrawWireSphere(pos, radius);
    }
}