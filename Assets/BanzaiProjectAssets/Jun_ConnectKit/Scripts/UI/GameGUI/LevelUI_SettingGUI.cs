using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI_SettingGUI : MonoBehaviour {

	public GUI_Button settingButton;
	public GUI_Button closeButton;

	public GameObject settingUI;

	public GUI_Button musicButton;
	public GUI_Button soundButton;

	public Sprite musicOffIcon;
	public Sprite musicOnIcon;
	public Sprite soundOffIcon;
	public Sprite soundOnIcon;

	// Use this for initialization
	void Start () {
		settingButton.SetNewListener (settingButton_OnClick);
		closeButton.SetNewListener (closeButton_OnClick);

		musicButton.SetNewListener (musicButton_OnClick);
		soundButton.SetNewListener (soundButton_OnClick);

		ShowMusicButtonIcon ();
		ShowSoundButtonIcon ();

		settingUI.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void settingButton_OnClick (GameObject go)
	{
		settingUI.SetActive (true);
	}

	void closeButton_OnClick (GameObject go)
	{
		settingUI.SetActive (false);
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
