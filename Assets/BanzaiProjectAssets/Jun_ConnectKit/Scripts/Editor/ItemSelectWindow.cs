using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class ItemSelectWindow : EditorWindow {

	public static void Open (SelectItem selectItemEvent)
	{
		ItemManger setItemManger = EditorTools.LoadObjectInAssets<ItemManger>("ItemManger");
		Open (selectItemEvent,setItemManger);
	}

	public static void Open (SelectItem selectItemEvent,ItemManger setItemManger)
	{
		ItemSelectWindow windows = EditorWindow.GetWindow<ItemSelectWindow>(true,"Item Select");
		windows.minSize = new Vector2(600,500);
		windows.selectItemEvent += selectItemEvent;
		windows.itemManger = setItemManger;
	}

	private ItemManger itemManger;
	private SelectItem selectItemEvent;

	private int curSelectTypeNumber;

	List<Item> curSelectItems = new List<Item>();

	void OnGUI ()
	{
		GUILayout.BeginVertical ();

		if(itemManger == null)
		{
			itemManger = (ItemManger)EditorGUILayout.ObjectField ("ItemManager:",itemManger,typeof (ItemManger),false);
		}
		else
		{
			GUILayout.Space (10);

			GUILayout.BeginHorizontal ();
			curSelectItems = new List<Item>();
			curSelectItems = itemManger.GetAllItemList ();
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();

			DrawSelectItemGUI ();

			GUILayout.EndHorizontal ();
		}
		GUILayout.EndVertical ();
	}

	void DrawSelectItemGUI ()
	{
		if(curSelectItems.Count > 0)
		{
			float horItemCount = 8.0f;
			int verCount = (int)((curSelectItems.Count + 1) / horItemCount + 1);
			GUILayout.BeginVertical ();
			for (int i = 0; i < verCount; i++)
			{
				GUILayout.BeginHorizontal ();
				for (int j = 0; j < (int)horItemCount; j++)
				{
					int itemInex = (int)(i*horItemCount + j);
					float itemIconSize = this.position.width/horItemCount - 12.5f;

					if(itemInex < curSelectItems.Count + 1)
					{
						
						if(itemInex > 0)
						{
							itemInex -= 1;
							GUILayout.BeginVertical ("Box");

							string itemName = "";

							if(curSelectItems[itemInex] != null)
							{
								Texture2D itemTexuture = null;
								if (curSelectItems [itemInex].GetUIShowTexture () != null)
									itemTexuture = curSelectItems [itemInex].GetUIShowTexture ();
								
								itemName = curSelectItems[itemInex].itemName;


								GUILayout.Label (itemName,GUILayout.Width (itemIconSize));
								if(itemTexuture != null)
								{
									GUI.contentColor = curSelectItems[itemInex].itemColor;
									if(GUILayout.Button (new GUIContent(itemTexuture,itemName),GUILayout.Width(itemIconSize),GUILayout.Height (itemIconSize)))
									{
										selectItemEvent(curSelectItems[itemInex]);
										Close ();
									}
									GUI.contentColor = Color.white;
								}
								else
								{
									if(GUILayout.Button (curSelectItems[itemInex].itemName,GUILayout.Width(itemIconSize),GUILayout.Height (itemIconSize)))
									{
										selectItemEvent(curSelectItems[itemInex]);
										Close ();
									}
								}


							}
							else
							{
								GUILayout.Label (itemName,GUILayout.Width (itemIconSize));
								GUILayout.Button ("NULL",GUILayout.Width(itemIconSize),GUILayout.Height (itemIconSize));
							}
							GUILayout.EndVertical ();
						}
						else
						{
							GUILayout.BeginVertical ("Box");
							GUILayout.Label ("None",GUILayout.Width (itemIconSize));
							if(GUILayout.Button ("None",GUILayout.Width(itemIconSize),GUILayout.Height (itemIconSize)))
							{
								selectItemEvent(null);
								Close ();
							}
							GUILayout.EndVertical ();
						}
					}
				}
				GUILayout.EndHorizontal ();
			}
			GUILayout.EndVertical ();
		}
	}
}
