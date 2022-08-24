using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
    public class SetBackground : Node
    {
        public Sprite sprite;
        public bool set_foreground;
        public bool fade_out;   //  Fades out, from white to black
        public bool fade_in;        // Set the background to black immediately, then fade in the background image

        public override void Run_Node()
        {
            activateImage();    // Ensure whatever image we're operating on is active

            if (!fade_in && !fade_out)// && !fade_out_then_in_background)
            {
                // Simply set background or foreground and end if we aren't fading
                if (!set_foreground)
                {
                    UIManager.ui_manager.background.sprite = sprite;
                    UIManager.ui_manager.background.color = Color.white;
                }
                else
                {
                    UIManager.ui_manager.foreground.sprite = sprite;
                    UIManager.ui_manager.foreground.color = Color.white;
                }

                Finish_Node();
            }
            else
            {
                if (fade_out && fade_in)
                    StartCoroutine(Fade_Out_Then_In_Coroutine(2.0f));
                else if (fade_in)
                    StartCoroutine(Fade_In_Coroutine(2.0f));
                else if (fade_out)
                    StartCoroutine(Fade_Out_Coroutine(2.0f));
            }


            // Ensure we save the foreground or background in case the user Saves and Loads the game later
            if (set_foreground)
                SaveManager.SetSaveFeature(this, UIManager.ui_manager.foreground.gameObject);
            else
                SaveManager.SetSaveFeature(this, UIManager.ui_manager.background.gameObject);
        }


        // In case the foreground or background is disabled, activate it
        private void activateImage()
        {
            if (set_foreground)
                UIManager.ui_manager.foreground.gameObject.SetActive(true);
            else
                UIManager.ui_manager.background.gameObject.SetActive(true);
        }
        private Image getImage()
        {
            if (set_foreground)
                return UIManager.ui_manager.foreground;
            else
                return UIManager.ui_manager.background;
        }


        IEnumerator Fade_Out_Then_In_Coroutine(float over_time)
        {
            float value = 0;
            over_time /= 2;
            // Set it to completely transparent
            Image img = getImage();
            img.color = Color.white;

            // Fade the image to black
            while (img.color != Color.black)
            {
                value += over_time / 100f;
                img.color = Color.Lerp(Color.white, Color.black, value);
                yield return new WaitForSeconds(over_time / 100);
            }

            // Change the image while it's black and you can't tell it's being changed
            img.sprite = sprite;
            img.color = Color.black;

            // Fade image from black to white so we can see it again
            value = 0;
            while (img.color != Color.white)
            {
                value += over_time / 100f;
                img.color = Color.Lerp(Color.black, Color.white, value);
                yield return new WaitForSeconds(over_time / 100);
            }

            Finish_Node();
            yield break;
        }


        IEnumerator Fade_In_Coroutine(float over_time)
        {
            Image img = getImage();

            // Change the background image and set it to black, and have it fade into white
            img.sprite = sprite;
            img.color = Color.black;

            // Fade the image from black to white, so we can see it
            float value = 0;
            while (img.color != Color.white)
            {
                value += over_time / 100f;
                img.color = Color.Lerp(Color.black, Color.white, value);
                yield return new WaitForSeconds(over_time / 100);
            }
            Debug.Log("FADDE");
            Finish_Node();
            yield break;
        }


        // Fades to black
        IEnumerator Fade_Out_Coroutine(float over_time)
        {
            float value = 0;

            // Set it to completely transparent
            Image img = getImage();
            img.color = Color.white;

            // Fade the image to black
            while (img.color != Color.black)
            {
                value += over_time / 100f;
                img.color = Color.Lerp(Color.white, Color.black, value);
                yield return new WaitForSeconds(over_time / 100);
            }

            Finish_Node();
            yield break;
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