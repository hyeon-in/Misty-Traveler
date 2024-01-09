using System.Collections;

/// <summary>
/// ĳ������ ����� ó�� Ŭ������ ���������� ������ �־�� �ϴ� �޼ҵ�� ��������Ʈ�� ������ �ִ� �������̽��Դϴ�.
/// </summary>
public interface IActorDamage
{
    public delegate void KnockbackEventHandler(KnockBack knockBack);    // �˹� ��������Ʈ
    public delegate IEnumerator DiedEventHandler();                     // ��� ó�� ��������Ʈ

    public int CurrentHealth {get; set;}    // ���� ü�� ������Ƽ
    public float GetHealthPercent();        // ���� ü���� �ۼ�Ʈ ��ġ�� �������� ���� �޼ҵ�

    public void TakeDamage(int damage, KnockBack knockBack);    // ������� �˹��� ó���ϱ� ���� �޼ҵ�
}