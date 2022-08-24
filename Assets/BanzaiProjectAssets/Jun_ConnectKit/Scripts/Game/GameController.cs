using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Jun_ComponentSingletonObject<GameController> 
{
	[System.Serializable]
	public class GameAudioSetting
	{
		public AudioClip itemClickAudio;
		public AudioClip[] linkAudio;
		public AudioClip linkCompleteAudio;
	}

	public delegate void OnGameStatusChange (GameStatus changeStatus);

	public OnGameStatusChange onGameStatusChange;

	public enum GameStatus
	{
		Off,
		Wait,
		LoadGround,
		LoadItem,
		WaitStart,
		Gaming,
		GameOver,
		GameSettlement,
		LevelCompletions
	}

	public static bool gamePuased = false;

	public static int star3Time = 10;
	public static int star2Time = 30;

	private static GameLevel _assetGameLevel;
	public static GameLevel assetGameLevel{get{return _assetGameLevel;}set {_assetGameLevel = value;}}

	public List<GameObject> gameCanvas;
	public Game_LatticeGUI latticeGUI;
	public Game_ItemGUI gameItemGUI;
	public GameLevel gameLevel;
	public bool isGameStart;

	public GameAudioSetting gameAudioSetting = new GameAudioSetting();

	GamingData _gamingData = new GamingData();
	public GamingData gamingData {get{return _gamingData;}}

	Transform itemUI;
	Transform latticeGUIRoot;
	Transform itemGUIRoot;
	Transform lineRoot;
    Transform overStar;
	GameLineController lineContorller;

	public GameStatus _gameStatus = GameStatus.Wait;
	public GameStatus gameStatus{get{return _gameStatus;}}

	Game_Camera gameCamera;
	LatticeMatrix latticeGUIMatrix = new LatticeMatrix();
	List<Game_ItemGUI> itemGUIs = new List<Game_ItemGUI>();

	public void Awake()
	{
		SetInstance (this);
		if(assetGameLevel != null)
			gameLevel = assetGameLevel;
	}
	
	public void Start()
	{
		Vector3 pos = new Vector3((gameLevel.xCount - 1)*0.5f,4,(gameLevel.yCount - 1)*0.5f);

		itemUI = transform.Find ("ItemUI");
		latticeGUIRoot = itemUI.transform.Find ("LatticeGUIRoot");
		itemGUIRoot = itemUI.transform.Find ("ItemGUIRoot");
		lineRoot = itemUI.transform.Find ("LineRoot");
		overStar = transform.Find("OverStar");
		lineContorller = GetComponentInChildren <GameLineController>();

		latticeGUIRoot.transform.position = -pos;
		itemGUIRoot.transform.position = -pos;
		lineRoot.transform.position = -pos;

		gameCamera = Game_Camera.instance;

		if(gameLevel.levelID < 30)
		{
			star3Time = 3000;
			star2Time = 6000;
		}
		else if(gameLevel.levelID < 100)
		{
			star3Time = 2000;
			star2Time = 4500;
		}
		else if(gameLevel.levelID < 500)
		{
			star3Time = 1200;
			star2Time = 4000;
		}
		else
		{
			star3Time = 1000;
			star2Time = 3000;
		}
	}

	public void StartGame()
	{
		isGameStart = true;
		SetGameStatus(GameStatus.Wait);	
	}

	public void SetGameStatus (GameStatus setStatus)
	{
		_gameStatus = setStatus;

		if(onGameStatusChange != null)
			onGameStatusChange (setStatus);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(gameLevel == null || !isGameStart)
			return;
		
		switch (gameStatus)
		{
            case GameStatus.Wait:
                SetGameStatus(GameStatus.LoadGround);
                isGameStart = true;
                break;

            case GameStatus.LoadGround:
                LoadGround();
                break;

            case GameStatus.LoadItem:
                LoadItem();
                break;

            case GameStatus.WaitStart:
                WaitStart();
                break;

            case GameStatus.Gaming:
                Gaming();
                break;

            case GameStatus.GameOver:
                SetGameStatus(GameStatus.GameSettlement);
                break;

            case GameStatus.GameSettlement:
                GameSettlement();
                break;
		}

        overStar.transform.position = itemUI.transform.position;
	}

	bool isDoLoadGround = false;
	void LoadGround ()
	{
		if(!isDoLoadGround)
		{
			isDoLoadGround = true;
			StartCoroutine (DoLoadGround ());
		}

	}

	IEnumerator DoLoadGround ()
	{
		_gamingData.Init (gameLevel);
		latticeGUIMatrix.CreateLattices (gameLevel.xCount,gameLevel.yCount);
		for (int x = 0; x < gameLevel.xCount; x ++)
		{
			for (int y = 0; y < gameLevel.yCount; y++)
			{
				Lattice thisLattice = gameLevel.GetLattice (x,y);

				if(!Lattice.IsNull (thisLattice))
				{
					GameObject newLatticeGUIObj = Instantiate (latticeGUI.gameObject);
					Game_LatticeGUI newLatticeGUI = newLatticeGUIObj.GetComponent <Game_LatticeGUI>();
					newLatticeGUIObj.transform.parent = latticeGUIRoot;
					newLatticeGUIObj.transform.localPosition = new Vector3(x,0,y);

					newLatticeGUI.SetLine (!Lattice.IsNull (gameLevel.GetLattice(x,y+1)),!Lattice.IsNull (gameLevel.GetLattice(x,y-1)),!Lattice.IsNull (gameLevel.GetLattice(x-1,y)),!Lattice.IsNull (gameLevel.GetLattice(x+1,y)));
					newLatticeGUI.SetLattice (thisLattice.GetNewLattice ());

					newLatticeGUIObj.name = "LatticeGUI(" + x + "_" + y + ")";

					latticeGUIMatrix.GetLattice (x,y).latticeObject = newLatticeGUI.gameObject;

					yield return new WaitForEndOfFrame ();
				}
			}
		}
		SetGameStatus (GameStatus.LoadItem);
	}

	bool isDoLoadItem = false;
	void LoadItem ()
	{
		if(!isDoLoadItem)
		{
			isDoLoadItem = true;
			StartCoroutine (DoLoadItem ());
		}
	}

	IEnumerator DoLoadItem ()
	{
		for (int i = 0; i < gameLevel.targetPointCount; i++)
		{
			GameLevel.TargetPointInfo thisTargetPoint = gameLevel.GetTargetPoint (i);
			if(thisTargetPoint != null)
			{
				int pointID = 0;
				while (pointID < 2)
				{
					GameObject newItemObj = Instantiate (gameItemGUI.gameObject);
					Game_ItemGUI newItemGUI = newItemObj.GetComponent <Game_ItemGUI>();
					newItemObj.transform.parent = itemGUIRoot;
					newItemGUI.SetItem (thisTargetPoint.item);
					itemGUIs.Add (newItemGUI);

					switch (pointID)
					{
					case 0:
						newItemObj.transform.localPosition = new Vector3(thisTargetPoint.lattice01Coord.x,0,thisTargetPoint.lattice01Coord.y);
						Game_LatticeGUI latticeGUI01 = GetLatticeGUI (thisTargetPoint.lattice01Coord.x,thisTargetPoint.lattice01Coord.y);
						if(latticeGUI01 != null)
						{
							latticeGUI01.itemGUI = newItemGUI;
						}
						break;

					case 1:
						newItemObj.transform.localPosition = new Vector3(thisTargetPoint.lattice02Coord.x,0,thisTargetPoint.lattice02Coord.y);

						Game_LatticeGUI latticeGUI02 = GetLatticeGUI (thisTargetPoint.lattice02Coord.x,thisTargetPoint.lattice02Coord.y);
						if(latticeGUI02 != null)
						{
							latticeGUI02.itemGUI = newItemGUI;
						}
						break;
					}
					pointID ++;

					yield return new WaitForEndOfFrame ();
				}
			}
		}

        DrawTargetLine();
		SetGameStatus (GameStatus.WaitStart);
	}

	void WaitStart ()
	{
		if (GUI_LoadingUI.instance)
		{
			if (GUI_LoadingUI.instance.isHide)
			{
				SetGameStatus(GameStatus.Gaming);
				timeNote = Time.time;
			}
		}
	}

	private Game_LatticeGUI curTouchLatticeGUI,clickDownLatticeGUId;
	private Lattice clickDownLattice;
	public float timeNote;

	void Gaming ()
	{
		if(gamePuased)
			return;
		
		_gamingData.gameTime = (int)((Time.time - timeNote)*100);
		
		if(Input.GetMouseButtonDown (0))
		{
			Game_LatticeGUI thisTouchLatticeGUI = CurTouchLatticeGUI ();
			SetClickLatticeGUI (thisTouchLatticeGUI);
			HideAllLatticeExclamation ();
		}

		if(Input.GetMouseButton (0))
		{
			Game_LatticeGUI thisTouchLatticeGUI = CurTouchLatticeGUI ();
			SetClickLatticeGUI(thisTouchLatticeGUI);
		}

		if(Input.GetMouseButtonUp (0))
		{
			switch (gameLevel.levelType)
			{
			case GameLevel.LevelType.GetAllCrystal:
				if(gamingData.isGetAllCrystal && gamingData.isAllLineComplete)
				{
					SetGameStatus (GameStatus.GameOver);
				}

				if(gamingData.isAllLineComplete && !gamingData.isGetAllCrystal)
				{
					ShowLatticeExclamation ();
				}
				break;
			}

			if(clickDownLattice != null)
			{
				if(clickDownLattice.isItemPoint)
				{
					GameLevel.TargetPointInfo thisTarget = GetTargetPointFromItem (clickDownLattice.item);

					Lattice startLattice = latticeGUIMatrix.GetLattice (thisTarget.lattice01Coord.x,thisTarget.lattice01Coord.y);
					Lattice targetLattice = latticeGUIMatrix.GetLattice (thisTarget.lattice02Coord.x,thisTarget.lattice02Coord.y);

					startLattice.latticeObject.GetComponent <Game_LatticeGUI>().IsComplete (false);
					targetLattice.latticeObject.GetComponent <Game_LatticeGUI>().IsComplete (false);

				}
			}

			SetClickLatticeGUI(null);
			curTouchLatticeGUI = null;
			clickDownLattice = null;
			addLatticeCombeID = 0;
		}
	}

	#region<GameSettlement>
	bool isDoGameSettlement = false;
	void GameSettlement ()
	{
		if(!isDoGameSettlement)
		{
			isDoGameSettlement = true;
			StartCoroutine (DoGameSettlement ());

			int starCount = 0;
			if(gamingData.gameTime < star3Time)
			{
				starCount = 3;
			}
			else if(gamingData.gameTime < star2Time)
			{
				starCount = 2;
			}
			else
			{
				starCount = 1;
			}
			gamingData.starCount = starCount;

			gameLevel.SetStar (starCount);
		}
	}

	IEnumerator DoGameSettlement ()
	{
		yield return new WaitForSeconds (0.4f);

		lineRoot.gameObject.SetActive (false);

		//itemUI.GetComponent <Jun_ScaleTween>().Play ();
		//itemUI.GetComponent <Jun_RotateTween>().Play ();
        itemUI.GetComponent<Jun_PositionTween>().Play();

		yield return new WaitForSeconds (1f);

		overStar.GetComponent <Jun_RotateTween>().Play ();
		overStar.GetComponent <Jun_AlphaTween>().Play ();
		overStar.GetComponent <Jun_ScaleTween>().Play ();

		AudionManger.inter.Play ("Star");
		yield return new WaitForSeconds (0.8f);

		gameLevel.OpenNextLevel ();
		SetGameStatus (GameStatus.LevelCompletions);
	}
	#endregion

	void SetClickLatticeGUI (Game_LatticeGUI clickGUI)
	{
		if(clickGUI != curTouchLatticeGUI)
		{
			if(curTouchLatticeGUI != null)
			{
				curTouchLatticeGUI.IsClick (false);
			}

			if(clickGUI != null)
			{
				clickGUI.IsClick (true);
			}

			curTouchLatticeGUI = clickGUI;
			if(clickDownLattice == null)
			{
				clickDownLattice = clickGUI.lattice;
				GameLevel.TargetPointInfo thisTargetPoint = LatticeInTargetLine (clickGUI.lattice);
				if(thisTargetPoint != null)
				{
					LatticeCoord firstCoord = thisTargetPoint.FirstLatticeCoord ();
					if(firstCoord != null && !clickGUI.lattice.isItemPoint)
						clickDownLattice = gamingData.latticeMatrix.GetLattice(firstCoord.x,firstCoord.y);
				}

				if(clickDownLattice.isItemPoint)
				{
					GameLevel.TargetPointInfo thisTarget = GetTargetPointFromItem (clickDownLattice.item);

					Lattice startLattice = latticeGUIMatrix.GetLattice (thisTarget.lattice01Coord.x,thisTarget.lattice01Coord.y);
					Lattice targetLattice = latticeGUIMatrix.GetLattice (thisTarget.lattice02Coord.x,thisTarget.lattice02Coord.y);

					startLattice.latticeObject.GetComponent <Game_LatticeGUI>().IsComplete (true);
					targetLattice.latticeObject.GetComponent <Game_LatticeGUI>().IsComplete (true);
				}
				Debug.Log ("Start Line!");
			}

			AddLatticeToLine (clickGUI);
		}
	}

	void AddLatticeToLine (Game_LatticeGUI addLatticeGUI)
	{
		if(addLatticeGUI == null)
			return;
		
		LatticeCoord addCoord = new LatticeCoord(addLatticeGUI.lattice.x,addLatticeGUI.lattice.y);
		if(clickDownLattice.isItemPoint)
		{
			//current editor line
			GameLevel.TargetPointInfo editorTargetPoint = GetTargetPointFromItem (clickDownLattice.item);

			GameLevel.TargetPointInfo thisLatticeTargetPoint = LatticeInTargetLine (addLatticeGUI.lattice);

			if(editorTargetPoint.lineLatticeCoords.Count > 0 && addLatticeGUI.isItemPoint && addLatticeGUI.item != editorTargetPoint.item)
				return;

			LatticeCoord editorLastCoord = editorTargetPoint.LastLatticeCoord ();

			//line complete?
			if(TargetLineIsComplete (editorTargetPoint) && !editorTargetPoint.ContainsCoord (addCoord))
				return;

			if(thisLatticeTargetPoint != null)
			{
				thisLatticeTargetPoint.RemoveCoordAtCoords (addCoord);
			}

			editorLastCoord = editorTargetPoint.LastLatticeCoord ();

			bool isStartLattice = false;
			if(editorLastCoord != null)
			{
				if(addLatticeGUI.lattice == clickDownLattice)
				{
					isStartLattice = true;
				}
				else
				{
					if(Lattice.IsSideLattice (_gamingData.latticeMatrix.GetLattice (editorLastCoord.x,editorLastCoord.y),addLatticeGUI.lattice))
					{
						AddLatticeToTargetLine (editorTargetPoint,addLatticeGUI.lattice);
					}
				}
			}
			else
			{
				isStartLattice = true;
			}

			if(isStartLattice)
			{
				editorTargetPoint.ClearLineCoords ();
				AddLatticeToTargetLine (editorTargetPoint,addLatticeGUI.lattice);
			}

			DrawTargetLine ();
			_gamingData.UpdateData ();
		}
	}
    
	int addLatticeCombeID = 0;
	int combeSpeed = 1;

	void AddLatticeToTargetLine (GameLevel.TargetPointInfo targetPoint,Lattice addLattice)
	{
		LatticeCoord addCoord = new LatticeCoord(addLattice.x,addLattice.y);
		targetPoint.AddLineCoords (addCoord);
        
		AudionManger.inter.Play (gameAudioSetting.linkAudio[addLatticeCombeID],false,false,0);

		addLatticeCombeID += combeSpeed;
		if(addLatticeCombeID >= gameAudioSetting.linkAudio.Length)
		{
			addLatticeCombeID = gameAudioSetting.linkAudio.Length - 2;
			combeSpeed = -1;
		}

		if(addLatticeCombeID < 0)
		{
			addLatticeCombeID = 1;
			combeSpeed = 1;
		}
	}

	bool TargetLineIsComplete (GameLevel.TargetPointInfo targetPoint)
	{
		LatticeCoord editorFirstCoord = targetPoint.FirstLatticeCoord ();
		LatticeCoord editorLastCoord = targetPoint.LastLatticeCoord ();
		if(editorFirstCoord != null && editorLastCoord != null && targetPoint.lineLatticeCoords.Count > 1)
		{
			Lattice firstLattice = _gamingData.latticeMatrix.GetLattice (editorFirstCoord.x,editorFirstCoord.y);
			Lattice lastLattice = _gamingData.latticeMatrix.GetLattice (editorLastCoord.x,editorLastCoord.y);

			if(firstLattice != lastLattice && firstLattice.item == lastLattice.item)
				return true;
		}
		return false;
	}

	Game_LatticeGUI CurTouchLatticeGUI ()
	{
		Ray ray = gameCamera.mainCamera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit))
		{
			Game_LatticeGUI hitLatticeGUI = hit.collider.GetComponent <Game_LatticeGUI>();
            if(hitLatticeGUI == null)
            {
                hitLatticeGUI = hit.collider.GetComponentInParent<Game_LatticeGUI>();
            }
			if(hitLatticeGUI != null)
			{
				return hitLatticeGUI;
			}
		}

		return null;
	}

	GameLevel.TargetPointInfo GetTargetPointFromItem (Item thisItem)
	{
		for (int i = 0; i < _gamingData.targetPoints.Count; i++)
		{
			GameLevel.TargetPointInfo thisTargetPoint = _gamingData.targetPoints[i];
			if(thisTargetPoint.item == thisItem)
				return thisTargetPoint;
		}
		return null;
	}

	GameLevel.TargetPointInfo LatticeInTargetLine (Lattice thisLattice)
	{
		for (int i = 0; i < _gamingData.targetPoints.Count; i++)
		{
			GameLevel.TargetPointInfo thisTargetPoint = _gamingData.targetPoints[i];
			for (int j = 0; j < thisTargetPoint.lineLatticeCoords.Count; j++)
			{
				LatticeCoord thisCoord = thisTargetPoint.lineLatticeCoords[j];
				if(thisLattice.x == thisCoord.x && thisLattice.y == thisCoord.y)
					return thisTargetPoint;
			}
		}
		return null;
	}

	Game_LatticeGUI GetLatticeGUI (int x, int y)
	{
		Lattice thisLattice = latticeGUIMatrix.GetLattice (x,y);
		if(!Lattice.IsNull (thisLattice) && thisLattice.latticeObject != null)
		{
			return thisLattice.latticeObject.GetComponent <Game_LatticeGUI>();
		}
		return null;
	}

	void DrawTargetLine ()
	{
		if(gameLevel.levelType == GameLevel.LevelType.GetAllCrystal)
		{
			for (int x = 0; x < latticeGUIMatrix.xCount; x++)
			{
				for (int y = 0; y < latticeGUIMatrix.yCount; y++)
				{
					Lattice thisLattice = latticeGUIMatrix.GetLattice(x,y);
					if(thisLattice.latticeObject != null)
					{
						Game_LatticeGUI thisGUI = thisLattice.latticeObject.GetComponent <Game_LatticeGUI>();
						thisGUI.ShowCrystal ();

                        thisGUI.SetPointColor(Color.white);
					}
				}
			}
		}

		for (int i = 0; i < _gamingData.targetPoints.Count; i++)
		{
			GameLevel.TargetPointInfo thisTargetPoint = _gamingData.targetPoints[i];
			if(lineContorller != null)
			{
				GameLineController.LineObject thisLine = lineContorller.DrawLine (thisTargetPoint.item,thisTargetPoint.GetLineVectors ());
				lineContorller.SetLineComplete (thisLine);
			}

			if(gameLevel.levelType == GameLevel.LevelType.GetAllCrystal)
			{
				for (int j = 0; j < thisTargetPoint.lineLatticeCoords.Count; j++)
				{
					LatticeCoord thisCoord = thisTargetPoint.lineLatticeCoords[j];
					Lattice lineLattice = gamingData.latticeMatrix.GetLattice (thisCoord.x,thisCoord.y);
					
                    Lattice thisGUILattice = latticeGUIMatrix.GetLattice(thisCoord.x, thisCoord.y);
                    if (thisGUILattice.latticeObject != null)
                    {
                        Game_LatticeGUI thisGUI = thisGUILattice.latticeObject.GetComponent<Game_LatticeGUI>();
                        thisGUI.SetPointColor(thisTargetPoint.item.itemColor);
                        if (!lineLattice.isItemPoint)
                        {
                            thisGUI.GetCrystal();

                            if (j == thisTargetPoint.lineLatticeCoords.Count - 1)
                                thisGUI.ShowAddEffect(thisTargetPoint.item.itemColor);
                        }
                    }
				}
			}
		}
	}

	void HideAllLatticeExclamation ()
	{
		if(gameLevel.levelType == GameLevel.LevelType.GetAllCrystal)
		{
			for (int x = 0; x < latticeGUIMatrix.xCount; x++)
			{
				for (int y = 0; y < latticeGUIMatrix.yCount; y++)
				{
					Lattice thisLattice = latticeGUIMatrix.GetLattice(x,y);
					if(thisLattice.latticeObject != null)
					{
						thisLattice.latticeObject.GetComponent <Game_LatticeGUI>().ShowExclamation (false);
					}
				}
			}
		}
	}

	void ShowLatticeExclamation ()
	{
		if(gameLevel.levelType == GameLevel.LevelType.GetAllCrystal)
		{
			for (int x = 0; x < latticeGUIMatrix.xCount; x++)
			{
				for (int y = 0; y < latticeGUIMatrix.yCount; y++)
				{
					Lattice thisLattice = latticeGUIMatrix.GetLattice(x,y);
					if(thisLattice.latticeObject != null)
					{
						if (LatticeInTargetLine (gamingData.latticeMatrix.GetLattice (x,y)) == null)
							thisLattice.latticeObject.GetComponent <Game_LatticeGUI>().ShowExclamation (true);
					}
				}
			}
		}
	}
}
