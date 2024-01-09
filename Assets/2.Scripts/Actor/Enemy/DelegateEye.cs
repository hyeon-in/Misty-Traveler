using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 보스 몬스터의 눈이 플레이어를 바라보게 하기 위한 클래스입니다.
/// </summary>
public class DelegateEye : MonoBehaviour
{
    [SerializeField] Transform _eye;
    Transform _playerTransform;

    void Awake()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        // 플레이어 방향 구하기
        Vector2 direction = (_playerTransform.position - transform.position).normalized;

        // 플레이어의 방향에 따라 보스의 눈 좌표 설정
        float posX = direction.x < -0.5f ? -0.125f :
                     direction.x > 0.5f ? 0.125f :
                     0;
        float posY = direction.y < -0.5f ? -0.125f :
                     direction.y > 0.5f ? 0.125f :
                     0;
        _eye.position = new Vector3(transform.position.x + posX, transform.position.y + posY, _eye.position.z);
    }
}
