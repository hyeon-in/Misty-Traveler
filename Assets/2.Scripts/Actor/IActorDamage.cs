using System.Collections;

/// <summary>
/// 캐릭터의 대미지 처리 클래스가 공통적으로 가지고 있어야 하는 메소드와 델리게이트를 가지고 있는 인터페이스입니다.
/// </summary>
public interface IActorDamage
{
    public delegate void KnockbackEventHandler(KnockBack knockBack);    // 넉백 델리게이트
    public delegate IEnumerator DiedEventHandler();                     // 사망 처리 델리게이트

    public int CurrentHealth {get; set;}    // 현재 체력 프로퍼티
    public float GetHealthPercent();        // 현재 체력의 퍼센트 수치를 가져오기 위한 메소드

    public void TakeDamage(int damage, KnockBack knockBack);    // 대미지와 넉백을 처리하기 위한 메소드
}