/// <summary>
/// 옵션 데이터를 관리하기 위한 정적 클래스입니다
/// </summary>
public static class OptionsData
{
    public static OptionsSaveData optionsSaveData;  // 옵션 데이터를 저장하고 불러오기 위한 클래스
    static bool hasInit;    // 초기화 여부 확인

    public static void Init()
    {
        // 아직 초기화되지 않았는지 체크
        if (!hasInit)
        {
            hasInit = true;

            // 옵션 세이브 데이터를 불러오고 없다면 새로 생성
            optionsSaveData = SaveSystem.OptionLoad();
            if (optionsSaveData == null)
            {
                optionsSaveData = new OptionsSaveData();
            }

            // 옵션 초기화
            VideoSettingsManager.VideoOptionsInit();
            GameInputManager.Init();
            SoundSettingsManager.Init();
            AccessibilitySettingsManager.Init();
            LanguageManager.Init();
        }
    }

    /// <summary>
    /// 옵션 데이터를 저장하는 메소드입니다.
    /// </summary>
    public static void OptionsSave()
    {
        SaveSystem.OptionSave(optionsSaveData);
    }
}