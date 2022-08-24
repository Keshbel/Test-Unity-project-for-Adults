using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum LatticeMatrixDirection
{
	Up,
	Down,
	Left,
	Right,
	UpLeft,
	UpRight,
	DownLeft,
	DownRight
}

[System.Serializable]
public class LatticeMatrix
{
	public class LatticeMatrixLength
	{
		public int startPos;
		public int length;

		public LatticeMatrixLength (int matrixLength,int lengthStartPos)
		{
			startPos = lengthStartPos;
			length = matrixLength;
		}
	}

	[SerializeField]List<Lattice> lattices = new List<Lattice>();
	[SerializeField]int _xCount;
	[SerializeField]int _yCount;
	public int xCount{get{return _xCount;}}
	public int yCount{get{return _yCount;}}

	public int latticeCount 
	{
		get
		{
			return lattices.Count;
		}
	}

	public List<Lattice> getAllLattice
	{
		get{return lattices;}
	}

	public LatticeMatrix GetNewLatticeMatrix ()
	{
		LatticeMatrix newMatrix = new LatticeMatrix();
		newMatrix.CreateLattices (xCount,yCount);
		for (int x = 0; x < xCount; x++)
		{
			for (int y = 0; y < yCount; y++)
			{
				newMatrix.SetLattice (x,y,GetLattice (x,y).GetNewLattice ());
			}
		}
		return newMatrix;
	}

	public void SetAllLatticeCoord ()
	{
		for (int i = 0; i < xCount; i++)
		{
			for(int j = 0; j < yCount; j++)
			{
				Lattice thisLattice = GetLattice (i,j);
				thisLattice.SetXY (i,j);
			}
		}
	}
	
	public void CreateLattices (int xLatticeCount,int yLatticeCount)
	{
		int count = xLatticeCount * yLatticeCount;
		_xCount = xLatticeCount;
		_yCount = yLatticeCount;
		for (int i = 0; i < count; i++)
		{
			lattices.Add (new Lattice());
		}

		SetAllLatticeCoord ();
	}
	
	public void AddXLattices ()
	{
		_xCount += 1;
		if(_yCount == 0)_yCount = 1;
		
		List<Lattice> oldLattices = new List<Lattice>();
		oldLattices.AddRange(lattices);
		
		List<Lattice> newLattices = GetNewLattices (xCount,yCount);

		
		for (int i = 0; i < xCount; i++)
		{
			for (int j = 0; j < yCount; j++)
			{
				int inex = GetInexInList(i,j,xCount - 1);
				if(i < xCount - 1)
				{
					if(inex < lattices.Count)
						newLattices = SetLatticeInList (i,j,xCount,newLattices,oldLattices[inex]);
				}
				else
				{
					newLattices = SetLatticeInList (i,j,xCount,newLattices,new Lattice());
				}
			}
		}
		
		lattices = newLattices;

		SetAllLatticeCoord ();
	}
	
	public void AddYLattices ()
	{
		_yCount += 1;
		if(_xCount == 0)_xCount = 1;
		
		List<Lattice> oldLattices = new List<Lattice>();
		oldLattices.AddRange(lattices);
		
		List<Lattice> newLattices = GetNewLattices (xCount,yCount);
		
		for (int i = 0; i < xCount; i++)
		{
			for (int j = 0; j < yCount; j++)
			{
				int inex = GetInexInList (i,j,xCount);
				if(j < yCount - 1)
				{
					if(inex < lattices.Count)
						newLattices = SetLatticeInList (i,j,xCount,newLattices,oldLattices[inex]);
				}
				else
				{
					newLattices = SetLatticeInList (i,j,xCount,newLattices,new Lattice());
				}
			}
		}
		
		lattices = newLattices;

		SetAllLatticeCoord ();
	}
	
	public void RemoveXLattices ()
	{
		if(_xCount > 1)
		{
			_xCount --;

			List<Lattice> oldLattices = new List<Lattice>();
			oldLattices.AddRange(lattices);
			
			List<Lattice> newLattices = GetNewLattices(xCount,yCount);
			
			for (int i = 0; i < xCount; i++)
			{
				for (int j = 0; j < yCount; j++)
				{
					int oldXCount = xCount + 1;
					int inex = GetInexInList(i,j,oldXCount);
					if(inex < oldLattices.Count)
					{
						newLattices = SetLatticeInList (i,j,xCount,newLattices,oldLattices[inex]);
					}
				}
			}
			lattices = newLattices;
		}
	}
	
	public void RemoveYLattices ()
	{
		if(_yCount > 1)
		{
			_yCount --;
			
			List<Lattice> oldLattices = new List<Lattice>();
			oldLattices.AddRange(lattices);
			
			List<Lattice> newLattices = GetNewLattices (xCount,yCount);
			
			for (int i = 0; i < xCount; i++)
			{
				for (int j = 0; j < yCount; j++)
				{
					int inex = GetInexInList (i,j,xCount);
					if(inex < oldLattices.Count)
					{
						newLattices = SetLatticeInList (i,j,xCount,newLattices,oldLattices[inex]);
					}
				}
			}
			lattices = newLattices;
		}
	}

	public List<Lattice> GetSideLattice (int xPos,int yPos)
	{
		return GetSideLattice (xPos,yPos,true);
	}

	public List<Lattice> GetSideLattice (int xPos,int yPos,bool isQuartet)
	{
		List<Lattice> newLattices = new List<Lattice>();

		Lattice upLattice = GetLattice (xPos,yPos,LatticeMatrixDirection.Up);
		Lattice downLattice =  GetLattice (xPos,yPos,LatticeMatrixDirection.Down);
		Lattice leftLattice = GetLattice (xPos,yPos,LatticeMatrixDirection.Left);
		Lattice rightLattice = GetLattice (xPos,yPos,LatticeMatrixDirection.Right);

		if(!Lattice.IsNull (upLattice)) newLattices.Add (upLattice);
		if(!Lattice.IsNull (downLattice)) newLattices.Add (downLattice);
		if(!Lattice.IsNull (leftLattice)) newLattices.Add (leftLattice);
		if(!Lattice.IsNull (rightLattice)) newLattices.Add (rightLattice);

		if(!isQuartet)
		{
			Lattice upLeftLattice = GetLattice (xPos,yPos,LatticeMatrixDirection.UpLeft);
			Lattice upRightLattice = GetLattice (xPos,yPos,LatticeMatrixDirection.UpRight);
			Lattice downLeftLattice = GetLattice (xPos,yPos,LatticeMatrixDirection.DownLeft);
			Lattice downRightLattice = GetLattice (xPos,yPos,LatticeMatrixDirection.DownRight);

			if(!Lattice.IsNull (upLeftLattice)) newLattices.Add (upLeftLattice);
			if(!Lattice.IsNull (upRightLattice)) newLattices.Add (upRightLattice);
			if(!Lattice.IsNull (downLeftLattice)) newLattices.Add (downLeftLattice);
			if(!Lattice.IsNull (downRightLattice)) newLattices.Add (downRightLattice);
		}

		List<Lattice> outOfOrderList = new List<Lattice>();
		while (newLattices.Count > 0)
		{
			int id = Random.Range (0,newLattices.Count);
			outOfOrderList.Add (newLattices[id]);
			newLattices.RemoveAt (id);
		}
		return outOfOrderList;
	}
	
	public Lattice GetLattice (int xPos,int yPos)
	{
		if (xPos >= 0 && yPos >= 0 && xPos < xCount && yPos < yCount)
		{
			int inex = GetInexInList (xPos, yPos, xCount);
			if (inex < latticeCount && inex >= 0)
				return lattices [inex];
		}
		return null;
	}

	public Lattice GetLattice (Lattice thisLattice,LatticeMatrixDirection dirction)
	{
		if(thisLattice == null)
			return null;
		
		return GetLattice (thisLattice.x,thisLattice.y,dirction);
	}

	public Lattice GetLattice (int xPos,int yPos,LatticeMatrixDirection dirction)
	{
		switch (dirction)
		{
		case LatticeMatrixDirection.Up:
			return GetLattice (xPos,yPos + 1);

		case LatticeMatrixDirection.Down:
			return GetLattice (xPos,yPos - 1);

		case LatticeMatrixDirection.Left:
			return GetLattice (xPos - 1,yPos);

		case LatticeMatrixDirection.Right:
			return GetLattice (xPos + 1,yPos);

		case LatticeMatrixDirection.UpLeft:
			return GetLattice (xPos - 1,yPos + 1);

		case LatticeMatrixDirection.UpRight:
			return GetLattice (xPos + 1,yPos + 1);

		case LatticeMatrixDirection.DownLeft:
			return GetLattice (xPos - 1,yPos - 1);

		case LatticeMatrixDirection.DownRight:
			return GetLattice (xPos + 1,yPos - 1);
		}
		return null;
	}
	
	public Lattice GetLatticeFromList (int xPos,int yPos,int xLatticeCount,List <Lattice> list)
	{
		int inex = GetInexInList (xPos,yPos,xLatticeCount);
		if(inex < list.Count)
			return list[inex];
		return null;
	}

	public void SetLattice (int xPos,int yPos,Lattice setLattice)
	{
	    if(xPos < xCount && yPos < yCount)
	    {
	        int inex = GetInexInList (xPos,yPos,xCount);
	        if(inex < latticeCount)
	            lattices[inex] = setLattice;
	    }
	}

	public List<Lattice> SetLatticeInList (int xPos,int yPos,int xLatticeCount,List <Lattice> list,Lattice setLattice)
	{
		int inex = GetInexInList (xPos,yPos,xLatticeCount);
		if(inex < list.Count)
		{
			list[inex] = setLattice;
		}
		return list;
	}
	
	public List<Lattice> GetNewLattices (int xLatticeCount,int yLatticeCount)
	{
		List <Lattice> newLattices = new List<Lattice>();
		for (int i = 0; i < xLatticeCount*yLatticeCount;i++)
		{
			newLattices.Add (new Lattice());
		}
		return newLattices;
	}

	public int GetInexInList (int xPos,int yPos,int xLatticeCount)
	{
		return xPos + yPos*xLatticeCount;
	}

	public LatticeMatrixLength GetMatrixWidth (int yPos)
	{
		int width = 0;
		int startPos = 0;
		bool isStart = false;
		for (int i = 0; i < xCount; i++)
		{
			Lattice thisLattice = GetLattice (i,yPos);
			if(!Lattice.IsNull (thisLattice))
			{
				width = i;
				isStart = true;
			}
			else
			{
				if(!isStart)
					startPos = i;
			}
		}
		return new LatticeMatrixLength(width - startPos,startPos);
	}

	public LatticeMatrixLength GetMatrixHeight (int xPos)
	{
		int height = 0;
		int startPos = 0;
		bool isStart = false;

		for (int j = 0; j < yCount; j++)
		{
			Lattice thisLattice = GetLattice (xPos,j);
			if(!Lattice.IsNull (thisLattice))
			{
				height = j;
				isStart = true;
			}
			else
			{
				if(!isStart)
					startPos = j;
			}
		}
		return new LatticeMatrixLength(height - startPos,startPos);
	}

	public bool AllLatticeIsFull ()
	{
		for (int x = 0; x < xCount; x++)
		{
			for (int y = 0; y < yCount; y++)
			{
				Lattice thisLattice = GetLattice (x,y);
				if(!thisLattice.isItemPoint & !thisLattice.isLinePoint)
				{
					return false;
				}
			}
		}
		return true;
	}

	public void ResetAllLattice ()
	{
		for (int x = 0; x < xCount; x++)
		{
			for (int y = 0; y < yCount; y++)
			{
				Lattice thisLattice = GetLattice (x,y);
				thisLattice.Reset ();
			}
		}
	}
}
