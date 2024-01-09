using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������ ����ȭ�鿡�� ����ϴ� ������ ������ Ŭ�����Դϴ�.
/// </summary>
[System.Serializable]
public class SelectProfile
{
    public Image profileImage;              // �������� �̹���
    public TextMeshProUGUI playTimeText;    // �÷��� Ÿ�� ǥ�� �ؽ�Ʈ
    public TextMeshProUGUI newText;         // �ش� �������� ��������� ��Ÿ���� �ؽ�Ʈ
    public Image saveDeleteGuage;           // �ش� �������� ���ﶧ ����ϴ� ������
}

/// <summary>
/// �÷��̾� ������ ���� �� ������ ���õ� ��ɵ��� ó���ϴ� Ŭ�����Դϴ�.
/// </summary>
public class SelectProfileScreen : MonoBehaviour
{
    // ���� ȭ������ �ǵ��ư��� ȣ��Ǵ� ��������Ʈ
    public delegate void PrevScreenReturnEventHandler();
    public PrevScreenReturnEventHandler PrevScreenReturn;

    public Sprite selectImage;                  // �ش� �������� ���õ� �������̶�� ���� ��Ÿ���� ���� ��������Ʈ
    public Sprite notSelectImage;               // �ش� �������� ���õ��� ���� �������̶�� ���� ��Ÿ���� ���� ��������Ʈ
    public TextMeshProUGUI selectProfileText;   // �ش� ȭ���� ������ ���� ȭ���̶�� ���� ��Ÿ���� ���� �ؽ�Ʈ
    public TextMeshProUGUI manualText;          // �޴��� �ؽ�Ʈ
    public SelectProfile[] profile;             // ������ �迭

    int _currentSaveFileIndex;  // ���� ������ ���̺� ������ �ε���


    void Awake()
    {
        SelectProfileInit();
        SelectProfileRefresh();
    }

    void OnEnable()
    {
        // ���� ��� ������ ���� �ؽ�Ʈ �ʱ�ȭ
        selectProfileText.text = LanguageManager.GetText("SelectProfile");

        string backKey = LanguageManager.GetText("Back");
        string selectKey = LanguageManager.GetText("Confirm");
        string deleteKey = LanguageManager.GetText("Delete");
        string backInput, selectInput, deleteInput;

        // �޴��� �޴� ���� ���� ��� ǥ��
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
        // �Է� �ޱ�
        bool upInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Up);
        bool downInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Down);
        bool backInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Cancle);
        bool selectInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Select);
        bool deleteInput = GameInputManager.MenuInput(GameInputManager.MenuControl.Delete);

        if (upInput)
        {
            // �� �Է½� �ε��� ����(���� �޴��� ���� �̵�)
            _currentSaveFileIndex--;
            SelectProfileRefresh();
        }
        else if (downInput)
        {
            // �Ʒ� �Է½� �ε��� ����(���� �޴��� �Ʒ��� �̵�)
            _currentSaveFileIndex++;
            SelectProfileRefresh();
        }

        if (selectInput)
        {
            // ���� �Է½� ���� ����
            GameManager.instance.GameLoad(_currentSaveFileIndex + 1);
            GameManager.instance.SetGameState(GameManager.GameState.Play);
        }
        else if (backInput)
        {
            // �ڷ� ���� �Է½� ���� ȭ������
            gameObject.SetActive(false);
            PrevScreenReturn();
        }
        if (deleteInput)
        {
            // ���� �Է½� ���� ���� ����
            SelectProfileDelete();
        }
        DeleteGaugeValueToDefault(deleteInput);
    }

    /// <summary>
    /// �������� �ʱ�ȭ�ϴ� �޼ҵ��Դϴ�.
    /// </summary>
    void SelectProfileInit()
    {
        for (int i = 0; i < profile.Length; i++)
        {
            GameSaveData saveData = SaveSystem.GameLoad(i + 1);
            if (saveData != null)
            {
                // ���̺� �����Ͱ� �����ϸ� �÷��� �ð� ǥ��
                profile[i].playTimeText.text = GameManager.instance.IntToTimeText(saveData.playTime);
                profile[i].newText.text = "";
            }
            else
            {
                // ���̺� �����Ͱ� ������ NEW �ؽ�Ʈ ǥ��
                profile[i].playTimeText.text = "";
                profile[i].newText.text = "NEW";
            }
            profile[i].saveDeleteGuage.fillAmount = 0f;
        }
    }

    /// <summary>
    /// �������� ���ΰ�ħ�ϴ� �޼ҵ��Դϴ�.
    /// ���� ������ �������� ǥ���� �� ����մϴ�.
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
    /// ������ �������� �����ϴ� �޼ҵ��Դϴ�.
    /// �Է��� �����ϰ� �־�� ������ �˴ϴ�.
    /// </summary>
    void SelectProfileDelete()
    {
        // ���̺� �����Ͱ� ������ �������� ����
        if(SaveSystem.GameLoad(_currentSaveFileIndex + 1) == null)
        {
            return;
        }

        // ���� �������� ����ؼ� ������
        float fillAmount = profile[_currentSaveFileIndex].saveDeleteGuage.fillAmount + (0.3f * Time.deltaTime);
        fillAmount = Mathf.Clamp(fillAmount, 0f, 1.0f);
        profile[_currentSaveFileIndex].saveDeleteGuage.fillAmount = fillAmount;
        
        if(GameInputManager.usingController)
        {
            // ��Ʈ�ѷ� ���� ���� �������� ������ ���� ������ ������(��� ��ȣ)
            GamepadVibrationManager.instance.GamepadRumbleStart(fillAmount * 0.5f, 0.02f);
        }

        // ���� �������� ä������ ä�������� ���� �Ӿ���
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
            // ���� �������� ���� ���� ���̺� ������ ����
            profile[_currentSaveFileIndex].saveDeleteGuage.fillAmount = 0;
            SaveSystem.GameDelete(_currentSaveFileIndex + 1);
            SelectProfileInit();
        }
    }

    /// <summary>
    /// ���̺� ������ ���� �������� ���������� �ʱⰪ���� �ǵ����� �޼ҵ��Դϴ�.
    /// </summary>
    /// <param name="deleteInput">���̺� ������ ���� ��ư �Է� ����</param>
    void DeleteGaugeValueToDefault(bool deleteInput)
    {
        for(int i = 0; i < profile.Length; i++)
        {
            if(profile[i].saveDeleteGuage.fillAmount == 0 || (deleteInput && i == _currentSaveFileIndex))
            {
                // ���� �������� 0�̰ų� ���� ������ ���̺� ������ �����ϰ� ���� ��� ���� ���̺� �����ͷ� �Ѿ
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
