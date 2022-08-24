using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUI_Main : MonoBehaviour {

	public Camera mainCamera;
	public GameLevelManger levelManger;
	public LevelUI_LevelIcon levelIconAsset;
	public LineRenderer lineRender;

	public Transform playerIcon;
	public Transform cloudMask;
    public GameObject comingSoonUI;
    public GameObject notUnlockedYetUI;

    Vector3[] settingPosition;
	List<LevelUI_LevelIcon> levelIcons = new List<LevelUI_LevelIcon>();
    LevelUI_LevelIcon lastUnlockIcon = null;
	CameraInputMove mainCameraMove;
    bool allLevelsCompleted = false;

    // Use this for initialization
    void Start () 
	{
		mainCameraMove = mainCamera.GetComponent <CameraInputMove>();

		settingPosition = new Vector3[lineRender.positionCount];
		for (int i = 0; i < settingPosition.Length; i++)
		{
			settingPosition[i] = lineRender.GetPosition (i);
		}

		int id = 0;
		int sectionID = 0;
		List<Vector3> linePos = new List<Vector3>();

		int unPassLevelCount = 0;

        allLevelsCompleted = true;
		for (int x = 0; x < levelManger.groupCount; x ++)
		{
			GameLevelGroup thisGroup = levelManger.GetGroup (x);
			for (int y = 0; y < thisGroup.levelCount; y++)
			{
                GameLevel thisLevel = thisGroup.GetLevel(y);
                if (thisLevel != null)
                {
                    if (unPassLevelCount > 15)
                    {
                        break;
                    }
                    else
                    {
                        GameObject newLeveIconObj = Instantiate <GameObject>(levelIconAsset.gameObject,transform);
						LevelUI_LevelIcon newLevelIcon = newLeveIconObj.GetComponent <LevelUI_LevelIcon>();
						newLevelIcon.SetLevel (thisLevel);

						Vector3 iconPosition = settingPosition[id];
						iconPosition = new Vector3(iconPosition.x,iconPosition.y,iconPosition.z + sectionID * settingPosition[settingPosition.Length - 1].z);

						newLevelIcon.transform.position = iconPosition;
						linePos.Add (iconPosition);

						if(thisLevel.isUnlock)
						{
							playerIcon.position = iconPosition;
							mainCameraMove.CenterOn (newLevelIcon.transform);
                            lastUnlockIcon = newLevelIcon;
						}
						else
						{
							unPassLevelCount ++;
                            allLevelsCompleted = false;
						}

						newLevelIcon.onClick += OnIconClick;
						levelIcons.Add (newLevelIcon);
					}

					id ++;

					if(id >= settingPosition.Length)
					{
						sectionID ++;
						id = 0;
					}
				}
			}
		}

		lineRender.positionCount = linePos.Count;
		lineRender.SetPositions (linePos.ToArray ());

		HideOrShowIcon ();

        float cameraMaxZ = lastUnlockIcon == null ? 10:lastUnlockIcon.transform.position.z + 4;
        float cloudMaskZ = lastUnlockIcon == null ? 16 : lastUnlockIcon.transform.position.z + 8;
        mainCameraMove.SetMaxMinZ (cameraMaxZ,levelIcons[0].transform.position.z);
        cloudMask.position = new Vector3(0, 4, cloudMaskZ);

        if(allLevelsCompleted)
        {
            //Show UI that "All levels completed, Game Over".
            notUnlockedYetUI.SetActive(false);
            comingSoonUI.SetActive(true);
        }
        else
        {
            notUnlockedYetUI.SetActive(true);
            comingSoonUI.SetActive(false);
        }
    }

	void HideOrShowIcon ()
	{
		for (int i = 0; i < levelIcons.Count; i++)
		{
			LevelUI_LevelIcon thisLevelIcon = levelIcons[i];
			float dir = Mathf.Abs(thisLevelIcon.transform.position.z - mainCamera.transform.position.z);

			if(dir < 17)
			{
				thisLevelIcon.gameObject.SetActive (true);
			}
			else
			{
				thisLevelIcon.gameObject.SetActive (false);
			}
		}
	}

	void OnIconClick (LevelUI_LevelIcon clickIcon)
	{
		if(clickIcon == null)
			return;

		if(clickIcon.gameLevel.isUnlock)
        {
            GameController.assetGameLevel = clickIcon.gameLevel;
            GUI_LoadingUI.instance.LoadLevel("Game");
        }
	}
	
	// Update is called once per frame
	void Update () {
		if(mainCameraMove.isMove)
        {
            HideOrShowIcon();
        }
	}
}
