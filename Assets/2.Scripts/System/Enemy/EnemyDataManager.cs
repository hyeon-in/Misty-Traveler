using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적 데이터를 초기화하여 Dictionary에 삽입하고, 이를 다른 클래스가 사용할 수 있도록 하는 적 데이터 관리 클래스입니다.
/// </summary>
public static class EnemyDataManager
{
    static bool hasInitialized = false;
    static Dictionary<string, EnemyData> enemiesData = new Dictionary <string, EnemyData>();  // 적 데이터 딕셔너리

    /// <summary>
    /// EnemyDataManager를 처음 호출할 때 자동으로 초기화
    /// </summary>
    static EnemyDataManager()
    {
        Init();
    }

    /// <summary>
    /// EnemyDataManager를 초기화하는 정적 메소드입니다.
    /// 이미 초기화를 했을 경우 초기화되지 않습니다.
    /// </summary>
    public static void Init()
    {
        if(hasInitialized) return;
        EnemyDataInit();
        hasInitialized = true;
    }

    /// <summary>
    /// 적 데이터를 담아놓은 CSV 파일에서 데이터들을 가져온 뒤 enemiesData 딕셔너리에 삽입하는 정적 메소드입니다.
    /// </summary>
    static void EnemyDataInit()
    {
        // Resources 폴더에서 데이터를 가져옵니다.
        List<Dictionary<string, object>> enemyDataList = CSVReader.Read("ActorData/EnemyData");
        Material blinkMaterial = (Material)Resources.Load("BlinkMaterial");

        for (var i = 0; i < enemyDataList.Count; i++)
        {
            // 새로운 EnemyData 클래스 생성
            var newEnemyData = (EnemyData)ScriptableObject.CreateInstance(typeof(EnemyData));

            // EnemyData 클래스에 값 대입
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

            // enemiesData 딕셔너리에 keyName을 키로 사용하고, 새로운 EnemyData 클래스를 값에 대입합니다.
            enemiesData.Add(keyName, newEnemyData);
        }
    }

    /// <summary>
    /// key를 사용하여 enemiesData 딕셔너리에서 원하는 적 데이터를 가져옵니다.
    /// </summary>
    /// <param name="keyName">가져오려는 적 데이터의 key</param>
    /// <returns>enemiesData에서 찾은 적 데이터 클래스</returns>
    public static EnemyData GetEnemyData(string keyName) => enemiesData[keyName];
}