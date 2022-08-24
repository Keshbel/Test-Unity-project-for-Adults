using UnityEngine;

public class GameGUI_Controller : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{

		GameController.instance.onGameStatusChange += OnGameStatusChange;

		GameGUI_GamingUI.instance.Hide ();
		GameGUI_LevelComplete.instance.Hide ();
		GameGUI_PuaseUI.instance.Hide ();
	}

	void OnGameStatusChange (GameController.GameStatus changeStatus)
	{
		switch (changeStatus)
		{
		case GameController.GameStatus.Gaming:
			GameGUI_GamingUI.instance.Show ();
			break;

		case GameController.GameStatus.GameSettlement:
			break;

		case GameController.GameStatus.GameOver:
			GameGUI_GamingUI.instance.Hide ();
			break;

		case GameController.GameStatus.LevelCompletions:
			GameGUI_LevelComplete.instance.Show ();
			break;
		}
	}
}
