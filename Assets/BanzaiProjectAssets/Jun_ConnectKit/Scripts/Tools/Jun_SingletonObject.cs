using UnityEngine;
using System.Collections;

public class Jun_SingletonObject<T> : MonoBehaviour  where T:MonoBehaviour {

	private static T _instance = null;

	public static T instance {
		get {
			return _instance;
		}
	} 

	public void SetInstance (T setI)
	{
		_instance = setI;
	}
}
