using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUI_Milight : MonoBehaviour {

	public Sprite openSprite;
	public Sprite closeSprite;

	private Image image;

	// Use this for initialization
	void Awake () {
		image = GetComponent <Image>();
	}

	public void SetLightOff (bool off)
	{
		if(off)
			Close ();
		else
			Open ();
	}

	public void Open ()
	{
		image.sprite = openSprite;
		image.SetNativeSize ();
	}

	public void Close ()
	{
		image.sprite = closeSprite;
		image.SetNativeSize ();
	}
}
