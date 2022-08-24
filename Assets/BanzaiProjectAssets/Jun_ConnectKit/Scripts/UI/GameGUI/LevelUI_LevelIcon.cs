using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUI_LevelIcon : MonoBehaviour {

	public delegate void OnClick (LevelUI_LevelIcon clickIcon);

	public OnClick onClick;

	public TextMesh textMesh;
	public SpriteRenderer[] stars;
	public Transform banner;

	public GameObject openUI;
	public GameObject lockUI;

	public AudioClip clickAudio;

	private GameLevel _gameLevel;
	public GameLevel gameLevel{get{return _gameLevel;}}

	public void SetLevel (GameLevel setLevel)
	{
		_gameLevel = setLevel;
		_gameLevel.LoadData ();

		if(_gameLevel.isUnlock)
		{
			openUI.SetActive (true);
			lockUI.SetActive (false);
		}
		else
		{
			openUI.SetActive (false);
			lockUI.SetActive (true);
		}

		Color hideStarColor = new Color(0,0,0,0.5f);
		stars[0].color = hideStarColor;
		stars[1].color = hideStarColor;
		stars[2].color = hideStarColor;

		switch (setLevel.starCount)
		{
		case 0:
			stars[0].gameObject.SetActive (false);
			stars[1].gameObject.SetActive (false);
			stars[2].gameObject.SetActive (false);
			break;

		case 1:
			stars[0].color = Color.white;
			break;

		case 2:
			stars[1].color = Color.white;
			stars[2].color = Color.white;
			break;

		case 3:
			stars[0].color = Color.white;
			stars[1].color = Color.white;
			stars[2].color = Color.white;
			break;
		}

		textMesh.text = (setLevel.levelID + 1).ToString ();
	}

	void OnMouseUpAsButton() {
		if(onClick != null)
		{
			onClick (this);
		}

		AudionManger.inter.Play (clickAudio,false,false,0);
	}
}
