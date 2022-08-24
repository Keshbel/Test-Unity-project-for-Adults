using UnityEngine;
using System.Collections;

namespace VNEngine
{
    public class CanSaveNode : NodeTemplate
    {
        public bool can_save = false;

        public override void Run_Node()
        {
            VNSceneManager.scene_manager.can_save = this.can_save;

            if (UIManager.ui_manager.save_button != null)
                UIManager.ui_manager.save_button.interactable = can_save;

            Finish_Node();
        }
    }
}