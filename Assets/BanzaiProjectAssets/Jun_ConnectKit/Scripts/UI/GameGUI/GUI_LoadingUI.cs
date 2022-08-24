using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUI_LoadingUI : Jun_ComponentSingletonObject<GUI_LoadingUI> {

	public static string loadLevelName = "Levels";

	Animation anima;
	bool isLoad = false;
	public bool isHide
	{
		get
		{
			if(anima == null)
				return false;

			if(anima["LoadGUIAnimation"].speed == -1 && !anima.isPlaying)
				return true;

			return false;
		}
	}

	// Use this for initialization
	void Start () {
		anima = GetComponent <Animation>();
		Open ();
	}

	Scene curScene;
	// Update is called once per frame
	void Update () 
	{
		if(isLoad)
		{
			if(!anima.isPlaying)
			{
				SceneManager.LoadScene (loadLevelName);
			}
		}
	}

	public void Close ()
	{
		anima["LoadGUIAnimation"].speed = 1;
		anima["LoadGUIAnimation"].time = 0;
		anima.Play ();
	}

	public void Open ()
	{
		anima["LoadGUIAnimation"].speed = -1;
		anima["LoadGUIAnimation"].time = anima["LoadGUIAnimation"].length;
		anima.Play ();
	}

	public void LoadLevel (string levelName)
	{
		loadLevelName = levelName;
		Close ();
		isLoad = true;
	}
}
