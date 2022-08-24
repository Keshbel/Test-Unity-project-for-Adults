using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
    // The speed of which the text from dialogue nodes is scrolled on the screen.
    public class SetTextScrollSpeedNode : Node
    {
        // The delay (in seconds) it takes to print a single character
        [Range(0.0F, 0.5F)]
        public float new_text_scroll_speed = 0.02f;


        public override void Run_Node()
        {
            VNSceneManager.text_scroll_speed = new_text_scroll_speed;

            Finish_Node();
        }


        public override void Button_Pressed()
        {

        }


        public override void Finish_Node()
        {
            StopAllCoroutines();

            base.Finish_Node();
        }
    }
}