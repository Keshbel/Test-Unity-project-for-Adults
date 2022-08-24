using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace VNEngine
{
    // Loads the specified scene. This should be the last component you want, as all conversations will be lost after this.
    public class LoadSceneNode : Node
    {
        public string level_to_load;

        public bool async_loading = false;  // If you want to use a loading screen, set this to true


        public override void Run_Node()
        {
            // Simply loads the specified scene
            Debug.Log("Switching level: " + level_to_load);

            Time.timeScale = 1;

            if (!async_loading)
                UnityEngine.SceneManagement.SceneManager.LoadScene(level_to_load);
            else
                StartCoroutine(Async_Load_Level());
        }


        IEnumerator Async_Load_Level()
        {
            UIManager.ui_manager.loading_icon.SetActive(true);
            UIManager.ui_manager.loading_text.gameObject.SetActive(true);
            string active_scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            DestroyImmediate(UnityEngine.EventSystems.EventSystem.current.gameObject);

            AsyncOperation AO = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(level_to_load, UnityEngine.SceneManagement.LoadSceneMode.Additive);
            AO.allowSceneActivation = false;
            int progress = (int)(AO.progress * 100f);
            while (AO.progress < 0.9f)
            {
                progress = Mathf.Max(progress, (int)(AO.progress * 100f));
                UIManager.ui_manager.loading_text.text = "Loading... " + progress + "%";
                yield return null;
            }
            AO.allowSceneActivation = true;
            while (AO.progress < 1f)
            {
                progress = Mathf.Max(progress, (int)(AO.progress * 100f));
                UIManager.ui_manager.loading_text.text = "Loading... " + progress + "%";
                yield return null;
            }

            yield return 0;

            UIManager.ui_manager.loading_icon.SetActive(false);
            UIManager.ui_manager.loading_text.gameObject.SetActive(false);

            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(active_scene);
            Debug.Log("Done Async loading & switching to level: " + level_to_load);
        }


        public override void Button_Pressed()
        {

        }


        public override void Finish_Node()
        {
            StopAllCoroutines();

            base.Finish_Node();
        }
    }
}