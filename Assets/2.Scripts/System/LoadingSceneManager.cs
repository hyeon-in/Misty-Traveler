using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬 불러오기 및 로딩과 관련된 기능을 처리하기 위한 클래스입니다.
/// </summary>
public class LoadingSceneManager : MonoBehaviour 
{ 
    public static string nextScene; // 다음에 불러오려는 씬

    void Start() 
    { 
        // 로딩 시작
        StartCoroutine(LoadSceneCoroutine()); 
    } 
    
    /// <summary>
    /// 씬의 이름을 이용해서 씬을 불러오는 메소드입니다.
    /// 씬을 불러오기 전에 로딩 씬으로 이동되며, GC를 호출합니다.
    /// </summary>
    /// <param name="sceneName">불러오려는 씬의 이름</param>
    public static void LoadScene(string sceneName)
    { 
        nextScene = sceneName; 
        SceneManager.LoadScene("LoadingScene");
        System.GC.Collect();
    } 
    
    /// <summary>
    /// 로딩 씬에서 게임 로딩을 
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadSceneCoroutine() 
    { 
        // 1프레임 대기
        yield return null; 

        // 다음 씬을 비동기적으로 로드함
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(nextScene);
        asyncOperation.allowSceneActivation = false;

        // 비동기 작업이 완료될 때까지 반복
        while(!asyncOperation.isDone)
        {
            yield return null;

            // 씬 로딩 진행률이 0.9 이상이면 실행
            if(asyncOperation.progress >= 0.9f)
            {
                // Time.timescale 복구 및 씬 활성화 허용
                Time.timeScale = 1f;
                asyncOperation.allowSceneActivation = true;
            }            
        }
    } 
}