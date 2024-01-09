using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �� �ҷ����� �� �ε��� ���õ� ����� ó���ϱ� ���� Ŭ�����Դϴ�.
/// </summary>
public class LoadingSceneManager : MonoBehaviour 
{ 
    public static string nextScene; // ������ �ҷ������� ��

    void Start() 
    { 
        // �ε� ����
        StartCoroutine(LoadSceneCoroutine()); 
    } 
    
    /// <summary>
    /// ���� �̸��� �̿��ؼ� ���� �ҷ����� �޼ҵ��Դϴ�.
    /// ���� �ҷ����� ���� �ε� ������ �̵��Ǹ�, GC�� ȣ���մϴ�.
    /// </summary>
    /// <param name="sceneName">�ҷ������� ���� �̸�</param>
    public static void LoadScene(string sceneName)
    { 
        nextScene = sceneName; 
        SceneManager.LoadScene("LoadingScene");
        System.GC.Collect();
    } 
    
    /// <summary>
    /// �ε� ������ ���� �ε��� 
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadSceneCoroutine() 
    { 
        // 1������ ���
        yield return null; 

        // ���� ���� �񵿱������� �ε���
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(nextScene);
        asyncOperation.allowSceneActivation = false;

        // �񵿱� �۾��� �Ϸ�� ������ �ݺ�
        while(!asyncOperation.isDone)
        {
            yield return null;

            // �� �ε� ������� 0.9 �̻��̸� ����
            if(asyncOperation.progress >= 0.9f)
            {
                // Time.timescale ���� �� �� Ȱ��ȭ ���
                Time.timeScale = 1f;
                asyncOperation.allowSceneActivation = true;
            }            
        }
    } 
}