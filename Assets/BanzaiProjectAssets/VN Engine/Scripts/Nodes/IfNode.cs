using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace VNEngine
{
    public enum Condition { Bool_Stat_Requirement, Float_Stat_Requirement, String_Stat_Requirement, Object_Is_Null, Object_Is_Active };

    public enum Boolean_Logic { And, Or };

    public enum Result { Is, Is_Not };

    // Are we looking for a true, or a false at the end?
    public enum Condition_Is { Met, Not_Met };

    // If the stat requirement is met, what should we do?
    public enum Requirement_Met_Action { Change_Conversation, Jump_to_Middle_of_Conversation, Custom_Events };

    // Checks some conditions, and if the conditions are met, some actions are performed.
    // If conditions are NOT met, the conversation is simply continued
    // CONDITIONS ARE EVALUATED IN ORDER. This means the LAST condition is by far the most important
    // Ex: false AND false AND false OR true = true
    // Ex: true OR true OR true AND false = false
    public class IfNode : Node
    {
        // DO NOT CHANGE. Maximum number of conditions allowed.
        public static int max_number_of_conditions = 10;

        [HideInInspector]
        public int Number_Of_Conditions = 1;    // Conditions can be changed together using AND and OR

        [HideInInspector]
        public Condition[] Conditions = new Condition[max_number_of_conditions];
        [HideInInspector]
        public string[] Stat_Name = new string[max_number_of_conditions];
        [HideInInspector]
        public bool[] Bool_Compare_Value = new bool[max_number_of_conditions];
        [HideInInspector]
        public float[] Float_Compare_Value = new float[max_number_of_conditions];
        [HideInInspector]
        public string[] String_Compare_Value = new string[max_number_of_conditions];
        [HideInInspector]
        public Result[] String_Is = new Result[max_number_of_conditions];
        [HideInInspector]
        public Float_Stat_Comparator[] Float_Stat_Is = new Float_Stat_Comparator[max_number_of_conditions];
        [HideInInspector]
        public GameObject[] Check_Null_Object = new GameObject[max_number_of_conditions];
        [HideInInspector]
        public GameObject[] Check_Active_Object = new GameObject[max_number_of_conditions];
        [HideInInspector]
        public Boolean_Logic[] Logic = new Boolean_Logic[max_number_of_conditions];

        [HideInInspector]
        public Condition_Is Is_Condition_Met;
        [HideInInspector]
        public Requirement_Met_Action Action = Requirement_Met_Action.Change_Conversation;
        [HideInInspector]
        public ConversationManager Conversation_To_Switch_To;
        [HideInInspector]
        public Node Node_To_Switch_To;
        [HideInInspector]
        public bool Continue_Conversation = true;

        public Button.ButtonClickedEvent Custom_Actions;   // Custom actions, can be assigned like normal Unity buttons


        public override void Run_Node()
        {
            StartCoroutine(Running());
        }
        public IEnumerator Running()
        {
            // Wait a frame so we can evaluate if objects we check for requirements have been destroyed
            yield return 0;

            bool conditions_met = true;

            for (int x = 0; x < Number_Of_Conditions; x++)
            {
                bool cur_condition = false;

                // Evaluate each condition sequentially
                switch (Conditions[x])
                {
                    case Condition.Bool_Stat_Requirement:
                        cur_condition = StatsManager.Compare_Bool_Stat_To(Stat_Name[x], Bool_Compare_Value[x]);
                        break;


                    case Condition.Float_Stat_Requirement:
                        cur_condition = StatsManager.Compare_Float_Stat(Stat_Name[x], Float_Stat_Is[x], Float_Compare_Value[x]);
                        break;

                    case Condition.String_Stat_Requirement:
                        bool the_same = true;
                        switch (String_Is[x])
                        {
                            case Result.Is:
                                the_same = true;
                                break;
                            case Result.Is_Not:
                                the_same = false;
                                break;
                        }
                        cur_condition = StatsManager.Compare_String_Stat_To(Stat_Name[x], String_Compare_Value[x], the_same);
                        break;

                    case Condition.Object_Is_Null:
                        // Check if object exists
                        // If the object doesn't exist, and the box is checked
                        // OR  the object exists and the box is not checked
                        cur_condition = Check_Null_Object[x] && !Bool_Compare_Value[x];
                        break;

                    case Condition.Object_Is_Active:
                        // Check if the gameobject is enabled
                        cur_condition = Check_Active_Object[x].activeSelf && Bool_Compare_Value[x];
                        break;
                }

                // Check if need to keep going with AND's or OR's
                if (x == 0)
                {
                    // First condition, nothing to AND or OR with
                    conditions_met = cur_condition;
                }
                else if (x < Number_Of_Conditions)
                {
                    // Must AND or OR the previous condition
                    switch (Logic[x - 1])
                    {
                        case Boolean_Logic.And:
                            conditions_met = conditions_met && cur_condition;
                            break;
                        case Boolean_Logic.Or:
                            conditions_met = conditions_met || cur_condition;
                            break;
                    }
                }
            }

            if ((Is_Condition_Met == Condition_Is.Met && conditions_met)
                || (Is_Condition_Met == Condition_Is.Not_Met && !conditions_met))
            {
                switch (Action)
                {
                    case Requirement_Met_Action.Change_Conversation:
                        if (Conversation_To_Switch_To != null)
                            Conversation_To_Switch_To.Start_Conversation();
                        else
                            Debug.LogError("No conversation to switch to", this.gameObject);
                        break;
                    case Requirement_Met_Action.Jump_to_Middle_of_Conversation:
                        if (!Node_To_Switch_To)
                        {
                            Debug.LogError("Node to switch to not set in IfNode", gameObject);
                            break;
                        }

                        ConversationManager cm = Node_To_Switch_To.GetComponentInParent<ConversationManager>();
                        if (cm)
                        {
                            cm.Start_At_Node(Node_To_Switch_To);
                        }
                        else
                            Debug.LogError("Couldn't find Conversation associated with Node: " + Node_To_Switch_To.name, gameObject);
                        break;
                    case Requirement_Met_Action.Custom_Events:
                        Custom_Actions.Invoke();

                        if (Continue_Conversation)
                            Finish_Node();
                        break;
                }
            }
            else
            {
                // Just continue conversation
                Finish_Node();
            }
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