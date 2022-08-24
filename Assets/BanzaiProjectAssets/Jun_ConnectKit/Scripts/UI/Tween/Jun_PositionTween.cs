using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jun_PositionTween : Jun_TweenBase {

	public Vector3 fromVector = Vector3.zero;
	public Vector3 toVector = Vector3.zero;

	public Vector3 value
	{
		get{return isLocal?transform.localPosition:transform.position;}
		set
		{
			if(isLocal)
			{
				transform.localPosition = value;
			}
			else
			{
				transform.position = value;
			}
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(isPlaying)
		{
			float curveValue = currentCurveValue;

			if(isLocal)
				transform.localPosition = (toVector - fromVector)*curveValue + fromVector;
			else
				transform.position = (toVector - fromVector)*curveValue + fromVector;
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
		    transform.localPosition = fromVector;
		else
			transform.position = fromVector;
	}

	[ContextMenu("Set 'From' to current value")]
	public void SetStartToCurrentValue () { fromVector = value; }

	[ContextMenu("Set 'To' to current value")]
	public void SetEndToCurrentValue () { toVector = value; }

	[ContextMenu("Assume value of 'From'")]
	void SetCurrentValueToStart () { value = fromVector; }

	[ContextMenu("Assume value of 'To'")]
	void SetCurrentValueToEnd () { value = toVector; }
}
