/// <summary>
/// �ɼ� �����͸� �����ϱ� ���� ���� Ŭ�����Դϴ�
/// </summary>
public static class OptionsData
{
    public static OptionsSaveData optionsSaveData;  // �ɼ� �����͸� �����ϰ� �ҷ����� ���� Ŭ����
    static bool hasInit;    // �ʱ�ȭ ���� Ȯ��

    public static void Init()
    {
        // ���� �ʱ�ȭ���� �ʾҴ��� üũ
        if (!hasInit)
        {
            hasInit = true;

            // �ɼ� ���̺� �����͸� �ҷ����� ���ٸ� ���� ����
            optionsSaveData = SaveSystem.OptionLoad();
            if (optionsSaveData == null)
            {
                optionsSaveData = new OptionsSaveData();
            }

            // �ɼ� �ʱ�ȭ
            VideoSettingsManager.VideoOptionsInit();
            GameInputManager.Init();
            SoundSettingsManager.Init();
            AccessibilitySettingsManager.Init();
            LanguageManager.Init();
        }
    }

    /// <summary>
    /// �ɼ� �����͸� �����ϴ� �޼ҵ��Դϴ�.
    /// </summary>
    public static void OptionsSave()
    {
        SaveSystem.OptionSave(optionsSaveData);
    }
}