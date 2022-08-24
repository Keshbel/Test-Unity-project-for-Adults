using UnityEngine;
using UnityEngine.UI;

namespace VNEngine
{
    public class SwapImageSprites : MonoBehaviour
    {
        private Sprite normal_sprite;
        public Sprite swapped_sprite;


        void Awake()
        {
            normal_sprite = this.GetComponent<Image>().sprite;
        }


        public void Swap_Sprites()
        {
            if (this.GetComponent<Image>().sprite == normal_sprite)
                this.GetComponent<Image>().sprite = swapped_sprite;
            else
                this.GetComponent<Image>().sprite = normal_sprite;
        }
    }
}