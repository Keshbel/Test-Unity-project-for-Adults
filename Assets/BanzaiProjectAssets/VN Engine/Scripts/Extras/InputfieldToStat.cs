using UnityEngine;
using UnityEngine.UI;

namespace VNEngine
{
    [RequireComponent(typeof(InputField))]
    public class InputfieldToStat : MonoBehaviour
    {
        public Stat stat_type;

        public void InputfieldInputToStats(string stat_name)
        {
            switch (stat_type)
            {
                case Stat.Boolean_Stat:
                    Debug.LogError("Boolean stats are not supported in InputfieldToStat", this.gameObject);
                    break;
                case Stat.Numbered_Stat:
                    StatsManager.Set_Numbered_Stat(stat_name, float.Parse(this.GetComponent<InputField>().text));
                    break;
                case Stat.String_Stat:
                    StatsManager.Set_String_Stat(stat_name, this.GetComponent<InputField>().text);
                    break;
            }
        }
    }
}