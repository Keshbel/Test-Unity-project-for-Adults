using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VNEngine
{
    // To be placed on all items. When clicked on, brings up the large image and description.
    // Must be placed a UI button
    [RequireComponent(typeof(Button))]
    public class Item : MonoBehaviour
    {
        public string Item_Name;
        public Sprite Large_Image;
        [TextArea(5, 10)]
        public string Description;

        void Start()
        {
            // Add listeners to button
            Button b = this.GetComponent<Button>();
            b.onClick.AddListener(() => UIManager.ui_manager.item_description.text = Description);
            b.onClick.AddListener(() => UIManager.ui_manager.large_item_image.sprite = Large_Image);
        }

        // Simulates a click of the user
        public void ClickItem()
        {
            this.GetComponent<Button>().onClick.Invoke();
        }
    }
}