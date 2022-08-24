using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
    // Disables the background
    public class DisableBackgroundNode : Node
    {

        public override void Run_Node()
        {
            UIManager.ui_manager.background.gameObject.SetActive(false);
            Finish_Node();
        }


        public override void Finish_Node()
        {
            StopAllCoroutines();

            base.Finish_Node();
        }
    }
}