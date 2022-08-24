using UnityEngine;
using System.Collections;

public class GUI_ButtonPlaySound : MonoBehaviour {

	GUI_Button button;

	public AudioClip audioClip;

	// Use this for initialization
	void Start () {
		button = GetComponent <GUI_Button>();
		if(button != null)
		    button.onClick.AddListener(OnClickPlay);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PlayAudio ()
	{
		AudionManger.inter.Play (audioClip,false,false,0);
	}

	void OnClickPlay ()
	{
		PlayAudio ();
	}
}
