using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditorMenu {


	[MenuItem("Window/JunConnectKit/LevelManger",false,0)]
	static public void OpenLevelManger ()
	{
		GameLevelMangerWindow.Open ();
	}

	[MenuItem("Window/JunConnectKit/ItemManger",false,1)]
	static public void OpenItemManger ()
	{
		ItemMangerWindow.Open ();
	}

    [MenuItem("Window/JunConnectKit/CaptureScreenshot", false, 1)]
    static public void CaptureScreenshot()
    {
        ScreenCapture.CaptureScreenshot("Text.png", 2);
    }
}
