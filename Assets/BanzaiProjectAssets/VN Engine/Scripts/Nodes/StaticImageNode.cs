using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
    // Used to place images on the screen (not actors)
    public class StaticImageNode : Node
    {
        public enum Image_Action { create_image, delete_image };

        public enum Image_Size { native_size, fit_whole_screen, custom_size };

        public Image_Action image_action = Image_Action.create_image;

        // Creating image variables
        public Sprite image_to_place;
        public bool place_in_foreground = true;
        public Image_Size size_option = Image_Size.native_size;
        public Vector2 custom_image_size;
        public Vector2 image_coordinates = new Vector2(0f, 0f);   // 0,0, being center, -0.5,-0.5 being bottom left, 0.5,0.5 being top right
        public bool fade_in_new_image = true;
        public float fade_in_time = 1.0f;

        // Deleting images variables
        public string image_to_delete;
        public bool fade_out_image;
        public float fade_out_time = 0.5f;


        public override void Run_Node()
        {
            // If deleting the image, find the image and delete it
            switch (image_action)
            {
                case Image_Action.create_image:
                    CreateImage();
                    break;
                case Image_Action.delete_image:
                    DeleteImage();
                    break;
            }

            Finish_Node();
        }


        public void CreateImage()
        {
            if (image_to_place == null)
            {
                Debug.LogError("image_to_place is empty in StaticImageNode. Make sure to fill that field in.", this.gameObject);
                Finish_Node();
                return;
            }

            // Create a new gameobject
            GameObject go = new GameObject(image_to_place.name);

            if (place_in_foreground)
            {
                go.transform.SetParent(UIManager.ui_manager.foreground_static_image_parent);
            }
            else
            {
                go.transform.SetParent(UIManager.ui_manager.background_static_image_parent);
            }

            go.transform.localScale = Vector3.one;
            Image im = go.AddComponent<Image>();
            im.sprite = image_to_place;
            SaveManager.SetSaveFeature(this, go);

            // Set the size of the image
            switch (size_option)
            {
                case Image_Size.native_size:
                    im.SetNativeSize();
                    // 0,0, being center, -0.5,-0.5 being bottom left, 0.5,0.5 being top right
                    go.transform.localPosition = new Vector3(image_coordinates.x * UIManager.ui_manager.canvas_width,
                                                             image_coordinates.y * UIManager.ui_manager.canvas_height,
                                                             1);
                    break;
                case Image_Size.fit_whole_screen:
                    im.rectTransform.anchorMin = new Vector2(0, 0);
                    im.rectTransform.anchorMax = new Vector2(1, 1);
                    im.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    im.rectTransform.sizeDelta = Vector2.zero;
                    im.rectTransform.localPosition = Vector2.zero;
                    break;
                case Image_Size.custom_size:
                    go.transform.localPosition = new Vector3(image_coordinates.x * UIManager.ui_manager.canvas_width,
                                                             image_coordinates.y * UIManager.ui_manager.canvas_height,
                                                             1);
                    im.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    im.rectTransform.sizeDelta = custom_image_size;
                    break;
            }

            if (fade_in_new_image)
            {
                StartCoroutine(UIManager.ui_manager.Fade_In_Image(im, fade_in_time));
            }
        }


        public void DeleteImage()
        {
            Transform t = UIManager.ui_manager.foreground_static_image_parent.Find(image_to_delete);
            if (t == null)
                t = UIManager.ui_manager.background_static_image_parent.Find(image_to_delete);

            if (t != null)
            {
                Destroy(t.GetComponent<FeatureToSave>());

                if (!fade_out_image)
                    Destroy(t);
                else
                {
                    Image i = t.GetComponent<Image>();
                    if (i != null)
                        StartCoroutine(UIManager.ui_manager.Fade_Out_Image(i, fade_out_time, true));
                    else
                        Debug.LogError(image_to_delete + "Object does not have image component to fade out", this.gameObject);
                }
            }
            else
            {
                Debug.LogError("Could not find Static Image " + image_to_delete, this.gameObject);
            }
        }


        public override void Stop_Node()
        {

        }
    }
}