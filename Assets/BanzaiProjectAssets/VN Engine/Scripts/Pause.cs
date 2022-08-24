using UnityEngine;
using System.Collections;

namespace VNEngine
{
    // Manages the paused state of the game
    public class Pause : MonoBehaviour
    {
        public static Pause pause;

        public GameObject pause_menu;   // Must be assigned for pausing to work
        public GameObject options_menu; // Must be assigned for options to be displayed

        public string toggle_pause_key = "Pause";

        public bool pause_all_audio_when_paused = false;
        public bool pause_voices_when_paused = false;
        public bool pause_music_when_paused = false;


        [HideInInspector]
        public bool paused = false;

        void Awake()
        {
            pause = this;
            AudioListener.pause = false;
        }
        void Start()
        {
            AudioListener.pause = false;
        }

        public void Toggle_Pause()
        {
            if (paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }


        // Pause
        public void PauseGame()
        {
            pause_menu.SetActive(true);
            Time.timeScale = 0;
            paused = true;

            if (pause_all_audio_when_paused)
                AudioListener.pause = true;
            if (pause_voices_when_paused)
                AudioManager.audio_manager.voice_audio_source.Pause();
            if (pause_music_when_paused)
                AudioManager.audio_manager.background_music_audio_source.Pause();
        }
        // Resume/Unpause
        public void ResumeGame()
        {
            pause_menu.SetActive(false);
            Time.timeScale = 1;
            paused = false;

            if (pause_all_audio_when_paused)
                AudioListener.pause = false;
            if (pause_voices_when_paused)
                AudioManager.audio_manager.voice_audio_source.UnPause();
            if (pause_music_when_paused)
                AudioManager.audio_manager.background_music_audio_source.UnPause();
        }



        // Toggles the displaying of the options menu
        public void Toggle_Options()
        {
            options_menu.SetActive(!options_menu.activeSelf);
        }


        void Update()
        {
            if (Input.GetButtonDown(toggle_pause_key))
                Toggle_Pause();

            // Record time played here
        }


        public void SaveButtonClicked()
        {
            Debug.Log("Saving...");
            SaveFile s = new SaveFile();
            s.Save();
        }
    }
}