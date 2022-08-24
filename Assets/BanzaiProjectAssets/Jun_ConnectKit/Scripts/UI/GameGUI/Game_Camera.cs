using UnityEngine;

public class Game_Camera : Jun_ComponentSingletonObject <Game_Camera> {

	Camera _mainCamera;
	public Camera mainCamera{get {return _mainCamera;}}
	GameLevel gameLevel;

	//Animation anima;

	public bool is3D = true;
	
	public void Start() 
	{
		_mainCamera = GetComponent <Camera>();
		gameLevel = GameController.instance.gameLevel;

		if(is3D)
		{
			_mainCamera.orthographic = false;
            int count = (gameLevel.xCount < 7 ? 7 : gameLevel.xCount);
            float y = 4 + (count - 5) * 2.0f;
            float x = 3.6f + (count - 5);
            _mainCamera.transform.localEulerAngles = new Vector3(60, 45, 0);
			_mainCamera.transform.position = new Vector3(-x, y, - x);

            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogStartDistance = y + 7;
            RenderSettings.fogEndDistance = y + 11;
		}
		else
		{
			_mainCamera.orthographic = true;
			_mainCamera.orthographicSize = gameLevel.xCount - 2.8f;

			if(_mainCamera.orthographicSize < 3)
				_mainCamera.orthographicSize = 3;
			         
			_mainCamera.transform.position = new Vector3 (0,5,0);
			_mainCamera.transform.localEulerAngles = new Vector3(90,0,0);
		}
	}
}
