using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
    public class SetBackgroundTransparent : Node
    {
        public Sprite sprite;
        public bool set_foreground; // Foreground sits in front of the background. By default we set the background.
        public bool fade_out;   //  Fades out, from white to black
        public bool fade_in;        // Set the background to black immediately, then fade in the background image
        public bool wait_for_all_fading = true;

        public override void Run_Node()
        {
            activateImage();    // Ensure whatever image we're operating on is active

            if (!fade_in && !fade_out)// && !fade_out_then_in_background)
            {
                // Simply set background and end if we aren't fading
                if (set_foreground)
                {
                    UIManager.ui_manager.foreground.sprite = getImage().sprite;
                    SaveManager.SetSaveFeature(this, UIManager.ui_manager.foreground.gameObject);
                }
                else
                {
                    UIManager.ui_manager.background.sprite = getImage().sprite;
                    SaveManager.SetSaveFeature(this, UIManager.ui_manager.background.gameObject);
                }

                Finish_Node();
				return;
            }
            else
            {
                if (fade_out && fade_in)
                    StartCoroutine(Fade_Out_Then_In_Coroutine(2.0f));
                else if (fade_in)
                {
                    StartCoroutine(Fade_In_Coroutine(2.0f));
                }
                else if (fade_out)
                    StartCoroutine(Fade_Out_Coroutine(2.0f));
            }

            if (!wait_for_all_fading)
                Finish_Node();
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
            Image img = (Image)getImage();
            img.color = Color.white;

            // Fade the image to completely transparent
            while (img.color.a != 0)
            {
                value += over_time / 100f;
                Color tmpColor = img.color;
                tmpColor.a = Mathf.Lerp(1, 0, value);
                img.color = tmpColor;
                yield return new WaitForSeconds(over_time / 100);
            }

            // Change the image while it's black and you can't tell it's being changed
            img.sprite = sprite;

            // Go from transparent to opaque (solid)
            value = 0;
            while (img.color.a != 1)
            {
                value += over_time / 100f;
                Color tmpColor = img.color;
                tmpColor.a = Mathf.Lerp(0, 1, value);
                img.color = tmpColor;
                yield return new WaitForSeconds(over_time / 100);
            }

            if (wait_for_all_fading)
                Finish_Node();
            yield break;
        }


        // Go from transparent to opaque
        IEnumerator Fade_In_Coroutine(float over_time)
        {
            Image img = getImage();

            img.sprite = sprite;
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0);

            float value = 0;
            while (img.color.a != 1)    // Don't stop until it's completely opaque
            {
                value += over_time / 100f;
                Color tmpColor = img.color;
                tmpColor.a = Mathf.Lerp(0, 1, value);
                img.color = tmpColor;
                yield return new WaitForSeconds(over_time / 100);
            }

            if (wait_for_all_fading)
                Finish_Node();
            yield break;
        }


        // Fades the image from opaque to transparent
        IEnumerator Fade_Out_Coroutine(float over_time)
        {
            // Set it to completely transparent
            Image img = getImage();
            img.color = new Color(img.color.r, img.color.g, img.color.b, 255);

            float value = 0;
            while (img.color.a != 0)    // Fade the image to completely transparent
            {
                value += over_time / 100f;
                Color tmpColor = img.color;
                tmpColor.a = Mathf.Lerp(1, 0, value);
                img.color = tmpColor;
                yield return new WaitForSeconds(over_time / 100);
            }

            if (wait_for_all_fading)
                Finish_Node();
            yield break;
        }



        public override void Button_Pressed()
        {

        }


        public override void Finish_Node()
        {
            //StopAllCoroutines();

            base.Finish_Node();
        }
    }
}