using UnityEngine;
using System.Collections;

namespace VNEngine
{
    public class LoadSaveButton : MonoBehaviour
    {
        public SaveFile save_to_load_when_clicked;


        void Start()
        {

        }


        public void LoadSave()
        {
            save_to_load_when_clicked.Load();
        }
    }
}