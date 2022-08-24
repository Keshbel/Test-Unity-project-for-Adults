using UnityEngine;
using System.Collections;

namespace VNEngine
{
    public class StopFastForwardingWhenDisabled : MonoBehaviour
    {
        void OnDisable()
        {
            VNSceneManager.scene_manager.Stop_Fast_Forward();
        }
    }
}