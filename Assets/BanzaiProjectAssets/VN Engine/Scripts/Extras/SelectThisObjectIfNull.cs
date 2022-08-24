using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace VNEngine
{
    public class SelectThisObjectIfNull : MonoBehaviour
    {
        void OnEnable()
        {
            StartCoroutine(SelectContinueButtonLater());
        }

        IEnumerator SelectContinueButtonLater()
        {
            yield return null;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(this.gameObject);
        }


        void Update()
        {
            if (EventSystem.current != null && (EventSystem.current.currentSelectedGameObject == null || !EventSystem.current.currentSelectedGameObject.activeInHierarchy))
            {
                EventSystem.current.SetSelectedGameObject(this.gameObject);
            }
        }
    }
}