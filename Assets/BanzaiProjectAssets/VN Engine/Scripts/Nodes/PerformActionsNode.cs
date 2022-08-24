using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
    // Performs all Actions. Can be used oike you would any normal Unity button
    // Suitable for turning on and off GameObjects
    public class PerformActionsNode : Node
    {
        public Button.ButtonClickedEvent Actions;

        public override void Run_Node()
        {
            Actions.Invoke();
            Finish_Node();
        }


        public override void Button_Pressed()
        {
            Finish_Node();
        }


        public override void Finish_Node()
        {
            StopAllCoroutines();

            base.Finish_Node();
        }
    }
}