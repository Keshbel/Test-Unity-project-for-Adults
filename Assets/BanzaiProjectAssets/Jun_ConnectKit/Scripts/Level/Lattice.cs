using UnityEngine;

public enum LatteceItemType
{
	StartLockItem,
	RandomItem
}

[System.Serializable]
public class Lattice
{   
	public bool isNull = false;
	public bool isItemPoint = false;
	public bool isLinePoint = false;

	public GameObject latticeObject;

	[HideInInspector][SerializeField]Item _item;
	public Item item{get{return _item;}set{_item = value;}}

	[SerializeField]int _x;
	[SerializeField]int _y;

	public int x {get{return _x;}}
	public int y {get{return _y;}}

	public Lattice GetNewLattice ()
	{
		Lattice newLattice = new Lattice();
		newLattice.isNull = isNull;
		newLattice.isItemPoint = isItemPoint;
		newLattice.isLinePoint = isLinePoint;
		newLattice.item = item;
		newLattice.SetXY (x,y);
		return newLattice;
	}

	public void SetXY (int setX,int setY)
	{
		_x = setX;
		_y = setY;
	}

	public void Copy (Lattice copyLattice)
	{
		isNull = copyLattice.isNull;
	}

	public static bool IsNull (Lattice thisLattice)
	{
		if (thisLattice == null)
			return true;
		else if (thisLattice.isNull)
			return true;
		return false;
	}

	public static bool IsSideLattice (Lattice currentLattice,Lattice sideLattice)
	{
		if(Mathf.Abs(currentLattice.x - sideLattice.x) + Mathf.Abs (currentLattice.y - sideLattice.y) < 2 && currentLattice != sideLattice)
		{
			return true;
		}

		return false;
	}

	public void SetItemPoint (Item setItem)
	{
		item = setItem;
		isItemPoint = true;
	}

	public void ClearItemPoint ()
	{
		item = null;
		isItemPoint = false;
	}

	public void Reset ()
	{
		ClearItemPoint ();
		isLinePoint = false;
	}
}
