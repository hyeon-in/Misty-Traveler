using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 프로필 선택화면에서 사용하는 프로필 데이터 클래스입니다.
/// </summary>
[System.Serializable]
public class SelectProfile
{
    public Image profileImage;              // 프로필의 이미지
    public TextMeshProUGUI playTimeText;    // 플레이 타임 표시 텍스트
    public TextMeshProUGUI newText;         // 해당 프로필이 비어있음을 나타내는 텍스트
    public Image saveDeleteGuage;           // 해당 프로필을 지울때 사용하는 게이지
}

/// <summary>
/// 플레이어 프로필 선택 및 관리와 관련된 기능들을 처리하는 클래스입니다.
/// </summary>
public class SelectProfileScreen : MonoBehaviour
{
    // 이전 화면으로 되돌아갈때 호출되는 델리게이트
    public delegate void PrevScreenReturnEventHandler();
    public PrevScreenReturnEventHandler PrevScreenReturn;

    public Sprite selectImage;                  // 해당 프로필이 선택된 프로필이라는 것을 나타내기 위한 스프라이트
    public Sprite notSelectImage;               // 해당 프로필이 선택되지 않은 프로필이라는 것을 나타내기 위한 스프라이트
    public TextMeshProUGUI selectProfileText;   // 해당 화면이 프로필 선택 화면이라는 것을 나타내기 위한 텍스트
    public TextMeshProUGUI manualText;          // 메뉴얼 텍스트
    public SelectProfile[] profile;             // 프로필 배열

    int _currentSaveFileIndex;  // 현재 선택한 세이브 파일의 인덱스


    void Awake()
    {
        SelectProfileInit();
        SelectProfileRefresh();
    }

    void OnEnable()
    {
        // 현재 언어 설정에 맞춰 텍스트 초기화
        selectProfileText.text = LanguageManager.GetText("SelectProfile");

        string backKey = LanguageManager.GetText("Back");
        string selectKey = LanguageManager.GetText("Confirm");
        string deleteKey = LanguageManager.GetText("Delete");
        string backInput, selectInput, deleteInput;

        // 메뉴얼에 메뉴 제어 조작 방법 표시
        if (GameInputManager.usingController)
        {
            backInput = GameInputManager.MenuControlToButtonText(GameInputManager.MenuControl.Cancle);
            selectInput = GameInputManager.MenuControlToButtonText(GameInputManager.MenuControl.Select);
            deleteInput = GameInputManager.MenuControlToButtonText(GameInputManager.MenuControl.Delete);
        }
        else
        {
            backInput = GameInputManager.MenuControlToKeyText(GameInputManager.MenuControl.Cancle);
            selectInput = GameInputManager.MenuControlToKeyText(GameInputManager.MenuControl.Select);
            deleteInput = GameInputManager.MenuControlToKeyText(GameInputManager.MenuControl.Delete);
        }
        manualText.text = string.Format("{0} [ <color=#ffaa5e>{1}</color> ] {2} [ <color=#ffaa5e>{3}</color> ] {4} [ <color=#ffaa5e>{5}</color> ]", backKey, backInput, selectKey, selectInput, deleteKey, deleteInput);
    }

    void Update()
    {
        // 입력 받기
        bool upInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Up);
        bool downInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Down);
        bool backInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Cancle);
        bool selectInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Select);
        bool deleteInput = GameInputManager.MenuInput(GameInputManager.MenuControl.Delete);

        if (upInput)
        {
            // 위 입력시 인덱스 감소(선택 메뉴를 위로 이동)
            _currentSaveFileIndex--;
            SelectProfileRefresh();
        }
        else if (downInput)
        {
            // 아래 입력시 인덱스 증가(선택 메뉴를 아래로 이동)
            _currentSaveFileIndex++;
            SelectProfileRefresh();
        }

        if (selectInput)
        {
            // 선택 입력시 게임 시작
            GameManager.instance.GameLoad(_currentSaveFileIndex + 1);
            GameManager.instance.SetGameState(GameManager.GameState.Play);
        }
        else if (backInput)
        {
            // 뒤로 가기 입력시 이전 화면으로
            gameObject.SetActive(false);
            PrevScreenReturn();
        }
        if (deleteInput)
        {
            // 삭제 입력시 파일 삭제 실행
            SelectProfileDelete();
        }
        DeleteGaugeValueToDefault(deleteInput);
    }

    /// <summary>
    /// 프로필을 초기화하는 메소드입니다.
    /// </summary>
    void SelectProfileInit()
    {
        for (int i = 0; i < profile.Length; i++)
        {
            GameSaveData saveData = SaveSystem.GameLoad(i + 1);
            if (saveData != null)
            {
                // 세이브 데이터가 존재하면 플레이 시간 표시
                profile[i].playTimeText.text = GameManager.instance.IntToTimeText(saveData.playTime);
                profile[i].newText.text = "";
            }
            else
            {
                // 세이브 데이터가 없으면 NEW 텍스트 표시
                profile[i].playTimeText.text = "";
                profile[i].newText.text = "NEW";
            }
            profile[i].saveDeleteGuage.fillAmount = 0f;
        }
    }

    /// <summary>
    /// 프로필을 새로고침하는 메소드입니다.
    /// 현재 선택한 프로필을 표시할 때 사용합니다.
    /// </summary>
    void SelectProfileRefresh()
    {
        if (_currentSaveFileIndex >= profile.Length)
        {
            _currentSaveFileIndex = 0;
        }
        else if (_currentSaveFileIndex < 0)
        {
            _currentSaveFileIndex = profile.Length - 1;
        }

        for(int i = 0; i < profile.Length; i++)
        {
            profile[i].profileImage.sprite = notSelectImage;
        }
        profile[_currentSaveFileIndex].profileImage.sprite = selectImage;
    }

    /// <summary>
    /// 선택한 프로필을 삭제하는 메소드입니다.
    /// 입력을 지속하고 있어야 삭제가 됩니다.
    /// </summary>
    void SelectProfileDelete()
    {
        // 세이브 데이터가 없으면 실행하지 않음
        if(SaveSystem.GameLoad(_currentSaveFileIndex + 1) == null)
        {
            return;
        }

        // 삭제 게이지가 계속해서 차오름
        float fillAmount = profile[_currentSaveFileIndex].saveDeleteGuage.fillAmount + (0.3f * Time.deltaTime);
        fillAmount = Mathf.Clamp(fillAmount, 0f, 1.0f);
        profile[_currentSaveFileIndex].saveDeleteGuage.fillAmount = fillAmount;
        
        if(GameInputManager.usingController)
        {
            // 컨트롤러 사용시 삭제 게이지가 차오를 수록 진동이 강해짐(경고 신호)
            GamepadVibrationManager.instance.GamepadRumbleStart(fillAmount * 0.5f, 0.02f);
        }

        // 삭제 게이지가 채워지면 채워질수록 색이 붉어짐
        if(fillAmount >= 0.5f && fillAmount < 0.75f)
        {
            profile[_currentSaveFileIndex].saveDeleteGuage.color = new Color32(255,170,94,255);
        }
        else if(fillAmount >= 0.75f && fillAmount < 1.0f)
        {
            profile[_currentSaveFileIndex].saveDeleteGuage.color = new Color32(142, 67, 91, 255);            
        }
        else if(fillAmount >= 1.0f)
        {
            // 삭제 게이지가 전부 차면 세이브 데이터 삭제
            profile[_currentSaveFileIndex].saveDeleteGuage.fillAmount = 0;
            SaveSystem.GameDelete(_currentSaveFileIndex + 1);
            SelectProfileInit();
        }
    }

    /// <summary>
    /// 세이브 데이터 삭제 게이지를 지속적으로 초기값으로 되돌리는 메소드입니다.
    /// </summary>
    /// <param name="deleteInput">세이브 데이터 삭제 버튼 입력 여부</param>
    void DeleteGaugeValueToDefault(bool deleteInput)
    {
        for(int i = 0; i < profile.Length; i++)
        {
            if(profile[i].saveDeleteGuage.fillAmount == 0 || (deleteInput && i == _currentSaveFileIndex))
            {
                // 삭제 게이지가 0이거나 현재 선택한 세이브 파일을 삭제하고 있을 경우 다음 세이브 데이터로 넘어감
                continue;
            }
            float fillAmount = profile[i].saveDeleteGuage.fillAmount - Time.deltaTime;
            fillAmount = Mathf.Clamp(fillAmount, 0f, 1.0f);
            profile[i].saveDeleteGuage.fillAmount = fillAmount;

            if (fillAmount >= 0.5f && fillAmount < 0.75f)
            {
                profile[_currentSaveFileIndex].saveDeleteGuage.color = new Color32(255, 170, 94, 255);
            }
            else if(fillAmount < 0.5f)
            {
                profile[_currentSaveFileIndex].saveDeleteGuage.color = new Color32(255, 236, 214, 255);
            }
        }
    }
}
