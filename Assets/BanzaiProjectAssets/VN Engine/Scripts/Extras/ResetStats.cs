using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VNEngine
{
    // Place in your main menu to reset stats between plays/loads.
    public class ResetStats : MonoBehaviour
    {
        void Start()
        {
            StatsManager.Clear_All_Stats();
        }
    }
}