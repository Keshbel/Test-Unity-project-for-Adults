using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace VNEngine
{
    // Class for created a button for each Saved game there is. Clicked a Saved game will load it.
    // Be sure that any Scenes you are using has been added to your BUILD
    public class UILoadSaveController : MonoBehaviour
    {
        public GameObject button_to_create;
        public string newgame_scene_to_load = "Tutorial";

        void Start()
        {
            SaveManager.LoadFromFile();

            for (int x = 0; x < SaveManager.saved_games.Count; x++)
            {
                SaveFile save = SaveManager.saved_games[x];

                InstantiateSavebutton(save);
            }
        }
        public void InstantiateSavebutton(SaveFile save)
        {
            // Create a button for each save
            GameObject button = Instantiate(button_to_create);

            button.SetActive(true);
            TimeSpan ts = TimeSpan.FromSeconds(save.time_played);
            string time_played_str = String.Format("{0:00}:{1:00}", ts.TotalHours, ts.TotalMinutes);

            // Name the button after the time played and the date the save was created
            button.transform.name = save.time_saved.ToShortDateString() + " " + save.time_saved.ToShortTimeString();
            button.GetComponentInChildren<Text>().text = "Time played: " + time_played_str + "   Saved: " + save.time_saved.ToShortDateString() + " " + save.time_saved.ToShortTimeString();
            button.transform.parent = this.transform;
            button.transform.localScale = Vector3.one;


            // Set up the button onClicks
            button.GetComponent<Button>().onClick.RemoveAllListeners();
            button.GetComponent<Button>().onClick = null;
            button.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();

            // Add the all important SaveFile.load() method to the button onClicks. save.Load() is the function that loads the level
            button.GetComponent<Button>().onClick.AddListener(() => { StartCoroutine(save.Load()); });


            // Add the onclick Listener to delete this individual save when they click the little X in the top right corner
            Button[] buttons = button.GetComponentsInChildren<Button>();
            if (buttons.Length >= 2)
            {
                Button delete_this_save = buttons[1];
                if (delete_this_save != null)
                {
                    delete_this_save.onClick.RemoveAllListeners();
                    delete_this_save.onClick = null;
                    delete_this_save.onClick = new Button.ButtonClickedEvent();
                    delete_this_save.onClick.AddListener(() => { SaveManager.DeleteSave(save); Destroy(button.gameObject); });
                }
            }
        }



        // Deletes ALL saves. You may or may not want a button doing this in your game
        public void DeleteSavesButtonClicked()
        {
            SaveManager.DeleteAllSaves();

            // Reload the scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }



        public void NewGameButtonClicked()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(newgame_scene_to_load);
        }
    }
}