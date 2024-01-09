using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카메라를 제어하기 위한 클래스로, 카메라가 특정 대상을 부드럽게 추적하는 기능과 카메라의 이동 경계를 제어하는 기능이 포함되어 있습니다.
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField] float _minSmoothTime = 6f;
    [SerializeField] float _maxSmoothTime = 8f;

    [SerializeField] float _dampingX = 2f;
    [SerializeField] float _dampingY = 5f;

    [SerializeField] Transform _target;         // 카메라가 추적하려는 대상
    [SerializeField] CameraRange _cameraRange;  // 카메라의 경계

    bool _isShaked;                  // 카메라가 흔들리고 있는 상태인지 체크하는 bool 변수
    float _currentSmoothTime;
    Transform _cameraTransform;

    void Awake()
    {
        _cameraTransform = Camera.main.transform;

        // _cameraRange가 null일 경우 CameraRange라는 오브젝트를 찾아본다
        if(_cameraRange == null && GameObject.Find("CameraRange").TryGetComponent(out CameraRange cameraRange))
        {
            _cameraRange = cameraRange;
        }

        // _target이 null일 경우 자동으로 Player를 타겟으로 정한다
        if(_target == null)
        {
            _target = GameObject.Find("Player").GetComponent<Transform>();
        }

        // 카메라의 초기 좌표를 _target의 좌표로 설정
        _cameraTransform.position = new Vector3(_target.position.x, 
                                                _target.position.y, 
                                                _cameraTransform.position.z);
    }

    void Start()
    {
        ScreenEffect.instance.StartShake += OnStartShake;
        ScreenEffect.instance.EndShake += OnEndShake;
    }

    void LateUpdate()
    {
        // 현재 화면이 흔들리고 있다면 실행하지 않음
        if(_isShaked) return;

        // 대상 위치와 거리 계산
        var targetPos = new Vector3(_target.position.x, _target.position.y, _cameraTransform.position.z);
        float distance = Vector3.Distance(_cameraTransform.position, targetPos);

        // 대상과의 거리를 기반으로 이동에 사용할 시간을 계산한 뒤 카메라의 위치를 보간
        _currentSmoothTime = Mathf.Clamp(distance, _minSmoothTime, _maxSmoothTime);
        _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, targetPos, _currentSmoothTime * Time.deltaTime);
        
        // 거리가 일정 이상 이내이면 좌표를 대상 위치로 설정
        if(distance < 0.05f)
        {
            _cameraTransform.position = targetPos;
        }

        // 댐핑 적용
        ApplyDamping(targetPos);

        // 카메라의 위치를 지정된 범위로 제한
        if (_cameraRange != null)
        {
            ClampCameraPosition();
        }
    }

    /// <summary>
    /// 댐핑을 적용하는 메소드입니다.
    /// </summary>
    /// <param name="targetPos">카메라가 추적하는 대상의 좌표</param>
    void ApplyDamping(Vector3 targetPos)
    {
        if (targetPos.x > _cameraTransform.position.x + _dampingX)
        {
            _cameraTransform.position = new Vector3(targetPos.x - _dampingX, _cameraTransform.position.y, _cameraTransform.position.z);
        }
        else if (targetPos.x < _cameraTransform.position.x - _dampingX)
        {
            _cameraTransform.position = new Vector3(targetPos.x + _dampingX, _cameraTransform.position.y, _cameraTransform.position.z);
        }

        if (targetPos.y > _cameraTransform.position.y + _dampingY)
        {
            _cameraTransform.position = new Vector3(_cameraTransform.position.x, targetPos.y - _dampingY, _cameraTransform.position.z);
        }
        else if (targetPos.y < _cameraTransform.position.y - _dampingY)
        {
            _cameraTransform.position = new Vector3(_cameraTransform.position.x, targetPos.y + _dampingY, _cameraTransform.position.z);
        }
    }

    /// <summary>
    /// 카메라의 좌표가 특정 범위를 넘어서지 않도록 제한하는 메소드입니다.
    /// </summary>
    void ClampCameraPosition()
    {
        float xEdge = Mathf.Clamp(_cameraTransform.position.x, _cameraRange.leftEdge, _cameraRange.rightEdge);
        float yEdge = Mathf.Clamp(_cameraTransform.position.y, _cameraRange.bottomEdge, _cameraRange.topEdge);

        _cameraTransform.position = new Vector3(xEdge, yEdge, _cameraTransform.position.z);
    }

    /// <summary>
    /// 화면의 흔들림이 시작됐을 경우 _isShaked를 true로 설정하는 콜백 메소드입니다.
    /// </summary>
    void OnStartShake() => _isShaked = true;
    /// <summary>
    /// 화면의 흔들림이 끝났을 경우 _isShaked를 false로 설정하는 콜백 메소드 입니다.
    /// </summary>
    void OnEndShake() => _isShaked = false;

    private void OnDisable()
    {
        ScreenEffect.instance.StartShake -= OnStartShake;
        ScreenEffect.instance.EndShake -= OnEndShake;
    }

    void OnDrawGizmos()
    {
        if(_cameraRange == null) return;

        float xEdge = Mathf.Clamp(transform.position.x, _cameraRange.leftEdge, _cameraRange.rightEdge);
        float yEdge = Mathf.Clamp(transform.position.y, _cameraRange.bottomEdge, _cameraRange.topEdge);

        transform.position = new Vector3(xEdge, yEdge, transform.position.z);
    }

    void OnDrawGizmosSelected()
    {
        Vector3 top = new Vector3(transform.position.x, transform.position.y + _dampingY);
        Vector3 bottom = new Vector3(transform.position.x, transform.position.y - _dampingY);
        Vector3 left = new Vector3(transform.position.x - _dampingX, transform.position.y);
        Vector3 right = new Vector3(transform.position.x + _dampingX, transform.position.y);

        Gizmos.DrawLine(top, bottom);
        Gizmos.DrawLine(left, right);
    }
}