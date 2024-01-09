using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 게임의 버전을 텍스트로 표시하는 클래스 입니다.
/// </summary>
public class VersionDisplay : MonoBehaviour
{
    TextMeshProUGUI _versionText;

    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = "v" + Application.version;
    }
}
