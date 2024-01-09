using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 현재 스테이지의 음악을 재생할 때 사용하는 클래스입니다.
/// </summary>
public class StageMusic : MonoBehaviour
{
    public AudioClip stageMusic;    // 스테이지에서 재생하려는 음악
    public float volume = 1.0f;     // 음악 볼륨

    void Start()
    {
        // 스테이지에서 재생하려는 음악이 없으면 음악 재생을 중단
        if(stageMusic == null)
        {
            SoundManager.instance.MusicStop();
        }
        SoundManager.instance.MusicPlay(stageMusic, volume);    // 음악 재생
    }
}
