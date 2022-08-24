using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VNEngine
{
    public enum Stat { Boolean_Stat, String_Stat, Numbered_Stat };

    // Contains dictionaries used for containing stats, numbers, booleans, triggers.
    // This is automatically SAVED and LOADED.
    // Is a static class that can be accessed anywhere, and only one exists (is kept automatically between scenes)
    // Ex: Player chose Route A, Player has 5.66$, Mary's affection is 8
    public static class StatsManager
    {
        // DO NOT ALTER THESE DICTIONARIES DIRECTLY, use the methods below (Get_.. or Set_..)
        // Numbered stats: Money, affection stats
        public static Dictionary<string, float> numbered_stats = new Dictionary<string, float>();
        // Boolean stats: used for triggers, keeping track of player progress
        public static Dictionary<string, bool> boolean_stats = new Dictionary<string, bool>();
        // String stats: could be used for player names, names of locations. Can be dynamically used in DialogueNodes
        public static Dictionary<string, string> string_stats = new Dictionary<string, string>();
        // Items: Used for the Item menu. These items are automatically loaded. They must be stored in Resources/Items folder
        public static List<string> items = new List<string>();



        // Returns a float associated with the given name
        // Returns -999 if a null/empty string is passed in
        // Returns 0 if the stat is not present in the dictionaries
        public static float Get_Numbered_Stat(string stat_name)
        {
            // Check if stat_name is null or empty
            if (string.IsNullOrEmpty(stat_name))
            {
                Debug.LogError("Get_Numbered_Stat passed empty string");
                return -999;
            }

            // Check if key exists
            if (!numbered_stats.ContainsKey(stat_name))
            {
                Debug.Log(stat_name + " does not exist in numbered_stats. Returning 0");
                return 0;
            }
            else
            {
                return numbered_stats[stat_name];
            }
        }
        // Returns true if the numbered stat exists, false otherwise
        public static bool Numbered_Stat_Exists(string stat_name)
        {
            return (numbered_stats.ContainsKey(stat_name));
        }
        // Sets the given stat to the given value
        public static void Set_Numbered_Stat(string stat_name, float value)
        {
            // Check if stat_name is null or empty
            if (string.IsNullOrEmpty(stat_name))
            {
                Debug.LogError("Set_Numbered_Stat passed empty string, stat not set");
                return;
            }

            // Check if key exists
            if (!numbered_stats.ContainsKey(stat_name))
            {
                // Key did not exist, add it to dictionary
                numbered_stats.Add(stat_name, value);
            }
            else
            {
                // Key exists, change it
                numbered_stats[stat_name] = value;
            }

            Debug.Log(stat_name + " set to " + value);
        }
        // Modifies a numbered stat. It ADDS number_to_add to stat_name. number_to_add could be positive or negative.
        // If key did not exist, a new one is created and initialized to 0, and then number_to_add is added to it
        // Returns the newly altered stat_name, or -999 if stat_name is null or empty
        public static float Add_To_Numbered_Stat(string stat_name, float number_to_add)
        {
            // Check if stat_name is null or empty
            if (string.IsNullOrEmpty(stat_name))
            {
                Debug.LogError("Add_To_Numbered_Stat passed empty string, stat not set");
                return -999;
            }

            // Check if key exists
            if (!Numbered_Stat_Exists(stat_name))
            {
                // Key did not exist, add it to dictionary
                numbered_stats.Add(stat_name, number_to_add);
                Debug.LogError(stat_name + " did not exist. Please use Set_Numbered_Stat to create numbered stats. " + stat_name + " set to " + number_to_add);
            }
            else
            {
                // Key exists. Add to it
                numbered_stats[stat_name] += number_to_add;
                Debug.Log("Added " + number_to_add + " to " + stat_name + ". Is now: " + numbered_stats[stat_name]);
            }

            return numbered_stats[stat_name];
        }

        // Checks if the given stat is > or < the value. Returns true/false.
        // If no stat of that name is found, 0 is given as the stat's value
        public static bool Compare_Float_Stat(string stat_name, Float_Stat_Comparator than, float value)
        {
            // Check if the stat exists
            if (!StatsManager.Numbered_Stat_Exists(stat_name))
            {
                Debug.Log(stat_name + " Numbered Stat does not exist. Returning default value for stat: 0");
            }

            bool result = false;
            // Check if it meets the requirements
            float stat = StatsManager.Get_Numbered_Stat(stat_name);
            switch (than)
            {
                case Float_Stat_Comparator.Greater_than:
                    result = stat > value;
                    break;
                case Float_Stat_Comparator.Less_than:
                    result = stat < value;
                    break;
            }
            return result;
        }






        // Returns a bool associated with the given name
        // Returns false if a null/empty string is passed in
        // Returns 0 if the stat is not present in the dictionaries
        public static bool Get_Boolean_Stat(string stat_name)
        {
            // Check if stat_name is null or empty
            if (string.IsNullOrEmpty(stat_name))
            {
                Debug.LogError("Get_Boolean_Stat passed empty string");
                return false;
            }

            // Check if key exists
            if (!boolean_stats.ContainsKey(stat_name))
            {
                //Debug.Log(stat_name + " does not exist in boolean_stats. Returning false");
                return false;
            }
            else
            {
                return boolean_stats[stat_name];
            }
        }
        // Returns true if the boolean stat exists, false otherwise
        public static bool Boolean_Stat_Exists(string stat_name)
        {
            return (boolean_stats.ContainsKey(stat_name));
        }
        // Sets the given stat to the given value
        public static void Set_Boolean_Stat(string stat_name, bool value)
        {
            // Check if stat_name is null or empty
            if (string.IsNullOrEmpty(stat_name))
            {
                Debug.LogError("Set_Boolean_Stat passed empty string, stat not set");
                return;
            }

            // Check if key exists
            if (!boolean_stats.ContainsKey(stat_name))
            {
                // Key did not exist, add it to dictionary
                boolean_stats.Add(stat_name, value);
            }
            else
            {
                // Key exists, set it
                boolean_stats[stat_name] = value;
            }

            Debug.Log(stat_name + " set to " + value);
        }
        // Toggles a boolean stat. Ex: false becomes true, true becomes false
        // If key did not exist, a new one is created and set to false
        // Returns the newly toggled stat_name, or false if stat_name is null or empty
        public static bool Toggle_Boolean_Stat(string stat_name)
        {
            // Check if stat_name is null or empty
            if (string.IsNullOrEmpty(stat_name))
            {
                Debug.LogError("Toggle_Boolean_Stat passed empty string, stat not set");
                return false;
            }

            // Check if key exists
            if (!Boolean_Stat_Exists(stat_name))
            {
                // Key did not exist, add it to dictionary
                boolean_stats.Add(stat_name, false);
                Debug.LogError(stat_name + " did not exist. Please use Set_Boolean_Stat to create boolean stats. " + stat_name + " set to false");
            }
            else
            {
                // Key exists. Toggle it
                boolean_stats[stat_name] = !boolean_stats[stat_name];
                Debug.Log(stat_name + " toggled to " + boolean_stats[stat_name]);
            }

            return boolean_stats[stat_name];
        }

        // Finds the stat of the given name and compares it to is_
        // If the stat is not found, false is given as the stat value
        public static bool Compare_Bool_Stat_To(string stat_name, bool is_)
        {
            // Check if the stat exists
            if (!StatsManager.Boolean_Stat_Exists(stat_name) && VNSceneManager.verbose_debug_logs)
            {
                Debug.Log("Comparing bool stat. " + stat_name + " does not exist");
            }

            return StatsManager.Get_Boolean_Stat(stat_name) == is_;
        }





        // Returns a bool associated with the given name
        // Returns false if a null/empty string is passed in
        // Returns 0 if the value is not present in the dictionaries
        public static string Get_String_Stat(string stat_name)
        {
            // Check if stat_name is null or empty
            if (string.IsNullOrEmpty(stat_name))
            {
                Debug.LogError("Get_String_Stat passed empty string");
                return "";
            }

            // Check if key exists
            if (!String_Stat_Exists(stat_name))
            {
                Debug.Log(stat_name + " does not exist in string_stats. Returning empty");
                return "";
            }
            else
            {
                return string_stats[stat_name];
            }
        }
        // Returns true if the boolean stat exists, false otherwise
        public static bool String_Stat_Exists(string stat_name)
        {
            return (string_stats.ContainsKey(stat_name));
        }
        // Sets the given stat to the given value
        public static void Set_String_Stat(string stat_name, string value)
        {
            // Check if stat_name is null or empty
            if (string.IsNullOrEmpty(stat_name))
            {
                Debug.LogError("Set_String_Stat passed empty string, stat not set");
                return;
            }

            // Check if key exists
            if (!string_stats.ContainsKey(stat_name))
            {
                // Key did not exist, add it to dictionary
                string_stats.Add(stat_name, value);
            }
            else
            {
                // Key exists, set it
                string_stats[stat_name] = value;
            }

            Debug.Log(stat_name + " set to " + value);
        }

        // Finds the stat of the given name and compares it to is_
        // If the stat is not found, false is given as the stat value
        // Equals checks if the strings are the same, or if they are different
        public static bool Compare_String_Stat_To(string stat_name, string is_, bool equals)
        {
            // Check if the stat exists
            if (!StatsManager.String_Stat_Exists(stat_name))
            {
                Debug.Log("Comparing string stat. " + stat_name + " does not exist");
            }

            return equals == (StatsManager.Get_String_Stat(stat_name).Equals(is_));
        }




        // Loads the given item_name and places it in the Item Grid
        // Also adds item_name to the items list if it not already present
        // Items must be stored in a Resources/Items folder
        // Can be safely called multiple times, as item duplicates are not allowed
        public static void Add_Item(string item_name)
        {
            if (!Has_Item(item_name))
            {
                items.Add(item_name);
                Spawn_Physical_Item(item_name);
            }
            else if (!Check_If_Physical_Item_Exists(item_name))
            {
                // The item might be added to the list already, but it might be physically spawned it
                Spawn_Physical_Item(item_name);
            }
            else
                Debug.Log("Item " + item_name + " already present. Will not add duplicate.");
        }
        // Returns true if the item is present
        public static bool Check_If_Physical_Item_Exists(string item_name)
        {
            Item[] existing_items = GameObject.FindObjectsOfType<Item>();

            foreach (Item i in existing_items)
            {
                if (i.Item_Name == item_name)
                    return true;
            }
            return false;
        }
        // Loads the item from Resources/Items and places it in the item_grid
        public static void Spawn_Physical_Item(string item_name)
        {
            if (Check_If_Physical_Item_Exists(item_name))
                return;

            GameObject item = GameObject.Instantiate(Resources.Load<GameObject>("Items/" + item_name));
            item.name = item_name;
            item.transform.SetParent(UIManager.ui_manager.item_grid.transform);
            item.transform.localScale = Vector3.one;
        }
        // Destroys the physical item object in the game, and removes it from the items list
        public static void Remove_Item(string item_name)
        {
            Item[] i = UIManager.ui_manager.item_grid.GetComponentsInChildren<Item>();
            for (int x = 0; x < i.Length; x++)
            {
                if (i[x].Item_Name == item_name)
                {
                    GameObject.Destroy(i[x].gameObject);
                    break;
                }
            }
            items.Remove(item_name);
        }
        // Returns whether or not item_name is present is the items list
        public static bool Has_Item(string item_name)
        {
            return items.Contains(item_name);
        }
        // Loads all Items currently in the items list. Used when loading a SaveFile or when switching a scene by the VNSCeneManager
        public static void Load_All_Items()
        {
            foreach (string s in items)
            {
                Debug.Log("Loading item " + s);
                Spawn_Physical_Item(s);
            }
        }



        // Clears all values in the stats dictionaries
        public static void Clear_All_Stats()
        {
            numbered_stats.Clear();
            boolean_stats.Clear();
            string_stats.Clear();
            items.Clear();
            Debug.Log("Stats cleared");
        }
        // Prints all numbered and booleans stats, and Items to the Log window
        public static void Print_All_Stats()
        {
            Debug.Log("Printing all stats in StatsManager");
            Debug.Log("Numbered Stats:");
            foreach (KeyValuePair<string, float> pair in numbered_stats)
            {
                Debug.Log(pair.Key + " " + pair.Value);
            }
            Debug.Log("Boolean Stats:");
            foreach (KeyValuePair<string, bool> pair in boolean_stats)
            {
                Debug.Log(pair.Key + " " + pair.Value);
            }
            Debug.Log("String Stats:");
            foreach (KeyValuePair<string, string> pair in string_stats)
            {
                Debug.Log(pair.Key + " " + pair.Value);
            }
            Debug.Log("Items:");
            foreach (string s in items)
            {
                Debug.Log(s);
            }
        }
    }
}