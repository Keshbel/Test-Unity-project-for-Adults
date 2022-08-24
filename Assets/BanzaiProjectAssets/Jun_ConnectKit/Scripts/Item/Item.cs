using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

	public string itemName;
	public Sprite itemIcon;
	public Color itemColor = Color.white;

	public Texture2D GetUIShowTexture ()
	{
		if(itemIcon != null)
			return itemIcon.texture;
		return null;
	}
}
