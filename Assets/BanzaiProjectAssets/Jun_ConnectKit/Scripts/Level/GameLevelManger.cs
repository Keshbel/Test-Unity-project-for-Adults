using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameLevelGroup
{
	[HideInInspector]public string name;
	[HideInInspector][SerializeField]public GameLevelManger levelManger;
	[HideInInspector][SerializeField]public int ID;
	[HideInInspector][SerializeField]public List<GameLevel> levels = new List<GameLevel>();
	[HideInInspector]public string sceneName;
	[HideInInspector]public bool isInGame = true;

	public int levelCount {get{return levels.Count;}}

	public bool isUnlock
	{
		get
		{
			if(levels.Count > 0)
			{
				if (levels[0].isUnlock)
					return true;
			}
			return false;
		}
	}

	public void AddLevel (GameLevel addLevel)
	{
		if(!levels.Contains (addLevel))
		{
			levels.Add (addLevel);
		}
	}

	public void RemoveLevel (GameLevel removeLevel)
	{
		if(levels.Contains (removeLevel))
			levels.Remove (removeLevel);
	}

	public void RemoveLevelAt (int inex)
	{
		if(inex < levelCount)
			levels.RemoveAt (inex);
	}

	public GameLevel GetLevel (int inex)
	{
		if(inex < levelCount)
			return levels[inex];
		return null;
	}
}

public class GameLevelManger : MonoBehaviour
{
	[HideInInspector][SerializeField]public List<GameLevelGroup> groups = new List<GameLevelGroup>();
	[HideInInspector]public List<string> aiNames = new List<string>();

	public int groupCount{get{return groups.Count;}}
	public string[] groupNames
	{
		get
		{
			string[] names = new string[groups.Count];
			for(int i = 0; i < groupCount; i++)
			{
				if(!string.IsNullOrEmpty(groups[i].name))
				    names[i] = groups[i].name;
				else
					names[i] = "Unnamed";
			}
			return names;
		}
	}

	public void AddGroup ()
	{
		groups.Add (new GameLevelGroup());
	}

	public void RemoveGroup (GameLevelGroup removeGroup)
	{
		if(groups.Contains (removeGroup))
			groups.Remove (removeGroup);
	}

	public GameLevelGroup GetGroup (int inex)
	{
		if(inex < groupCount)
			return groups[inex];
		return null;
	}
}
