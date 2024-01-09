using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 코루틴 대기 시간을 최적화하기 위해 WaitForSeconds 및 WaitForSecondsRealtime의 인스턴스를 캐시하는 유틸리티 클래스입니다.
/// </summary>
public static class YieldInstructionCache
{
    /// <summary>
    /// float 값 비교를 위한 IEqualityComparer를 구현한 클래스입니다.
    /// </summary>
    class FloatComparer : IEqualityComparer<float>
    {
        bool IEqualityComparer<float>.Equals(float x, float y)
        {
            return x == y;
        }
        int IEqualityComparer<float>.GetHashCode(float obj)
        {
            return obj.GetHashCode();
        }
    }

    // WaitForSeconds와 WaitForSecondsRealtime 인스턴스를 캐시하는 Dictionary
    static readonly Dictionary<float, WaitForSeconds> _timeInterval = 
                new Dictionary<float, WaitForSeconds>(new FloatComparer());

    static readonly Dictionary<float, WaitForSecondsRealtime> _realTimeInterval =
                new Dictionary<float, WaitForSecondsRealtime>(new FloatComparer());

    /// <summary>
    /// 주어진 시간만큼 대기하는 WaitForSeconds 인스턴스를 반환하는 정적 메소드입니다.
    /// </summary>
    /// <param name="seconds">대기 시간(초)</param>
    /// <returns>WaitForSeconds 인스턴스</returns>
    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        WaitForSeconds waitForSeconds;

        // 동일한 대기 시간을 가진 인스턴스가 존재하면 찾아서 반환하고, 없으면 새로 생성하여 반환한다
        if(!_timeInterval.TryGetValue(seconds, out waitForSeconds))
        {
            _timeInterval.Add(seconds, waitForSeconds = new WaitForSeconds(seconds));
        }
        return waitForSeconds;
    }

    /// <summary>
    /// 주어진 시간만큼 대기하는 WaitForSecondsRealtime 인스턴스를 반환하는 정적 메소드입니다.
    /// </summary>
    /// <param name="seconds">대기 시간(초)</param>
    /// <returns>WaitForSecondsRealtime 인스턴스</returns>
    public static WaitForSecondsRealtime WaitForSecondsRealtime(float seconds)
    {
        WaitForSecondsRealtime waitForSecondsRealtime;

        // 동일한 대기 시간을 가진 인스턴스가 존재하면 찾아서 반환하고, 없으면 새로 생성하여 반환한다
        if (!_realTimeInterval.TryGetValue(seconds, out waitForSecondsRealtime))
        {
            _realTimeInterval.Add(seconds, waitForSecondsRealtime = new WaitForSecondsRealtime(seconds));
        }
        return waitForSecondsRealtime;
    }
}