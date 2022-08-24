using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevel : MonoBehaviour 
{
	public enum LevelType
	{
		GetAllCrystal
	}

	public class AStarPath
	{
		public List<LatticeCoord> path = new List<LatticeCoord>();
		public int pathLength {get{return path.Count;}}
	}

	public class AStarNote
	{
		public Lattice lattices = null;
		public List<Lattice> linkLattice = new List<Lattice>();

		public AStarNote (Lattice thisLattice)
		{
			lattices = thisLattice;
		}

		public void SetLattice (Lattice setLattice)
		{
			lattices = setLattice;
		}

		public void AddLinkLattice (Lattice addLattice)
		{
			if(!linkLattice.Contains (addLattice))
				linkLattice.Add (addLattice);
		}
	}

	[System.Serializable]
	public class TargetPointInfo
	{
		public Item item;
		public LatticeCoord lattice01Coord;
		public LatticeCoord lattice02Coord;
		public List<LatticeCoord> lineLatticeCoords = new List<LatticeCoord>();

		public string[] lineCoordsString
		{
			get
			{
				string[] str = new string[lineLatticeCoords.Count];
				for (int i = 0; i < lineLatticeCoords.Count; i++)
				{
					str[i] = "(" + lineLatticeCoords[i].x + "," + lineLatticeCoords[i].y + ")";
				}
				return str;
			}
		}

		public Vector3[] GetLineVectors ()
		{
			int count = lineLatticeCoords.Count;
			Vector3[] lineVectors = new Vector3[count];
			for (int i = 0; i < lineLatticeCoords.Count; i++)
			{
				lineVectors[i] = new Vector3 (lineLatticeCoords[i].x,0,lineLatticeCoords[i].y);
			}
			return lineVectors;
		}

		public TargetPointInfo GetNewInfo ()
		{
			TargetPointInfo newInfo = new TargetPointInfo();
			newInfo.item = item;
			newInfo.lattice01Coord = lattice01Coord;
			newInfo.lattice02Coord = lattice02Coord;

			return newInfo;
		}

		public bool ContainsCoord (LatticeCoord coord)
		{
			for (int i = 0; i < lineLatticeCoords.Count; i++)
			{
				LatticeCoord thisCoord = lineLatticeCoords[i];
				if(thisCoord.x == coord.x && thisCoord.y == coord.y)
				{
					return true;
				}
			}
			return false;
		}

		public bool AddLineCoords (LatticeCoord addCoord)
		{
			if(!ContainsCoord (addCoord))
			{
				lineLatticeCoords.Add (addCoord);
				return true;
			}
			else
			{
				RemoveCoordAtCoords (addCoord);
				lineLatticeCoords.Add (addCoord);
				return true;
			}
		}

		public void RemoveCoordAtCoords (LatticeCoord removeCoord)
		{
			for (int i = 0; i < lineLatticeCoords.Count; i++)
			{
				LatticeCoord thisCoord = lineLatticeCoords[i];
				if(thisCoord.x == removeCoord.x && thisCoord.y == removeCoord.y)
				{
					lineLatticeCoords.RemoveRange (i,lineLatticeCoords.Count - i);
				}
			}
		}

		public void ClearLineCoords ()
		{
			lineLatticeCoords.Clear ();
		}

		public LatticeCoord LastLatticeCoord ()
		{
			if(lineLatticeCoords.Count > 0)
				return lineLatticeCoords[lineLatticeCoords.Count - 1];
			return null;
		}

		public LatticeCoord FirstLatticeCoord ()
		{
			if(lineLatticeCoords.Count > 0)
				return lineLatticeCoords[0];
			return null;
		}
	}

	public LevelType levelType = LevelType.GetAllCrystal;
	[HideInInspector][SerializeField]LatticeMatrix _latticeMatrix = new LatticeMatrix();
	public LatticeMatrix latticeMatrix {get{return _latticeMatrix;}}

	public int xCount {get{return _latticeMatrix.xCount;}}
	public int yCount {get{return _latticeMatrix.yCount;}}
	public int latticeCount {get{return _latticeMatrix.latticeCount;}}

	[HideInInspector][SerializeField]List<TargetPointInfo> targetPoints = new List<TargetPointInfo>();
	public int targetPointCount {get{return targetPoints.Count;}}

	#region<is unlock>
	[HideInInspector] public bool m_isUnlock = false;
    private bool m_isPlayerUnlock = false;
    public bool isUnlock
    {
        get
        {
            if (m_isUnlock)
                return true;
            return m_isPlayerUnlock;
        }
    }
	#endregion

	[HideInInspector] int getStarCount = 0;
	public int starCount {get {return getStarCount;}}

	[HideInInspector]public bool isEditor = false;

	[HideInInspector]public string sceneName;
	[HideInInspector][SerializeField]int _myGroupID;//所在组的组ID
	[HideInInspector][SerializeField]int _levelID;//在所有关卡里面的ID
	[HideInInspector][SerializeField]int _levelInGroupID;//在组里面的ID
	[HideInInspector] [SerializeField] GameLevelManger _levelManager;

	public int myGroupID {get{return _myGroupID;}set {if(_myGroupID != value) isEditor = true; _myGroupID = value;}}
	public int levelID {get{return _levelID;}set {if(_levelID != value) isEditor = true;_levelID = value;}}
	public int levelInGroupID {get{return _levelInGroupID;} set {if(_levelInGroupID != value) isEditor = true;_levelInGroupID = value;}}

	public int crystalCount
	{
		get
		{
			int _crystalCount = 0;
			for (int x = 0; x < latticeMatrix.xCount; x++)
			{
				for (int y = 0; y < latticeMatrix.yCount; y++)
				{
					Lattice thisLattice = latticeMatrix.GetLattice (x,y);
					if(!Lattice.IsNull (thisLattice) && !thisLattice.isItemPoint)
						_crystalCount ++;
				}
			}
			return _crystalCount;
		}
	}

	public void SetXYCount (int x,int y)
	{
		for (int i = 0; i < x; i++)
		{
			AddXLattices ();
		}

		for (int i = 0; i < y - 1; i++)
		{
			AddYLattices ();
		}
	}

	public void AddXLattices ()
	{
		_latticeMatrix.AddXLattices ();
	}

	public void AddYLattices ()
	{
		_latticeMatrix.AddYLattices ();
	}

	public void RemoveXLattices ()
	{
		if(xCount < 5)
			return;
		
		for (int i = 0; i < yCount; i++)
		{
			Lattice thisLattice = GetLattice (xCount - 1,i);
			if(thisLattice.isItemPoint)
				return;
		}
		_latticeMatrix.RemoveXLattices ();
	}

	public void RemoveYLattices ()
	{
		if(yCount < 5)
			return;
		for (int i = 0; i < xCount; i++)
		{
			Lattice thisLattice = GetLattice (i,yCount - 1);
			if(thisLattice.isItemPoint)
				return;
		}
		_latticeMatrix.RemoveYLattices ();
	}

	public Lattice GetLattice (int x,int y)
	{
		return _latticeMatrix.GetLattice (x,y);
	}

	public Lattice GetLattice (LatticeCoord coord)
	{
		return _latticeMatrix.GetLattice (coord.x,coord.y);
	}

	public Lattice GetLattice (int x,int y,LatticeMatrixDirection direction)
	{
		return _latticeMatrix.GetLattice (x,y,direction);
	}

	public TargetPointInfo AddTargetPoint ()
	{
		TargetPointInfo newPoint = new TargetPointInfo();
		targetPoints.Add (newPoint);
		return newPoint;
	}

	public TargetPointInfo GetTargetPoint (int index)
	{
		if(index < targetPointCount)
			return targetPoints[index];
		return null;
	}

	public TargetPointInfo GetTargetPoint (Item getItem)
	{
		for (int i = 0; i < targetPointCount; i++)
		{
			if(targetPoints[i].item == getItem)
				return targetPoints[i];
		}
		return null;
	}

	public void RemoveTargetPoint (TargetPointInfo removePoint)
	{
		if(targetPoints.Contains (removePoint))
		{
			Lattice lattice01 = GetLattice (removePoint.lattice01Coord);
			Lattice lattice02 = GetLattice (removePoint.lattice02Coord);
			if(!Lattice.IsNull (lattice01))
				lattice01.ClearItemPoint ();
			if(!Lattice.IsNull (lattice02))
				lattice02.ClearItemPoint ();
			
			targetPoints.Remove (removePoint);
		}
	}

	public void ClearAllTargetPoint ()
	{
		while (targetPoints.Count > 0)
		{
			RemoveTargetPoint (targetPoints[0]);
		}

		_latticeMatrix.ResetAllLattice ();
	}

	#region<Load And Save>
	public void LoadData ()
	{
		GetStar ();
		GetPass ();
	}

	public int GetStar ()
	{
		getStarCount = PlayerPrefs.GetInt ("StarCount:GroupID" + myGroupID + "LevelID" + levelInGroupID);
		return getStarCount;
	}

	public void SetStar (int starC)
	{
		if(starC > getStarCount)
		{
			PlayerPrefs.SetInt ("StarCount:GroupID" + myGroupID + "LevelID" + levelInGroupID,starC);
			getStarCount = starC;
		}
	}

	public bool GetPass ()
	{
		if(PlayerPrefs.GetString ("LevelUnlock:GroupID" + myGroupID + "LevelID" + levelInGroupID) == "True")
			m_isPlayerUnlock = true;
		else
			m_isPlayerUnlock = false;
		return isUnlock;
	}

	public void SetPass ()
	{
		PlayerPrefs.SetString ("LevelUnlock:GroupID" + myGroupID + "LevelID" + levelInGroupID,"True");
	}

	public GameLevel NextLevel ()
	{
		if (_levelManager == null)
			return null;
		
		GameLevel nextLevel = null;
		GameLevelGroup parentGroup = _levelManager.GetGroup(myGroupID);
		if(levelInGroupID + 1 < parentGroup.levelCount)
			nextLevel = parentGroup.GetLevel (levelInGroupID + 1);
		else
		{
			if(parentGroup.ID < parentGroup.levelManger.groupCount - 1)
			{
				nextLevel = parentGroup.levelManger.GetGroup (parentGroup.ID + 1).GetLevel (0);
			}
		}

        if (nextLevel == null)
            return this;

		return nextLevel;
	}

	public void OpenNextLevel ()
	{
		GameLevel nextLevel = NextLevel ();

		if(nextLevel != null)
		{
			nextLevel.SetPass ();
		}
	}
	#endregion

	#region<Auto set game level target>
	public void AutoSetTargetPoint (Item[] items)
	{
		if(items.Length == 0)
			return;

		while (true)
		{
			ClearAllTargetPoint ();
			SetNewTargetPointFromItems (items);


			if(LineFullAllLattice ())
				break;
		}
	}

	void SetNewTargetPointFromItems (Item[] items)
	{
		for (int i = 0; i < items.Length; i++)
		{
			if(items[i] != null)
			{
				int c = 0;
				while (true)
				{
					TargetPointInfo newTarget = new TargetPointInfo();
					List<Lattice> canSetLattices = GetCanSetItemPointLattcies ();

					if(canSetLattices.Count > 1)
					{
						Lattice startLattice = canSetLattices [Random.Range (0,canSetLattices.Count)];
						canSetLattices.Remove (startLattice);
						Lattice targetLattice = canSetLattices [Random.Range (0,canSetLattices.Count)];

						AStarPath aStarPath = GetAStarPath (startLattice,targetLattice);

						if(aStarPath != null)
						{
							newTarget.lattice01Coord = new LatticeCoord(startLattice.x,startLattice.y);
							newTarget.lattice02Coord = new LatticeCoord(targetLattice.x,targetLattice.y);



							newTarget.lineLatticeCoords = aStarPath.path;


							newTarget.item = items[i];

							startLattice.SetItemPoint (newTarget.item);
							targetLattice.SetItemPoint (newTarget.item);

							targetPoints.Add (newTarget);
							break;
						}
					}
					else
					{
						break;
					}

					c++;
					if(c > 100)
						break;
				}


			}
		}
	}

	List<Lattice> GetCanSetItemPointLattcies ()
	{
		List<Lattice> canSetLatticeList = new List<Lattice>();
		for (int x = 0; x < xCount; x ++)
		{
			for (int y = 0; y < yCount; y++)
			{
				Lattice thisLattice = GetLattice (x,y);
				if(!thisLattice.isItemPoint && !LatticeInTargetLine (thisLattice) && !Lattice.IsNull (thisLattice))
				{
					canSetLatticeList.Add (thisLattice);
				}
			}
		}

		return canSetLatticeList;
	}

	AStarPath GetAStarPath (Lattice startLattice,Lattice targetLattice)
	{
		if(Lattice.IsNull (startLattice) || Lattice.IsNull (targetLattice))
			return null;
		
		List <AStarNote> lineNotes = new List<AStarNote>();
		List <LatticeCoord> lineCoords = new List<LatticeCoord>();

		lineNotes.Add (new AStarNote(startLattice));

		bool nullPath = false;
		int c = 0;

		while (true)
		{
			AStarNote lastNote = lineNotes[lineNotes.Count - 1];

			if(lineNotes.Count > 1)
			{
				lastNote.AddLinkLattice (lineNotes[lineNotes.Count - 2].lattices);
			}
			List<Lattice> sideLattices = _latticeMatrix.GetSideLattice (lastNote.lattices.x,lastNote.lattices.y);

			float dir = float.MaxValue;
			Lattice minDirLattice = null;

			for (int sideID = 0; sideID < sideLattices.Count; sideID ++)
			{
				Lattice thisSideLattice = sideLattices[sideID];
				bool canAdd = true;

				if(LatticeInTargetLine (thisSideLattice) || Lattice.IsNull (thisSideLattice) || AStarNoteLineContains (lineNotes,thisSideLattice))
					canAdd = false;

				if(lastNote.linkLattice.Contains (thisSideLattice))
					canAdd = false;

				if(thisSideLattice.isItemPoint && thisSideLattice.item != startLattice.item)
					canAdd = false;

				if(canAdd)
				{
					float thisDir = lineNotes.Count + Mathf.Abs (thisSideLattice.x - targetLattice.x) + Mathf.Abs (thisSideLattice.y - targetLattice.y);
					if(thisDir < dir)
					{
						minDirLattice = thisSideLattice;
						dir = thisDir;
					}
				}
			}

			bool re_election = false;
			if(!Lattice.IsNull (minDirLattice))
			{
				lastNote.AddLinkLattice (minDirLattice);
				lineNotes.Add (new AStarNote(minDirLattice));
			}
			else
			{
				re_election = true;
			}

			if(minDirLattice == targetLattice)
			{
				break;
			}
			else if(sideLattices.Count <= 0)
			{
				re_election = true;
			}

			if(re_election)
			{
				AStarNote lastCrossNote = GetLastCrossNote (lineNotes);

				if(lastCrossNote != null)
				{
					int removeID = -1;
					for (int i = 0; i < lineNotes.Count; i++)
					{
						if(lineNotes[i] == lastCrossNote)
						{
							removeID = i + 1;
							break;
						}
					}

					if(removeID >= 0 && removeID < lineNotes.Count)
					{
						lineNotes.RemoveRange (removeID,lineNotes.Count - removeID);
					}
					else
					{
						break;
					}
				}
				else
				{
					nullPath = true;
					break;
				}
			}

			c++;
			if(c > 1000)
			{
				nullPath = true;
				break;
			}
		}


		if(!nullPath)
		{
			foreach (AStarNote thisNote in lineNotes)
			{
				lineCoords.Add (new LatticeCoord(thisNote.lattices.x,thisNote.lattices.y));
			}

			AStarPath newPath = new AStarPath();
			newPath.path = lineCoords;
			return newPath;
		}

		return null;
	}

	bool LatticeInTargetLine (Lattice thisLattice)
	{
		for (int i = 0; i < targetPoints.Count; i++)
		{
			GameLevel.TargetPointInfo thisTargetPoint = targetPoints[i];
			for (int j = 0; j < thisTargetPoint.lineLatticeCoords.Count; j++)
			{
				LatticeCoord thisCoord = thisTargetPoint.lineLatticeCoords[j];
				if(thisLattice.x == thisCoord.x && thisLattice.y == thisCoord.y)
				{
					return true;
				}
			}
		}
		return false;
	}

	bool AStarNoteLineContains (List<AStarNote> noteLine,Lattice thisLattice)
	{
		foreach (AStarNote thisNote in noteLine)
		{
			if(thisNote.lattices == thisLattice)
				return true;
		}
		return false;
	}

	AStarNote GetLastCrossNote (List <AStarNote> noteLine)
	{
		for (int i = noteLine.Count - 2;i >= 0; i--)
		{
			AStarNote thisNote = noteLine[i];
			List<Lattice> sideLattice = latticeMatrix.GetSideLattice (thisNote.lattices.x,thisNote.lattices.y);

			if(thisNote.linkLattice.Count < sideLattice.Count)
				return thisNote;
		}
		return null;
	}
	#endregion

	#region<Full all lattice>
	public bool LineFullAllLattice ()
	{
		while (true)
		{
			bool haveFull = false;
			for (int i = 0; i < targetPoints.Count; i++)
			{
				GameLevel.TargetPointInfo thisTargetPoint = targetPoints[i];
				if(FullSideLattice (thisTargetPoint))
					haveFull = true;
			}

			if(!haveFull)
				break;
		}

		return CheckAllNote ();
	}

	bool FullSideLattice (GameLevel.TargetPointInfo thisTargetPoint)
	{
		Lattice lattice01 = latticeMatrix.GetLattice (thisTargetPoint.lattice01Coord.x,thisTargetPoint.lattice01Coord.y);
		Lattice lattice02 = latticeMatrix.GetLattice (thisTargetPoint.lattice02Coord.x,thisTargetPoint.lattice02Coord.y);

		Lattice[] lattices = new Lattice[2]{lattice01,lattice02};

		LatticeCoord firstCoord = thisTargetPoint.FirstLatticeCoord ();

		bool isFull = false;
		for (int latticeId = 0; latticeId < lattices.Length; latticeId++)
		{
			LatticeCoord thisCoord = new LatticeCoord(lattices[latticeId].x,lattices[latticeId].y);

			List<Lattice> sideLattice = latticeMatrix.GetSideLattice (thisCoord.x,thisCoord.y);
			for (int i = 0; i < sideLattice.Count; i++)
			{
				Lattice thisSideLattice = sideLattice[i];
				LatticeCoord thisSicdCoord = new LatticeCoord(thisSideLattice.x,thisSideLattice.y);
				if(!LatticeInTargetLine (thisSideLattice))
				{
					thisSideLattice.SetItemPoint (thisTargetPoint.item);
					lattices[latticeId].ClearItemPoint ();

					if(thisCoord.Equal (firstCoord))
					{
						thisTargetPoint.lineLatticeCoords.Insert (0,thisSicdCoord);
					}
					else
					{
						thisTargetPoint.lineLatticeCoords.Add (thisSicdCoord);
					}

					switch (latticeId)
					{
					case 0:
						thisTargetPoint.lattice01Coord = thisSicdCoord;
						break;

					case 1:
						thisTargetPoint.lattice02Coord = thisSicdCoord;
						break;
					}
					isFull = true;
					break;
				}
			}
		}

		return isFull;
	}

	bool CheckAllNote ()
	{
		for (int x = 0; x < latticeMatrix.xCount; x++)
		{
			for (int y = 0; y < latticeMatrix.yCount; y++)
			{
				Lattice thisLattice = latticeMatrix.GetLattice (x,y);
				if(!LatticeInTargetLine (thisLattice))
				{
					return false;
				}
			}
		}
		return true;
	}
	#endregion

	float LatticeSistance (Lattice lattice01,Lattice lattice02)
	{
		return Mathf.Abs(lattice01.x - lattice02.x) + Mathf.Abs(lattice01.y - lattice02.y);
	}
}
