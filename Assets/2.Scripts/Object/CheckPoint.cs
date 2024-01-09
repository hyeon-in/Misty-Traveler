using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 체크 포인트 오브젝트 클래스로, 플레이어가 해당 오브젝트와 충돌한 상태에서 특정 버튼을 입력하면 휴식을 취할 수 있습니다.
/// 플레이어가 사망할 경우, 혹은 게임을 이어서 할 경우 체크포인트의 위치에서 게임이 시작됩니다.
/// </summary>
public class CheckPoint : MonoBehaviour
{
    [SerializeField] float _radius = 0.75f; // 충돌 범위
    [SerializeField] Transform _circlePos;  // 충돌 원점 좌표
    [SerializeField] HUDManual _hudManual;  // HUD 메뉴얼

    private bool _isHUDManualOpened;    // HUD 메뉴얼이 열린 상태인지 체크

    private Player _player;
    private LayerMask _playerLayer;

    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _playerLayer = LayerMask.GetMask("Player");
    }

    void Update()
    {
        // 게임이 플레이중인 상태가 아닐 경우 실행하지 않음
        if(GameManager.instance.currentGameState != GameManager.GameState.Play) return;

        // 플레이어와의 충돌 여부를 가져옴
        bool playerColl = Physics2D.OverlapCircle(_circlePos.position, _radius, _playerLayer);

        if (playerColl)
        {
            // HUD 메뉴얼을 해당 오브젝트 위치에 표시
            if(!_isHUDManualOpened)
            {
                _hudManual.DisplayManual("Rest", GameInputManager.PlayerActions.MoveUp, transform.position);
                _isHUDManualOpened = true;
            }

            // 플레이어가 위쪽을 입력할 경우 플레이어를 휴식 상태로 만든다
            if (GameInputManager.PlayerInputDown(GameInputManager.PlayerActions.MoveUp))
            {
                Vector3 checkPointPos = transform.position;
                checkPointPos.x -= 2.0f;
                _player.RestAtCheckPoint(checkPointPos);
            }
        }
        else
        {
            // HUD 메뉴얼 종료
            if (_isHUDManualOpened)
            {
                _hudManual.HideManual();
                _isHUDManualOpened = false;
            }
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(_circlePos.position, _radius);
    }
}
