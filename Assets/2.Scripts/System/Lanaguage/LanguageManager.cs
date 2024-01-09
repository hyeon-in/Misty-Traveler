using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

/// <summary>
/// 언어 관련 기능과 데이터를 관리하기 위한 정적 클래스입니다.
/// </summary>
public static class LanguageManager
{
    // 지원하는 언어 목록
    public enum Language
    {
        English, // 영어
        Korean,  // 한국어
        Last     // Language의 크기 및 언어 데이터가 설정되어있지 않음을 표시할 때 사용
    }
    public static Language currentLanguage; // 현재 사용하고 있는 언어

    static Dictionary<string, LanguageData> languageData = new Dictionary<string, LanguageData>();

    /// <summary>
    /// 언어 관리 클래스를 초기화하는 메소드입니다.
    /// </summary>
    public static void Init()
    {
        if (OptionsData.optionsSaveData.language != (int)Language.Last)
        {
            // 옵션 세이브 데이터에서 설정된 언어가 있다면(Last가 아니라면) 불러옴
            currentLanguage = (Language)OptionsData.optionsSaveData.language;
        }
        else
        {
            // 언어가 설정되어 있지 않다면 문화권에 따라 언어를 설정합니다.
            switch (CultureInfo.CurrentCulture.Name)
            {
                case "ko-KR":
                    // 한국 문화권이면 한국어로 설정
                    currentLanguage = Language.Korean;
                    break;
                default:
                    // 한국 문화권이 아니면 영어로 설정
                    currentLanguage = Language.English;
                    break;
            }
        }

        // UI의 언어 데이터를 담아놓은 CSV 파일을 읽어와서 담은 후, 언어 데이터 클래스 추가
        List<Dictionary<string, object>> getUILanguageData = CSVReader.Read("LanguageData/UILanguageData");
        for (int i = 0; i < getUILanguageData.Count; i++)
        {
            var newLanguageData = (LanguageData)ScriptableObject.CreateInstance(typeof(LanguageData));
            string keyName = getUILanguageData[i]["Key"].ToString();                // 키
            newLanguageData.english = getUILanguageData[i]["English"].ToString();   // 영어
            newLanguageData.korean = getUILanguageData[i]["Korean"].ToString();     // 한국어
            languageData.Add(keyName, newLanguageData);
        }
        OptionsData.optionsSaveData.language = (int)currentLanguage;    // 현재 언어를 옵션 세이브 데이터에 저장
    }

    /// <summary>
    /// key를 이용하여 텍스트를 가져오는 정적 메소드입니다.
    /// </summary>
    /// <param name="key">가져오려는 텍스트의 key</param>
    /// <returns>key 및 현재 언어 설정을 기반으로 현지화된 텍스트</returns>
    public static string GetText(string key)
    {
        string text;
        switch(currentLanguage)
        {
            case Language.English:
                text = languageData[key].english;
                break;
            case Language.Korean:
                text = languageData[key].korean;
                break;
            default:
                text = "ERROR!!";
                break;
        }
        return text;
    }

    /// <summary>
    /// 현재 사용하는 언어를 설정할 때 사용하는 정적 메소드입니다.
    /// </summary>
    /// <param name="right">true일 경우 index가 증가하며, false일 경우 감소합니다.</param>
    public static void SetLanguage(bool right)
    {
        if (right)
        {
            currentLanguage++;
            if (currentLanguage >= Language.Last)
            {
                currentLanguage = 0;
            }
        }
        else
        {
            currentLanguage--;
            if (currentLanguage < 0)
            {
                currentLanguage = Language.Last - 1;
            }
        }
        OptionsData.optionsSaveData.language = (int)currentLanguage; // 현재 언어를 옵션 세이브 데이터에 저장
    }

    /// <summary>
    /// 현재 언어 설정이 무엇으로 됐는지 옵션에서 확인하기 위한 명칭을 Text로 가져오는 메소드입니다.
    /// </summary>
    /// <returns>현재 설정된 언어의 명칭</returns>
    public static string GetCurrentLanguageToText()
    {
        string languageToString;
        switch(currentLanguage)
        {
            case Language.English:
                languageToString = "ENGLISH";
                break;
            case Language.Korean:
                languageToString = "한국어";
                break;
            default:
                languageToString = "ERROR!!";
                break;
        }

        return languageToString;
    }
}