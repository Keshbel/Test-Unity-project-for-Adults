using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMaterialOffset : MonoBehaviour {

	Renderer render;
	Vector2 offset;

	// Use this for initialization
	void Start () 
	{
		render = GetComponent <Renderer>();
		offset = render.material.mainTextureOffset;
	}
	
	// Update is called once per frame
	void Update () 
	{
		offset += new Vector2(-1,0)*Time.deltaTime;
		render.material.mainTextureOffset = offset;
	}
}
