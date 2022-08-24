using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GUI_Button : Button {

	public delegate void OnGUI_ButtonEventClick (GameObject go);

	private OnGUI_ButtonEventClick onButtonEventClick;
	private UnityEngine.Events.UnityEventCallState buttonEventState = UnityEngine.Events.UnityEventCallState.RuntimeOnly;

	public Text buttonText;

	new void Awake ()
	{
		base.Awake();
		onClick.AddListener (OnClick);
	}

	public void SetText (string str)
	{
		if(buttonText != null)
			buttonText.text = str;
	}

	public void SetActive (bool active)
	{
		if(active)
		{
			CancelGrayShader ();
		}
		else
		{
			SetGrayShader ();
		}
		gameObject.SetActive (active);
	}

	public void SetGrayShader ()
	{
		//return;
		if(targetGraphic != null)
			targetGraphic.material = Resources.Load ("Materials/ImageMaterialGray",typeof (Material)) as Material;

		//Debug.Log (onClick.GetPersistentEventCount ());
		buttonEventState = UnityEngine.Events.UnityEventCallState.Off;
	}

	public void CancelGrayShader ()
	{
		if(targetGraphic != null)
			targetGraphic.material = Resources.Load ("Materials/ImageMaterial",typeof (Material)) as Material;
		buttonEventState = UnityEngine.Events.UnityEventCallState.RuntimeOnly;
	}

	public void SetNewListener (GUI_Button.OnGUI_ButtonEventClick action)
	{
		onButtonEventClick = null;
		onButtonEventClick += action;
	}

	void OnClick ()
	{
		if(buttonEventState == UnityEngine.Events.UnityEventCallState.RuntimeOnly && onButtonEventClick != null)
			onButtonEventClick (this.gameObject);
	}

	// Use this for initialization
	new void Start ()
	{
		base.Start();
		if(buttonText == null)
			buttonText = GetComponentInChildren <Text>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
