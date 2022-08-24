using UnityEngine;
using System.Collections;

namespace VNEngine
{
    public class LoadLevel : MonoBehaviour
    {
        // Be sure to add this Scene to your build settings
        public void Load_Scene(string scene)
        {
            Time.timeScale = 0;
            UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
        }
    }
}