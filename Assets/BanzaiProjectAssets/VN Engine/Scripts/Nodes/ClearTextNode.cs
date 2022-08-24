using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
    public class ClearTextNode : Node
    {
        public override void Run_Node()
        {
            UIManager.ui_manager.text_panel.text = "";
            UIManager.ui_manager.speaker_panel.text = "";
            Finish_Node();
        }


        public override void Button_Pressed()
        {

        }


        public override void Finish_Node()
        {
            base.Finish_Node();
        }
    }
}