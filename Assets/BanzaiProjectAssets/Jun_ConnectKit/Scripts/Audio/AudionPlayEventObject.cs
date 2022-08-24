using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudionPlayEventObject : MonoBehaviour {

	private AudioSource audioSource;

	void Awake ()
	{
		audioSource = GetComponent <AudioSource>();
	}

	public void PlayAudio ()
	{
		if(!AudionManger.soundOff)
		{
		    if(audioSource != null)
				audioSource.Play ();
		}
	}

	public void PlayAudio (AudioClip clip)
	{
		if(!AudionManger.soundOff)
		{
			AudionManger.inter.Play (clip,false,false,0);
		}
	}

}
