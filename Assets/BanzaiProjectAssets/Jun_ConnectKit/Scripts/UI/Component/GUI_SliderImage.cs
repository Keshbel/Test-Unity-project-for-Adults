using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUI_SliderImage : MonoBehaviour {

	public float sliderValue = 1.0f;

	private float curLenght;
	private float curHight;
	private Image image;

	// Use this for initialization
	void Start () {
		image = GetComponent <Image>();
		curLenght = image.rectTransform.sizeDelta.x;
		curHight = image.rectTransform.sizeDelta.y;
	}
	
	// Update is called once per frame
	void Update () 
	{
		image.rectTransform.sizeDelta = new Vector2(sliderValue*curLenght,curHight);
	}
}
