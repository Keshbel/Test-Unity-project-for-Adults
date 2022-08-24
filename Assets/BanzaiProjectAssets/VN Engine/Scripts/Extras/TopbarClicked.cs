using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VNEngine
{
    public class TopbarClicked : MonoBehaviour
    {
        public void ItemsButtonClicked()
        {
            // Turn off item menu
            if (UIManager.ui_manager.item_menu.activeSelf)
                UIManager.ui_manager.item_menu.SetActive(false);
            // Turn on menu
            else
            {
                UIManager.ui_manager.item_menu.SetActive(true);
                UIManager.ui_manager.log_menu.SetActive(false);
            }
        }


        public void LogButtonClicked()
        {
            // Turn off log menu
            if (UIManager.ui_manager.log_menu.activeSelf)
                UIManager.ui_manager.log_menu.SetActive(false);
            // Turn on menu
            else
            {
                UIManager.ui_manager.log_menu.SetActive(true);
                UIManager.ui_manager.item_menu.SetActive(false);
                UIManager.ui_manager.scroll_log_rect.transform.parent.gameObject.SetActive(false);
                Canvas.ForceUpdateCanvases();
                UIManager.ui_manager.scroll_log_rect.verticalNormalizedPosition = 1;
                Canvas.ForceUpdateCanvases();
            }
        }
    }
}