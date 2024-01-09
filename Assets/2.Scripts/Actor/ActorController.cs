using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 내의 캐릭터들의 충돌이나 좌표 이동을 처리하는 클래스입니다.
/// </summary>
public class ActorController : MonoBehaviour
{
    // 충돌 관련
    [Header("Collision")]

    // 충돌체 사용 설정
    [SerializeField] bool _useCollider = true;
    [SerializeField] float _skinWidth = 0.001f;
    // 레이 갯수
    [SerializeField] int _rayCountX = 3;
    [SerializeField] int _rayCountY = 2;        
    // 충돌체 원점 및 크기
    [SerializeField] Vector2 _colliderCenter = Vector2.zero;
    [SerializeField] Vector2 _colliderSize = Vector2.one;

    [Header("Gravity")]
    [SerializeField] bool _useGravity = true;    // 중력 사용 여부
    [SerializeField] float _gravityScale = 140f; // 중력 크기
    [SerializeField] float _maxFallSpeed = 40f;  // 최대 낙하 속도

    float _raySpacingX, _raySpacingY;  // 레이 간격   
                                       
    float _velocityX, _velocityY;      // 속도 변수
    float _movePosX, _movePosY;        // 실제 이동을 처리하기 위한 변수
    float _slideVelocity, _slideDirection, _slideDeceleration;  // 미끄러지는 움직임을 처리하기 위한 변수

    float _deltaTime;                  // Time.deltaTime을 캐싱하는 변수 

    bool _isRoofed, _isGrounded, _isLeftWalled, _isRightWalled; // 충돌 상태 체크

    Vector2 _leftRayOrigin, _rightRayOrigin, _topRayOrigin, _bottomRayOrigin;    // 충돌체 원점 좌표
    Vector2 _colliderSizeStore; // 충돌체 크기 저장

    LayerMask _groundLayer;  // 땅 레이어

    Transform _actorTransform;  // 배우의 트랜스폼

    // 충돌 상태 가져오기
    public bool IsRoofed => _isRoofed;
    public bool IsGrounded => _isGrounded;
    public bool IsLeftWalled => _isLeftWalled;
    public bool IsRightWalled => _isRightWalled;
    public bool IsWalled => _isLeftWalled || _isRightWalled;

    // 중력 사용 여부 프로퍼티
    public bool UseGravity
    {
        get
        {
            return _useGravity;
        }
        set
        {
            _useGravity = value;

            if (!value)
            {
                _velocityY = 0;
            }
        }
    }
    
    // 중력 관련 프로퍼티
    public float GravityScale { get => _gravityScale; set => _gravityScale = value; }
    public float MaxFallSpeed { get => _maxFallSpeed; set => _maxFallSpeed = value; }

    // 속도 관련 프로파티
    public float VelocityX { get => _velocityX; set => _velocityX = value; }
    public float VelocityY { get => _velocityY; set => _velocityY = value; }
    public bool IsSliding => _slideVelocity > 0;

    void Start()
    {
        _groundLayer = LayerMask.GetMask("Ground");
        _actorTransform = GetComponent<Transform>();

        // 충돌체의 크기가 x나 y중 하나라도 0일 경우 충돌체와 중력을 사용하지 않는 상태로 설정
        if (_colliderSize.x <= 0 || _colliderSize.y <= 0)
        {
            _useCollider = _useGravity = false;
        }
    }

    void Update()
    {
        // Time.deltaTime 캐싱
        _deltaTime = Time.deltaTime;

        // 중력을 사용하는 상태일 경우 중력 적용
        if (_useGravity)
        {
            HandleGravity();
        }

        // Velocity를 movePos로 변경
        _movePosX = (_velocityX + (_slideVelocity * _slideDirection)) * _deltaTime;
        _movePosY = _velocityY * _deltaTime;

        // 충돌체를 사용하는 상태일 경우 충돌체 업데이트 및 충돌 처리
        if (_useCollider)
        {
            ColliderUpdate();
            HandleCollision();
        }
        // 배우를 movePos만큼 이동시킴
        _actorTransform.Translate(new Vector2(_movePosX, _movePosY));

        // 미끄러지는 속도가 일정 이상일 경우 천천히 감소
        if (_slideVelocity > 3.0f)
        {
            _slideVelocity -= _deltaTime * _slideDeceleration;
            _slideVelocity = Mathf.Clamp(_slideVelocity, 0f, float.MaxValue);
        }
        else
        {
            _slideVelocity = 0;
        }

        // 이동 값을 0으로 설정
        _velocityX = 0;
        _movePosX = _movePosY = 0;
    }

    /// <summary>
    /// 충돌체를 실시간으로 업데이트 하는 메소드입니다.
    /// </summary>
    void ColliderUpdate()
    {
        // 중심점을 구함
        float centerX = _actorTransform.position.x + _colliderCenter.x;
        float centerY = _actorTransform.position.y + _colliderCenter.y;

        // 충돌체의 반쪽 크기를 구함
        var colliderHalfSize = _colliderSize * 0.5f;

        // 충돌체의 최소 좌표와 최대 좌표 가져옴
        var min = new Vector2(centerX - colliderHalfSize.x, centerY - colliderHalfSize.y);
        var max = new Vector2(centerX + colliderHalfSize.x, centerY + colliderHalfSize.y);

        // 레이 원점 갱신 
        _leftRayOrigin = new Vector2(min.x, min.y);
        _rightRayOrigin = new Vector2(max.x, min.y);
        _topRayOrigin = new Vector2(min.x, max.y);
        _bottomRayOrigin = new Vector2(min.x, min.y);
        
        // 충돌체 크기 변경시 레이 간격 갱신
        if(_colliderSizeStore != _colliderSize)
        {
            _raySpacingX = _colliderSize.y / (_rayCountX - 1);
            _raySpacingY = _colliderSize.x / (_rayCountY - 1);
            _colliderSizeStore = _colliderSize;
        }
    }

    /// <summary>
    /// 충돌 처리를 위한 메소드입니다.
    /// </summary>
    void HandleCollision()
    {
        // 수평 충돌
        if (_rayCountX > 0)
        {
            // 왼쪽 및 오른쪽 충돌 체크 후 충돌 여부를 받아옴
            _isLeftWalled = _movePosX <= 0 && RayCollision(_rayCountX, -_movePosX, _raySpacingX, _leftRayOrigin, Vector2.left);
            _isRightWalled = _movePosX >= 0 && RayCollision(_rayCountX, _movePosX, _raySpacingX, _rightRayOrigin, Vector2.right);

            // 충돌했을 경우 X 속도를 0으로 변경
            if ((_movePosX < 0 && _isLeftWalled) || (_movePosX > 0 && _isRightWalled))
            {
                _velocityX = 0;
            }
        }
        // 수직 충돌
        if(_rayCountY > 0)
        {
            // 위쪽 및 아래쪽 충돌 체크 후 충돌 여부를 받아옴
            _isRoofed = _movePosY >= 0 && RayCollision(_rayCountY, _movePosY, _raySpacingY, _topRayOrigin, Vector2.up);
            _isGrounded = _movePosY <= 0 && RayCollision(_rayCountY, -_movePosY, _raySpacingY, _bottomRayOrigin, Vector2.down);

            // 충돌했을 경우 Y 속도를 0으로 변경
            if((_movePosY > 0 && _isRoofed) || (_movePosY < 0 && _isGrounded))
            {
                _velocityY = 0;
            }
        }
    }

    /// <summary>
    /// 레이를 사용하여 오브젝트와의 충돌을 처리하기 위한 메소드입니다.
    /// </summary>
    /// <param name="rayCount">충돌 감지에 사용하려는 레이의 갯수</param>
    /// <param name="rayLength">충돌 감지에 사용하려는 레이의 길이</param>
    /// <param name="raySpacing">충돌 감지에 사용하려는 레이의 간격</param>
    /// <param name="rayOrigin">충돌 감지에 사용하려는 레이의 원점</param>
    /// <param name="rayDirection">충돌 감지에 사용하려는 레이의 방향</param>
    /// <returns>충돌 여부</returns>
    bool RayCollision(int rayCount, float rayLength, float raySpacing, Vector2 rayOrigin, Vector2 rayDirection)
    {
        rayLength += _skinWidth + 0.01f;
        bool isHorizontalCollisionCheck = rayDirection.x != 0;

        // 여러 개의 레이를 쏴서 충돌 처리
        for (int i = 0; i < rayCount; i++)
        {
            Vector2 currentRayOrigin = rayOrigin;
            Vector2 rayOriginDir = isHorizontalCollisionCheck ? Vector2.up : Vector2.right;
            currentRayOrigin += rayOriginDir * (raySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(currentRayOrigin, rayDirection, rayLength, _groundLayer);
#if UNITY_EDITOR
            Debug.DrawRay(currentRayOrigin, rayDirection * rayLength * 3, Color.red);
#endif
            // 레이 충돌을 감지했을 경우 이동값을 알맞게 변경하고 true 반환
            if(hit)
            {
                if(isHorizontalCollisionCheck)
                {
                    _movePosX = (hit.distance - _skinWidth) * rayDirection.x;
                }
                else
                {
                    _movePosY = (hit.distance - _skinWidth) * rayDirection.y;
                }
                return true;
            }
        }

        // 충돌이 없을 경우 false 반환
        return false;
    }

    /// <summary>
    /// 중력을 처리하는 메소드입니다.
    /// </summary>
    void HandleGravity()
    {
        if(_isGrounded) return;

        _velocityY -= _gravityScale * _deltaTime;
        _velocityY = Mathf.Clamp(_velocityY, -_maxFallSpeed, float.MaxValue);
    }

    /// <summary>
    /// 미끄러지는 움직임을 실행하기 위한 메소드입니다.
    /// </summary>
    /// <param name="velocity">미끄러지는 속도</param>
    /// <param name="slideDirection">미끄러지는 방향</param>
    /// <param name="slideDeceleration">감속(기본값 = 50)</param>
    public void SlideMove(float velocity, float slideDirection, float slideDeceleration = 50f)
    {
        _slideVelocity = velocity;
        _slideDirection = slideDirection;
        _slideDeceleration = slideDeceleration;
    }

    /// <summary>
    /// 미끄러지는 움직임을 중단하는 메소드입니다.
    /// </summary>
    public void SlideCancle()
    {
        _slideVelocity = 0;
    }


    void OnDrawGizmosSelected()
    {
        float offsetX = transform.position.x + _colliderCenter.x;
        float offsetY = transform.position.y + _colliderCenter.y;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector2(offsetX, offsetY), _colliderSize);
    }
}