using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카메라의 범위를 제한하기 위한 클래스입니다.
/// </summary>
public class CameraRange : MonoBehaviour
{
    public int left, right;     // 좌우 범위 
    public int up, down;        // 상하 범위

    private float _halfSizeX, _halfSizeY;

    [HideInInspector] public float leftEdge;
    [HideInInspector] public float rightEdge;
    [HideInInspector] public float topEdge;
    [HideInInspector] public float bottomEdge;

    private Vector3 _topLeft, _topRight;
    private Vector3 _bottomLeft, _bottomRight;

    void Awake()
    {
        _halfSizeY = Camera.main.orthographicSize;
        _halfSizeX = _halfSizeY * Camera.main.aspect;

        float leftSize = (left * (_halfSizeX * 2)) + _halfSizeX;
        float rightSize = (right * (_halfSizeX * 2)) + _halfSizeX;
        float topSize = (up * (_halfSizeY * 2)) + _halfSizeY;
        float bottomSize = (down * (_halfSizeY * 2)) + _halfSizeY;

        float posX = transform.position.x;
        float posY = transform.position.y;

        _topLeft = new Vector3(posX - leftSize, posY + topSize);
        _topRight = new Vector3(posX + rightSize, posY + topSize);
        _bottomLeft = new Vector3(posX - leftSize, posY - bottomSize);
        _bottomRight = new Vector3(posX + rightSize, posY - bottomSize);

        leftEdge = _topLeft.x + _halfSizeX;
        rightEdge = _topRight.x - _halfSizeX;
        topEdge = _topLeft.y - _halfSizeY;
        bottomEdge = _bottomLeft.y + _halfSizeY;
    }

    void OnDrawGizmosSelected()
    {
        _halfSizeY = Camera.main.orthographicSize;
        _halfSizeX = _halfSizeY * Camera.main.aspect;

        float leftSize = (left * (_halfSizeX * 2)) + _halfSizeX;
        float rightSize = (right * (_halfSizeX * 2)) + _halfSizeX;
        float topSize = (up * (_halfSizeY * 2)) + _halfSizeY;
        float bottomSize = (down * (_halfSizeY * 2)) + _halfSizeY;

        float posX = transform.position.x;
        float posY = transform.position.y;

        _topLeft = new Vector3(posX - leftSize, posY + topSize);
        _topRight = new Vector3(posX + rightSize, posY + topSize);
        _bottomLeft = new Vector3(posX - leftSize, posY - bottomSize);
        _bottomRight = new Vector3(posX + rightSize, posY - bottomSize);

        leftEdge = _topLeft.x + _halfSizeX;
        rightEdge = _topRight.x - _halfSizeX;
        topEdge = _topLeft.y - _halfSizeY;
        bottomEdge = _bottomLeft.y + _halfSizeY;

        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(_topLeft, _topRight);
        Gizmos.DrawLine(_bottomLeft, _bottomRight);
        Gizmos.DrawLine(_topLeft, _bottomLeft);
        Gizmos.DrawLine(_topRight, _bottomRight);
    }
}
