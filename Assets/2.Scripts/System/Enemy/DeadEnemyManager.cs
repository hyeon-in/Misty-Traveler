using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

/// <summary>
/// ���� ���̳� ������ �����ϱ� ���� ���� Ŭ�����Դϴ�.
/// ���� ���� ������ ������ �� ��, �÷��̾ ����� �� ��Ȱ�� ��, �÷��̾ üũ����Ʈ���� �޽��� ���� ��Ȱ�մϴ�.
/// </summary>
public static class DeadEnemyManager
{
    /// <summary>
    /// ���� ���� �����͸� ��� �ִ� ����ü�Դϴ�.
    /// </summary>
    struct DeadEnemy
    {
        int _sceneIndex;     // ���� ���� ��ġ�� ��, ���� �ε��� ������ �޾ƿ�
        string _enemyName;   // ���� �̸�(��Ȯ���� ���� ������Ʈ �̸�)

        public DeadEnemy(int sceneIndex, string enemyName)
        {
            _sceneIndex = sceneIndex;
            _enemyName = enemyName;
        }
    }
    static HashSet<DeadEnemy> _deadEnemies = new HashSet<DeadEnemy>();  // ���� ���� ����ü�� �����ϴ� �ؽ�
    static HashSet<string> _deadBosses = new HashSet<string>();         // ���� ������ String���� �����ϴ� �ؽ�

    /// <summary>
    /// ���� ���� �ؽÿ� �߰��ϴ� ���� �޼ҵ��Դϴ�.
    /// �ؽÿ� �߰��� ���� ��Ȱ�ϱ� �� ���� �������� �ʽ��ϴ�.
    /// </summary>
    /// <param name="enemyName">���� ���� �̸�(������Ʈ �̸�)<//param>
    public static void AddDeadEnemy(string enemyName)
    {
        // ���� ���� �ε����� ������
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        _deadEnemies.Add(new DeadEnemy(sceneIndex, enemyName));
    }

    /// <summary>
    /// ���� �̸�(������Ʈ �̸�)�� �̿��ؼ� �ش� ���� �̹� �׾��� ������ �ؽÿ��� ã�� ���� �޼ҵ��Դϴ�.
    /// ���� �ؽÿ� �ش� ���� �̸��� �ִٸ� true�� �����մϴ�.
    /// </summary>
    /// <param name="enemyName">���� �̸�(������Ʈ �̸�)</param>
    public static bool IsDeadEnemy(string enemyName)
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        DeadEnemy deadEnemy = new DeadEnemy(sceneIndex, enemyName);
        return _deadEnemies.Contains(deadEnemy);
    }

    /// <summary>
    /// ���� ������ ��Ƴ��� �ؽø� �ʱ�ȭ�ϴ� ���� �޼ҵ��Դϴ�.
    /// </summary>
    public static void ClearDeadEnemies() => _deadEnemies.Clear();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bossName"></param>
    public static void AddDeadBoss(string bossName) => _deadBosses.Add(bossName);


    /// <summary>
    /// ������ �̸�(key name)�� �̿��ؼ� �ش� ������ �̹� �׾��� �������� �ؽÿ��� ã�� ���� �޼ҵ��Դϴ�.
    /// ���� �ؽÿ� �ش� ������ �̸��� �ִٸ� true�� �����մϴ�.
    /// </summary>
    /// <param name="bossName">������ �̸�(key name)</param>
    public static bool IsDeadBoss(string bossName) => _deadBosses.Contains(bossName);

    /// <summary>
    /// ���� ������ ��Ƴ��� �ؽø� �ʱ�ȭ�ϴ� ���� �޼ҵ��Դϴ�.
    /// </summary>
    public static void ClearDeadBosses() => _deadBosses.Clear();

    /// <summary>
    /// ���� ������ ��Ƴ��� �ؽø� string ����Ʈ�� �������� ���� �޼ҵ��Դϴ�.
    /// </summary>
    /// <returns>���� �������� string ����Ʈ</returns>
    public static List<string> GetDeadBosses() => _deadBosses.ToList();
}