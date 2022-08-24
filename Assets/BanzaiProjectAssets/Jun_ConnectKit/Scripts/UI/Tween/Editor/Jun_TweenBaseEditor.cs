using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof (Jun_TweenBase))]
public class Jun_TweenBaseEditor : Editor 
{
	SerializedProperty m_OnClickProperty;

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
	}
}
