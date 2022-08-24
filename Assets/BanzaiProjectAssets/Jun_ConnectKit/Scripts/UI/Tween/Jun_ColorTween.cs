using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Jun_ColorTween : Jun_TweenBase {

	public Color fromColor = Color.white;
	public Color toColor = Color.white;

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
	
	// Update is called once per frame
	void Update () {
		if(isPlaying)
		{
			float curveValue = currentCurveValue;
			Color curColor = (toColor - fromColor)*curveValue + fromColor;

			if(maskableGraphic != null)
				maskableGraphic.color = curColor;

			if(render != null)
			{
				if(render.material != null)
					render.material.color = curColor;
			}

			if(spriteRender != null)
				spriteRender.color = curColor;

			if(particle != null)
			{
				var particleMain = particle.main;
				particleMain.startColor = curColor;
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
	}
}
