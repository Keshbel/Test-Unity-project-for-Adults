using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public delegate void SelectItem (Item selectItem);

public class ItemMangerWindow : EditorWindow
{
	private static string SavePath = "ItemManger";

	public static void Open ()
	{
		ItemMangerWindow windows = EditorWindow.GetWindow<ItemMangerWindow>(true,"Item Manger");
		windows.minSize = new Vector2(600,500);
		windows.Load();
	}

	public static ItemManger GetData ()
	{
		ItemManger thisItemManger = EditorTools.LoadObjectInAssets<ItemManger>(SavePath);
		return thisItemManger;
	}

	public void Load ()
	{
		itemManger = GetData ();
	}

	void OnDestroy ()
	{
		Save();
	}
	
	void Save ()
	{
		EditorUtility.SetDirty(itemManger.gameObject);
		for (int i = 0; i < itemManger.itemCount; i++)
		{
			EditorUtility.SetDirty (itemManger.GetItem (i).gameObject);
		}
	}

	private ItemManger itemManger;
	private int curSelectTypeNumber;
	private Item curEditorItem,waitDeleteItem;
	List<Item> curSelectItems = new List<Item>();

	void OnGUI ()
	{
		GUILayout.BeginVertical ();

		if(itemManger == null)
		{
			if(GUILayout.Button ("Creat Item Manger!"))
			{
				itemManger = EditorTools.CreatePrefabDate<ItemManger>(SavePath);
			}
		}
		else
		{
			GUILayout.Space (10);

			GUILayout.BeginHorizontal ();
			curSelectItems = new List<Item>();

			if (GUILayout.Button("AddItem", EditorStyles.miniButton))
            {
                Item newItem = CreateItem("New Item");
                if (newItem != null)
                {
                    itemManger.AddItem(newItem);
                }
            }

			curSelectItems = itemManger.GetAllItemList();

			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			DrawItemListTagGUI ();
			DrawItemGUI ();
			GUILayout.EndHorizontal ();

			if(curSelectItems.Count > 0)
			{
				for (int i = 0; i < curSelectItems.Count; i++)
				{
					if(curSelectItems[i] == null)
					{
						itemManger.RemoveItemAt (i);
					}
				}
			}
		}
		GUILayout.EndVertical ();
	}

	Vector2 itemListTagPos,itemDataPos,itemLevelListPos,expandItemListPos;
	void DrawItemListTagGUI ()
	{
		GUILayout.BeginVertical ("Box",GUILayout.Width (200));
		GUILayout.Label ("Items");
		itemListTagPos = EditorGUILayout.BeginScrollView (itemListTagPos);
		if(curSelectItems.Count > 0)
		{
			for (int i = 0; i < curSelectItems.Count; i++)
			{
				Item thisItem = curSelectItems[i];

				if(thisItem != null)
				{
					if(curEditorItem == thisItem)
						GUI.backgroundColor = Color.green;
					else
					    GUI.backgroundColor = new Color(0.7f,0.7f,0.7f,1);
					GUILayout.BeginHorizontal ("Box",GUILayout.Height (60));
					Texture2D itemTexture = thisItem.GetUIShowTexture ();

					GUI.contentColor = thisItem.itemColor;
					if(GUILayout.Button (itemTexture,GUILayout.Width (60),GUILayout.Height (60)))
					{
						curEditorItem = thisItem;
					}
					GUI.contentColor = Color.white;

					GUILayout.BeginVertical ();
					GUILayout.BeginHorizontal ();
					GUILayout.Label (thisItem.itemName + i*60);

					if(waitDeleteItem != thisItem)
					{
						GUI.backgroundColor = Color.red;
					    if(GUILayout.Button ("X",GUILayout.Width(20)))
					    {
						    waitDeleteItem = thisItem;
						}
					}
					else
					{
						GUI.backgroundColor = Color.red;
						if(GUILayout.Button ("Y",GUILayout.Width(20)))
						{
							EditorTools.DeletePrefabData (thisItem.gameObject);
						}
						GUI.backgroundColor = Color.green;
						if(GUILayout.Button ("N",GUILayout.Width(20)))
						{
							waitDeleteItem = null;
						}
					}
					GUI.backgroundColor = Color.white;
					GUILayout.EndHorizontal ();

					GUILayout.EndVertical ();

					GUILayout.EndHorizontal ();
				}
				GUI.backgroundColor = Color.white;
			}
		}
		EditorGUILayout.EndScrollView ();
		GUILayout.EndVertical ();
	}

	void DrawItemGUI ()
	{
		if(curEditorItem == null)
			return;
		
		GUILayout.BeginVertical ();
		curEditorItem.itemName = EditorGUILayout.TextField ("ItemName:",curEditorItem.itemName);
		curEditorItem.itemIcon = (Sprite)EditorGUILayout.ObjectField ("ItemIcon:",curEditorItem.itemIcon,typeof (Sprite),false);
		curEditorItem.itemColor = EditorGUILayout.ColorField ("ItemColor:",curEditorItem.itemColor);
		GUILayout.EndVertical ();
	}
		
	Item CreateItem (string itemName)
	{
		string path = EditorUtility.SaveFilePanelInProject("Save As", itemName + ".prefab", "prefab", "Save as...");
		if(!string.IsNullOrEmpty(path))
		{
			GameObject go = new GameObject();
			go.name = name;
			go.AddComponent<Item>();
			PrefabUtility.CreatePrefab(path,go);
			Object.DestroyImmediate(go);
			return AssetDatabase.LoadAssetAtPath(path,typeof(Item)) as Item;
		}
		return null;
	}
}
