using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

/// <summary>
/// 죽은 적이나 보스를 관리하기 위한 정적 클래스입니다.
/// 죽은 적은 게임을 재접속 할 때, 플레이어가 사망한 후 부활할 때, 플레이어가 체크포인트에서 휴식할 때에 부활합니다.
/// </summary>
public static class DeadEnemyManager
{
    /// <summary>
    /// 죽은 적의 데이터를 담고 있는 구조체입니다.
    /// </summary>
    struct DeadEnemy
    {
        int _sceneIndex;     // 죽은 적이 위치한 씬, 씬은 인덱스 값으로 받아옴
        string _enemyName;   // 적의 이름(정확히는 적의 오브젝트 이름)

        public DeadEnemy(int sceneIndex, string enemyName)
        {
            _sceneIndex = sceneIndex;
            _enemyName = enemyName;
        }
    }
    static HashSet<DeadEnemy> _deadEnemies = new HashSet<DeadEnemy>();  // 죽은 적을 구조체로 보관하는 해시
    static HashSet<string> _deadBosses = new HashSet<string>();         // 죽은 보스를 String으로 보관하는 해시

    /// <summary>
    /// 죽은 적을 해시에 추가하는 정적 메소드입니다.
    /// 해시에 추가된 적은 부활하기 전 까지 등장하지 않습니다.
    /// </summary>
    /// <param name="enemyName">죽은 적의 이름(오브젝트 이름)<//param>
    public static void AddDeadEnemy(string enemyName)
    {
        // 현재 씬을 인덱스로 가져옴
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        _deadEnemies.Add(new DeadEnemy(sceneIndex, enemyName));
    }

    /// <summary>
    /// 적의 이름(오브젝트 이름)을 이용해서 해당 적이 이미 죽었던 적인지 해시에서 찾는 정적 메소드입니다.
    /// 만약 해시에 해당 적의 이름이 있다면 true를 리턴합니다.
    /// </summary>
    /// <param name="enemyName">적의 이름(오브젝트 이름)</param>
    public static bool IsDeadEnemy(string enemyName)
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        DeadEnemy deadEnemy = new DeadEnemy(sceneIndex, enemyName);
        return _deadEnemies.Contains(deadEnemy);
    }

    /// <summary>
    /// 죽은 적들을 담아놓은 해시를 초기화하는 정적 메소드입니다.
    /// </summary>
    public static void ClearDeadEnemies() => _deadEnemies.Clear();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bossName"></param>
    public static void AddDeadBoss(string bossName) => _deadBosses.Add(bossName);


    /// <summary>
    /// 보스의 이름(key name)을 이용해서 해당 보스가 이미 죽었던 보스인지 해시에서 찾는 정적 메소드입니다.
    /// 만약 해시에 해당 보스의 이름이 있다면 true를 리턴합니다.
    /// </summary>
    /// <param name="bossName">보스의 이름(key name)</param>
    public static bool IsDeadBoss(string bossName) => _deadBosses.Contains(bossName);

    /// <summary>
    /// 죽은 보스를 담아놓은 해시를 초기화하는 정적 메소드입니다.
    /// </summary>
    public static void ClearDeadBosses() => _deadBosses.Clear();

    /// <summary>
    /// 죽은 보스를 담아놓은 해시를 string 리스트로 가져오는 정적 메소드입니다.
    /// </summary>
    /// <returns>죽은 보스들의 string 리스트</returns>
    public static List<string> GetDeadBosses() => _deadBosses.ToList();
}