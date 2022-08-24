using UnityEngine;
using System.Collections;

namespace VNEngine
{
    // Enables a game object upon awakening, then destroys itself
    public class SetActiveOnAwake : MonoBehaviour
    {
        public GameObject object_to_activate;

        void Awake()
        {
            object_to_activate.SetActive(true);
            Destroy(this.gameObject);
        }
    }
}