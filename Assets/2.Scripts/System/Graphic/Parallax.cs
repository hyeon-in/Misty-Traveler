using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카메라 좌표에 기반하여 여러 배경 레이어에 패럴랙스 스크롤 효과를 구현합니다.
/// </summary>
public class Parallax : MonoBehaviour
{
	[System.Serializable]
    public class Background
    {
        public Transform transform;
        [Range(0,1)] public float horizontalParallax;
		[Range(0,1)] public float verticalParallax;
		[HideInInspector] public Vector3 origin;
    }
	public Background[] backgrounds;

	private Transform _camera;

	void Awake()
	{
		_camera = Camera.main.transform;
		for(int i = 0; i < backgrounds.Length; i++)
		{
			backgrounds[i].origin = backgrounds[i].transform.position;
		}
	}

	void LateUpdate()
	{
		for(int i = 0; i < backgrounds.Length; i++)
		{
			var origin = backgrounds[i].origin;
			var parralexEffect = new Vector2(backgrounds[i].horizontalParallax, backgrounds[i].verticalParallax);

			var distance = new Vector2(_camera.transform.position.x * parralexEffect.x, _camera.transform.position.y * parralexEffect.y);

			float xPos = origin.x + distance.x;
			float yPos = origin.y + distance.y;

            backgrounds[i].transform.position = new Vector3(xPos, yPos, backgrounds[i].transform.position.z);
		}
	}
}