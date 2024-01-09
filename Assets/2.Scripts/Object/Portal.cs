using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어와 충돌하면 씬을 전환하는 포탈 오브젝트 클래스입니다.
/// </summary>
public class Portal : MonoBehaviour
{
    [SerializeField] string _nextScene;              // 로드할 씬
    [SerializeField] Vector2 _size = Vector2.one;       // 포탈 크기
    [SerializeField] bool _isPlayerFacingRight = true;  // 플레이어가 씬을 이동한 후 오른쪽을 봐야 하는지에 대한 여부

    LayerMask _playerLayer;
    Transform _portalTransform;

    bool _isHit; // 플레이어가 포탈에 계속 충돌하는 경우를 방지

    void Awake() 
    {
        _playerLayer = LayerMask.GetMask("Player");
        _portalTransform = GetComponent<Transform>();
    }

    void Update()
    {
        if(_isHit) return;

        var coll = Physics2D.OverlapBox(_portalTransform.position, _size, 0, _playerLayer);

        // 플레이어가 포탈에 충돌했을 경우 장면 이동
        if(coll)
        {
            _isHit = true;
            SceneTransition.instance.LoadScene(_nextScene);
            // 플레이어가 다음 씬에서 시작할 위치와 바라보는 방향 설정
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