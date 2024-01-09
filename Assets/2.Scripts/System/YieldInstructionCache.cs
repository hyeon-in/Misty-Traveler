using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ڷ�ƾ ��� �ð��� ����ȭ�ϱ� ���� WaitForSeconds �� WaitForSecondsRealtime�� �ν��Ͻ��� ĳ���ϴ� ��ƿ��Ƽ Ŭ�����Դϴ�.
/// </summary>
public static class YieldInstructionCache
{
    /// <summary>
    /// float �� �񱳸� ���� IEqualityComparer�� ������ Ŭ�����Դϴ�.
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

    // WaitForSeconds�� WaitForSecondsRealtime �ν��Ͻ��� ĳ���ϴ� Dictionary
    static readonly Dictionary<float, WaitForSeconds> _timeInterval = 
                new Dictionary<float, WaitForSeconds>(new FloatComparer());

    static readonly Dictionary<float, WaitForSecondsRealtime> _realTimeInterval =
                new Dictionary<float, WaitForSecondsRealtime>(new FloatComparer());

    /// <summary>
    /// �־��� �ð���ŭ ����ϴ� WaitForSeconds �ν��Ͻ��� ��ȯ�ϴ� ���� �޼ҵ��Դϴ�.
    /// </summary>
    /// <param name="seconds">��� �ð�(��)</param>
    /// <returns>WaitForSeconds �ν��Ͻ�</returns>
    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        WaitForSeconds waitForSeconds;

        // ������ ��� �ð��� ���� �ν��Ͻ��� �����ϸ� ã�Ƽ� ��ȯ�ϰ�, ������ ���� �����Ͽ� ��ȯ�Ѵ�
        if(!_timeInterval.TryGetValue(seconds, out waitForSeconds))
        {
            _timeInterval.Add(seconds, waitForSeconds = new WaitForSeconds(seconds));
        }
        return waitForSeconds;
    }

    /// <summary>
    /// �־��� �ð���ŭ ����ϴ� WaitForSecondsRealtime �ν��Ͻ��� ��ȯ�ϴ� ���� �޼ҵ��Դϴ�.
    /// </summary>
    /// <param name="seconds">��� �ð�(��)</param>
    /// <returns>WaitForSecondsRealtime �ν��Ͻ�</returns>
    public static WaitForSecondsRealtime WaitForSecondsRealtime(float seconds)
    {
        WaitForSecondsRealtime waitForSecondsRealtime;

        // ������ ��� �ð��� ���� �ν��Ͻ��� �����ϸ� ã�Ƽ� ��ȯ�ϰ�, ������ ���� �����Ͽ� ��ȯ�Ѵ�
        if (!_realTimeInterval.TryGetValue(seconds, out waitForSecondsRealtime))
        {
            _realTimeInterval.Add(seconds, waitForSecondsRealtime = new WaitForSecondsRealtime(seconds));
        }
        return waitForSecondsRealtime;
    }
}