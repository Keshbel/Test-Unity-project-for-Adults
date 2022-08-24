using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLineController : MonoBehaviour {

	public class LineObject
	{
		public LineRenderer lineRender;
		public Item item;
		public Vector3[] path;

		public LineObject (Item setItem, LineRenderer setLine)
		{
			item = setItem;
			lineRender = setLine;
		}
	}

	LineRenderer lineRender;
	List<LineObject> lineObjects = new List<LineObject>();
	public Material defulMaterial;
	public Material comMaterial;

	// Use this for initialization
	void Start () 
    {
		lineRender = GetComponentInChildren <LineRenderer>();
	}
	
	public LineObject DrawLine (Item item,Vector3[] linePath)
	{
		LineObject thisLine = GetLineObject (item);
		if(thisLine == null)
		{
			thisLine = AddLineObject (item);
		}
		thisLine.lineRender.positionCount = linePath.Length;
		thisLine.lineRender.material.SetColor ("_Color",item.itemColor);
		thisLine.lineRender.material.SetColor ("_EmissionColor",item.itemColor);
		thisLine.lineRender.SetPositions (linePath);
		thisLine.path = linePath;

		return thisLine;
	}

	public LineObject GetLineObject (Item thisItem)
	{
		foreach (LineObject thisLine in lineObjects)
		{
			if(thisLine.item == thisItem)
				return thisLine;
		}
		return null;
	}

	public LineObject AddLineObject (Item thisItem)
	{
		LineRenderer addLineRender = lineRender;
		if(lineObjects.Count > 0)
		{
			GameObject newObj = Instantiate <GameObject>(lineRender.gameObject,transform);
			addLineRender = newObj.GetComponent <LineRenderer>();
		}

		LineObject newLine = new LineObject(thisItem,addLineRender);
		lineObjects.Add (newLine);
		return newLine;
	}

	public void SetLineComplete (LineObject setLine)
	{
		if(setLine == null)
			return;

		if(setLine.lineRender == null)
			return;

		setLine.lineRender.material.SetColor ("_Color",setLine.item.itemColor);
		setLine.lineRender.material.SetColor ("_EmissionColor",setLine.item.itemColor);
	}
}
