using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class GUI_ValueUI : MonoBehaviour {

	public Image background;
	public Image icon;
	public Text valueText;
	public Text iconText;
	public GUI_Button eventButton;
	public Slider slider;

	public bool showIconText = false;
	public bool showEventButton = false;
	public bool showSlider = false;

	void Awake ()
	{
		SetWidth (300);
	}

	// Use this for initialization
	void Start () {
		if(!showIconText)
			iconText.gameObject.SetActive (false);
		if(!showEventButton)
			eventButton.gameObject.SetActive (false);
		if(!showSlider)
			slider.gameObject.SetActive (false);
	}

	public void SetValue (string str)
	{
		if(valueText != null)
		    valueText.text = str;
	}

	public void SetSliderValue (float value)
	{
		slider.value = value;
	}

	public void SetIconValue (string str)
	{
		if(iconText != null)
		    iconText.text = str;
	}

	public void SetWidth (int width)
	{
		if(background != null)
		    background.rectTransform.sizeDelta = new Vector2(width + 12,60);
		if(slider != null)
			slider.GetComponent<RectTransform>().sizeDelta = new Vector2(width - 20,60 - 20);
	}

	public void SetButtonEvent (UnityEngine.Events.UnityAction callback)
	{
		if(eventButton != null)
		    eventButton.onClick.AddListener (callback);
	}
}
