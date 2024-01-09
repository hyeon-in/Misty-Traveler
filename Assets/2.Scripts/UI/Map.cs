using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 지도를 관리하는 클래스로, 플레이어가 지도를 밝힐 수 있게 하는 역할을 합니다.
/// </summary>
public static class MapManager
{
    // 플레이어가 지금까지 찾은 맵 좌표 목록
    static List<Vector2> discoveredMapPositions = new List<Vector2>();

    // 플레이어가 찾은 맵 목록을 전부 제거합니다.
    public static void ClearDiscoveredMaps()
    {
        discoveredMapPositions.Clear();
    }

    /// <summary>
    /// 플레이어가 찾아낸 맵을 목록에 추가하기 위한 메소드입니다.
    /// </summary>
    /// <param name="mapImage">플레이어가 찾아낸 맵의 타일 이미지</param>
    public static void AddDiscoveredMap(Image mapImage)
    {
        discoveredMapPositions.Add(mapImage.rectTransform.anchoredPosition);
    }

    /// <summary>
    /// 플레이어가 찾아낸 맵들을 좌표값을 이용해 한 꺼번에 추가하는 메소드입니다.
    /// </summary>
    /// <param name="mapPos">맵의 좌표 리스트</param>
    public static void AddDiscoveredMaps(List<Vector2> mapPositions)
    {
        discoveredMapPositions.AddRange(mapPositions);
    }

    /// <summary>
    /// 발견한 맵의 리스트의 좌표를 가져오는 메소드
    /// </summary>
    /// <returns>발견한 맵 리스트의 좌표</returns>
    public static List<Vector2> GetDiscoveredMaps() => discoveredMapPositions;
}

/// <summary>
/// UI 메뉴에서의 설명에서 조작법과 제어하려는 메뉴를 표시하기 위한 클래스입니다.
/// </summary>
[System.Serializable]
public class MenuControlAndKey
{
    public GameInputManager.MenuControl[] menuControl;  // 메뉴를 제어하는 키나 버튼을 알려주기 위한 메뉴 컨트롤 리스트
    public string menuKey;                                  // 제어하려는 메뉴의 이름을 가진 key, 언어 데이터에서 key 값을 가져옵니다
}

/// <summary>
/// 지도 UI의 기능들을 처리하는 메소드입니다.
/// </summary>
public class Map : MonoBehaviour
{
    const int MapTileSpacing = 8;   // 맵 타일의 간격
    const int MaxZoomLevel = 3;  // 최대 지도 확대 단계
    const int MinZoomLevel = 1;  // 최소 지도 확대 단계

    [SerializeField] TextMeshProUGUI _mapTitleText;      // 언어 별 대응을 위한 지도 타이틀 텍스트
    [SerializeField] TextMeshProUGUI _manualText;   // 조작법 설명을 표시하기 위한 텍스트
    [SerializeField] RectTransform _mapTileContainer;        // 맵 타일 리스트를 모아놓은 게임 오브젝트
    [SerializeField] RectTransform _playerIcon;     // 플레이어의 위치를 표시하는 아이콘
    [SerializeField] List<Image> _mapTiles;         // UI 상으로 보여지는 맵 타일들을 담아놓은 목록
    [SerializeField] MenuControlAndKey[] _menuControlAndKey;    // UI 메뉴에서의 설명에서 조작법과 제어하려는 메뉴를 표시하기 위한 클래스 리스트

    int _mapZoomLevel = 1;       // 현재 지도 확대 단계

    Vector2 _mapOriginPos = Vector2.zero; // 지도의 원점
    Camera _camera;
    Transform _cameraTransform;

    void Awake()
    {
        transform.GetComponent<PauseScreen>().MapOpend += OnMapOpend;

        _camera = Camera.main;
        _cameraTransform = _camera.GetComponent<Transform>();

        // 전 맵 비활성화
        foreach (var mapTile in _mapTiles)
        {
            mapTile.gameObject.SetActive(false);
        }

        // 지금까지 플레이어가 찾아낸 맵 리스트를 가져와서 해당 맵들을 활성화
        foreach (Vector2 mapPos in MapManager.GetDiscoveredMaps())
        {
            Image mapImage = _mapTiles.Find(x => x.rectTransform.anchoredPosition == mapPos);
            mapImage?.gameObject.SetActive(true);
        }
        _mapOriginPos = _mapTileContainer.anchoredPosition;
    }

    void Update()
    {
        UpdatePlayerIcon();
        DiscoverMap();
        MoveMap();
        ZoomMap();
    }

    /// <summary>
    /// 플레이어의 현재 위치를 실시간으로 지도에 표시하기 위한 메소드입니다.
    /// </summary>
    void UpdatePlayerIcon()
    {
        float cameraHalfSizeY = _camera.orthographicSize;
        float cameraHalfSizeX = cameraHalfSizeY * _camera.aspect;

        float xPos = (_cameraTransform.position.x + cameraHalfSizeX) / (cameraHalfSizeX * 2);
        float yPos = (_cameraTransform.position.y + cameraHalfSizeY) / (cameraHalfSizeY * 2);

        int xPosIndex = Mathf.FloorToInt(xPos) * MapTileSpacing;
        int yPosIndex = Mathf.FloorToInt(yPos) * MapTileSpacing;

        _playerIcon.anchoredPosition = new Vector2(xPosIndex, yPosIndex);
    }

    /// <summary>
    /// 플레이어가 현재 위치한 맵을 맵 매니저에 찾아낸 맵으로 추가하는 메소드입니다.
    /// </summary>
    void DiscoverMap()
    {
        // 맵 타일 리스트에서 플레이어의 아이콘과 같은 위치에 있는 맵 타일을 찾아 가져온다
        // 만약 해당 맵이 활성화되지 않은 상태라면 해당 맵을 활성화 한 후 맵 매니저의 리스트에 해당 맵을 추가한다
        Image mapImage = _mapTiles.Find(x => x.rectTransform.anchoredPosition == _playerIcon.anchoredPosition);
        if (!mapImage.gameObject.activeSelf)
        {
            mapImage.gameObject.SetActive(true);
            MapManager.AddDiscoveredMap(mapImage);
        }
    }

    /// <summary>
    /// 플레이어의 입력을 통해 지도를 상하좌우로 움직이는 클래스입니다.
    /// </summary>
    void MoveMap()
    {
        float yMove = GameInputManager.MenuInput(GameInputManager.MenuControl.Up) ? 1 :
                      GameInputManager.MenuInput(GameInputManager.MenuControl.Down) ? -1 :
                      0;
        float xMove = GameInputManager.MenuInput(GameInputManager.MenuControl.Right) ? 1 :
                      GameInputManager.MenuInput(GameInputManager.MenuControl.Left) ? -1 :
                      0;

        // 입력에 따라 지도 이동
        _mapTileContainer.Translate(new Vector2(xMove, yMove) * 10f * Time.unscaledDeltaTime);

        // 지도의 이동 범위 제한
        _mapTileContainer.anchoredPosition = new Vector2(Mathf.Clamp(_mapTileContainer.anchoredPosition.x, -50f, 20f), 
                                                         Mathf.Clamp(_mapTileContainer.anchoredPosition.y, -20f, 20f));
    }

    /// <summary>
    /// 지도의 크기를 확대하기 위한 메소드입니다. 지도의 크기가 최대 크기를 넘어서면 최소 크기로 돌아옵니다.
    /// </summary>
    void ZoomMap()
    {
        if (GameInputManager.MenuInputDown(GameInputManager.MenuControl.Select))
        {
            if (_mapZoomLevel < MaxZoomLevel)
            {
                _mapTileContainer.localScale += Vector3.one;
                _mapZoomLevel++;
            }
            else
            {
                _mapTileContainer.localScale = Vector3.one;
                _mapZoomLevel = MinZoomLevel;
            }
        }
    }

    /// <summary>
    /// 지도가 실행됐을 때 호출되는 초기화 메소드
    /// </summary>
    public void OnMapOpend()
    {
        _mapTitleText.text = LanguageManager.GetText("Map");
        SetManualText();

        _mapZoomLevel = 1;
        _mapTileContainer.localScale = Vector3.one;
        
        _mapTileContainer.anchoredPosition = _mapOriginPos - _playerIcon.anchoredPosition;
    }

    /// <summary>
    /// 맵을 제어하는 방법을 설명하는 텍스트를 설정하는 메소드
    /// </summary>
    void SetManualText()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < _menuControlAndKey.Length; i++)
        {

            for (int j = 0; j < _menuControlAndKey[i].menuControl.Length; j++)
            {
                // 메뉴의 제어 키, 혹은 버튼을 string에 추가한 뒤 스트링 빌더에 추가
                string menuControlToText;
                if (GameInputManager.usingController)
                {
                    menuControlToText = GameInputManager.MenuControlToButtonText(_menuControlAndKey[i].menuControl[j]);
                }
                else
                {
                    menuControlToText = GameInputManager.MenuControlToKeyText(_menuControlAndKey[i].menuControl[j]);
                }
                sb.AppendFormat("[ <color=#ffaa5e>{0}</color> ] ", menuControlToText);
            }
            // 제어하려는 메뉴의 이름을 key를 이용해서 메뉴 제어의 이름을 가져옴
            string keyToName = LanguageManager.GetText(_menuControlAndKey[i].menuKey);
            sb.Append(keyToName);

            // i가 마지막 인덱스가 아니면 공백 추가
            if (i < _menuControlAndKey.Length - 1)
            {
                sb.Append("  ");
            }
        }
        _manualText.text = sb.ToString();
    }
}