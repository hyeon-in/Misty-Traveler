using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// 사운드 설정에 관련된 것을 관리하는 정적 클래스입니다.
/// </summary>
public static class SoundSettingsManager
{
    public static float masterVolume = 1.0f;    // 마스터 볼륨
    public static float musicVolume = 1.0f;     // 음악 볼륨
    public static float effectsVolume = 1.0f;   // 효과음 볼륨

    /// <summary>
    /// 사운드 설정을 초기화하는 정적 메소드입니다.
    /// </summary>
    public static void Init()
    {
        // 옵션 세이브 데이터에서 볼륨 설정을 가져옴
        masterVolume = OptionsData.optionsSaveData.masterVolume;
        musicVolume = OptionsData.optionsSaveData.musicVolume;
        effectsVolume = OptionsData.optionsSaveData.effectsVolume;

        AudioListener.volume = masterVolume;    // 오디오 리스너의 볼륨을 마스터 볼륨으로 설정
    }

    /// <summary>
    /// 마스터 볼륨을 설정하는 정적 메소드입니다.
    /// </summary>
    /// <param name="increase">true일 경우 볼륨이 증가하며, false이면 볼륨이 감소합니다.</param>
    public static void SetMasterVolume(bool increase)
    {
        // 볼륨 설정
        if (increase)
        {
            if (masterVolume < 1f)
            {
                masterVolume += 0.1f;
            }
        }
        else
        {
            if (masterVolume > 0f)
            {
                masterVolume -= 0.1f;
            }
        }

        AudioListener.volume = masterVolume;    // 마스터 볼륨 적용
        OptionsData.optionsSaveData.masterVolume = masterVolume;
    }

    /// <summary>
    /// 현재 마스터 볼륨을 텍스트 형태로 가져오는 메소드입니다.
    /// </summary>
    /// <returns>볼륨을 나타내는 문자열, filled ("■")와 empty ("□") 사각형을 사용.</returns>
    public static string GetMasterVolumeToTextUI()
    {
        int volume = Mathf.RoundToInt(AudioListener.volume * 10);
        StringBuilder volumeToText = new StringBuilder();
        for (int i = 0; i < 10; i++)
        {
            if (i < volume)
            {
                volumeToText.Append("■");
            }
            else
            {
                volumeToText.Append("□");
            }
        }
        return volumeToText.ToString();
    }

    /// <summary>
    /// 음악 볼륨을 설정하는 정적 메소드입니다.
    /// </summary>
    /// <param name="increase">true일 경우 볼륨이 증가하며, false이면 볼륨이 감소합니다.</param>
    public static void SetMusicVolume(bool increase)
    {
        if(increase)
        {
            if(musicVolume < 1f)
            {
                musicVolume += 0.1f;
            }
        }
        else
        {
            if (musicVolume > 0f)
            {
                musicVolume -= 0.1f;
            }
        }

        OptionsData.optionsSaveData.musicVolume = musicVolume;
    }

    /// <summary>
    /// 현재 음악 볼륨을 텍스트 형태로 가져오는 메소드입니다.
    /// </summary>
    /// <returns>볼륨을 나타내는 문자열, filled ("■")와 empty ("□") 사각형을 사용.</returns>
    public static string GetMusicVolumeToTextUI()
    {
        int volume = Mathf.RoundToInt(musicVolume * 10);
        StringBuilder volumeToText = new StringBuilder();
        for (int i = 0; i < 10; i ++)
        {
            if(i < volume)
            {
                volumeToText.Append("■");
            }
            else
            {
                volumeToText.Append("□");
            }
        }
        return volumeToText.ToString();
    }

    /// <summary>
    /// 효과음 볼륨을 설정하는 정적 메소드입니다.
    /// </summary>
    /// <param name="increase">true일 경우 볼륨이 증가하며, false이면 볼륨이 감소합니다.</param>
    public static void SetEffectsVolume(bool increase)
    {
        if (increase)
        {
            if (effectsVolume < 1)
            {
                effectsVolume += 0.1f;
            }
        }
        else
        {
            if (effectsVolume > 0)
            {
                effectsVolume -= 0.1f;
            }
        }

        OptionsData.optionsSaveData.effectsVolume = effectsVolume;
    }

    /// <summary>
    /// 현재 효과음 볼륨을 텍스트 형태로 가져오는 메소드입니다.
    /// </summary>
    /// <returns>볼륨을 나타내는 문자열, filled ("■")와 empty ("□") 사각형을 사용.</returns>
    public static string GetEffectsVolumeToTextUI()
    {
        int volume = Mathf.RoundToInt(effectsVolume * 10);
        StringBuilder volumeToText = new StringBuilder();
        for (int i = 0; i < 10; i++)
        {
            if (i < volume)
            {
                volumeToText.Append("■");
            }
            else
            {
                volumeToText.Append("□");
            }
        }
        return volumeToText.ToString();
    }
}