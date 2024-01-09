using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using System;

/// <summary>
/// ���� ���� Ʃ�丮����� �����ϴ� ���� Ŭ�����Դϴ�.
/// </summary>
public static class TutorialManager
{
    /// <summary>
    /// Ʃ�丮����� ������ ������
    /// </summary>
    public enum Tutorial
    {
        MoveAndJump,
        Attack,
        SpecialAttack,
        Dodge,
        Map
    }

    static List<Tutorial> seenTutorials = new List<Tutorial>(); // �̹� �� Ʃ�丮����� ��Ƴ��� ����Ʈ

    /// <summary>
    /// Ʃ�丮���� �÷��̾��� �ൿ�� Ű�� ���� Ŭ������ ��ȯ�Ͽ� �������� ���� �޼ҵ��Դϴ�.
    /// </summary>
    /// <param name="tutorial">�ൿ�� Ű�� �޾ƿ��� ���� ����ϴ� Ʃ�丮��</param>
    /// <returns>�ش� Ʃ�丮�� �ش��ϴ� �ൿ�� Ű</returns>
    public static ActionsAndKey[] GetActionsAndKeysForTutorial(Tutorial tutorial)
    {
        List<ActionsAndKey> actionsAndKey = new List<ActionsAndKey>();
        GameInputManager.PlayerActions[] actions = new GameInputManager.PlayerActions[1];
        string key;

        if(tutorial == Tutorial.MoveAndJump)
        {
            // �̵��� ������ �˷��ִ� Ʃ�丮��

            // �¿� �̵�
            actions = new GameInputManager.PlayerActions[]
            {
                    GameInputManager.PlayerActions.MoveLeft,
                    GameInputManager.PlayerActions.MoveRight
            };
            key = "Move";
            actionsAndKey.Add(new ActionsAndKey(actions, key));

            // ����
            actions = new GameInputManager.PlayerActions[]
            {
                    GameInputManager.PlayerActions.Jump
            };
            key = "Jump";
            actionsAndKey.Add(new ActionsAndKey(actions, key));
        }
        else
        {
            // �� ��
            switch (tutorial)
            {
                case Tutorial.Attack: // ���� Ʃ�丮��
                    actions[0] = GameInputManager.PlayerActions.Attack;
                    break;
                case Tutorial.SpecialAttack: // ����� Ʃ�丮��
                    actions[0] = GameInputManager.PlayerActions.SpecialAttack;
                    break;
                case Tutorial.Dodge: // ȸ�� Ʃ�丮��
                    actions[0] = GameInputManager.PlayerActions.Dodge;
                    break;
                case Tutorial.Map: // ���� Ʃ�丮��
                    actions[0] = GameInputManager.PlayerActions.Map;
                    break;
            }
            key = tutorial.ToString();
            actionsAndKey.Add(new ActionsAndKey(actions, key));
        }
        return actionsAndKey.ToArray();
    }

    /// <summary>
    /// �̹� �� ���� �ִ� Ʃ�丮������ üũ�ϱ� ���� ���� �޼ҵ��Դϴ�.
    /// </summary>
    /// <param name="tutorial">�� ���� �ִ��� üũ�ϱ� ���� Ʃ�丮��</param>
    /// <returns>�̹� �� Ʃ�丮�� ����</returns>
    public static bool HasSeenTutorial(Tutorial tutorial) => seenTutorials.Contains(tutorial);

    /// <summary>
    /// Tutorial �������� ����Ͽ� �̹� �� Ʃ�丮�� ����Ʈ�� �߰��ϴ� ���� �޼ҵ��Դϴ�.
    /// </summary>
    /// <param name="tutorial">�� ���� �ִ� Ʃ�丮��(������)</param>
    public static void AddSeenTutorial(Tutorial tutorial) => seenTutorials.Add(tutorial);

    /// <summary>
    /// string�� ����Ͽ� �̹� �� Ʃ�丮�� ����Ʈ�� �߰��ϴ� ���� �޼ҵ��Դϴ�.
    /// </summary>
    /// <param name="tutorial">�� ���� �ִ� Ʃ�丮��(string)</param>
    public static void AddSeenTutorial(string tutorial)
    {
        Tutorial stringToTutorial = (Tutorial)Enum.Parse(typeof(Tutorial), tutorial);
        seenTutorials.Add(stringToTutorial);
    }

    /// <summary>
    /// �̹� �� Ʃ�丮�� ����Ʈ�� �������� ���� �޼ҵ��Դϴ�.
    /// </summary>
    /// <returns>�̹� �� Ʃ�丮�� ����Ʈ</returns>
    public static List<Tutorial> GetSeenTutorials() => seenTutorials;

    /// <summary>
    /// �̹� �� Ʃ�丮����� string ����Ʈ�� �������� ���� �޼ҵ��Դϴ�.
    /// </summary>
    /// <returns>�̹� �� Ʃ�丮�� ����Ʈ(string)</returns>
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
    /// �̹� �� Ʃ�丮�� ����Ʈ�� ���� ���� �޼ҵ��Դϴ�.
    /// </summary>
    public static void SeenTutorialClear() => seenTutorials.Clear();
}

/// <summary>
/// �÷��̾��� �ൿ �迭�� string Ű �����͸� ��Ƶα� ���� Ŭ�����Դϴ�.
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
/// ���� �󿡼� ��µǴ� Ʃ�丮�� ȭ���� ó���ϴ� Ŭ�����Դϴ�.
/// </summary>
public class TutorialScreen : MonoBehaviour
{
    public TextMeshProUGUI explain;         // ���� �ؽ�Ʈ
    IEnumerator _tutorialCoroutine = null;  // Ʃ�丮�� �ڷ�ƾ

    /// <summary>
    /// Ʃ�丮���� �ڷ�ƾ�� �����ϴ� �޼ҵ��Դϴ�.
    /// </summary>
    /// <param name="tutorial">ȭ�鿡 ������ Ʃ�丮��</param>
    public void TotorialStart(TutorialManager.Tutorial tutorial)
    {
        // Ʃ�丮�� �������� �Ű������� ����Ͽ� ȭ�鿡 ǥ���Ϸ��� �ൿ�� Ű�� �����ɴϴ�.
        ActionsAndKey[] actionsAndKey = TutorialManager.GetActionsAndKeysForTutorial(tutorial);
        if(_tutorialCoroutine != null)
        {
            // �̹� ȭ�� ������ �������� Ʃ�丮���� �ߴ��մϴ�.
            StopCoroutine(_tutorialCoroutine);
            _tutorialCoroutine = null;
        }
        _tutorialCoroutine = TutorialCoroutine(actionsAndKey);
        StartCoroutine(_tutorialCoroutine);
    }

    /// <summary>
    /// Ʃ�丮���� ȭ�� �� ǥ���ϴ� �ڷ�ƾ�Դϴ�.
    /// </summary>
    /// <param name="actionsAndKey">ȭ�鿡 ǥ���Ϸ��� �ൿ�� Ű</param>
    IEnumerator TutorialCoroutine(ActionsAndKey[] actionsAndKey)
    {
        yield return null;

        // ��Ʈ�� ������ �׼ǰ� Ű���� �ؽ�Ʈ�� �߰��� �� �ؽ�Ʈ�� ���� ��, ȭ�鿡 8�ʵ��� ����Ѵ�.
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
        
        // Ʃ�丮���� ���� �� ȭ�� �󿡼� �����
        explain.text = "";
        _tutorialCoroutine = null;
    }
}
