using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �� �����͸� �ʱ�ȭ�Ͽ� Dictionary�� �����ϰ�, �̸� �ٸ� Ŭ������ ����� �� �ֵ��� �ϴ� �� ������ ���� Ŭ�����Դϴ�.
/// </summary>
public static class EnemyDataManager
{
    static bool hasInitialized = false;
    static Dictionary<string, EnemyData> enemiesData = new Dictionary <string, EnemyData>();  // �� ������ ��ųʸ�

    /// <summary>
    /// EnemyDataManager�� ó�� ȣ���� �� �ڵ����� �ʱ�ȭ
    /// </summary>
    static EnemyDataManager()
    {
        Init();
    }

    /// <summary>
    /// EnemyDataManager�� �ʱ�ȭ�ϴ� ���� �޼ҵ��Դϴ�.
    /// �̹� �ʱ�ȭ�� ���� ��� �ʱ�ȭ���� �ʽ��ϴ�.
    /// </summary>
    public static void Init()
    {
        if(hasInitialized) return;
        EnemyDataInit();
        hasInitialized = true;
    }

    /// <summary>
    /// �� �����͸� ��Ƴ��� CSV ���Ͽ��� �����͵��� ������ �� enemiesData ��ųʸ��� �����ϴ� ���� �޼ҵ��Դϴ�.
    /// </summary>
    static void EnemyDataInit()
    {
        // Resources �������� �����͸� �����ɴϴ�.
        List<Dictionary<string, object>> enemyDataList = CSVReader.Read("ActorData/EnemyData");
        Material blinkMaterial = (Material)Resources.Load("BlinkMaterial");

        for (var i = 0; i < enemyDataList.Count; i++)
        {
            // ���ο� EnemyData Ŭ���� ����
            var newEnemyData = (EnemyData)ScriptableObject.CreateInstance(typeof(EnemyData));

            // EnemyData Ŭ������ �� ����
            string keyName = enemyDataList[i]["Key"].ToString();
            newEnemyData.enemyName = enemyDataList[i]["Name"].ToString();
            newEnemyData.patrolSpeed = (float)enemyDataList[i]["PatrolSpeed"];
            newEnemyData.followSpeed = (float)enemyDataList[i]["FollowSpeed"];
            newEnemyData.isBodyTackled = (bool)enemyDataList[i]["IsBodyTackled"];
            newEnemyData.bodyTackleDamage = (int)enemyDataList[i]["BodyTackleDamage"];
            newEnemyData.detectRange = (float)enemyDataList[i]["DetectRange"];
            newEnemyData.attackDelay = (float)enemyDataList[i]["AttackDelay"];
            newEnemyData.attackDamage = (int)enemyDataList[i]["AttackDamage"];
            newEnemyData.health = (int)enemyDataList[i]["Health"];
            newEnemyData.superArmor = (bool)enemyDataList[i]["SuperArmor"];
            newEnemyData.money = (int)enemyDataList[i]["Money"];
            newEnemyData.blinkMaterial = blinkMaterial;

            // enemiesData ��ųʸ��� keyName�� Ű�� ����ϰ�, ���ο� EnemyData Ŭ������ ���� �����մϴ�.
            enemiesData.Add(keyName, newEnemyData);
        }
    }

    /// <summary>
    /// key�� ����Ͽ� enemiesData ��ųʸ����� ���ϴ� �� �����͸� �����ɴϴ�.
    /// </summary>
    /// <param name="keyName">���������� �� �������� key</param>
    /// <returns>enemiesData���� ã�� �� ������ Ŭ����</returns>
    public static EnemyData GetEnemyData(string keyName) => enemiesData[keyName];
}