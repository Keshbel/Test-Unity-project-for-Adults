using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameGUI_PuaseUI : Jun_ComponentSingletonObject<GameGUI_PuaseUI> {

	public GUI_Button playButton;
	public GUI_Button homeButton;
	public GUI_Button restartButton;

	public GUI_Button musicButton;
	public GUI_Button soundButton;

	public Sprite musicOffIcon;
	public Sprite musicOnIcon;
	public Sprite soundOffIcon;
	public Sprite soundOnIcon;

	// Use this for initialization
	void Start () {
		playButton.SetNewListener (playButton_OnClick);
		homeButton.SetNewListener (homeButton_OnClick);
		restartButton.SetNewListener (restartButton_OnClick);

		musicButton.SetNewListener (musicButton_OnClick);
		soundButton.SetNewListener (soundButton_OnClick);

		ShowMusicButtonIcon ();
		ShowSoundButtonIcon ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void Show ()
	{
		base.Show ();
		Time.timeScale = 0;
		GameController.gamePuased = true;
	}

	public override void Hide ()
	{
		base.Hide ();
		Time.timeScale = 1;
		GameController.gamePuased = false;
	}

	void playButton_OnClick (GameObject go)
	{
		Hide ();
		GameGUI_GamingUI.instance.Show ();
	}

	void homeButton_OnClick (GameObject go)
	{
		GUI_LoadingUI.instance.LoadLevel ("Levels");
		Hide ();
	}

	void restartButton_OnClick (GameObject go)
	{
		GUI_LoadingUI.instance.LoadLevel ("Game");
		Hide ();
	}

	void musicButton_OnClick (GameObject go)
	{
		AudionManger.musiceOff = !AudionManger.musiceOff;
		ShowMusicButtonIcon ();
	}

	void ShowMusicButtonIcon ()
	{
		Image image = (Image)musicButton.targetGraphic;
		if(AudionManger.musiceOff)
		{
			image.sprite = musicOffIcon;
		}
		else
		{
			image.sprite = musicOnIcon;
		}
	}

	void soundButton_OnClick (GameObject go)
	{
		AudionManger.soundOff = !AudionManger.soundOff;
		ShowSoundButtonIcon ();
	}

	void ShowSoundButtonIcon ()
	{
		Image image = (Image)soundButton.targetGraphic;
		if(AudionManger.soundOff)
		{
			image.sprite = soundOffIcon;
		}
		else
		{
			image.sprite = soundOnIcon;
		}
	}
}
