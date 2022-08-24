using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ChangeAdultAnimation : MonoBehaviour
{
    public SkeletonAnimation adultAnimation;
    
    public int idAnimation = 1;

    public TMP_Text numberAnimationText;

    public Button buttonNext;
    public Button buttonPrev;
    
    // Start is called before the first frame update
    private void Start()
    {
        //подписка методов на кнопки
        if (buttonNext)
            buttonNext.onClick.AddListener(ChangeAnimationNext);
        if (buttonPrev)
            buttonPrev.onClick.AddListener(ChangeAnimationPrev);
    }

    private void ChangeAnimationNext() //следующая анимация
    {
        idAnimation++;

        CheckLimiter();

        UpdateInfo();
    }
    
    private void ChangeAnimationPrev() //предыдуща анимация
    {
        idAnimation--;

        CheckLimiter();

        UpdateInfo();
    }

    private void CheckLimiter() //проверка на выход за границы и зацикленность
    {
        if (idAnimation < 1)
            idAnimation = 5;
        if (idAnimation > 5)
            idAnimation = 1;
    }

    private void UpdateInfo() //обновление информации
    {
        adultAnimation.AnimationName = idAnimation.ToString();
        numberAnimationText.text = idAnimation.ToString();
    }
}
