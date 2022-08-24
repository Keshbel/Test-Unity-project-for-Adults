using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jun_ScaleTween : Jun_TweenBase {

	public Vector3 fromVector = Vector3.one;
	public Vector3 toVector = Vector3.one;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(isPlaying)
		{
			float curveValue = currentCurveValue;
			transform.localScale = (toVector - fromVector)*curveValue + fromVector;
		}
	}

	public override void StopPlay ()
	{
		base.StopPlay ();
	}

	public override void Play ()
	{
		base.Play ();
		transform.localScale = fromVector;
	}
}
