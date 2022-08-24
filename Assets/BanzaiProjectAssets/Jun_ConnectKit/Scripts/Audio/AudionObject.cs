using UnityEngine;
using System.Collections;

public class AudionObject : MonoBehaviour {

	public AudioSource audionSource;
	public bool isMusice = false;
	public float time;
	public bool isPlaying = false;
	public AudioClip audioClip;

	public void Play()
	{
		if(audionSource != null)
		{
			gameObject.SetActive(true);
			isPlaying = true;
			audionSource.Play();
		}
		else
		{
			Debug.Log("No Have AudionSource!");
		}
	}
	
	public void Play (AudioClip clip,bool _isMusice)
	{
		if(audionSource == null)
		{
			AddSource();
		}

		audionSource.playOnAwake = false;
		isMusice = _isMusice;
		
		if(_isMusice)
		{
			audionSource.loop = true;
		}
		else
		{
			audionSource.loop = false;
		}
		
		audioClip = clip;
		audionSource.clip = clip;
		gameObject.name = clip.name;
		gameObject.SetActive(true);
		isPlaying = true;

		if(_isMusice && !AudionManger.musiceOff)
		    audionSource.Play();

		if(!_isMusice && !AudionManger.soundOff)
			audionSource.Play ();
	}

	public void Stop ()
	{
		if(audionSource != null)
		{
			audionSource.Stop();
			//audionSource.gameObject.SetActive(false);
		}
	}

	void AddSource ()
	{
		if(audionSource == null)
		{
			if(GetComponent<AudioSource>() == null)
			{
				gameObject.AddComponent<AudioSource>();
				audionSource = gameObject.GetComponent<AudioSource>();
			}
			else
			{
				audionSource = gameObject.GetComponent<AudioSource>();
			}
		}
	}

	void Update ()
	{
		if(audionSource != null)
		{
			isPlaying = audionSource.isPlaying;
		}
		if(!isPlaying && !isMusice)
		{
			time = Time.time;
			gameObject.SetActive(false);
		}
	}
}
