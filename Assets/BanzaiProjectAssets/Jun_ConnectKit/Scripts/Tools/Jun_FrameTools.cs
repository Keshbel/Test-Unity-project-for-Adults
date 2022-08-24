using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jun_FrameTools : MonoBehaviour {

	static Jun_FrameTools _inter;
	public static Jun_FrameTools inter
	{
		get
		{
			if(_inter == null)
			{
				GameObject newObj = new GameObject();
				DontDestroyOnLoad (newObj);
				newObj.name = "Jun_FrameTools";
				_inter = newObj.AddComponent <Jun_FrameTools>();
			}
			return _inter;
		}
	}

	float frameTime = 0;
	float frameRate = 0;

	public float getFloatFrameRate {get{return frameRate;}}
	public float getIntFrameRate {get{return (int)frameRate;}}
	
	// Update is called once per frame
	void Update () 
	{
		frameTime = Time.time - frameTime;
		frameRate = 1.0f/frameTime;
		frameTime = Time.time;

		Debug.Log (getIntFrameRate.ToString ());
	}

	public float GetValueFromFrameRate (float currentValeu,float targetValue,float speed)
	{
		float j = targetValue - currentValeu;
		float timeV = j / speed;
		float needFrame = timeV * getFloatFrameRate;
		float moveV = j/needFrame;
		return moveV;
	}
}
