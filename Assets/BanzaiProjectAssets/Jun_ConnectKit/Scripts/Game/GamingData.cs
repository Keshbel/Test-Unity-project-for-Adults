using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamingData
{
	public LatticeMatrix latticeMatrix = new LatticeMatrix();
	public List <GameLevel.TargetPointInfo> targetPoints = new List<GameLevel.TargetPointInfo>();

	int _completeLine = 0;
	int _getCrystal = 0;
	int _gameTime = 0;
	int _starCount = 0;
	public int completeLine {get{return _completeLine;}}
	public int getCrystal {get{return _getCrystal;}}
	public int gameTime{get{return _gameTime;}set{_gameTime = value;}}
	public int starCount {get{return _starCount;}set {_starCount = value;}}

	private int _crystalCount;
	public int crystalCount {get{return _crystalCount;}}
	public int allLatticeCount {get{return latticeMatrix.xCount * latticeMatrix.yCount;}}

	public bool isGetAllCrystal {get{if(_getCrystal >= crystalCount) return true; return false;}}
	public bool isAllLineComplete {get{if(_completeLine >= targetPoints.Count) return true; return false;}}

	public void Init (GameLevel initLevel)
	{
		latticeMatrix = initLevel.latticeMatrix.GetNewLatticeMatrix ();

		for (int x = 0; x < latticeMatrix.xCount; x++)
		{
			for (int y = 0; y < latticeMatrix.yCount; y++)
			{
				Lattice thisLattice = latticeMatrix.GetLattice (x,y);
				if(!Lattice.IsNull (thisLattice) && !thisLattice.isItemPoint)
					_crystalCount ++;
			}
		}

		for (int i = 0; i < initLevel.targetPointCount; i++)
		{
			targetPoints.Add (initLevel.GetTargetPoint (i).GetNewInfo ());
			targetPoints[i].ClearLineCoords ();
		}
	}

	public void UpdateData ()
	{
		_completeLine = 0;
		_getCrystal = 0;

		List<Lattice> allLattice = new List<Lattice>();
		for (int x = 0; x < latticeMatrix.xCount; x++)
		{
			for (int y = 0; y < latticeMatrix.yCount; y++)
			{
				Lattice thisLattice = latticeMatrix.GetLattice (x,y);

				if(!thisLattice.isItemPoint)
				    allLattice.Add (thisLattice);
			}
		}

		for (int i = 0; i < targetPoints.Count; i++)
		{
			GameLevel.TargetPointInfo thisTarget = targetPoints[i];
			LatticeCoord firstCoord = thisTarget.FirstLatticeCoord ();
			LatticeCoord lastCoord = thisTarget.LastLatticeCoord ();
            
			if(firstCoord != null && lastCoord != null)
			{
			    Lattice firstLattice = latticeMatrix.GetLattice (firstCoord.x,firstCoord.y);
				Lattice lastLattice = latticeMatrix.GetLattice (lastCoord.x,lastCoord.y);

				if(!Lattice.IsNull(firstLattice) && !Lattice.IsNull(lastLattice))
				{
					if(firstLattice != lastLattice && firstLattice.item == lastLattice.item)
						_completeLine ++;
				}
			}
            
			for (int lineID = 0; lineID < thisTarget.lineLatticeCoords.Count; lineID ++)
			{
				LatticeCoord thisCoord = thisTarget.lineLatticeCoords[lineID];
				Lattice thisLattice = latticeMatrix.GetLattice (thisCoord.x,thisCoord.y);
				if(allLattice.Contains (thisLattice))
					allLattice.Remove (thisLattice);
			}
		}

		_getCrystal = latticeMatrix.xCount * latticeMatrix.yCount - targetPoints.Count*2 - allLattice.Count;
	}
}
