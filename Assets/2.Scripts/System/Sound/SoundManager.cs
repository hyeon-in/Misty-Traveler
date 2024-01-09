using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 사운드를 재생하고 관리하는 기능을 하는 싱글톤 클래스입니다.
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null; // 싱글톤 클래스

    [SerializeField] AudioSource _music;
    [SerializeField] AudioSource _effect;

    float currentMusicVolume;   // 현재 음악 볼륨
    AudioClip _musicClip;
    Coroutine _musicChange = null;

    void Awake()
    {
        // 싱글톤 클래스로 설정
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        if(_music == null)
        {
            _music = transform.Find("Music").GetComponent<AudioSource>();
        }
        if(_effect == null)
        {
            _effect = transform.Find("Effect").GetComponent<AudioSource>();
        }

        _music.loop = true; // 음악은 항상 loop로 설정
    }

    /// <summary>
    /// 음악을 재생할 때 사용하는 메소드입니다.
    /// 현재 재생되는 음악을 변경하는 코루틴이 실행됩니다.
    /// </summary>
    /// <param name="audioClip">재생하려는 음악</param>
    /// <param name="volume">재생하려는 음악의 볼륨</param>
    public void MusicPlay(AudioClip audioClip, float volume = 1.0f)
    {
        if(audioClip == null || _musicClip == audioClip) return;
        _musicClip = audioClip;

        // 음악이 변경되고 있었다면 기존 음악 변경 중단
        if(_musicChange != null)
        {
            StopCoroutine(_musicChange);
            _musicChange =null;
        }

        // 음악을 변경하는 코루틴
        _musicChange = StartCoroutine(MusicChange(volume));
    }

    /// <summary>
    /// 현재 재생중인 음악의 재생을 중단하는 메소드입니다.
    /// </summary>
    public void MusicStop()
    {
        // 음악이 변경되고 있었다면 기존 음악 변경 중단
        if (_musicChange != null)
        {
            StopCoroutine(_musicChange);
            _musicChange = null;
        }
        _musicClip = null;
        _musicChange = StartCoroutine(MusicChange(0));  // 음악의 볼륨을 0으로 설정
    }

    /// <summary>
    /// 현재 재생중인 음악을 가져오는 메소드입니다.
    /// </summary>
    public AudioClip GetCurrentMusic() => _music.clip;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="setVolume"></param>
    /// <returns></returns>
    IEnumerator MusicChange(float setVolume)
    {
        // 현재의 음악 볼륨을 가져옴
        currentMusicVolume = SoundSettingsManager.musicVolume;

        float volume = _music.volume;               // 이전에 재생하던 음악의 볼륨
        float volumeReduction = volume * 0.015f;    // 음악 볼륨 강소량

        // 이전에 재생하던 음악의 볼륨이 서서히 감소
        while(volume > 0f)
        {
            volume -= volumeReduction;
            _music.volume = Mathf.Clamp(volume, 0f, 1.0f);
            yield return YieldInstructionCache.WaitForSecondsRealtime(0.01f);
        }
        // 음악 중단
        _music.Stop();

        // 재생하려는 음악으로 변경한 후 재생
        _music.clip = _musicClip;
        _music.Play();
        
        // 음악 볼륨이 서서히 증가하며, 설정해놓은 음악 볼륨까지 증가함
        float volumeIncrease = setVolume * 0.015f;
        while (volume < setVolume * currentMusicVolume)
        {
            volume += volumeIncrease;
            _music.volume = Mathf.Clamp(volume, 0f, setVolume * currentMusicVolume);
            yield return YieldInstructionCache.WaitForSecondsRealtime(0.01f);
        }

        // 음악을 변경하는 도중 볼륨을 설정할 때를 대비
        if(currentMusicVolume != SoundSettingsManager.musicVolume)
        {
            _music.volume = setVolume * SoundSettingsManager.musicVolume;
        }

        _musicChange = null;
    }

    /// <summary>
    /// 효과음을 재생하는 메소드입니다.
    /// </summary>
    /// <param name="audioClip">재생하려는 효과음</param>
    public void SoundEffectPlay(AudioClip audioClip)
    {
        // 효과음은 한 번만 재생
        _effect.volume = SoundSettingsManager.effectsVolume;
        _effect.PlayOneShot(audioClip);
    }

    /// <summary>
    /// 음악 재생 중 옵션에서 볼륨을 조절했을 경우 현재 재생중인 음악의 볼륨을 다시 조절하는 메소드입니다.
    /// </summary>
    public void MusicVolumeRefresh()
    {
        // 음악이 변경되고 있었다면 기존 음악 변경 중단
        if (_musicChange != null)
        {
            StopCoroutine(_musicChange);
            _musicChange = null;
        }
        _music.volume = currentMusicVolume * SoundSettingsManager.musicVolume;
    }
}
