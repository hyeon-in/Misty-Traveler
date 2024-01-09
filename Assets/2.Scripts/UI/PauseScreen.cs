using System.Collections;
using System.Collections.Generic;
using TMPro;
using MenuUI;
using UnityEngine;

/// <summary>
/// ���� ���� �پ��� ���� ȭ���� �����ϰ� �����ϴ� ����� �ϴ� Ŭ�����Դϴ�.
/// </summary>
public class PauseScreen : MonoBehaviour
{
    // ������ ������ �� ȣ��Ǵ� ��������Ʈ
    public delegate void MapOpendEventHandler();
    public MapOpendEventHandler MapOpend;

    [SerializeField] GameObject _pauseScreen;       // ���� ȭ��
    [SerializeField] GameObject _optionsMenuScreen; // �ɼ� �޴� ȭ��
    [SerializeField] GameObject _mapScreen;         // ���� ȭ��

    bool _playerDead;   // �÷��̾� ��� ���� üũ(�÷��̾ ������� �� ���� ȭ���� ������� �ʰ� ��)

    void Awake()
    {
        _pauseScreen.SetActive(false);
    }

    void Start()
    {
        GameManager.instance.PlayerDieEvent += OnPlayerDied;
    }

    void Update()
    {
        // �Է� ó��
        bool optionsMenuInput = GameInputManager.PlayerInputDown(GameInputManager.PlayerActions.Pause);
        bool mapInput = GameInputManager.PlayerInputDown(GameInputManager.PlayerActions.Map);
        bool backInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Cancle);

        if (GameManager.instance.currentGameState == GameManager.GameState.Play)
        {
            // ������ �÷��� ������ �� �ɼ��� ���ų� ������ �� �� ����
            if (!_playerDead)
            {
                if (optionsMenuInput)
                {
                    OptionsMenuOpen();
                }
                else if (mapInput)
                {
                    MapOpen();
                }
            }
        }
        else if (GameManager.instance.currentGameState == GameManager.GameState.MenuOpen)
        {
            // �ɼ� �޴��� �������� ��� �ش� �ɼ��� ���� ��ư(Ű)�̳� Cancle ��ư(Ű)�� �Է½� ���� �÷��̷� ���ư� 
            if (_optionsMenuScreen.activeSelf == true)
            {
                if (optionsMenuInput || backInput)
                {
                    ReturnToGamePlay();
                }
            }
            else if (_mapScreen.activeSelf == true)
            {
                if (mapInput || backInput)
                {
                    ReturnToGamePlay();
                }
            }
        }
    }

    /// <summary>
    /// ���� �÷��̸� �簳�ϴ� �޼ҵ��Դϴ�.
    /// </summary>
    void ReturnToGamePlay()
    {
        GameManager.instance.SetGameState(GameManager.GameState.Play);
        _pauseScreen.SetActive(false);
    }

    /// <summary>
    /// �ɼ� �޴��� ���� �޼ҵ��Դϴ�.
    /// </summary>
    void OptionsMenuOpen()
    {
        GameManager.instance.SetGameState(GameManager.GameState.MenuOpen);
        _pauseScreen.SetActive(true);
        _optionsMenuScreen.SetActive(true);
        _mapScreen.SetActive(false);
    }

    /// <summary>
    /// ������ ���� �޼ҵ��Դϴ�.
    /// </summary>
    void MapOpen()
    {
        GameManager.instance.SetGameState(GameManager.GameState.MenuOpen);
        _pauseScreen.SetActive(true);
        _mapScreen.SetActive(true);
        _optionsMenuScreen.SetActive(false);
        MapOpend?.Invoke();
    }

    /// <summary>
    /// �÷��̾ ������� ��� ȣ��Ǵ� �޼ҵ��Դϴ�.
    /// </summary>
    void OnPlayerDied()
    {
        _playerDead = true;
    }
}