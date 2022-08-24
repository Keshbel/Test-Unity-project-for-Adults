using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectColorFlash : MonoBehaviour {

	public float minValue = 0;
	public float maxValue = 1;
	public float flashSpeed = 1.0f;

	private float curAlpha = 1.0f;
	private int speed = 1;

	public SpriteRenderer spriteRender;
	public Image image;

	public bool startRandom = true;

	// Use this for initialization
	void Start () {
		int[] speedA = new int[2]{-1,1};
		if(startRandom)
		{
			curAlpha = Random.Range (minValue,maxValue);
			speed = speedA[Random.Range (0,speedA.Length)];
		}
			
		if(image == null)
		{
			image = GetComponent <Image>();
		}

		if(spriteRender == null)
		{
			spriteRender = GetComponent <SpriteRenderer>();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		curAlpha += speed*flashSpeed*Time.unscaledDeltaTime;
		if(curAlpha > maxValue)
			speed = -1;

		if(curAlpha < minValue)
			speed = 1;

		if(image != null)
		    image.color = new Color(image.color.r,image.color.g,image.color.b,curAlpha);
		if(spriteRender != null)
			spriteRender.color = new Color(spriteRender.color.r,spriteRender.color.g,spriteRender.color.b,curAlpha);
	}
}
