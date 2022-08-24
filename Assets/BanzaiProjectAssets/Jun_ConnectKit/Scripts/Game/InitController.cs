using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitController : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        
        Application.targetFrameRate = 60;
        int maxWidth = 1080;
        if (Screen.width > Screen.height)
            maxWidth = 1920;

        if(Screen.width > maxWidth)
        {
            int height = (int)((Screen.height * 1.0f / Screen.width) * maxWidth);
            Screen.SetResolution(maxWidth, height, true);
        }
        SceneManager.LoadScene("Levels");
	}
}
