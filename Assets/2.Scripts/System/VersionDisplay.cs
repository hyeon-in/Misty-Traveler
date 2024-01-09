using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// ������ ������ �ؽ�Ʈ�� ǥ���ϴ� Ŭ���� �Դϴ�.
/// </summary>
public class VersionDisplay : MonoBehaviour
{
    TextMeshProUGUI _versionText;

    void Start()
    {
        GetComponent<TextMeshProUGUI>().text = "v" + Application.version;
    }
}
