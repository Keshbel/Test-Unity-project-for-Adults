using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene_Anchors : MonoBehaviour {

	public enum Anchors
	{
		Midder,
		LeftUp,
		Up,
		RightUp,
		Left,
		Right,
		LeftBottom,
		Bottom,
		RightBottom
	}

	public Anchors anchors = Anchors.Midder;
	GameLevel gameLevel;
	// Use this for initialization
	void Start ()
	{
		gameLevel = GameController.instance.gameLevel;

		switch (anchors)
		{
		case Anchors.LeftUp:
		    transform.position = new Vector3(-1,0,gameLevel.yCount);
			break;

		case Anchors.RightUp:
			transform.position = new Vector3(gameLevel.xCount,0,gameLevel.yCount);
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
