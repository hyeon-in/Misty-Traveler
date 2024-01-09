using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// üũ ����Ʈ ������Ʈ Ŭ������, �÷��̾ �ش� ������Ʈ�� �浹�� ���¿��� Ư�� ��ư�� �Է��ϸ� �޽��� ���� �� �ֽ��ϴ�.
/// �÷��̾ ����� ���, Ȥ�� ������ �̾ �� ��� üũ����Ʈ�� ��ġ���� ������ ���۵˴ϴ�.
/// </summary>
public class CheckPoint : MonoBehaviour
{
    [SerializeField] float _radius = 0.75f; // �浹 ����
    [SerializeField] Transform _circlePos;  // �浹 ���� ��ǥ
    [SerializeField] HUDManual _hudManual;  // HUD �޴���

    private bool _isHUDManualOpened;    // HUD �޴����� ���� �������� üũ

    private Player _player;
    private LayerMask _playerLayer;

    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _playerLayer = LayerMask.GetMask("Player");
    }

    void Update()
    {
        // ������ �÷������� ���°� �ƴ� ��� �������� ����
        if(GameManager.instance.currentGameState != GameManager.GameState.Play) return;

        // �÷��̾���� �浹 ���θ� ������
        bool playerColl = Physics2D.OverlapCircle(_circlePos.position, _radius, _playerLayer);

        if (playerColl)
        {
            // HUD �޴����� �ش� ������Ʈ ��ġ�� ǥ��
            if(!_isHUDManualOpened)
            {
                _hudManual.DisplayManual("Rest", GameInputManager.PlayerActions.MoveUp, transform.position);
                _isHUDManualOpened = true;
            }

            // �÷��̾ ������ �Է��� ��� �÷��̾ �޽� ���·� �����
            if (GameInputManager.PlayerInputDown(GameInputManager.PlayerActions.MoveUp))
            {
                Vector3 checkPointPos = transform.position;
                checkPointPos.x -= 2.0f;
                _player.RestAtCheckPoint(checkPointPos);
            }
        }
        else
        {
            // HUD �޴��� ����
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
