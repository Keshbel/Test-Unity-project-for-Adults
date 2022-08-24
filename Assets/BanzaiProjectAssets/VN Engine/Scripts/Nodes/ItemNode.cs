using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
    public enum Item_Node_Action { Add_Item, Remove_Item };

    // Removes or adds an Item to the UIManager's Item grid
    // Item prefabs must be placed in the Resources/Items folder
    // Please use this node to add or remove items, so Items are properly saved and loaded when using the VN Engine's SaveFile
    public class ItemNode : Node
    {
        public Item_Node_Action Action;
        public string Item_Name;

        public override void Run_Node()
        {
            if (!string.IsNullOrEmpty(Item_Name))
            {
                switch (Action)
                {
                    case Item_Node_Action.Add_Item:
                        StatsManager.Add_Item(Item_Name);
                        break;
                    case Item_Node_Action.Remove_Item:
                        StatsManager.Remove_Item(Item_Name);
                        break;
                }
            }
            else
            {
                Debug.LogError("Item_Name not set for ItemNode", gameObject);
            }

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