using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudionManger : MonoBehaviour {

	private static AudionManger _inter;
	public static AudionManger inter{
		get
		{
			if(_inter == null)
			{
				GameObject go = new GameObject();
				go.name = "AudionManger";
				go.AddComponent<AudionManger>();
				DontDestroyOnLoad(go);
				_inter = go.GetComponent<AudionManger>();
				_inter.Load ();
				return _inter;
			}
			return _inter;
		}
	}

	public bool _musiceOff = false;
	public bool _soundOff = false;
	public static bool musiceOff 
	{
		get 
		{
			return inter._musiceOff;
		}
		set
		{
			inter._musiceOff = value;
			inter.Save ();
		}
	}

	public static bool soundOff
	{
		get
		{
			return inter._soundOff;
		}
		set
		{
			inter._soundOff = value;
			inter.Save ();
		}
	}

	public List<AudionObject> audioSources = new List<AudionObject>();//音頻對象列表
	public AudionObject musiceObj;

	public AudionObject Play(string audionName)
	{
		return Play(audionName,false,false);
	}

	public AudionObject Play(string audionName,bool isMusice)
	{
		return Play(audionName,isMusice,false);
	}

	public AudionObject Play(string audionName , bool isMusice,bool isOnlyOne)
	{
		if(AudionManger.soundOff)
		{
			return null;
		}

		AudioClip audionClip;
		try
		{
			if(isMusice)
				audionName = "Musice/" + audionName;
			else
				audionName = "Sound/" + audionName;
			audionClip = Resources.Load(audionName) as AudioClip;
		}
		catch
		{
			Debug.Log("Not Found Audion:" + audionName);
			return null;
		}
			
		return Play (audionClip,isMusice,isOnlyOne);
	}

	public AudionObject Play (AudioClip audionClip,bool isMusice,bool isOnlyOne)
	{
		return Play (audionClip,isMusice,isOnlyOne,0);
	}

	public AudionObject Play (AudioClip audionClip,bool isMusice,bool isOnlyOne,float waitTime)
	{

		if(audionClip == null)
			return null;
		
		for(int i = 0;i<audioSources.Count;i++)
		{
			if(isMusice)
			{
				if(audioSources[i].isMusice)
				{
					if(!audioSources[i].gameObject.activeSelf)
					{
						audioSources[i].gameObject.SetActive(true);
						audioSources[i].Play(audionClip,isMusice);
					}
					if(audioSources[i].audioClip != audionClip)
						audioSources[i].Play(audionClip,isMusice);

					return audioSources[i];
				}
			}
			else
			{
				if(AudionManger.soundOff)
					return null;

				if(isOnlyOne == true)
				{
					if(audioSources[i].audioClip == audionClip)
					{
						if(audioSources[i].time > 0.2f)
						    audioSources[i].Play(audionClip,isMusice);
						return audioSources[i];
					}
				}
				else
				{
					if(!audioSources[i].gameObject.activeSelf)
					{
						audioSources[i].Play(audionClip,isMusice);
						return audioSources[i];
					}
				}
			}
		}
		
		GameObject go = new GameObject();
		go.name = audionClip.name;
		go.transform.parent = this.transform;
		go.AddComponent<AudionObject>();
		go.GetComponent<AudionObject>().Play(audionClip,isMusice);
		audioSources.Add(go.GetComponent<AudionObject>());
		if(isMusice) musiceObj = go.GetComponent<AudionObject>();
		return go.GetComponent<AudionObject>();
	}

	public void Stop(AudionObject audionObj)
	{
		audionObj.time = Time.time;
		audionObj.gameObject.SetActive(false);
	}

	public void StopMusice ()
	{
		if(musiceObj != null)
		{
			Stop(musiceObj);
		}
	}

	public bool curMusiceOff = false;
	void Update ()
	{
		if (curMusiceOff != musiceOff) 
		{
			curMusiceOff = musiceOff;
			for (int i = 0; i<audioSources.Count; i++) {
				if (AudionManger.musiceOff) 
				{
					if (audioSources [i].isMusice && audioSources [i].isPlaying)
					{
						//audioSources [i].gameObject.SetActive (false);
						audioSources [i].Stop ();
					}
				} 
				else 
				{
					if (audioSources [i].isMusice && !audioSources [i].isPlaying) {
						audioSources [i].gameObject.SetActive (true);
						audioSources [i].Play ();
					}
				}
			}
		}
	}

	private const string musiceSaveID = "AudionSetting_MusiceOff";
	private const string soundSaveID = "AudionSetting_SoundOff";

	void Load ()
	{
		if(PlayerPrefs.GetString (musiceSaveID) == "True")
			musiceOff = true;
		else
			musiceOff = false;

		if(PlayerPrefs.GetString (soundSaveID) == "True")
			soundOff = true;
		else
			soundOff = false;
	}

	void Save ()
	{
		if(musiceOff)
			PlayerPrefs.SetString (musiceSaveID,"True");
		else
			PlayerPrefs.SetString (musiceSaveID,"False");

		if(soundOff)
			PlayerPrefs.SetString (soundSaveID,"True");
		else
			PlayerPrefs.SetString (soundSaveID,"False");
	}

	void OnApplicationQuit ()
	{
		Save ();
	}

	void OnDisable ()
	{
		Save ();
	}

	void OnDestroy ()
	{
		Save ();
	}
}
