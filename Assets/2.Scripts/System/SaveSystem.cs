using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

/// <summary>
/// 게임의 옵션 저장 데이터를 담고 있는 클래스입니다.
/// </summary>
[System.Serializable]
public class OptionsSaveData
{
    public bool fullScreenMode = true;
    public Resolution? resolution = null;
    public bool vSync = true;

    public List<int> keyMapping = new List<int>();
    public List<int> buttonMapping = new List<int>();

    public float masterVolume = 1.0f;
    public float musicVolume = 1.0f;
    public float effectsVolume = 1.0f;

    public float gamepadVibration = 1.0f;
    public bool screenShake = true;
    public bool screenFlashes = true;

    public int language = (int)LanguageManager.Language.Last;
}

/// <summary>
/// 게임의 저장 데이터를 담고 있는 클래스입니다.
/// </summary>
[System.Serializable]
public class GameSaveData
{
    public int playTime;                                        // 플레이어의 게임 플레이 시간
    public string sceneName;                                    // 저장한 장소의 씬 이름
    public Vector2 playerPos;                                   // 플레이어의 좌표
    public bool hasLearnedClimbingWall;                         // 플레이어가 벽차기를 배웠는지에 대한 여부
    public bool hasLearnedDoubleJump;                           // 플레이어가 더블 점프를 배웠는지에 대한 여부
    public List<Vector2> foundMaps = new List<Vector2>();       // 지금까지 플레이어가 발견한 지도 리스트
    public List<string> deadBosses = new List<string>();        // 지금까지 플레이어가 처치한 보스의 리스트
    public List<string> seenTutorials = new List<string>();    // 지금까지 플레이어가 본 튜토리얼 리스트

    public GameSaveData(int playTime,
                        string sceneName,
                        Vector2 playerPos,
                        bool hasLearnedClimbingWall,
                        bool hasLearnedDoubleJump,
                        List<Vector2> foundMaps,
                        List<string> deadBosses,
                        List<string> seenTutorials)
    {
        this.playTime = playTime;
        this.sceneName = sceneName;
        this.playerPos = playerPos;
        this.hasLearnedClimbingWall = hasLearnedClimbingWall;
        this.hasLearnedDoubleJump = hasLearnedDoubleJump;
        this.foundMaps = foundMaps;
        this.deadBosses = deadBosses;
        this.seenTutorials = seenTutorials;
    }
}

/// <summary>
/// 게임의 데이터를 저장하고 불러오는 저장 시스템을 처리하는 정적 클래스입니다.
/// </summary>
public static class SaveSystem
{
    // 암호화에 사용되는 private키
    private static readonly string privateKey = "EKDe$BMqvxvVf6z^ovKrhuf%JIJUg01CgCnadXYcOeGT%Iu5kS0xrj^09%^N";

    /// <summary>
    /// 옵션 데이터를 파일에 저장하는 정적 메소드입니다.
    /// </summary>
    /// <param name="optionsSaveData">저장할 옵션 데이터</param>
    public static void OptionSave(OptionsSaveData optionsSaveData)
    {
        // 옵션 데이터를 JSON으로 변환하고 암호화 한 뒤 파일에 저장한다.
        string jsonString = JsonUtility.ToJson(optionsSaveData);
        string encryptString = Encrypt(jsonString);

        using (FileStream fs = new FileStream(OptionsGetPath(), FileMode.Create, FileAccess.Write))
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(encryptString);
            fs.Write(bytes, 0, bytes.Length);
        }
#if UNITY_EDITOR
        Debug.Log("Save Success: " + OptionsGetPath());
#endif
    }

    /// <summary>
    /// 파일에서 옵션 데이터를 불러오는 정적 메소드입니다.
    /// </summary>
    /// <returns>불러온 옵션 데이터</returns>
    public static OptionsSaveData OptionLoad()
    {
        // 파일이 존재하는지 확인한 후 없으면 null을 반환한다.
        if (!File.Exists(OptionsGetPath()))
        {
#if UNITY_EDITOR
            Debug.Log("저장 된 파일이 존재하지 않습니다.");
#endif
            return null;
        }

        // 파일에서 암호화된 데이터를 읽어온다.
        string encryptData;
        using (FileStream fs = new FileStream(OptionsGetPath(), FileMode.Open, FileAccess.Read))
        {
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, (int)fs.Length);
            encryptData = System.Text.Encoding.UTF8.GetString(bytes);
        }

        // 데이터를 복호화하고 반환한다
        string decryptData = Decrypt(encryptData);
        OptionsSaveData saveData = JsonUtility.FromJson<OptionsSaveData>(decryptData);
        return saveData;
    }

    /// <summary>
    /// 게임 데이터를 파일에 저장하는 정적 메소드입니다.
    /// </summary>
    /// <param name="saveData">저장할 게임 데이터</param>
    /// <param name="dataNum">저장 파일 식별자</param>
    public static void GameSave(GameSaveData saveData, int dataNum)
    {
        // 게임 데이터를 JSON으로 변환하고 암호화한 뒤 파일에 저장한다.
        string jsonString = JsonUtility.ToJson(saveData);
        string encryptString = Encrypt(jsonString);

        using (FileStream fs = new FileStream(GetPath(dataNum), FileMode.Create, FileAccess.Write))
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(encryptString);
            fs.Write(bytes, 0, bytes.Length);
        }

#if UNITY_EDITOR
		    Debug.Log("저장 성공: " + GetPath(dataNum));
#endif
	}

    /// <summary>
    /// 파일에서 게임 데이터를 불러오는 정적 메소드입니다.
    /// </summary>
    /// <param name="dataNum">불러올 저장 파일 식별자</param>
    /// <returns>불러온 게임 데이터</returns>
	public static GameSaveData GameLoad(int dataNum)
	{
        // 파일이 존재하는지 확인한다
        if (!File.Exists(GetPath(dataNum)))
        {
#if UNITY_EDITOR
            Debug.Log(dataNum + "번 저장 파일이 존재하지 않음.");
#endif
            return null;
        }

        // 파일에서 암호화된 데이터를 읽어온 뒤 복호화하고 반환한다
        string encryptData;
        using (FileStream fs = new FileStream(GetPath(dataNum), FileMode.Open, FileAccess.Read))
        {
            byte[] bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, (int)fs.Length);
            encryptData = System.Text.Encoding.UTF8.GetString(bytes);
        }

        string decryptData = Decrypt(encryptData);
#if UNITY_EDITOR
        Debug.Log(decryptData);
#endif
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(decryptData);
        return saveData;
    }
    /// <summary>
    /// 특정한 게임 데이터 저장 파일을 삭제하기 위한 정적 메소드입니다.
    /// </summary>
    /// <param name="dataNum">삭제할 저장 파일 식별자</param>
    public static void GameDelete(int dataNum)
    {
        if (!File.Exists(GetPath(dataNum)))
        {
#if UNITY_EDITOR
            Debug.Log("해당 저장 파일은 존재하지 않습니다.");
#endif
            return;
        }
        File.Delete(GetPath(dataNum));
    }

    /// <summary>
    /// 게임 데이터 저장 파일에 대한 파일 경로를 생성해서 가져오는 정적 메소드입니다.
    /// </summary>
    /// <param name="dataNum">파일 경로를 생성할 때 사용할 식별자</param>
    /// <returns>생성된 파일 경로</returns>
    static string GetPath(int dataNum) => Path.Combine(Application.persistentDataPath + @"/save0" + dataNum + ".save");

    /// <summary>
    /// 옵션 데이터 파일에 대한 파일 경로를 생성해서 가져오는 정적 메소드입니다.
    /// </summary>
    /// <returns>생성된 파일 경로</returns>
    static string OptionsGetPath() => Path.Combine(Application.persistentDataPath + @"/options.save");

    /// <summary>
    /// JSON 데이터를 암호화하는 정적 메소드입니다.
    /// </summary>
    /// <param name="data">암호화 하려는 JSON 데이터</param>
    /// <returns>암호화 된 데이터</returns>
    static string Encrypt(string data)
    {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
        RijndaelManaged rm = CreateRijndaelManaged();
        ICryptoTransform ct = rm.CreateEncryptor();
        byte[] results = ct.TransformFinalBlock(bytes, 0, bytes.Length);
        return System.Convert.ToBase64String(results, 0, results.Length);

    }

    /// <summary>
    /// JSON 데이터를 복호화하는 정적 메소드입니다.
    /// </summary>
    /// <param name="data">복호화 하려는 데이터</param>
    /// <returns>복호화 된 데이터</returns>
    static string Decrypt(string data)
    {
        byte[] bytes = System.Convert.FromBase64String(data);
        RijndaelManaged rm = CreateRijndaelManaged();
        ICryptoTransform ct = rm.CreateDecryptor();
        byte[] resultArray = ct.TransformFinalBlock(bytes, 0, bytes.Length);
        return System.Text.Encoding.UTF8.GetString(resultArray);
    }

    /// <summary>
    /// RijndaelManaged 인스턴스를 생성하는 정적 메소드 입니다.
    /// </summary>
    /// <returns>생성된 RijndaelManaged 인스턴스</returns>
    static RijndaelManaged CreateRijndaelManaged()
    {
        byte[] keyArray = System.Text.Encoding.UTF8.GetBytes(privateKey);
        RijndaelManaged result = new RijndaelManaged();

        byte[] newKeysArray = new byte[16];
        System.Array.Copy(keyArray, 0, newKeysArray, 0, 16);

        result.Key = newKeysArray;
        result.Mode = CipherMode.ECB;
        result.Padding = PaddingMode.PKCS7;
        return result;
    }
}
