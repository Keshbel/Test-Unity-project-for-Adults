using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
    // Used to generate a random number, and save it to a specified Numbered Stat
    // Can also have existing Stat numbers be added to this newly generated number
    // This can be used to eprform 'Stat Checks', like you would see in RPGS.
    // Ex: Place a GenerateRandomNumberNode, and have it add 1-20 to your Numbered Stat 'Str', and save it in a new stat called 'StrCheck'
    // Ex cont'd: then afterwards use a ChoiceNode that uses 'StrCheck' to determine what options are available to the player based on how well they did 
    public class GenerateRandomNumberNode : Node
    {
        public float minimum_random_number = 1;   // Inclusive
        public float maximum_random_number = 20;  // Inclusive
        public bool use_whole_numbers = true;
        public string Stat_To_Add_To_Random_Number; // Name of a Numbered Stat to add to the result of the random number.
        public string Stat_To_Save_To;      // Name of Numbered Stat to save the result to
        public bool print_result_to_console = true;

        public override void Run_Node()
        {
            if (string.IsNullOrEmpty(Stat_To_Save_To))
            {
                Debug.LogError("Stat_To_Save_To is empty. Please specify the name of a Stat to save to for this GenerateRandomNumberNode", this.gameObject);
                Finish_Node();
                return;
            }

            float value_from_stat = 0;
            float randomly_generated_num;

            if (use_whole_numbers)
                randomly_generated_num = (float)Random.Range((int)minimum_random_number, (int)maximum_random_number + 1);
            else
                randomly_generated_num = Random.Range(minimum_random_number, maximum_random_number);

            if (!string.IsNullOrEmpty(Stat_To_Add_To_Random_Number))
            {
                value_from_stat = StatsManager.Get_Numbered_Stat(Stat_To_Add_To_Random_Number);
            }

            float result = randomly_generated_num + value_from_stat;
            StatsManager.Set_Numbered_Stat(Stat_To_Save_To, result);

            if (print_result_to_console)
                Debug.Log("Rolled " + randomly_generated_num + ", added to value from Stat " + value_from_stat + " gives total: " + result + " saved in " + Stat_To_Save_To, this.gameObject);

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