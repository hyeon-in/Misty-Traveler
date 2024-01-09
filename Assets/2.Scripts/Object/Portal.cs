using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾�� �浹�ϸ� ���� ��ȯ�ϴ� ��Ż ������Ʈ Ŭ�����Դϴ�.
/// </summary>
public class Portal : MonoBehaviour
{
    [SerializeField] string _nextScene;              // �ε��� ��
    [SerializeField] Vector2 _size = Vector2.one;       // ��Ż ũ��
    [SerializeField] bool _isPlayerFacingRight = true;  // �÷��̾ ���� �̵��� �� �������� ���� �ϴ����� ���� ����

    LayerMask _playerLayer;
    Transform _portalTransform;

    bool _isHit; // �÷��̾ ��Ż�� ��� �浹�ϴ� ��츦 ����

    void Awake() 
    {
        _playerLayer = LayerMask.GetMask("Player");
        _portalTransform = GetComponent<Transform>();
    }

    void Update()
    {
        if(_isHit) return;

        var coll = Physics2D.OverlapBox(_portalTransform.position, _size, 0, _playerLayer);

        // �÷��̾ ��Ż�� �浹���� ��� ��� �̵�
        if(coll)
        {
            _isHit = true;
            SceneTransition.instance.LoadScene(_nextScene);
            // �÷��̾ ���� ������ ������ ��ġ�� �ٶ󺸴� ���� ����
            GameManager.instance.playerStartPos = coll.transform.position;
            GameManager.instance.playerStartlocalScaleX = _isPlayerFacingRight ? 1f : -1f;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, _size);
    }
}