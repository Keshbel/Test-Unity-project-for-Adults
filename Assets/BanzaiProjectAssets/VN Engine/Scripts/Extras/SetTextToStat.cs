using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace VNEngine
{
    public enum Type_of_Stat { Numbered_Stat, String_Stat, Boolean_Stat };

    // Requires a UI text element to attach to.
    // Grabs a Stat from StatsManager during the update loop and assigns it to the UI Text element on this object
    // Can assign a string you want to appear before the stat value, and a string after the stat value
    [RequireComponent(typeof(Text))]
    public class SetTextToStat : MonoBehaviour
    {
        public string name_of_stat_to_retrieve = "";
        public Type_of_Stat type_of_stat_to_retrieve = Type_of_Stat.Numbered_Stat;
        public string message_before_stat = "";     // Ex: "Strength: "
        public string message_after_stat = "";      // Ex: " ."
        public bool display_nothing_if_stat_not_present = false;
        Text text_element;

        void Start()
        {
            text_element = this.GetComponent<Text>();
        }


        void Update()
        {
            // Don't do anything if no Stat name is defined
            if (string.IsNullOrEmpty(name_of_stat_to_retrieve))
                return;


            string stat = "";
            // Retrieve the correct stat
            switch (type_of_stat_to_retrieve)
            {
                case Type_of_Stat.Numbered_Stat:
                    // Check if we should display nothing if the stat does not exist
                    if (display_nothing_if_stat_not_present && !StatsManager.Numbered_Stat_Exists(name_of_stat_to_retrieve))
                    {
                        text_element.text = "";
                        return;
                    }

                    stat = "" + StatsManager.Get_Numbered_Stat(name_of_stat_to_retrieve);
                    break;
                case Type_of_Stat.String_Stat:
                    if (display_nothing_if_stat_not_present && !StatsManager.String_Stat_Exists(name_of_stat_to_retrieve))
                    {
                        text_element.text = "";
                        return;
                    }

                    stat = StatsManager.Get_String_Stat(name_of_stat_to_retrieve);
                    break;
                case Type_of_Stat.Boolean_Stat:
                    if (display_nothing_if_stat_not_present && !StatsManager.Boolean_Stat_Exists(name_of_stat_to_retrieve))
                    {
                        text_element.text = "";
                        return;
                    }

                    stat = "" + StatsManager.Get_Boolean_Stat(name_of_stat_to_retrieve);
                    break;
            }

            // Construct the actual string
            text_element.text = message_before_stat + stat + message_after_stat;
        }
    }
}