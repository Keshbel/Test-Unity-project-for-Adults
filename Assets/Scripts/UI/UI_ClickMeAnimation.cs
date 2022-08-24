using System.Collections;
using DG.Tweening;
using UnityEngine;

public class UI_ClickMeAnimation : MonoBehaviour
{
    public float duration = 5f;
    private void Start()
    {
        StartCoroutine(InfinityScaling());
    }

    public IEnumerator InfinityScaling()
    {
        while (true)
        {
            gameObject.transform.DOPunchScale(new Vector3(0.3f,0.3f,0.3f), duration, 1);
            yield return new WaitForSeconds(duration);
        }
    }
}
