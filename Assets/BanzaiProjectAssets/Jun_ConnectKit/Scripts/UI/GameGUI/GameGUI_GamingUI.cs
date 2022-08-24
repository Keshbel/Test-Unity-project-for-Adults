using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameGUI_GamingUI : Jun_ComponentSingletonObject <GameGUI_GamingUI> {

	public Text timeText;
	public Text targetText;
	public Text linkText;

	public GameObject[] stars;

	public GUI_Button puaseButton;

	// Use this for initialization
	void Start () {
		puaseButton.SetNewListener (puaseButton_OnClick);
	}
	
	// Update is called once per frame
	void Update ()
	{
		switch (GameController.instance.gameLevel.levelType)
		{
		/*case GameLevel.LevelType.Line:
			targetText.text = GameController.instance.gamingData.completeLine + "/" +GameController.instance.gamingData.targetPoints.Count;
			break;*/

		case GameLevel.LevelType.GetAllCrystal:
			targetText.text = GameController.instance.gamingData.getCrystal + "/" +GameController.instance.gamingData.crystalCount;
			linkText.text = GameController.instance.gamingData.completeLine + "/" +GameController.instance.gamingData.targetPoints.Count;
			break;
		}

		timeText.text = Jun_GameTool.TimeString (GameController.instance.gamingData.gameTime);

		float gameTime = GameController.instance.gamingData.gameTime;
		if(gameTime > GameController.star2Time)
		{
			stars[1].SetActive (false);
		}
		else if (gameTime > GameController.star3Time)
		{
			stars[2].SetActive (false);
		}
	}

	void puaseButton_OnClick (GameObject go)
	{
		GameGUI_PuaseUI.instance.Show ();
		Hide ();
	}
}
