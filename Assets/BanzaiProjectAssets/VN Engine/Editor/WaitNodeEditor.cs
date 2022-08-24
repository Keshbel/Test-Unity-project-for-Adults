using UnityEngine;
using UnityEditor;

namespace VNEngine
{
    // Give this script a custom UI interface that shows and hides some fields based on the stat_type being used
    [CustomEditor(typeof(WaitNode))]
    public class WaitNodeEditor : Editor
    {
        override public void OnInspectorGUI()
        {
            // Redraw this every frame
            EditorUtility.SetDirty(target);

            var myScript = target as WaitNode;

            myScript.waiting_for = (Wait_Until)EditorGUILayout.EnumPopup("What are we waiting for? ", myScript.waiting_for);

            // Only show some fields based on stat_type
            switch (myScript.waiting_for)
            {
                case Wait_Until.Time:
                    myScript.wait_for_seconds = EditorGUILayout.FloatField("Wait time (seconds) ", myScript.wait_for_seconds);
                    break;
                case Wait_Until.Bool_is_True:
                    EditorGUILayout.HelpBox("The game will wait until the following bool is set to true. This can be done through code:\nVNSceneManager.Waiting_till_true = true;", MessageType.Info);
                    VNSceneManager.Waiting_till_true = EditorGUILayout.Toggle("Proceed when true", VNSceneManager.Waiting_till_true);
                    break;
            }
        }
    }
}
