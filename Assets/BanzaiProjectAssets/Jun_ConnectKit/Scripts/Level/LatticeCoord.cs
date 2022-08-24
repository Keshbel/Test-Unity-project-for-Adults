using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LatticeCoord 
{
	public int x;
	public int y;

	public LatticeCoord (int setX,int setY)
	{
		x = setX;
		y = setY;
	}

	public bool Equal (LatticeCoord coord)
	{
		if(coord == null)
			return false;
		if(x == coord.x && y == coord.y)
			return true;
		return false;
	}
}
