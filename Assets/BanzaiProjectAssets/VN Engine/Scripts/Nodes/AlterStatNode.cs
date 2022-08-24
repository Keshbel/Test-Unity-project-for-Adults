using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
    public enum Stat_Type { Set_Number, Modify_Number, Set_Boolean, Toggle_Boolean, Set_String };

    // Alters the saved stats in StatsManager
    // There are 2 types of stats: numbers and booleans
    // Use this node to add/alter stats used to keep progress points, routes taken, player stats, etc
    // Stats are automatically saved
    public class AlterStatNode : Node
    {
        // Always visible
        public bool print_all_stats_to_console = false;
        public Stat_Type stat_type = Stat_Type.Set_Number;
        public string stat_name;

        // Numbered stats, only visible if stat_type is set to Number
        [HideInInspector]
        public float set_number_to = 0;
        [HideInInspector]
        public float modify_number_amount = 0;
        // Boolean stats, only visible if stat_type is set to Boolean
        [HideInInspector]
        public bool set_bool_to = false;
        // String stats, only visible if stat_type is set to String
        [HideInInspector]
        public string set_string_to = "";


        public override void Run_Node()
        {
            switch (stat_type)
            {
                case Stat_Type.Set_Number:
                    StatsManager.Set_Numbered_Stat(stat_name, set_number_to);
                    break;
                case Stat_Type.Modify_Number:
                    StatsManager.Add_To_Numbered_Stat(stat_name, modify_number_amount);
                    break;
                case Stat_Type.Set_Boolean:
                    StatsManager.Set_Boolean_Stat(stat_name, set_bool_to);
                    break;
                case Stat_Type.Toggle_Boolean:
                    StatsManager.Toggle_Boolean_Stat(stat_name);
                    break;
                case Stat_Type.Set_String:
                    StatsManager.Set_String_Stat(stat_name, set_string_to);
                    break;
            }

            if (print_all_stats_to_console)
                StatsManager.Print_All_Stats();

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