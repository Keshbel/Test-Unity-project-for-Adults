using UnityEngine;
using System.Collections;

namespace VNEngine
{
    // Listens for any buttons presses, and sets the given object to active and disables this object
    public class EnableIfKeyPressed : MonoBehaviour
    {
        void Update()
        {
            if (Input.anyKeyDown)
            {
                VNSceneManager.scene_manager.Show_UI(true);
            }
        }
    }
}