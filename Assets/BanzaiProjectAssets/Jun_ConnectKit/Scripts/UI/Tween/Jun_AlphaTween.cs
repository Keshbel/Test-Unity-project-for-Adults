using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Jun_AlphaTween : Jun_TweenBase {

	public float fromAlpha = 0;
	public float toAlpha = 1;

	public bool isStartRandomValue = false;

	private MaskableGraphic maskableGraphic;
	private Renderer render;
	private SpriteRenderer spriteRender;
	private ParticleSystem particle;

	// Use this for initialization
	void Start () 
	{
		maskableGraphic = GetComponent <MaskableGraphic>();
		render = GetComponent <Renderer>();
		spriteRender = GetComponent <SpriteRenderer>();
		particle = GetComponent <ParticleSystem>();
	}

	void Update ()
	{
		if(isPlaying)
		{
			float curveValue = currentCurveValue;
			float curAlpha = (toAlpha - fromAlpha)*curveValue + fromAlpha;

			if(maskableGraphic != null)
				maskableGraphic.color = new Color(maskableGraphic.color.r,maskableGraphic.color.g,maskableGraphic.color.b,curAlpha);

			if(spriteRender != null)
				spriteRender.color = new Color(spriteRender.color.r,spriteRender.color.g,spriteRender.color.b,curAlpha);
			else
			{
				if(render != null)
				{
					if(render.material != null)
						render.material.color = new Color(render.material.color.r,render.material.color.g,render.material.color.b,curAlpha);
				}
			}

			if(particle != null)
			{
				var particleMain = particle.main;
				particleMain.startColor = new Color (particleMain.startColor.color.r,particleMain.startColor.color.g,particleMain.startColor.color.b,curAlpha);
			}
		}
	}

	public override void StopPlay ()
	{
		base.StopPlay ();
	}

	public override void Play ()
	{
		base.Play ();
		if(isStartRandomValue)
		{
			playTimeNote = Time.time + Random.Range (0.0f,animationTime);
		}
	}
}
