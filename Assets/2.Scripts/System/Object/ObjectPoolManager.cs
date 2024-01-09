using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오브젝트 풀의 데이터를 담아놓기 위한 클래스입니다.
/// </summary>
[System.Serializable]
public class PoolObjectData
{
    public string key;          // 키
    public GameObject prefab;   // 게임 오브젝트 프리팹
    public int count = 5;       // 초기 생성 횟수
}

/// <summary>
/// 오브젝트 풀을 보관하고 오브젝트 풀링과 관련된 기능을 처리하는 싱글톤 클래스 입니다.
/// 다른 싱글톤 매니저 클래스들과 달리 해당 오브젝트가 있는 씬에서만 유지됩니다.
/// </summary>
public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance = null;    // 싱글톤 클래스

    [SerializeField] List<PoolObjectData> _poolObjectDataList;  // 오브젝트 풀을 초기화 하기 위한 풀 오브젝트 데이터 리스트
    
    Dictionary<GameObject, string> _keyByPoolObject;        // 풀 오브젝트로 key를 찾기 위한 Dictionary
    Dictionary<string, GameObject> _keyBySampleObject;      // 풀 오브젝트의 스택이 비어있을 경우 해당 Dictionary에서 복제
    Dictionary<string, Stack<GameObject>> _poolObjectByKey; // key로 스택에 저장된 풀 오브젝트를 찾기 위한 Dictionary

    void Awake()
    {
        instance = this; // 싱글톤 클래스

        _keyByPoolObject = new Dictionary<GameObject, string>();
        _keyBySampleObject = new Dictionary<string, GameObject>();
        _poolObjectByKey = new Dictionary<string, Stack<GameObject>>();
        for(int i = 0; i < _poolObjectDataList.Count; i++)
        {
            // 오브젝트 풀 생성
            CreatePool(_poolObjectDataList[i]);
        }
    }

    /// <summary>
    /// 오브젝트 풀을 생성하기 위한 메소드입니다.
    /// </summary>
    /// <param name="data">생성하려는 오브젝트 풀의 데이터</param>
    public void CreatePool(PoolObjectData data)
    {
        string parentObjectName = "Pool <" + data.key + ">";
        var parentObject = GameObject.Find(parentObjectName); // 풀 오브젝트의 부모를 찾음

        GameObject newGameObject = null;
        if(_poolObjectByKey.ContainsKey(data.key))
        {
            // 이미 해당 게임 오브젝트가 있다면 기존에 생성한 오브젝트 풀에 새로운 오브젝트를 삽입하고 실행 종료
#if UNITY_EDITOR
            Debug.Log(data.key + "는 겹치는 키가 있어요");
#endif
            for (int i = 0; i < data.count; i++)
            {
                newGameObject = Instantiate(_keyBySampleObject[data.key], parentObject.transform);
                _keyByPoolObject.Add(newGameObject, data.key);
            }

            return;
        }

        // 부모 오브젝트가 업을 경우 풀 오브젝트들을 쉽게 구분하기 위한 부모 게임 오브젝트 생성
        // 풀 오브젝트는 해당 부모 게임 오브젝트의 하위에 생성
        if (parentObject == null)
        {
            parentObject = new GameObject(parentObjectName);
            parentObject.transform.parent = transform;
        }

        // 풀 오브젝트 생성
        var poolObject = new Stack<GameObject>();
        for (int i = 0; i < data.count; i++)
        {
            newGameObject = Instantiate(data.prefab, parentObject.transform);   // 프리팹으로 게임 오브젝트 생성
            newGameObject.SetActive(false); // 비활성화
            poolObject.Push(newGameObject); // 스택에 게임 오브젝트 삽입
            _keyByPoolObject.Add(newGameObject, data.key);
        }
        _poolObjectByKey.Add(data.key, poolObject);
        _keyBySampleObject.Add(data.key, data.prefab);

#if UNITY_EDITOR
        Debug.Log("오브젝트 이름: " + data.key + "\n 오브젝트 수량: " + parentObject.transform.childCount);
#endif
    }

    /// <summary>
    /// 풀 오브젝트를 가져오는 메소드입니다.
    /// </summary>
    /// <param name="key">가져오려는 오브젝트의 키</param>
    /// <param name="pos">가져오려는 좌표</param>
    /// <param name="scaleX">가져오려는 scaleX</param>
    /// <param name="angle">가져오려는 각도</param>
    /// <returns>풀 오브젝트</returns>
    public GameObject GetPoolObject(string key, Vector2 pos, float scaleX = 1,float angle = 0)
    {
        if(!_poolObjectByKey.TryGetValue(key, out var poolObject))
        {
            // 풀 오브젝트에 해당 key가 존재하지 않을 경우 null을 반환(에러)
#if UNITY_EDITOR
            Debug.Log(key + "라는 오브젝트는 없어요!");
#endif
            return null;
        }

        GameObject getPoolObject;
        if(poolObject.Count > 0)
        {
            // 스택에 들어있는 오브젝트의 수량이 1개 이상이면 Pop함
            getPoolObject = poolObject.Pop();
        }
        else
        {
            // 스택에 오브젝트가 없을 경우 새로운 오브젝트 생성
            #if UNITY_EDITOR
                Debug.Log(key + "의 수가 적어 한 개 추가");
            #endif
            string parentObjectName = "Pool <" + key + ">";
            var parentObject = GameObject.Find(parentObjectName);
            getPoolObject = Instantiate(_keyBySampleObject[key], parentObject.transform);
            _keyByPoolObject.Add(getPoolObject, key);
        }

        // 받아온 메개변수로 값 설정
        getPoolObject.transform.position = new Vector3(pos.x, pos.y, getPoolObject.transform.position.z);
        getPoolObject.transform.rotation = Quaternion.Euler(0, 0, angle);
        getPoolObject.transform.localScale = new Vector3(scaleX, 1, 1);
        getPoolObject.SetActive(true);  // 활성화

        // 풀 오브젝트 반환
        return getPoolObject;
    }

    /// <summary>
    /// 사용이 끝난 풀 오브젝트를 다시 스택에 반환하는 메소드입니다.
    /// 해당 메소드를 실행하면 반환한 게임 오브젝트는 비활성화 됩니다.
    /// </summary>
    /// <param name="returnGameObject">반환하려는 게임 오브젝트입니다.</param>
    public void ReturnPoolObject(GameObject returnGameObject)
    {
        returnGameObject.SetActive(false); // 비활성화
        string key = _keyByPoolObject[returnGameObject];
        _poolObjectByKey[key].Push(returnGameObject);   // 스택에 삽입
    }
}
