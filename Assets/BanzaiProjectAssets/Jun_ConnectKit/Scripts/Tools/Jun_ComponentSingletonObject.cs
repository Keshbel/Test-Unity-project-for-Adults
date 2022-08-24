using UnityEngine;
using System.Collections;

public class Jun_ComponentSingletonObject <T> : MonoBehaviour  where T:MonoBehaviour {

	private static MonoBehaviour _instance = null;

	public static T instance {
		get {
			return (T)_instance;
		}
	} 

	void Awake ()
	{
		SetInstance (this);
	}

	public void SetInstance (MonoBehaviour setI)
	{
		_instance = setI;
	}

	public virtual void Show ()
	{
		_instance.gameObject.SetActive (true);
	}

	public virtual void Hide ()
	{
		_instance.gameObject.SetActive (false);
	}
}
