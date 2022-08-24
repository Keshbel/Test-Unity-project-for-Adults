using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VNEngine
{
    [CustomEditor(typeof(VideoNode))]
    public class VideoNodeEditor : Editor
    {
        private SerializedProperty current_action;

        private SerializedProperty m_camera;
        private SerializedProperty m_video;
        private SerializedProperty m_volume;
        private SerializedProperty hide_ui;
        private SerializedProperty m_transparency;
        private SerializedProperty wait_until_video_is_finished;
        private SerializedProperty allow_skipping;
        private SerializedProperty is_prepared;
        private SerializedProperty looping_video;
        private SerializedProperty playback_speed;
        private SerializedProperty when_to_continue;
        private SerializedProperty time_to_wait;

        private SerializedProperty fade_out_video;
        private SerializedProperty fade_speed;
        private SerializedProperty hide_video_after;


        private void OnEnable()
        {
            current_action = serializedObject.FindProperty("current_action");

            m_video = serializedObject.FindProperty("video");
            m_camera = serializedObject.FindProperty("camera_to_use");
            hide_ui = serializedObject.FindProperty("hide_ui");
            m_volume = serializedObject.FindProperty("volume");
            m_transparency = serializedObject.FindProperty("video_player_transparency");
            allow_skipping = serializedObject.FindProperty("allow_skipping");
            looping_video = serializedObject.FindProperty("looping_video");
            playback_speed = serializedObject.FindProperty("playback_speed");
            when_to_continue = serializedObject.FindProperty("when_to_continue");
            time_to_wait = serializedObject.FindProperty("time_to_wait");
            is_prepared = serializedObject.FindProperty("is_prepared");

            fade_out_video = serializedObject.FindProperty("fade_out_video");
            fade_speed = serializedObject.FindProperty("fade_speed");
            hide_video_after = serializedObject.FindProperty("hide_video_after");
        }


        override public void OnInspectorGUI()
        {
            serializedObject.Update();

            var node = target as VideoNode;

            EditorGUILayout.HelpBox("THIS CODE IS IN BETA, AND VIDEO FORMATS MAY NOT WORK ON ALL DEVICES (especially mobile devices)", MessageType.Warning);
            EditorGUILayout.HelpBox("It is recommended to disable the background and foreground when playing videos, as it renders on a camera (not the canvas).  You may also wish to add a WaitNode after this video if you want to nicely fade in a new background/foreground right before the video ends", MessageType.Info);


            EditorGUILayout.PropertyField(current_action, new GUIContent("", "Should we play a video, pause a video, stop a video or resume a video?"));

            switch (node.current_action)
            {
                case VideoNode.Video_Action.Play_Video:
                    EditorGUILayout.PropertyField(m_video, new GUIContent("Video", "Videos are rendered to a specific camera, meaning UI elements can block it"));


                    if (node.camera_to_use == null)
                        EditorGUILayout.HelpBox("Camera.main will be used if no camera is specified in Camera_to_use field", MessageType.Error);
                    EditorGUILayout.PropertyField(m_camera, new GUIContent("Camera to use", "Video to play"));

                    EditorGUILayout.PropertyField(hide_ui, new GUIContent("Hide UI"));
                    EditorGUILayout.Slider(m_volume, 0f, 1f, new GUIContent("Video Volume"));
                    EditorGUILayout.Slider(m_transparency, 0f, 1f, new GUIContent("Video Transparency", "Change to allow users to aprtially see through the video"));
                    EditorGUILayout.PropertyField(allow_skipping, new GUIContent("Allow user to skip video?", "If allowed, user can press the submit key (default is SPACEBAR) to skip this video"));

                    if (node.is_prepared)
                        EditorGUILayout.HelpBox("If your editor version is less than 2017.1, Load Video First WILL NOT WORK. Your editor version is: " + Application.unityVersion, MessageType.Warning);
                    EditorGUILayout.PropertyField(is_prepared, new GUIContent("Load Video First", "Enabling this will ensure the video is fully loaded before it's played, and should remove any stuttering frames at video playback. This may cause a delay when playing the video depending on video size."));

                    EditorGUILayout.PropertyField(looping_video, new GUIContent("Video loops", "If true, video will cpontinually loop"));
                    EditorGUILayout.PropertyField(playback_speed, new GUIContent("Playback speed", "Speed of which video should play at. Default is 1"));

                    if (node.when_to_continue == VideoNode.When_To_Continue.Continue_immediately)
                        EditorGUILayout.HelpBox("The video will not be automatically disabled when it finishes playing. Use a PlayVideoNode set to Pause or Stop to manually stop the video", MessageType.Info);

                    EditorGUILayout.PropertyField(when_to_continue, new GUIContent("When to continue", "When should regular execution of the game continue?"));
                    switch (node.when_to_continue)
                    {
                        case VideoNode.When_To_Continue.Continue_immediately:

                            break;
                        case VideoNode.When_To_Continue.Wait_for_video:

                            break;
                        case VideoNode.When_To_Continue.Specify_Delay:
                            EditorGUILayout.PropertyField(time_to_wait, new GUIContent("Time to wait (seconds)", "Continue execution of nodes after X seconds"));
                            break;
                    }

                    break;
                case VideoNode.Video_Action.Pause_Video:
                    EditorGUILayout.PropertyField(fade_out_video, new GUIContent("Fade out video"));
                    EditorGUILayout.PropertyField(hide_video_after, new GUIContent("Hide video after"));
                    break;
                case VideoNode.Video_Action.Stop_Video:
                    EditorGUILayout.PropertyField(fade_out_video, new GUIContent("Fade out video"));
                    EditorGUILayout.PropertyField(hide_video_after, new GUIContent("Hide video after"));
                    break;
                case VideoNode.Video_Action.Resume_Video:

                    break;
            }

            serializedObject.ApplyModifiedProperties();

            //base.OnInspectorGUI();
        }
    }
}