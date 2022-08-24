using UnityEngine;

public class Game_ItemGUI : MonoBehaviour 
{
	private Item _item;
	public Item item{get{return _item;}}

	public SpriteRenderer itemIconImage;
	public SpriteRenderer itemFlashImage;

	public void SetItem (Item setItem)
	{
		_item = setItem;

		itemIconImage.color = itemFlashImage.color = setItem.itemColor;
	}
}
