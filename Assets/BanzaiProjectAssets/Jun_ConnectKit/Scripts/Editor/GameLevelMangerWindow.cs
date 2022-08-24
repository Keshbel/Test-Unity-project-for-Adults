using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class GameLevelMangerWindow : EditorWindow {

	public class LineColor 
	{
		public Color color = Color.white;
		public bool showLine = false;

		public LineColor (Color setColor,bool setShow)
		{
			color = setColor;
			showLine = setShow;
		}
	}

	private Vector2 listPos,targetPointListPos;
	private int curSelectGroup;
	private int curStartGroupID;

	private GameLevelManger levelManger;
	private ItemManger itemManger;
	private GameLevelGroup waitToDeleteGroup;
	private GameLevel waitToDeleteLevel,curAddAICarLevel,curRemoveAICarLevel;
	private GameLevel.TargetPointInfo waitToDeleteTargetPoint,curEditorTargetPoint;
	private Lattice curSelectLattice,waitAddLattice;
	private GUISkin editorGUISkin;

	private GameLevel m_curEditorLevel;
	private GameLevel curEditorLevel
	{
		get { return m_curEditorLevel; }
		set 
		{
			if(m_curEditorLevel != value)
			{
				m_curEditorLevel = value;
                if(value != null)
				    m_curEditorLevel.isEditor = true;
			}
		}
	}

	public static void Open ()
	{
		GameLevelMangerWindow windows = EditorWindow.GetWindow<GameLevelMangerWindow>(true,"Game Level Manger");
		windows.minSize = new Vector2(800,400);
		windows.Load();
	}

	public void Load ()
	{
		levelManger = EditorTools.LoadObjectInAssets<GameLevelManger>("GameLevelManger");
		itemManger = ItemMangerWindow.GetData ();

		if(levelManger == null)
			return;
		if(levelManger.groupCount > 0)
		{
			if(levelManger.GetGroup(0).levelCount > 0)
			{
				curEditorLevel = levelManger.GetGroup (0).GetLevel (0);
			}
		}
		editorGUISkin = Resources.Load ("EGUIGUISkin",typeof (GUISkin)) as GUISkin;
	}

	void OnDestroy ()
	{
		Save();
	}

	void Save ()
	{
		if(levelManger == null)
			return;
		EditorUtility.SetDirty(levelManger.gameObject);
		int levelCount = 0;
		for (int i = 0; i < levelManger.groupCount; i++)
		{
			GameLevelGroup thisGroup = levelManger.GetGroup (i);
			if(thisGroup != null) 
			{
				thisGroup.ID = i;
				thisGroup.levelManger = levelManger;
				for (int j = 0; j < levelManger.GetGroup (i).levelCount; j++)
				{
					GameLevel thisLevel = levelManger.GetGroup (i).GetLevel (j);
					if(thisLevel != null)
					{
						SerializedObject thisLevelSObj = new SerializedObject(thisLevel);
						thisLevelSObj.Update();
						SerializedProperty _myGroupID = thisLevelSObj.FindProperty("_myGroupID");
						SerializedProperty _levelID = thisLevelSObj.FindProperty("_levelID");
						SerializedProperty _levelInGroupID = thisLevelSObj.FindProperty("_levelInGroupID");
						SerializedProperty _levelManager = thisLevelSObj.FindProperty("_levelManager");

						_myGroupID.intValue = i;
						_levelID.intValue = levelCount;
						_levelInGroupID.intValue = j;
						_levelManager.objectReferenceValue = levelManger;
						if(thisLevel.isEditor)
						{
							EditorUtility.SetDirty(thisLevel.gameObject);
							thisLevel.isEditor = false;
						}
						levelCount ++;

						thisLevelSObj.ApplyModifiedProperties();
					}
				}
			}
		}
        
		AssetDatabase.SaveAssets ();
	}

	void OnGUI ()
	{
        if (Application.isPlaying)
            GUI.enabled = false;
		GUILayout.BeginVertical ();
		if(levelManger == null)
		{
			if(GUILayout.Button ("Creat Level Manger!"))
			{
				levelManger = EditorTools.CreatePrefabDate<GameLevelManger>("GameLevelManger");
			}
		}
		else
		{
			if(GUILayout.Button ("Add Group"))
			{
				levelManger.AddGroup ();
			}

			if(levelManger.groupCount > 0)
			{
				int selectGroupNO = EditorTools.DrawTagGUI (curSelectGroup,levelManger.groupNames,ref curStartGroupID,15);

				if(curSelectGroup != selectGroupNO)
				{
					curSelectGroup = selectGroupNO;
					curEditorLevel = levelManger.GetGroup (curSelectGroup).GetLevel (0);
				}

				GameLevelGroup thisGroup = levelManger.GetGroup (curSelectGroup);

				if(thisGroup != null)
				{
					thisGroup.ID = curSelectGroup;
					thisGroup.levelManger = levelManger;

					GUILayout.BeginHorizontal ();
					DrawGroup (thisGroup);
					GUILayout.EndHorizontal ();
				}
			}
		}
		GUILayout.EndVertical ();
	}

	void DrawGroup (GameLevelGroup group)
	{
		GUI.backgroundColor = new Color(0.7f,0.7f,0.7f,1);
		GUILayout.BeginVertical ("Box");
		GUI.backgroundColor = Color.white;

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("GroupName:",GUILayout.Width (50));
		group.name = EditorGUILayout.TextField (group.name,GUILayout.Width (100));

		GUILayout.TextArea ("",GUILayout.Width (1));

		//GUILayout.Label ("UseInGame:",GUILayout.Width (120));
		//group.isInGame = EditorGUILayout.Toggle (group.isInGame,GUILayout.Width (30));

		GUILayout.TextArea ("",GUILayout.Width (1));

		//GUI.backgroundColor = Color.white;

		GUILayout.Label ("");

		GUI.backgroundColor = Color.red;

		if(waitToDeleteGroup != group)
		{
			if(GUILayout.Button ("Delete",GUILayout.Width (80)))
			{
				waitToDeleteGroup = group;
			}
		}
		else
		{
			if(GUILayout.Button ("Yes",GUILayout.Width (40)))
			{
                for (int i = group.levelCount - 1; i >= 0; i--)
                {
                    GameLevel thisGameLevel = group.GetLevel(i);
                    group.RemoveLevel(thisGameLevel);
                    EditorTools.DeletePrefabData(thisGameLevel.gameObject);
                }

				if(group.levelCount == 0)
				{
					levelManger.RemoveGroup (group);
				}
			}

			GUI.backgroundColor = Color.green;
			if(GUILayout.Button ("No",GUILayout.Width (40)))
			{
				waitToDeleteGroup = null;
			}
		}
		GUI.backgroundColor = Color.white;
		GUILayout.EndHorizontal ();

		EditorTools.SpaceLine (2);
		group.levelManger = levelManger;

		GUILayout.BeginHorizontal ();
		if(GUILayout.Button ("AddLevel",EditorStyles.miniButtonLeft))
		{
			GameLevel newLevel = CreateLevel (group);
			if(newLevel != null)
				group.AddLevel (newLevel);
			curEditorLevel = newLevel;
		}

		if(GUILayout.Button ("LoadLevel",EditorStyles.miniButtonRight))
		{
			GameLevel loadLevel = EditorTools.GetPrefabDate <GameLevel>();
			if(loadLevel != null)
				group.AddLevel (loadLevel);
		}
		GUILayout.EndHorizontal ();

		GUILayout.BeginHorizontal ("Box");

		if(group != null)
		{
			LevelListGUI (group);

			LevelEditorGUI (curEditorLevel);
		}
		GUILayout.EndHorizontal ();

		GUILayout.EndVertical ();
	}

	void LevelListGUI (GameLevelGroup group)
	{
		if(group.levelCount > 0)
		{
			GUILayout.BeginVertical ("Box",GUILayout.Width (200));
			listPos = EditorGUILayout.BeginScrollView (listPos);
			for (int i = 0; i < group.levelCount; i++)
			{
				GameLevel thisLevel = group.GetLevel (i);
				if(thisLevel != null)
				{               
					bool isSelect = false;
					if(curEditorLevel == thisLevel)
					{
						isSelect = true;
						GUI.backgroundColor = new Color(0.3f,0.3f,0.3f,1);
					}
					else
						GUI.backgroundColor = Color.white;


					//GUILayout.BeginHorizontal ();
					if(GUILayout.Toggle (isSelect,"Editor Level:" + (thisLevel.levelID + 1),EditorStyles.toolbarButton))
					{
						if (curEditorLevel != thisLevel)
						{
							curEditorLevel = thisLevel;
							curSelectLattice = null;
						}
					}
				}
			}
			GUILayout.EndScrollView ();
			GUILayout.EndVertical ();
		}
	}

	int levelSettingHeight = 120;
	bool isCopyLatticeEvent = false;
	bool isClickLatticeEvent = false,isStartNewLine = false;
	Vector2 mousePosition;
	Lattice mouseDownLattice,lastMouseLattice;

	void LevelEditorGUI (GameLevel level)
	{
		Event inputEvent = Event.current;
		if(inputEvent != null)
		{         
			if(inputEvent.isMouse)
			{
				if(inputEvent.button == 1)
				{
					if(inputEvent.type == EventType.MouseDown)
					{
						isCopyLatticeEvent = true;
						isClickLatticeEvent = true;
					}

					if(inputEvent.type == EventType.MouseUp)
					{
						isClickLatticeEvent = false;
						isStartNewLine = false;

						if(isCopyLatticeEvent)
						{
							OnMouseUp_SetCopyLattice ();
							isCopyLatticeEvent = false;
						}
						mouseDownLattice = null;
					}
				}

				else if(inputEvent.button == 0)
				{
					if(inputEvent.type == EventType.MouseDown)
					{
						isClickLatticeEvent = true;
					}

					if(inputEvent.type == EventType.MouseUp)
					{
						isClickLatticeEvent = false;
						isStartNewLine = false;

						if(isCopyLatticeEvent)
						{
							OnMouseUp_SetCopyLattice ();
							isCopyLatticeEvent = false;
						}
						mouseDownLattice = null;
					}
				}

				mousePosition = inputEvent.mousePosition;
			}
		}

		//GUILayout.Label ("");
        if(level == null || curEditorLevel == null)return;
		GUILayout.BeginVertical ();

		#region<Level Setting>
		LevelSettingGUI ();
        #endregion

        #region<Level Item Setting>
        LevelMapEditor();
		#endregion

		GUILayout.EndVertical ();
	}
		
	void LevelSettingGUI ()
	{
		GUI.backgroundColor = new Color(0.9f,0.9f,0.9f,1);
		GUILayout.BeginVertical ("Box",GUILayout.Height (levelSettingHeight));
		GUI.backgroundColor = Color.white;

		GUILayout.BeginHorizontal ();
		GUILayout.Label ("Level Setting:");

		if(waitToDeleteLevel != curEditorLevel)
		{
			GUI.backgroundColor = Color.red;
			if(GUILayout.Button ("Delete",EditorStyles.miniButton,GUILayout.Width (60)))
			{
				waitToDeleteLevel = curEditorLevel;
			}
		}
		else
		{
			GUI.backgroundColor = Color.red;
			if(GUILayout.Button ("Yes",EditorStyles.miniButtonLeft,GUILayout.Width (30)))
			{
				GameLevelGroup thisGroup = levelManger.GetGroup (curSelectGroup);
				if(thisGroup != null)
				{
					curEditorLevel = null;
					GameObject removeLevel = waitToDeleteLevel.gameObject;
					thisGroup.RemoveLevel (waitToDeleteLevel);
					EditorTools.DeletePrefabData (removeLevel);
				}
			}

			GUI.backgroundColor = Color.green;
			if(GUILayout.Button ("No",EditorStyles.miniButtonRight,GUILayout.Width (30)))
			{
				waitToDeleteLevel = null;
			}
		}
		GUI.backgroundColor = Color.white;
		GUILayout.EndHorizontal ();

		if(curEditorLevel != null)
		{
			GUILayout.BeginHorizontal ();
			GUI.backgroundColor = Color.green;
			if(GUILayout.Button ("Test Level",EditorStyles.miniButton,GUILayout.Width (100)))
			{
                Save();
                EditorSceneManager.OpenScene ("Assets/Jun_ConnectKit/Scenes/Game.unity");
				GameObject gameControllerObj = GameObject.Find ("GameController");
				GameController gameController = gameControllerObj.GetComponent <GameController>();
				gameController.gameLevel = curEditorLevel;
				EditorApplication.isPlaying = true;
			}

			GUI.backgroundColor = Color.white;

			if(GUILayout.Button ("Auto Set",EditorStyles.miniButton,GUILayout.Width (100)))
			{
				if(itemManger != null)
				{
					int targetCount = curEditorLevel.targetPointCount;
                    if (targetCount <= 0)
                        targetCount = Random.Range(3, 6);
                    Item[] randomItems = itemManger.GetRandomItems(targetCount).ToArray();
                    curEditorLevel.AutoSetTargetPoint(randomItems);
				}
			}

			GUILayout.Space (40);

			GUILayout.Label ("Unlock:",GUILayout.Width (80));
			curEditorLevel.m_isUnlock = EditorGUILayout.Toggle (curEditorLevel.m_isUnlock,GUILayout.Width (30));
			GUILayout.Label ("Type:",GUILayout.Width (55));
			curEditorLevel.levelType = (GameLevel.LevelType)EditorGUILayout.EnumPopup (curEditorLevel.levelType);
			GUILayout.EndHorizontal ();


			EditorTools.SpaceLine (2);

			GUILayout.BeginHorizontal ();
			//App new point
			if(GUILayout.Button ("+",GUILayout.Width (20),GUILayout.Height (levelSettingHeight - 30)))
			{
				GameLevel.TargetPointInfo newTargetPoint = curEditorLevel.AddTargetPoint ();

				List <Lattice> canSetLattices = new List<Lattice>();
				for (int i = 0; i < curEditorLevel.xCount; i++)
				{
					for (int j = 0; j < curEditorLevel.yCount; j++)
					{
						Lattice thisLattice = curEditorLevel.GetLattice (i,j);
						if(thisLattice.item == null)
						{
							canSetLattices.Add (thisLattice);
						}
					}
				}

				Lattice setLattice01 = canSetLattices[Random.Range (0,canSetLattices.Count)];
				newTargetPoint.lattice01Coord = new LatticeCoord(setLattice01.x,setLattice01.y);
				canSetLattices.Remove (setLattice01);
				Lattice setLattice02 = canSetLattices [Random.Range (0,canSetLattices.Count)];
				newTargetPoint.lattice02Coord = new LatticeCoord(setLattice02.x,setLattice02.y);

				curEditorTargetPoint = newTargetPoint;
				ItemSelectWindow.Open (OnAddTargetPoint);
			}

			float targetUISize = 120;
			targetPointListPos = EditorGUILayout.BeginScrollView (targetPointListPos);
			GUILayout.BeginHorizontal ();
			for (int i = 0; i < curEditorLevel.targetPointCount; i++)
			{
				GameLevel.TargetPointInfo thisTargetPoint = curEditorLevel.GetTargetPoint (i);
				if(thisTargetPoint != null)
				{
					GUILayout.BeginHorizontal ("Box",GUILayout.Width (targetUISize));

					Texture2D targetPointItemIcon = thisTargetPoint.item == null?null:thisTargetPoint.item.GetUIShowTexture ();
					GUI.contentColor = thisTargetPoint.item == null?Color.white:thisTargetPoint.item.itemColor;
					if(GUILayout.Button (targetPointItemIcon,GUILayout.Width (60),GUILayout.Height (60)))
					{
						curEditorTargetPoint = thisTargetPoint;
						ItemSelectWindow.Open (OnAddTargetPoint);
					}
					GUI.contentColor = Color.white;

					GUILayout.BeginVertical ();

					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Target：" + (i + 1));

					if(waitToDeleteTargetPoint != thisTargetPoint)
					{
						GUI.backgroundColor = Color.red;
						if(GUILayout.Button ("X",EditorStyles.miniButton,GUILayout.Width (20)))
						{
							waitToDeleteTargetPoint = thisTargetPoint;
						}
						GUI.backgroundColor = Color.white;
					}
					else
					{
						GUI.backgroundColor = Color.red;
						if(GUILayout.Button ("Y",EditorStyles.miniButtonLeft,GUILayout.Width (20)))
						{
							curEditorLevel.RemoveTargetPoint (thisTargetPoint);
						}
						GUI.backgroundColor = Color.green;
						if(GUILayout.Button ("N",EditorStyles.miniButtonRight,GUILayout.Width (20)))
						{
							waitToDeleteTargetPoint = null;
						}
						GUI.backgroundColor = Color.white;
					}

					GUILayout.EndHorizontal ();

					GUILayout.BeginHorizontal ();
					GUILayout.Button ("Start:(" + thisTargetPoint.lattice01Coord.x + "," + thisTargetPoint.lattice01Coord.y + ")",EditorStyles.miniButtonLeft,GUILayout.Width (targetUISize * 0.5f));
					GUILayout.Button ("End:(" + thisTargetPoint.lattice02Coord.x + "," + thisTargetPoint.lattice02Coord.y + ")",EditorStyles.miniButtonRight,GUILayout.Width (targetUISize * 0.5f));
					GUILayout.EndHorizontal ();

					GUILayout.EndVertical ();
					GUILayout.EndHorizontal ();
				}
			}
			GUILayout.EndHorizontal ();
			GUILayout.EndScrollView ();
			GUILayout.EndHorizontal ();
		}

		GUILayout.EndVertical ();
	}

    void LevelMapEditor ()
    {
        if (curEditorLevel == null)
            return;
        
        GameLevel level = curEditorLevel;
        GUILayout.BeginHorizontal();
        GUI.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 1);
        GUILayout.BeginVertical("Box", GUILayout.Width(100));
        GUI.backgroundColor = Color.white;

        int rectWidth = (int)(this.position.width - 240);
        int rectHight = (int)(this.position.height - levelSettingHeight - 140);
        Rect itemRect = GUILayoutUtility.GetRect(rectWidth, rectHight);

        if (level.xCount > 0 && level.yCount > 0)
        {
            float itemSizeY = (rectHight - 20) * 1.0f / level.yCount;
            float itemSizeX = (rectWidth) * 1.0f / level.xCount;
            int itemSize = itemSizeX <= itemSizeY ? (int)itemSizeX : (int)itemSizeY;

            if (itemSize > 60)
                itemSize = 60;

            for (int i = 0; i < level.xCount; i++)
            {
                for (int j = 0; j < level.yCount; j++)
                {
                    Lattice thisLattice = level.GetLattice(i, j);
                    //Lattice rect
                    Rect thisItemRect = new Rect(itemRect.x + i * itemSize, itemRect.y + rectHight - j * itemSize - 20 - itemSize, itemSize, itemSize);

                    //Lattice size
                    float latticeLineLength = thisItemRect.width * 0.4f;
                    float latticeLineWidth = itemSize * 0.08f;
                    Rect newItemRect = new Rect(thisItemRect.x, thisItemRect.y + 20, latticeLineLength, latticeLineLength);


                    #region<Draw Lattice>
                    //Top line rect
                    Rect upLineRect = new Rect(newItemRect.x + latticeLineLength * 0.5f - latticeLineWidth * 0.5f, newItemRect.y - itemSize + latticeLineLength * 0.5f, latticeLineWidth, thisItemRect.height);
                    //right line rect
                    Rect rightLienRect = new Rect(newItemRect.x + latticeLineLength * 0.5f, newItemRect.y + latticeLineLength * 0.5f - latticeLineWidth * 0.5f, thisItemRect.width, latticeLineWidth);

                    GUIStyle lineStyle = EditorStyles.textArea;
                    LineColor lineColor = new LineColor(Color.white, false);

                    if (i < level.xCount - 1 && !Lattice.IsNull(level.GetLattice(i, j, LatticeMatrixDirection.Right)) && !Lattice.IsNull(thisLattice))
                    {

                        lineColor = GetLineColor(thisLattice, level.GetLattice(i, j, LatticeMatrixDirection.Right));
                        GUI.color = lineColor.color;
                        if (lineColor.showLine == true)
                        {
                            lineStyle = editorGUISkin.customStyles[0];
                        }
                        else
                        {
                            lineStyle = EditorStyles.textArea;
                        }

                        GUI.Box(rightLienRect, "", lineStyle);
                        GUI.color = Color.white;
                    }

                    if (j < level.yCount - 1 && !Lattice.IsNull(level.GetLattice(i, j, LatticeMatrixDirection.Up)) && !Lattice.IsNull(thisLattice))
                    {
                        lineColor = GetLineColor(thisLattice, level.GetLattice(i, j, LatticeMatrixDirection.Up));
                        GUI.color = lineColor.color;
                        if (lineColor.showLine == true)
                        {
                            lineStyle = editorGUISkin.customStyles[0];
                        }
                        else
                        {
                            lineStyle = EditorStyles.textArea;
                        }
                        GUI.Box(upLineRect, "", lineStyle);
                        GUI.color = Color.white;
                    }
                    GUI.backgroundColor = Color.white;
                    #endregion

                    #region<Draw Lattice Background>
                    //set color
                    if (curSelectLattice == level.GetLattice(i, j))
                    {
                        if (thisLattice.isNull)
                            GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 0.5f);
                        else
                        {
                            GUI.backgroundColor = new Color(0.6f, 0.6f, 0.6f, 1);
                        }
                    }
                    else
                    {
                        if (thisLattice.isNull)
                            GUI.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.2f);
                        else
                        {
                            GUI.backgroundColor = Color.white;
                        }
                    }

                    //mouse event
                    if (isClickLatticeEvent)
                    {
                        if (newItemRect.Contains(mousePosition))
                        {
                            bool isStopLine = false;
                            if (mouseDownLattice == null)
                            {
                                if (thisLattice.isItemPoint)
                                {
                                    mouseDownLattice = thisLattice;
                                    isStopLine = true;
                                }
                                else
                                {
                                    isStopLine = true;
                                }
                            }
                            else if (mouseDownLattice != thisLattice)
                            {
                                if (thisLattice.isItemPoint && thisLattice.item != mouseDownLattice.item)
                                {
                                    isStopLine = true;
                                }
                                else
                                {
                                    if (!isStartNewLine && !Lattice.IsNull(mouseDownLattice))
                                    {
                                        GameLevel.TargetPointInfo thisTargetPoint = level.GetTargetPoint(mouseDownLattice.item);
                                        if (thisTargetPoint != null)
                                        {
                                            thisTargetPoint.ClearLineCoords();
                                            thisTargetPoint.AddLineCoords(new LatticeCoord(mouseDownLattice.x, mouseDownLattice.y));
                                        }
                                        isStartNewLine = true;
                                    }
                                }
                            }

                            if (isCopyLatticeEvent)
                            {
                                isStopLine = true;
                            }

                            //line complete
                            if (!Lattice.IsNull(mouseDownLattice) && mouseDownLattice.isItemPoint)
                            {
                                GameLevel.TargetPointInfo thisTargetPoint = level.GetTargetPoint(mouseDownLattice.item);
                                if (thisTargetPoint.lineLatticeCoords.Count > 1)
                                {
                                    Lattice firstLineLattice = level.GetLattice(thisTargetPoint.FirstLatticeCoord().x, thisTargetPoint.FirstLatticeCoord().y);
                                    Lattice lastLineLattice = level.GetLattice(thisTargetPoint.LastLatticeCoord().x, thisTargetPoint.LastLatticeCoord().y);

                                    if (!Lattice.IsNull(firstLineLattice) && !Lattice.IsNull(lastLineLattice))
                                    {
                                        if (firstLineLattice != lastLineLattice && firstLineLattice.item == lastLineLattice.item)
                                            isStopLine = true;
                                    }
                                }
                            }


                            //add line path
                            if (!isStopLine)
                            {
                                if (!Lattice.IsNull(mouseDownLattice) && mouseDownLattice.isItemPoint)
                                {
                                    GameLevel.TargetPointInfo thisTargetPoint = level.GetTargetPoint(mouseDownLattice.item);

                                    LatticeCoord lastLatticeCoord = thisTargetPoint.LastLatticeCoord();
                                    if (lastLatticeCoord != null)
                                    {
                                        Lattice lastLineLattice = level.GetLattice(lastLatticeCoord.x, lastLatticeCoord.y);

                                        if (Lattice.IsSideLattice(thisLattice, lastLineLattice))
                                        {
                                            thisTargetPoint.AddLineCoords(new LatticeCoord(thisLattice.x, thisLattice.y));
                                        }
                                        else
                                        {
                                            if (thisTargetPoint.ContainsCoord(new LatticeCoord(thisLattice.x, thisLattice.y)))
                                            {
                                                thisTargetPoint.AddLineCoords(new LatticeCoord(thisLattice.x, thisLattice.y));
                                            }
                                        }
                                    }
                                }
                            }
                            lastMouseLattice = thisLattice;
                            GUI.backgroundColor = new Color(0.6f, 0.6f, 0.6f, 1);
                        }
                    }


                    LineColor latticeColor = GetLinePointColor(thisLattice);
                    GUIStyle latticeStyle = EditorStyles.miniButton;
                    if (latticeColor.showLine && !thisLattice.isItemPoint)
                    {
                        newItemRect = new Rect(newItemRect.x + newItemRect.width * 0.25f, newItemRect.y + newItemRect.height * 0.25f, newItemRect.width * 0.5f, newItemRect.height * 0.5f);
                        latticeStyle = editorGUISkin.customStyles[1];
                        GUI.color = latticeColor.color;
                    }

                    if (thisLattice.isItemPoint)
                        latticeStyle = editorGUISkin.customStyles[1];

                    if (GUI.Button(newItemRect, "", latticeStyle))
                    {
                        if (curSelectLattice != thisLattice)
                        {
                            curSelectLattice = level.GetLattice(i, j);
                        }
                        else
                            curSelectLattice = null;
                    }
                    GUI.color = Color.white;

                    //Draw Item
                    if (thisLattice.item != null)
                    {
                        GUI.color = thisLattice.item.itemColor;

                        if (thisLattice == curSelectLattice)
                            GUI.color = new Color(GUI.color.r * 0.6f, GUI.color.g * 0.6f, GUI.color.b * 0.6f, 1);
                        Rect textureRect = new Rect(newItemRect.x - newItemRect.width * 0.5f, newItemRect.y - newItemRect.height * 0.5f, newItemRect.width * 2, newItemRect.height * 2);
                        GUI.DrawTexture(textureRect, thisLattice.item.GetUIShowTexture());
                        GUI.color = Color.white;
                    }

                    //Draw drag texture
                    if (isCopyLatticeEvent)
                    {
                        if (!Lattice.IsNull(mouseDownLattice))
                        {
                            if (mouseDownLattice.isItemPoint)
                            {
                                Rect mouseDragRect = new Rect(mousePosition.x - newItemRect.width, mousePosition.y - newItemRect.height, newItemRect.width * 2, newItemRect.height * 2);
                                GUI.color = mouseDownLattice.item.itemColor;
                                GUI.DrawTexture(mouseDragRect, mouseDownLattice.item.GetUIShowTexture());
                                GUI.color = Color.white;
                            }
                        }
                    }
                    #endregion
                    GUI.backgroundColor = Color.white;
                }
            }
        }

        //Map size
        GUI.Label(itemRect, "Map size:" + curEditorLevel.xCount + "X" + curEditorLevel.yCount);

        if (GUI.Button(new Rect(itemRect.x + rectWidth - 20, itemRect.y, 20, (rectHight - 20) * 0.5f), "+"))
        {
            if(level.xCount < 10)
                level.AddXLattices();
        }
        if (GUI.Button(new Rect(itemRect.x + rectWidth - 20, itemRect.y + (rectHight - 20) * 0.5f, 20, (rectHight - 20) * 0.5f), "-"))
        {
            level.RemoveXLattices();
        }

        if (GUI.Button(new Rect(itemRect.x, itemRect.y + rectHight - 20, (rectWidth - 20) * 0.5f, 20), "+"))
        {
            if(level.yCount < 10)
                level.AddYLattices();
        }

        if (GUI.Button(new Rect(itemRect.x + (rectWidth - 20) * 0.5f, itemRect.y + rectHight - 20, (rectWidth - 20) * 0.5f, 20), "-"))
        {
            level.RemoveYLattices();
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

	LineColor GetLinePointColor (Lattice pointLattice)
	{
		if(Lattice.IsNull (pointLattice))
			return new LineColor(Color.white,false);

        if(curEditorLevel == null)
            return new LineColor(Color.white, false);
        
		for (int i = 0; i < curEditorLevel.targetPointCount; i++)
		{
			GameLevel.TargetPointInfo thisTargetPoint = curEditorLevel.GetTargetPoint (i);
			if(thisTargetPoint != null)
			{
				for (int j = 0; j < thisTargetPoint.lineLatticeCoords.Count; j++)
				{
					LatticeCoord thisCoord = thisTargetPoint.lineLatticeCoords[j];
					if(thisCoord.x == pointLattice.x && thisCoord.y == pointLattice.y)
					{
						if(thisTargetPoint.item != null)
							return new LineColor(thisTargetPoint.item.itemColor,true);
					}
				}
			}
		}
		return new LineColor(Color.white,false);
	}

	LineColor GetLineColor (Lattice lattice01,Lattice lattice02)
	{
		if(!Lattice.IsSideLattice (lattice01,lattice02))
			return new LineColor(Color.white,false);

		if(Lattice.IsNull (lattice01) || Lattice.IsNull (lattice02))
			return new LineColor(Color.white,false);

        if (curEditorLevel == null)
            return new LineColor(Color.white, false);
        
		for (int i = 0; i < curEditorLevel.targetPointCount; i++)
		{
			GameLevel.TargetPointInfo thisTargetPoint = curEditorLevel.GetTargetPoint (i);
			if(thisTargetPoint != null)
			{
				for (int j = 0; j < thisTargetPoint.lineLatticeCoords.Count; j++)
				{
					LatticeCoord thisCoord = thisTargetPoint.lineLatticeCoords[j];
					if(thisCoord.x == lattice01.x && thisCoord.y == lattice01.y)
					{
						LatticeCoord backCoord = j > 0?thisTargetPoint.lineLatticeCoords[j - 1]:null;
						LatticeCoord nextCoord = j < thisTargetPoint.lineLatticeCoords.Count - 1?thisTargetPoint.lineLatticeCoords[j + 1]:null;

						if(backCoord != null)
						{
							if(backCoord.x == lattice02.x && backCoord.y == lattice02.y)
							{
							    if(thisTargetPoint.item != null)
									return new LineColor(thisTargetPoint.item.itemColor,true);
							}
						}

						if(nextCoord != null)
						{
							if(nextCoord.x == lattice02.x && nextCoord.y == lattice02.y)
								return new LineColor(thisTargetPoint.item.itemColor,true);
						}
					}
				}
			}
		}
		return new LineColor(Color.white,false);
	}

	void OnMouseUp_SetCopyLattice ()
	{
		if(Lattice.IsNull (mouseDownLattice) || !mouseDownLattice.isItemPoint)
		{
			Debug.Log ("Click Lattice is null");
			return;
		}

		if(Lattice.IsNull (lastMouseLattice) || lastMouseLattice.isItemPoint)
		{
			Debug.Log ("Last lattice is null");
			return;
		}

		lastMouseLattice.SetItemPoint (mouseDownLattice.item);
		for (int i = 0; i < curEditorLevel.targetPointCount; i++)
		{
			GameLevel.TargetPointInfo thisTargetPoint = curEditorLevel.GetTargetPoint (i);
			if(thisTargetPoint.lattice01Coord.x == mouseDownLattice.x && thisTargetPoint.lattice01Coord.y == mouseDownLattice.y)
			{
				thisTargetPoint.lattice01Coord = new LatticeCoord(lastMouseLattice.x,lastMouseLattice.y);
				thisTargetPoint.ClearLineCoords ();
			}

			if(thisTargetPoint.lattice02Coord.x == mouseDownLattice.x && thisTargetPoint.lattice02Coord.y == mouseDownLattice.y)
			{
				thisTargetPoint.lattice02Coord = new LatticeCoord(lastMouseLattice.x,lastMouseLattice.y);
				thisTargetPoint.ClearLineCoords ();
			}

		}
		mouseDownLattice.ClearItemPoint ();
		Debug.Log ("Move target point");
	}

	GameLevel CreateLevel (GameLevelGroup group)
	{
		string path = EditorUtility.SaveFilePanelInProject("Save As", "GameLevel" + group.levelCount + ".prefab", "prefab", "Save as...");
		if(!string.IsNullOrEmpty(path))
		{
			GameObject go = new GameObject();
			go.name = name;
			GameLevel newLevel = go.AddComponent<GameLevel>();
			newLevel.SetXYCount (6,6);
			PrefabUtility.CreatePrefab(path,go);
			Object.DestroyImmediate(go);
			return AssetDatabase.LoadAssetAtPath(path,typeof(GameLevel)) as GameLevel;
		}
		return null;
	}

	void OnAddTargetPoint (Item addItem)
	{
		if(addItem == null)
			return;
		curEditorTargetPoint.item = addItem;
		curEditorLevel.GetLattice(curEditorTargetPoint.lattice01Coord).SetItemPoint(addItem);
		curEditorLevel.GetLattice(curEditorTargetPoint.lattice02Coord).SetItemPoint(addItem);
	}
}
