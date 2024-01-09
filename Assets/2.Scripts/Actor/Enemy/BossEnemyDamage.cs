using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������ ü�� �� ������� ���õ� ����� �����ϱ� ���� Ŭ�����Դϴ�.
/// </summary>
public class BossEnemyDamage : EnemyDamage
{
    public Image bossHelathUI; // ���� ü�� UI
    public List<float> nextPhaseHealthPercent = new List<float>(); // ����� ��ȯ�Ǵ� ü�� ����Ʈ(�ۼ�Ʈ)

    // ����� ��ȯ�� �� ����Ǵ� ��������Ʈ
    public delegate void PhaseChangedEventHandler();
    public PhaseChangedEventHandler phaseChangedEvent;

    Coroutine damageUIEffect = null;

    protected override void Start()
    {
        base.Start();
        nextPhaseHealthPercent.Sort(); // ü�� �ۼ�Ʈ�� ���ĵǾ����� ���� ��Ȳ�� ���� ����ؼ� ����
    }

    /// <summary>
    /// �÷��̾ ������ ������� ������ �˹��� ó���ϱ� ���� �޼ҵ��Դϴ�.
    /// </summary>
    /// <param name="damage">���ط�</param>
    /// <param name="knockBack">�˹� ����ü</param>
    public override void TakeDamage(int damage, KnockBack knockBack)
    {
        base.TakeDamage(damage,knockBack);

        // ����� UI ����Ʈ �ڷ�ƾ ����
        if (damageUIEffect != null)
        {
            StopCoroutine(damageUIEffect);
            damageUIEffect = null;
        }
        damageUIEffect = StartCoroutine(DamageUIEffect());

        //ü���� ���� ����� ����� �ۼ�Ʈ ��ġ ���ϸ� ���� ������� �Ѿ
        if (nextPhaseHealthPercent.Count == 0) return;
        else if(GetHealthPercent() <= nextPhaseHealthPercent[0])
        {
            nextPhaseHealthPercent.RemoveAt(0);
            phaseChangedEvent();
        }
    }

    /// <summary>
    /// ������ ������� �Ծ��� �� UI ȿ���� �����ϴ� �ڷ�ƾ�Դϴ�.
    /// </summary>
    IEnumerator DamageUIEffect()
    {
        // ���� ü�� �ۼ�Ʈ�� ������ �� ������ ü�� ǥ�ð� �ش� �ۼ�Ʈ�� ������ �� ���� ������ ������
        float percent = GetHealthPercent();
        while(bossHelathUI.fillAmount < percent)
        {
            bossHelathUI.fillAmount -= 0.005f;
            yield return YieldInstructionCache.WaitForSeconds(0.02f);
        }
        bossHelathUI.fillAmount = percent;

        damageUIEffect = null;
    }
}
