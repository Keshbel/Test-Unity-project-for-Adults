using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace VNEngine
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager audio_manager;
        public AudioSource background_music_audio_source;    // Music that plays in background. Only one track of music will ever be playing at once
        public AudioSource voice_audio_source;    // Audio source playing the talking of the voice actors
        public AudioSource ambience_audio_source;   // Rain falling, city streets, vague murmuring. Allows an extra layer of sounds to be used in addition to background_music

        bool muted = false;     // If muted, NO audio will play
        public GameObject sound_effect_prefab;  // Must be an object with an audiosource, and necessary for PlaySoundEffect nodes

        // Volume controls
        [HideInInspector]
        public float master_volume = 1;
        [HideInInspector]
        public float music_volume = 1;
        [HideInInspector]
        public float voice_volume = 1;
        [HideInInspector]
        public float effects_volume = 1;

        public Slider Master_Volume_Slider;
        public Slider Music_Volume_Slider;
        public Slider Voice_Volume_Slider;
        public Slider Effects_Volume_Slider;

        public List<AudioSource> talking_beeps_list;


        void Awake()
        {
            audio_manager = this;
        }
        void Start()
        {
            // Load player preferences
            Master_Volume_Changed(PlayerPrefs.GetFloat("MasterVolume", 1));
            Voice_Volume_Changed(PlayerPrefs.GetFloat("VoiceVolume", 1));
            Music_Volume_Changed(PlayerPrefs.GetFloat("MusicVolume", 1));
            Effects_Volume_Changed(PlayerPrefs.GetFloat("EffectsVolume", 1));

            // Set sliders
            if (Master_Volume_Slider)
                Master_Volume_Slider.value = master_volume;
            if (Music_Volume_Slider)
                Music_Volume_Slider.value = music_volume;
            if (Voice_Volume_Slider)
                Voice_Volume_Slider.value = voice_volume;
            if (Effects_Volume_Slider)
                Effects_Volume_Slider.value = effects_volume;
        }


        // Fades out the current background music over seconds_it_takes_to_fade_out
        public IEnumerator Fade_Out_Music(float seconds_it_takes_to_fade_out)
        {

            if (background_music_audio_source != null)
            {
                while (background_music_audio_source.volume > 0)
                {
                    background_music_audio_source.volume -= (1 / seconds_it_takes_to_fade_out) * Time.deltaTime;
                    yield return null;
                }
            }
        }
        // Fades out the current background music over seconds_it_takes_to_fade_out
        public IEnumerator Fade_Out_Ambience(float seconds_it_takes_to_fade_out)
        {
            if (ambience_audio_source != null)
            {
                while (ambience_audio_source.volume > 0)
                {
                    ambience_audio_source.volume -= (1 / seconds_it_takes_to_fade_out) * Time.deltaTime;
                    yield return null;
                }
            }
        }


        // Starts the new background music and starts it playing
        public void Set_Music(AudioClip new_music)
        {
            background_music_audio_source.clip = new_music;
            background_music_audio_source.loop = true;
            background_music_audio_source.volume = music_volume;
            background_music_audio_source.Play();
        }
        // Starts the new ambience music and starts it playing
        public void Set_Ambience(AudioClip new_music)
        {
            ambience_audio_source.clip = new_music;
            ambience_audio_source.loop = true;
            ambience_audio_source.volume = music_volume;
            ambience_audio_source.Play();
        }


        // Toggles the muting of ALL audio in the scene
        public void Toggle_Audio_Muting()
        {
            Debug.Log("Toggling audio muting");

            if (!muted)
                AudioListener.volume = 0;
            else
                AudioListener.volume = master_volume;

            muted = !muted;
        }


        public void Play_Voice_Clip(AudioClip voice_clip)
        {
            voice_audio_source.Stop();
            voice_audio_source.clip = voice_clip;
            voice_audio_source.Play();
        }


        // Creates a new gameobject with its own AudioSource that destroys itself after the sound is done playing
        public void Play_Sound_Effect(AudioClip voice_clip)
        {
            GameObject sound_obj = Instantiate(sound_effect_prefab) as GameObject;
            AudioSource audio = sound_obj.GetComponent<AudioSource>();
            audio.clip = voice_clip;
            audio.playOnAwake = true;
            audio.volume = effects_volume;
        }


        // Called every time a character is printed to the screen, plays 1 instance of the given sound
        public void Play_Talking_Beep(AudioClip beep)
        {
            bool found_free_audiosource = false;
            foreach (AudioSource aud in talking_beeps_list)
            {
                // Check if the audiosource is playing
                if (!aud.isPlaying)
                {
                    // AudioSource is not playing, make it play the beep
                    aud.clip = beep;
                    aud.Play();
                    found_free_audiosource = true;
                    break;
                }
            }
            if (!found_free_audiosource)
                Debug.Log("Could not find silent AudioSource for Play_Talking_Beep to use. Please add more AudioSources to the talking_beeps_list in AudioManager", this.gameObject);
        }


        // Volume options managed by the sliders in the pause menu
        public void Master_Volume_Changed(float new_volume)
        {
            master_volume = new_volume;

            // Change the audio listener's volume
            AudioListener.volume = master_volume;

            SavePlayerPreference("MasterVolume", new_volume);
        }
        public void Voice_Volume_Changed(float new_volume)
        {
            voice_volume = new_volume;
            voice_audio_source.volume = new_volume;

            // Change the talking beeps volume, as they count as voice volume
            foreach (AudioSource aud in talking_beeps_list)
            {
                aud.volume = voice_volume;
            }

            SavePlayerPreference("VoiceVolume", new_volume);
        }
        public void Music_Volume_Changed(float new_volume)
        {
            music_volume = new_volume;
            background_music_audio_source.volume = new_volume;
            ambience_audio_source.volume = new_volume;

            SavePlayerPreference("MusicVolume", new_volume);
        }
        public void Effects_Volume_Changed(float new_volume)
        {
            effects_volume = new_volume;
            SavePlayerPreference("EffectsVolume", new_volume);
        }


        // Player preferences live in Windows registries and persist between plays
        public void SavePlayerPreference(string key, bool value)
        {
            PlayerPrefs.SetInt(key, System.Convert.ToInt32(value));
            PlayerPrefs.Save();
        }
        public void SavePlayerPreference(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
            PlayerPrefs.Save();
        }
    }
}