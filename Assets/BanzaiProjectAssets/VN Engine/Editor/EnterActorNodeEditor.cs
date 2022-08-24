using UnityEngine;
using UnityEditor;

namespace VNEngine
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(EnterActorNode))]
    public class EnterActorNodeEditor : Editor
    {
        float normal_label_width;
        private SerializedProperty m_actorname;
        private SerializedProperty m_actor_name_from;
        private SerializedProperty m_entrancetype;
        private SerializedProperty m_fadeintime;
        private SerializedProperty m_destination;
        private SerializedProperty m_customposition;

        private void OnEnable()
        {
            m_actorname = serializedObject.FindProperty("actor_name");
            m_actor_name_from = serializedObject.FindProperty("actor_name_from");
            m_entrancetype = serializedObject.FindProperty("entrance_type");
            m_fadeintime = serializedObject.FindProperty("fade_in_time");
            m_destination = serializedObject.FindProperty("destination");
            m_customposition = serializedObject.FindProperty("custom_position");
        }

        override public void OnInspectorGUI()
        {
            serializedObject.Update();

            var node = target as EnterActorNode;
            normal_label_width = EditorGUIUtility.labelWidth;

            EditorGUILayout.PropertyField(m_actor_name_from, new GUIContent("Actor Name From", "Source of the actor name, whether simply from this textfield, a string stat named after this text value, or a CSV value with this text value as its key"));
            EditorGUILayout.PropertyField(m_actorname, new GUIContent("Actor Name", "Exact name of the Actor prefab located in your Resources folder"));
            EditorGUILayout.PropertyField(m_entrancetype, new GUIContent("Entrance Type", "How should the actor enter?"));
            switch (node.entrance_type)
            {
                case Entrance_Type.Fade_In:
                    EditorGUILayout.PropertyField(m_fadeintime, new GUIContent("Fade in Time", "How many seconds does it take to fade in?"));
                    break;
                case Entrance_Type.Slide_In:

                    break;
                case Entrance_Type.None:

                    break;
            }

            EditorGUILayout.PropertyField(m_destination, new GUIContent("Destination", "What side should the actor appear on?"));
            switch (node.destination)
            {
                case Actor_Positions.CUSTOM:
                    EditorGUILayout.PropertyField(m_customposition, new GUIContent("Custom Position", "What transform should this actor be anchored to?"));
                    break;
                default:
                    EditorGUILayout.HelpBox("The Actor will automatically be placed, and when new Actors appear on the same DESTINATION, Actors will automatically shift over to give the most room.", MessageType.Info);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}