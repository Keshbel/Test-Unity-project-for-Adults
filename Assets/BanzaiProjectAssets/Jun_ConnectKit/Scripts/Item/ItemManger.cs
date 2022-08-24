using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemManger : MonoBehaviour 
{
	[HideInInspector][SerializeField]List<Item> items = new List<Item>();

	public int itemCount {get {return items.Count;}}

	public void AddItem (Item addItem)
	{
		if(!items.Contains (addItem))
			items.Add (addItem);
	}

	public void RemoveItem (Item removeItem)
	{
		if(items.Contains (removeItem))
			items.Remove (removeItem);
	}

	public void RemoveItemAt (int inex)
	{
		if(inex < itemCount)
		{
			Item thisItem = items[inex];
			if(thisItem != null)
			{
				RemoveItem (thisItem);
			}
			else
			{
				items.RemoveAt (inex);
			}
		}
	}

	public Item GetItem (int inex)
	{
		if(inex < itemCount)
			return items[inex];
		return null;
	}


	public List<Item> GetAllItemList ()
	{
		return items;
	}


	public List<Item> GetRandomItems (int count)
	{
		List<Item> newItemList = new List<Item>();
		newItemList.AddRange (items);

		if(count >= items.Count)
			return newItemList;

		List<Item> randomItems = new List<Item>();
		for (int i = 0; i < count; i++)
		{
			Item thisRandomItem = newItemList[Random.Range (0,newItemList.Count)];
			randomItems.Add (thisRandomItem);
			newItemList.Remove (thisRandomItem);
		}

		return randomItems;
	}
}
