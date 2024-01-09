using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 비디오 설정 옵션을 관리하는 정적 클래스입니다.
/// </summary>
public static class VideoSettingsManager
{
    static List<Resolution> resolutions = new List<Resolution>();   // 화면 해상도 리스트
    static bool fullScreen = Screen.fullScreen; // 전체 화면 여부
    static int prevResolutionIndex;             // 이전 화면 해상도 인덱스
    static int currentResolutionIndex;          // 현재 화면 해상도 인덱스

    public static bool vsync;           // VSync 활성화 여부

    /// <summary>
    /// 비디오 옵션을 초기화하는 메소드입니다.
    /// </summary>
    public static void VideoOptionsInit()
    {
        // 화면 해상도 리스트가 비어있지 않으면 이미 초기화 된 것으로 간주하고 실행 중단
        if (resolutions.Count > 0) return;

        // 해상도 목록 초기화
        resolutions = Screen.resolutions.ToList();
        for (int i = resolutions.Count - 1; i > 0; i--)
        {
            // width와 height가 동일한 해상도를 모두 삭제
            int prevResolutionWidth = resolutions[i - 1].width;
            int prevResolutionHeight = resolutions[i - 1].height;
            int currentResolutionWidth = resolutions[i].width;
            int currentResolutionHeight = resolutions[i].height;

            if (prevResolutionWidth == currentResolutionWidth && prevResolutionHeight == currentResolutionHeight)
            {
                resolutions.RemoveAt(i);
            }
        }

        // 옵션 세이브 데이터에서 전체 화면 여부와 VSync여부를 가져옴
        Screen.fullScreen = fullScreen = OptionsData.optionsSaveData.fullScreenMode;
        vsync = OptionsData.optionsSaveData.vSync;
        QualitySettings.vSyncCount = vsync ? 1 : 0; // vsync가 true일 경우 1로 설정

        if (OptionsData.optionsSaveData.resolution == null)
        {
            // 옵션 세이브 데이터의 해상도 인덱스가 null이면 사용자의 모니터 크기로 해상도 설정
            SetResolutionToScreenSize();
        }
        else
        {
            // 옵션 세이브 데이터의 해상도 인덱스가 null이 아닐 경우
            // 옵션 세이브 데이터에서 해상도를 가져온 뒤 현재 화면 해상도로 설정
            bool isFound = false;
            Resolution optionResolution = (Resolution)OptionsData.optionsSaveData.resolution;
            for (int i = 0; i < resolutions.Count; i++)
            {
                // 해상도가 일치하는 화면의 인덱스 가져오기
                if (optionResolution.width == resolutions[i].width && optionResolution.height == resolutions[i].height)
                {
                    // 이전 해상도와 현재 해상도 모두 현재 화면의 인덱스로 설정
                    prevResolutionIndex = currentResolutionIndex = i;
                    isFound = true;
                    break;
                }
            }
            // 만약 일치하는 해상도가 없으면 사용자의 모니터 크기로 해상도 설정
            if (!isFound)
            {
                SetResolutionToScreenSize();
            }
        }

        // 인덱스를 통해 현재 화면 해상도 설정
        int newWidth = resolutions[currentResolutionIndex].width;
        int newHeight = resolutions[currentResolutionIndex].height;
        Screen.SetResolution(newWidth, newHeight, fullScreen);
    }

    /// <summary>
    /// 현재 게임의 해상도를 플레이어가 사용하는 모니터의 크기와 일치시키는 정적 메소드입니다.
    /// </summary>
    static void SetResolutionToScreenSize()
    {
        int width = Screen.width;
        int height = Screen.height;

        // 사용자의 화면과 일치하는 해상도를 현재 해상도로 설정 
        for (int i = 0; i < resolutions.Count; i++)
        {
            // 해상도가 일치하는 화면의 인덱스 가져오기
            if (width == resolutions[i].width && height == resolutions[i].height)
            {
                // 이전 해상도와 현재 해상도 모두 현재 화면의 인덱스로 설정
                prevResolutionIndex = currentResolutionIndex = i;
                break;
            }
        }

        // 현재 해상도를 옵션 세이브 데이터에 저장
        OptionsData.optionsSaveData.resolution = resolutions[currentResolutionIndex];
    }

    /// <summary>
    /// 전체 화면 여부를 설정하는 정적 메소드입니다.
    /// 현재 화면이 전체화면이면 창화면으로 전환하고, 창화면이라면 전체화면으로 전환합니다.
    /// </summary>
    public static void SetFullScreen()
    {
        fullScreen = !fullScreen;
        Screen.fullScreen = fullScreen;
        OptionsData.optionsSaveData.fullScreenMode = fullScreen; // 전체 화면 여부 옵션 세이브 데이터에 저장
    }

    /// <summary>
    /// 전체 화면이 활성화 됐는지 비활성화 됐는지를 텍스트로 가져오는 정적 메소드입니다.(설정 메뉴에서 사용)
    /// </summary>
    /// <returns>전체 화면이 활성화 됐을 경우 Enabled, 아니면 Disabled</returns>
    public static string GetFullScreenStatusText() => fullScreen ? LanguageManager.GetText("Enabled") : LanguageManager.GetText("Disabled");

    /// <summary>
    /// 화면 해상도를 설정하는 정적 메소드입니다.
    /// </summary>
    /// <param name="increase">true이면 현재 해상도 인덱스가 증가하며, false이면 감소함</param>
    public static void SetResolution(bool increase)
    {
        if (increase)
        {
            // 받아온 값이 true이면 현재 해상도 인덱스 증가
            currentResolutionIndex++;
            if (currentResolutionIndex >= resolutions.Count)
            {
                currentResolutionIndex = 0;
            }
        }
        else
        {
            // 받아온 값이 false이면 현재 해상도 인덱스 감소
            currentResolutionIndex--;
            if (currentResolutionIndex < 0)
            {
                currentResolutionIndex = resolutions.Count - 1;
            }
        }
    }

    /// <summary>
    /// 현재 화면 해상도를 Text로 가져오는 정적 메소드입니다.(설정 메뉴에서 사용)
    /// </summary>
    /// <returns>현재 화면 해상도(string)</returns>
    public static string GetCurrentResolutionText()
    {
        int width = resolutions[currentResolutionIndex].width;
        int height = resolutions[currentResolutionIndex].height;
        string resolutionToString = width + " x " + height;
        return resolutionToString;
    }

    /// <summary>
    /// 현재 해상도를 이전에 설정했단 해상도로 되돌리는 정적 메소드입니다.
    /// </summary>
    public static void ResolutionIndexReturn() => currentResolutionIndex = prevResolutionIndex;

    /// <summary>
    /// 현재 해상도를 새로운 화면 해상도로 변경하는 정적 메소드입니다.
    /// </summary>
    public static void NewResolutionAccept()
    {
        // 이전 해상도와 현재 해상도가 동일하면 실행하지 않습니다.
        if (prevResolutionIndex == currentResolutionIndex) return;

        // 화면 해상도 설정
        int width = resolutions[currentResolutionIndex].width;
        int height = resolutions[currentResolutionIndex].height;
        Screen.SetResolution(width, height, fullScreen);

        prevResolutionIndex = currentResolutionIndex;   // 이전 해상도를 현재 해상도로 설정

        OptionsData.optionsSaveData.resolution = resolutions[currentResolutionIndex];    // 옵션 세이브 데이터에 해상도 저장
    }

    /// <summary>
    /// VSync 사용 여부를 설정하는 정적 메소드입니다.
    /// 현재 VSync 사용 여부에 따라 ON/OFF 합니다
    /// </summary>
    public static void SetVSync()
    {
        vsync = !vsync;
        QualitySettings.vSyncCount = vsync ? 1 : 0; // vsync 설정
        OptionsData.optionsSaveData.vSync = vsync;  // 옵션 세이브 데이터에 해상도 저장
    }

    /// <summary>
    /// VSync가 활성화 됐는지 비활성화 됐는지를 텍스트로 가져오는 정적 메소드입니다.(설정 메뉴에서 사용)
    /// </summary>
    /// <returns>VSync가 활성화 됐을 경우 Enabled, 아니면 Disabled</returns>
    public static string GetVSyncStatusText() => vsync ? LanguageManager.GetText("Enabled") : LanguageManager.GetText("Disabled");
}