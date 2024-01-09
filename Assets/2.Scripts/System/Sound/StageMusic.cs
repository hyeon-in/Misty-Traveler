using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ���������� ������ ����� �� ����ϴ� Ŭ�����Դϴ�.
/// </summary>
public class StageMusic : MonoBehaviour
{
    public AudioClip stageMusic;    // ������������ ����Ϸ��� ����
    public float volume = 1.0f;     // ���� ����

    void Start()
    {
        // ������������ ����Ϸ��� ������ ������ ���� ����� �ߴ�
        if(stageMusic == null)
        {
            SoundManager.instance.MusicStop();
        }
        SoundManager.instance.MusicPlay(stageMusic, volume);    // ���� ���
    }
}
