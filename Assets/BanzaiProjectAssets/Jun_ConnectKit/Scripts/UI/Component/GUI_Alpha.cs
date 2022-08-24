using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUI_Alpha : MonoBehaviour {

	public class ItemAlphaInfo 
	{
		public Graphic graphic;
		public float startAlpha;

		public ItemAlphaInfo (Graphic setGraphic,float setStartAlpha)
		{
			graphic = setGraphic;
			startAlpha = setStartAlpha;
		}
	}

	public float alpha = 1.0f;

	public Graphic[] graphics;

	private ItemAlphaInfo[] alphaInfos;

	// Use this for initialization
	void Start () 
	{
		GetAlphaInfo ();
		SetAlpha ();
	}
	
	// Update is called once per frame
	void Update () {
		SetAlpha ();
	}

	void SetAlpha ()
	{
		for (int i = 0; i < alphaInfos.Length; i++)
		{
			ItemAlphaInfo thisAlphaInfo = alphaInfos[i];

			Color thisColor = thisAlphaInfo.graphic.color;

			thisAlphaInfo.graphic.color = new Color(thisColor.r,thisColor.g,thisColor.b,thisAlphaInfo.startAlpha*alpha);
		}
	}

	void GetAlphaInfo ()
	{
		alphaInfos = new ItemAlphaInfo[graphics.Length];

		for (int i = 0; i < graphics.Length; i++)
		{
			Color thisColor = graphics[i].color;

			alphaInfos[i] = new ItemAlphaInfo(graphics[i],thisColor.a);
		}
	}
}
