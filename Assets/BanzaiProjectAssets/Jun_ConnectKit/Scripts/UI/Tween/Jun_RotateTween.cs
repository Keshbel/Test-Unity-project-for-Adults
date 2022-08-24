using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jun_RotateTween : Jun_TweenBase
{

	public Vector3 fromVector = Vector3.zero;
	public Vector3 toVector = Vector3.zero;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(isPlaying)
		{
			float curveValue = currentCurveValue;

			if(isLocal)
			    transform.localEulerAngles = (toVector - fromVector)*curveValue + fromVector;
			else
				transform.eulerAngles = (toVector - fromVector)*curveValue + fromVector;
		}
	}

	public override void StopPlay ()
	{
		base.StopPlay ();
	}

	public override void Play ()
	{
		base.Play ();

		if(isLocal)
		    transform.localEulerAngles = fromVector;
		else
			transform.eulerAngles = fromVector;
	}
}
