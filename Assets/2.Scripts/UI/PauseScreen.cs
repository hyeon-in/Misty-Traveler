using System.Collections;
using System.Collections.Generic;
using TMPro;
using MenuUI;
using UnityEngine;

/// <summary>
/// 게임 내의 다양한 정지 화면을 실행하고 종료하는 기능을 하는 클래스입니다.
/// </summary>
public class PauseScreen : MonoBehaviour
{
    // 지도가 열렸을 때 호출되는 델리게이트
    public delegate void MapOpendEventHandler();
    public MapOpendEventHandler MapOpend;

    [SerializeField] GameObject _pauseScreen;       // 정지 화면
    [SerializeField] GameObject _optionsMenuScreen; // 옵션 메뉴 화면
    [SerializeField] GameObject _mapScreen;         // 지도 화면

    bool _playerDead;   // 플레이어 사망 여부 체크(플레이어가 사망했을 때 정지 화면이 실행되지 않게 함)

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
        // 입력 처리
        bool optionsMenuInput = GameInputManager.PlayerInputDown(GameInputManager.PlayerActions.Pause);
        bool mapInput = GameInputManager.PlayerInputDown(GameInputManager.PlayerActions.Map);
        bool backInput = GameInputManager.MenuInputDown(GameInputManager.MenuControl.Cancle);

        if (GameManager.instance.currentGameState == GameManager.GameState.Play)
        {
            // 게임이 플레이 상태일 때 옵션을 열거나 지도를 열 수 있음
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
            // 옵션 메뉴가 열려있을 경우 해당 옵션을 여는 버튼(키)이나 Cancle 버튼(키)를 입력시 게임 플레이로 돌아감 
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
    /// 게임 플레이를 재개하는 메소드입니다.
    /// </summary>
    void ReturnToGamePlay()
    {
        GameManager.instance.SetGameState(GameManager.GameState.Play);
        _pauseScreen.SetActive(false);
    }

    /// <summary>
    /// 옵션 메뉴를 여는 메소드입니다.
    /// </summary>
    void OptionsMenuOpen()
    {
        GameManager.instance.SetGameState(GameManager.GameState.MenuOpen);
        _pauseScreen.SetActive(true);
        _optionsMenuScreen.SetActive(true);
        _mapScreen.SetActive(false);
    }

    /// <summary>
    /// 지도를 여는 메소드입니다.
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
    /// 플레이어가 사망했을 경우 호출되는 메소드입니다.
    /// </summary>
    void OnPlayerDied()
    {
        _playerDead = true;
    }
}