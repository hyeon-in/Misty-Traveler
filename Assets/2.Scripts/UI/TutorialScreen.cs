using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using System;

/// <summary>
/// 게임 내의 튜토리얼들을 관리하는 정적 클래스입니다.
/// </summary>
public static class TutorialManager
{
    /// <summary>
    /// 튜토리얼들을 나열한 열거형
    /// </summary>
    public enum Tutorial
    {
        MoveAndJump,
        Attack,
        SpecialAttack,
        Dodge,
        Map
    }

    static List<Tutorial> seenTutorials = new List<Tutorial>(); // 이미 본 튜토리얼들을 담아놓은 리스트

    /// <summary>
    /// 튜토리얼을 플레이어의 행동과 키를 담은 클래스로 변환하여 가져오는 정적 메소드입니다.
    /// </summary>
    /// <param name="tutorial">행동과 키를 받아오기 위해 사용하는 튜토리얼</param>
    /// <returns>해당 튜토리얼에 해당하는 행동과 키</returns>
    public static ActionsAndKey[] GetActionsAndKeysForTutorial(Tutorial tutorial)
    {
        List<ActionsAndKey> actionsAndKey = new List<ActionsAndKey>();
        GameInputManager.PlayerActions[] actions = new GameInputManager.PlayerActions[1];
        string key;

        if(tutorial == Tutorial.MoveAndJump)
        {
            // 이동과 점프를 알려주는 튜토리얼

            // 좌우 이동
            actions = new GameInputManager.PlayerActions[]
            {
                    GameInputManager.PlayerActions.MoveLeft,
                    GameInputManager.PlayerActions.MoveRight
            };
            key = "Move";
            actionsAndKey.Add(new ActionsAndKey(actions, key));

            // 점프
            actions = new GameInputManager.PlayerActions[]
            {
                    GameInputManager.PlayerActions.Jump
            };
            key = "Jump";
            actionsAndKey.Add(new ActionsAndKey(actions, key));
        }
        else
        {
            // 그 외
            switch (tutorial)
            {
                case Tutorial.Attack: // 공격 튜토리얼
                    actions[0] = GameInputManager.PlayerActions.Attack;
                    break;
                case Tutorial.SpecialAttack: // 스페셜 튜토리얼
                    actions[0] = GameInputManager.PlayerActions.SpecialAttack;
                    break;
                case Tutorial.Dodge: // 회피 튜토리얼
                    actions[0] = GameInputManager.PlayerActions.Dodge;
                    break;
                case Tutorial.Map: // 지도 튜토리얼
                    actions[0] = GameInputManager.PlayerActions.Map;
                    break;
            }
            key = tutorial.ToString();
            actionsAndKey.Add(new ActionsAndKey(actions, key));
        }
        return actionsAndKey.ToArray();
    }

    /// <summary>
    /// 이미 본 적이 있는 튜토리얼인지 체크하기 위한 정적 메소드입니다.
    /// </summary>
    /// <param name="tutorial">본 적이 있는지 체크하기 위한 튜토리얼</param>
    /// <returns>이미 본 튜토리얼 여부</returns>
    public static bool HasSeenTutorial(Tutorial tutorial) => seenTutorials.Contains(tutorial);

    /// <summary>
    /// Tutorial 열거형을 사용하여 이미 본 튜토리얼 리스트에 추가하는 정적 메소드입니다.
    /// </summary>
    /// <param name="tutorial">본 적이 있는 튜토리얼(열거형)</param>
    public static void AddSeenTutorial(Tutorial tutorial) => seenTutorials.Add(tutorial);

    /// <summary>
    /// string을 사용하여 이미 본 튜토리얼 리스트에 추가하는 정적 메소드입니다.
    /// </summary>
    /// <param name="tutorial">본 적이 있는 튜토리얼(string)</param>
    public static void AddSeenTutorial(string tutorial)
    {
        Tutorial stringToTutorial = (Tutorial)Enum.Parse(typeof(Tutorial), tutorial);
        seenTutorials.Add(stringToTutorial);
    }

    /// <summary>
    /// 이미 본 튜토리얼 리스트를 가져오는 정적 메소드입니다.
    /// </summary>
    /// <returns>이미 본 튜토리얼 리스트</returns>
    public static List<Tutorial> GetSeenTutorials() => seenTutorials;

    /// <summary>
    /// 이미 본 튜토리얼들을 string 리스트로 가져오는 정적 메소드입니다.
    /// </summary>
    /// <returns>이미 본 튜토리얼 리스트(string)</returns>
    public static List<string> GetSeenTutorialsToString()
    {
        List<string> tutorialToString = new List<string>();
        for(int i = 0; i < seenTutorials.Count; i++)
        {
            tutorialToString.Add(seenTutorials[i].ToString());
        }
        return tutorialToString;
    }

    /// <summary>
    /// 이미 본 튜토리얼 리스트를 비우는 정적 메소드입니다.
    /// </summary>
    public static void SeenTutorialClear() => seenTutorials.Clear();
}

/// <summary>
/// 플레이어의 행동 배열과 string 키 데이터를 담아두기 위한 클래스입니다.
/// </summary>
[System.Serializable]
public class ActionsAndKey
{
    public GameInputManager.PlayerActions[] action;
    public string key;
    
    public ActionsAndKey(GameInputManager.PlayerActions[] action, string key)
    {
        this.action = action;
        this.key = key;
    }
}

/// <summary>
/// 게임 상에서 출력되는 튜토리얼 화면을 처리하는 클래스입니다.
/// </summary>
public class TutorialScreen : MonoBehaviour
{
    public TextMeshProUGUI explain;         // 설명 텍스트
    IEnumerator _tutorialCoroutine = null;  // 튜토리얼 코루틴

    /// <summary>
    /// 튜토리얼을 코루틴을 실행하는 메소드입니다.
    /// </summary>
    /// <param name="tutorial">화면에 띄울려는 튜토리얼</param>
    public void TotorialStart(TutorialManager.Tutorial tutorial)
    {
        // 튜토리얼 열거형을 매개변수로 사용하여 화면에 표시하려는 행동과 키를 가져옵니다.
        ActionsAndKey[] actionsAndKey = TutorialManager.GetActionsAndKeysForTutorial(tutorial);
        if(_tutorialCoroutine != null)
        {
            // 이미 화면 상으로 보여지던 튜토리얼은 중단합니다.
            StopCoroutine(_tutorialCoroutine);
            _tutorialCoroutine = null;
        }
        _tutorialCoroutine = TutorialCoroutine(actionsAndKey);
        StartCoroutine(_tutorialCoroutine);
    }

    /// <summary>
    /// 튜토리얼을 화면 상에 표시하는 코루틴입니다.
    /// </summary>
    /// <param name="actionsAndKey">화면에 표시하려는 행동과 키</param>
    IEnumerator TutorialCoroutine(ActionsAndKey[] actionsAndKey)
    {
        yield return null;

        // 스트링 빌더에 액션과 키들을 텍스트로 추가한 후 텍스트에 담은 뒤, 화면에 8초동안 출력한다.
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < actionsAndKey.Length; i++)
        {
            for(int j = 0; j < actionsAndKey[i].action.Length; j++)
            {
                string action = GameInputManager.usingController ? GameInputManager.ActionToButtonText(actionsAndKey[i].action[j])
                                                                : GameInputManager.ActionToKeyText(actionsAndKey[i].action[j]);
                sb.AppendFormat("[ <color=#ffaa5e>{0}</color> ]", action);
                sb.Append(" ");
            }
            string key = LanguageManager.GetText(actionsAndKey[i].key);
            sb.Append(key);
            if(i < actionsAndKey.Length - 1)
            {
                sb.Append(" ");
            }
        }
        explain.text = sb.ToString();
        yield return YieldInstructionCache.WaitForSeconds(8.0f);
        
        // 튜토리얼이 끝난 후 화면 상에서 사라짐
        explain.text = "";
        _tutorialCoroutine = null;
    }
}
