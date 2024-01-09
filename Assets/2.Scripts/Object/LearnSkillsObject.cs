using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ų �н� ������Ʈ�� ����� ó���ϴ� Ŭ�����Դϴ�.
/// </summary>
/// <remarks>�÷��̾ ���˽� �÷��̾�� ��ų�� �н���Ű�� ����� ������ �ִ� ������Ʈ�Դϴ�.</remarks>
public class LearnSkillsObject : MonoBehaviour
{
    // �н� �ϴ� ��ų ����
    public enum Skills
    {
        ClimbingWall,
        DoubleJump
    }
    public SkillLearnEffect skillLearnEffect;   // ��ų�� �н��ߴٴ� UI�� ǥ���ϴ� Ŭ����
    public Skills skill;    // ��ų ����
    public float radius;    // �浹 ũ��(radius)

    Animator _anim;
    LayerMask _playerLayer;

    void Awake()
    {
        // �̹� �н��� ��ų�̸� ������Ʈ ���� 
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
        // �÷��̾ ������Ʈ�� �浹�� ��ų �н�
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