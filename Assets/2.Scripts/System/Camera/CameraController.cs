using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ī�޶� �����ϱ� ���� Ŭ������, ī�޶� Ư�� ����� �ε巴�� �����ϴ� ��ɰ� ī�޶��� �̵� ��踦 �����ϴ� ����� ���ԵǾ� �ֽ��ϴ�.
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField] float _minSmoothTime = 6f;
    [SerializeField] float _maxSmoothTime = 8f;

    [SerializeField] float _dampingX = 2f;
    [SerializeField] float _dampingY = 5f;

    [SerializeField] Transform _target;         // ī�޶� �����Ϸ��� ���
    [SerializeField] CameraRange _cameraRange;  // ī�޶��� ���

    bool _isShaked;                  // ī�޶� ��鸮�� �ִ� �������� üũ�ϴ� bool ����
    float _currentSmoothTime;
    Transform _cameraTransform;

    void Awake()
    {
        _cameraTransform = Camera.main.transform;

        // _cameraRange�� null�� ��� CameraRange��� ������Ʈ�� ã�ƺ���
        if(_cameraRange == null && GameObject.Find("CameraRange").TryGetComponent(out CameraRange cameraRange))
        {
            _cameraRange = cameraRange;
        }

        // _target�� null�� ��� �ڵ����� Player�� Ÿ������ ���Ѵ�
        if(_target == null)
        {
            _target = GameObject.Find("Player").GetComponent<Transform>();
        }

        // ī�޶��� �ʱ� ��ǥ�� _target�� ��ǥ�� ����
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
        // ���� ȭ���� ��鸮�� �ִٸ� �������� ����
        if(_isShaked) return;

        // ��� ��ġ�� �Ÿ� ���
        var targetPos = new Vector3(_target.position.x, _target.position.y, _cameraTransform.position.z);
        float distance = Vector3.Distance(_cameraTransform.position, targetPos);

        // ������ �Ÿ��� ������� �̵��� ����� �ð��� ����� �� ī�޶��� ��ġ�� ����
        _currentSmoothTime = Mathf.Clamp(distance, _minSmoothTime, _maxSmoothTime);
        _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, targetPos, _currentSmoothTime * Time.deltaTime);
        
        // �Ÿ��� ���� �̻� �̳��̸� ��ǥ�� ��� ��ġ�� ����
        if(distance < 0.05f)
        {
            _cameraTransform.position = targetPos;
        }

        // ���� ����
        ApplyDamping(targetPos);

        // ī�޶��� ��ġ�� ������ ������ ����
        if (_cameraRange != null)
        {
            ClampCameraPosition();
        }
    }

    /// <summary>
    /// ������ �����ϴ� �޼ҵ��Դϴ�.
    /// </summary>
    /// <param name="targetPos">ī�޶� �����ϴ� ����� ��ǥ</param>
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
    /// ī�޶��� ��ǥ�� Ư�� ������ �Ѿ�� �ʵ��� �����ϴ� �޼ҵ��Դϴ�.
    /// </summary>
    void ClampCameraPosition()
    {
        float xEdge = Mathf.Clamp(_cameraTransform.position.x, _cameraRange.leftEdge, _cameraRange.rightEdge);
        float yEdge = Mathf.Clamp(_cameraTransform.position.y, _cameraRange.bottomEdge, _cameraRange.topEdge);

        _cameraTransform.position = new Vector3(xEdge, yEdge, _cameraTransform.position.z);
    }

    /// <summary>
    /// ȭ���� ��鸲�� ���۵��� ��� _isShaked�� true�� �����ϴ� �ݹ� �޼ҵ��Դϴ�.
    /// </summary>
    void OnStartShake() => _isShaked = true;
    /// <summary>
    /// ȭ���� ��鸲�� ������ ��� _isShaked�� false�� �����ϴ� �ݹ� �޼ҵ� �Դϴ�.
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