using UnityEngine;
using System.Collections;

public class GUI_MiLightList : MonoBehaviour {

	public enum LightType
	{
		flash,
		move
	}

	public bool randomType = false;
	public LightType lightType = LightType.flash;
	public float speed = 0.3f;
	private float time;

	GUI_Milight[] lights;

	float randomTime = 0.8f;
	float randomTimeNote;

	// Use this for initialization
	void Start () {
		lights = GetComponentsInChildren <GUI_Milight>();
		Light ();

		time = randomTimeNote = Time.time;

		if(randomType)
			RandomLight ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Time.time - time >= speed)
		{
			Light ();
			time = Time.time;
		}

		if(randomType)
		{
			if(Time.time - randomTimeNote >= randomTime)
			{
				RandomLight ();

				randomTimeNote = Time.time;
				randomTime = Random.Range (2f,8f);
			}
		}
	}

	void RandomLight ()
	{
		int typeValue = Random.Range (0,2);
		switch (typeValue)
		{
		case 0:
			lightType = LightType.flash;
			break;

		case 1:
			lightType = LightType.move;
				break;
		}
	}

	void Light ()
	{
		switch (lightType)
		{
		case LightType.flash:
			FlashLight ();
			break;

		case LightType.move:
			MoveLight ();
			break;
		}
	}

	bool flashOff = false;
	void FlashLight ()
	{
		for (int i = 0; i < lights.Length; i++)
		{
			if(i%2 == 0)
			{
				lights[i].SetLightOff (flashOff);
			}
			else
			{
				lights[i].SetLightOff (!flashOff);
			}
		}

		flashOff = !flashOff;
	}

	int curLight = 0;
	public int openCount = 7;
	void MoveLight ()
	{
		for (int i = 0; i < lights.Length; i++)
		{
			int maxV = curLight + openCount;
			int v = maxV >= lights.Length?maxV - lights.Length:maxV;

			bool isOpenLight = false;

			if(v > curLight)
			{
				if(i < v && i >= curLight)
					isOpenLight = true;
			}
			else
			if(v < curLight)
			{
				if(i > curLight || i < v)
					isOpenLight = true;
			}

			lights[i].SetLightOff (!isOpenLight);
		}
		curLight ++;

		curLight = curLight >= lights.Length?0:curLight;
	}
}
