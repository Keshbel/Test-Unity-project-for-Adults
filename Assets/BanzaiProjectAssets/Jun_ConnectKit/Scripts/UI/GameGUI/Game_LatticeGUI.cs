using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game_LatticeGUI : MonoBehaviour {

	public Transform crystal;
	public Transform clickBackground;

	public SpriteRenderer pointSprite;
	public SpriteRenderer addEffectSprite;

	public SpriteRenderer upLine;
	public SpriteRenderer rightLine;

	public GameObject exclamationIcon;

	private Lattice _lattice;
	public Lattice lattice {get{return _lattice;}}

	public SpriteRenderer completeEffect;
    public MeshRenderer groundRender;

	private Game_ItemGUI _itemGUI;
	public Game_ItemGUI itemGUI{get{return _itemGUI;} set {_itemGUI = value;}}
	public Item item
	{
		get
		{
			return lattice.item;
		}
	}   
	public bool isItemPoint
	{
		get
		{
			if(_lattice == null)
				return false;
			return _lattice.isItemPoint;
		}
	}

	// Use this for initialization
	void Start () {
		IsClick (false);
		completeEffect.gameObject.SetActive (false);
		ShowExclamation (false);

        if(Game_Camera.instance.is3D)
        {
            if (groundRender != null)
                groundRender.gameObject.SetActive(true);
        }
        else
        {
            if (groundRender != null)
                groundRender.gameObject.SetActive(false);
        }
	}

	public void ShowCrystal ()
	{
		if(!lattice.isItemPoint)
		    crystal.gameObject.SetActive (true);
	}

	public void GetCrystal ()
	{
		if(!lattice.isItemPoint)
		    crystal.gameObject.SetActive (false);
	}

	public void SetLattice (Lattice setLattice)
	{
		if(Lattice.IsNull (setLattice))
			return;
		
		_lattice = setLattice;

		if(GameController.instance.gameLevel.levelType == GameLevel.LevelType.GetAllCrystal)
		{
			if(setLattice.isItemPoint)
			{
				crystal.gameObject.SetActive (false);
			}
			else
			{
				crystal.gameObject.SetActive (true);
			}
		}
		else
		{
			crystal.gameObject.SetActive (false);
		}

		if(setLattice.isItemPoint)
		{
			pointSprite.gameObject.SetActive (false);
		}
		else
		{
			pointSprite.transform.localScale = Vector3.one * 0.1f;
		}
	}

	public void SetLine (bool up,bool down,bool left,bool right)
	{
		upLine.gameObject.SetActive (up);
		rightLine.gameObject.SetActive (right);
	}

	public void IsClick (bool isClick)
	{
		if(clickBackground == null)
			return;

		clickBackground.gameObject.SetActive (isClick);
	}

	public void IsComplete (bool complete)
	{
		completeEffect.gameObject.SetActive (complete);

		if(isItemPoint)
		{
			completeEffect.color = lattice.item.itemColor;
		}
	}

	public void ShowExclamation (bool isShow)
	{
		exclamationIcon.SetActive (isShow);
	}

	public void SetPointColor (Color color)
	{
		if(pointSprite != null)
			pointSprite.color = color;

        if (groundRender != null)
            groundRender.material.color = color;
	}

	public void ShowAddEffect (Color color)
	{
		if(addEffectSprite != null)
		{
			addEffectSprite.color = color;
			if(!addEffectSprite.GetComponent <Jun_ScaleTween>().enabled)
			    addEffectSprite.GetComponent <Jun_ScaleTween>().Play ();

			if(!addEffectSprite.GetComponent <Jun_AlphaTween>().enabled)
			    addEffectSprite.GetComponent <Jun_AlphaTween>().Play ();
		}
	}
}
