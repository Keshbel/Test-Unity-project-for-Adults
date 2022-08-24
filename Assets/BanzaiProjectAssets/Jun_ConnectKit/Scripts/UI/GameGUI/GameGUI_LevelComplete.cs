using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameGUI_LevelComplete : Jun_ComponentSingletonObject <GameGUI_LevelComplete> 
{

	public GUI_Button nextButton;
	public GUI_Button homeButton;
	public GUI_Button restartButton;

	public Image[] stars;

	public Text timeText;
	public Text targetText;

	public AudioClip starAudio;
    public GameObject gameOverUI;

	// Use this for initialization
	void Start () {
		nextButton.SetNewListener (nextButton_OnClick);
		homeButton.SetNewListener (homeButton_OnClick);
		restartButton.SetNewListener (restartButton_OnClick);
        gameOverUI.SetActive(false);
	}
	   
	public override void Show ()
	{
		base.Show ();
		HideAllStar ();
		timeText.text = Jun_GameTool.TimeString (GameController.instance.gamingData.gameTime);
		targetText.text = GameController.instance.gamingData.crystalCount.ToString ();

		StartCoroutine (ShowStar ());

		AudionManger.inter.Play ("Win");

	}

	IEnumerator ShowStar ()
	{
		switch (GameController.instance.gamingData.starCount)
		{
		case 1:
			stars[0].color = Color.white;
			stars[0].GetComponent <Jun_ScaleTween>().Play ();
			yield return new WaitForSeconds (0.4f);
			AudionManger.inter.Play ("Star_01");
			break;

		case 2:
			stars[1].color = Color.white;
			stars[1].GetComponent <Jun_ScaleTween>().Play ();
			yield return new WaitForSeconds (0.4f);
			AudionManger.inter.Play ("Star_01");
			stars[2].color = Color.white;
			stars[2].GetComponent <Jun_ScaleTween>().Play ();
			yield return new WaitForSeconds (0.4f);
			AudionManger.inter.Play ("Star_02");
			break;

		case 3:
			stars[0].color = Color.white;
			stars[0].GetComponent <Jun_ScaleTween>().Play ();
			yield return new WaitForSeconds (0.4f);
			AudionManger.inter.Play ("Star_01");
			stars[1].color = Color.white;
			stars[1].GetComponent <Jun_ScaleTween>().Play ();
			yield return new WaitForSeconds (0.4f);
			AudionManger.inter.Play ("Star_02");
			stars[2].color = Color.white;
			stars[2].GetComponent <Jun_ScaleTween>().Play ();
			yield return new WaitForSeconds (0.4f);
			AudionManger.inter.Play ("Star_03");
			break;
		}
	}

	void HideAllStar ()
	{
		for (int i = 0; i < stars.Length; i++)
		{
			stars[i].color = new Color(0,0,0,0.5f);
		}
	}

	void nextButton_OnClick (GameObject go)
	{
        GameLevel nextLevel = GameController.instance.gameLevel.NextLevel();

        if(nextLevel == GameController.instance.gameLevel || nextLevel == null)
        {
            gameOverUI.SetActive(true);
            gameOverUI.GetComponent<Jun_ScaleTween>().Play();
        }
        else
        {
            GameController.assetGameLevel = nextLevel;
            GUI_LoadingUI.instance.LoadLevel("AdultScene");
        }
	}

	void homeButton_OnClick (GameObject go)
	{
		GUI_LoadingUI.instance.LoadLevel ("Menu");
	}

	void restartButton_OnClick (GameObject go)
	{
		GUI_LoadingUI.instance.LoadLevel ("GameplayScene");
	}
}
