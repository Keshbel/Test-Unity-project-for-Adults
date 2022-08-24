using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Video;

namespace VNEngine
{
    // THIS CODE IS IN BETA, AND VIDEO FORMATS MAY NOT WORK ON ALL DEVICES (especially mobile devices)
    // It is recommended to disable the background and foreground when playing videos, as it appears behind the canvas
    // You may also wish to add a WaitNode after this video if you want to nicely fade in a new background/foreground right before the video ends
    public class VideoNode : Node
    {
        public enum Video_Action { Play_Video, Pause_Video, Stop_Video, Resume_Video };
        public Video_Action current_action = Video_Action.Play_Video;

        public enum When_To_Continue { Wait_for_video, Specify_Delay, Continue_immediately, }
        public When_To_Continue when_to_continue = When_To_Continue.Wait_for_video;
        public float time_to_wait = 5.0f;

        // PLAY VIDEO
        public VideoClip video;
        public float volume = 1.0f;
        public float video_player_transparency = 1.0f;
        public bool hide_ui;
        public Camera camera_to_use;  // If left empty, will try to get the main camera
        public bool allow_skipping; // If the player presses Spacebar or clicks on dialogue text, video is stopped and skipped
        public bool looping_video = false;
        public bool is_prepared = false;
        public float playback_speed = 1.0f;

        // PAUSE VIDEO / STOP VIDEO
        public bool fade_out_video = false;
        float fade_speed = 1.0f;
        public bool hide_video_after = false;


        public override void Run_Node()
        {
            // Check to make sure we have a videoplayer assigned in the UIManager
            if (UIManager.ui_manager.video_player == null)
            {
                Debug.LogError("VideoPlayer is not assigned in the UIManager, used in VideoNode", this.gameObject);
                Finish_Node();
                return;
            }


            switch (current_action)
            {
                case Video_Action.Pause_Video:
                    if (fade_out_video)
                        StartCoroutine(FadeOutVideo());
                    else
                        DoneFadingVideo();
                    break;

                case Video_Action.Stop_Video:
                    if (fade_out_video)
                        StartCoroutine(FadeOutVideo());
                    else
                        DoneFadingVideo();
                    break;

                case Video_Action.Resume_Video:
                    if (UIManager.ui_manager.video_player.clip != null)
                        UIManager.ui_manager.video_player.Play();
                    Finish_Node();
                    break;

                case Video_Action.Play_Video:
                    if (camera_to_use == null && Camera.main == null)
                    {
                        Debug.LogError("No camera found to render video. Please assign a camera in VideoNode", this.gameObject);
                        Finish_Node();
                        return;
                    }
                    if (camera_to_use == null)
                        camera_to_use = Camera.main;

                    // Stop any previous video if it's playing
                    if (UIManager.ui_manager.video_player.isPlaying)
                        UIManager.ui_manager.video_player.Stop();

                    if (hide_ui)
                        VNSceneManager.scene_manager.Show_UI(!hide_ui);

                    UIManager.ui_manager.video_player.targetCamera = camera_to_use;
                    UIManager.ui_manager.video_player.targetCameraAlpha = video_player_transparency;
                    UIManager.ui_manager.video_player.playbackSpeed = playback_speed;
                    UIManager.ui_manager.video_player.clip = video;
                    UIManager.ui_manager.video_player.isLooping = looping_video;

                    if (UIManager.ui_manager.video_player.canSetDirectAudioVolume)
                    {
                        UIManager.ui_manager.video_player.audioOutputMode = VideoAudioOutputMode.Direct;
                        UIManager.ui_manager.video_player.SetDirectAudioVolume(0, volume);
                    }
                    else
                    {
                        UIManager.ui_manager.video_player.audioOutputMode = VideoAudioOutputMode.AudioSource;
                        UIManager.ui_manager.video_player.SetTargetAudioSource(0, UIManager.ui_manager.video_player.GetComponent<AudioSource>());
                    }

                    UIManager.ui_manager.video_player.gameObject.SetActive(true);

                    // Should we wait for the video to be fully loaded first? This could stop a delay
                    if (is_prepared)
                    {
                        StartCoroutine(StartWhenVideoIsLoaded());
                    }
                    // Play the video without preparation
                    else
                    {
                        UIManager.ui_manager.video_player.Play();
                        SetContinueParameter();
                    }
                    break;
            }
        }


        // Prepare the video, play when it's loaded
        public IEnumerator StartWhenVideoIsLoaded()
        {
            //PrepareCompleted = UIManager.ui_manager.video_player.prepareCompleted;
            UIManager.ui_manager.video_player.Prepare();
            while (!UIManager.ui_manager.video_player.isPrepared)
            {
                yield return null;
            }

            UIManager.ui_manager.video_player.Play();
            SetContinueParameter();
        }


        public void SetContinueParameter()
        {
            switch (when_to_continue)
            {
                case When_To_Continue.Wait_for_video:
                    StartCoroutine(Wait_Till_Video_Finishes());
                    break;
                case When_To_Continue.Continue_immediately:
                    Finish_Node();
                    break;
                case When_To_Continue.Specify_Delay:
                    StartCoroutine(Wait_For_Time(time_to_wait));
                    break;
            }
        }


        public IEnumerator FadeOutVideo()
        {
            while (UIManager.ui_manager.video_player.targetCameraAlpha > 0)
            {
                UIManager.ui_manager.video_player.targetCameraAlpha -= Time.deltaTime * fade_speed;
                yield return null;
            }

            DoneFadingVideo();
        }
        public void DoneFadingVideo()
        {
            switch (current_action)
            {
                case Video_Action.Pause_Video:
                    if (UIManager.ui_manager.video_player.isPlaying)
                        UIManager.ui_manager.video_player.Pause();
                    break;
                case Video_Action.Stop_Video:
                    if (UIManager.ui_manager.video_player.isPlaying)
                        UIManager.ui_manager.video_player.Stop();
                    break;
            }

            if (hide_video_after)
                UIManager.ui_manager.video_player.gameObject.SetActive(false);

            Finish_Node();
        }


        public IEnumerator Wait_Till_Video_Finishes()
        {
            while (UIManager.ui_manager.video_player.isPlaying)
            {
                yield return null;
            }
            StopVideo();
            Finish_Node();
        }


        public IEnumerator Wait_For_Time(float delay_time)
        {
            while (delay_time >= 0)
            {
                delay_time -= Time.deltaTime;
                yield return null;
            }

            Finish_Node();
        }


        public override void Button_Pressed()
        {
            if (allow_skipping)
            {
                StopVideo();
                Finish_Node();
            }
        }


        // Call this to disable the video
        public void StopVideo()
        {
            if (UIManager.ui_manager.video_player == null)
                return;

            if (UIManager.ui_manager.video_player.isPlaying)
                UIManager.ui_manager.video_player.Stop();

            UIManager.ui_manager.video_player.gameObject.SetActive(false);
        }


        public override void Finish_Node()
        {
            base.Finish_Node();
        }
    }
}