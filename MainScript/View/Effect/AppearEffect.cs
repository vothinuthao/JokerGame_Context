
using DG.Tweening;
using TMPro;
using UnityEngine;

public class AppearEffect : MonoBehaviour
{
    void OnEnable()
    {
        this.gameObject.transform.DOLocalMoveY(Screen.height / 2, 1f).SetEase(Ease.OutBounce).OnComplete(OnHideComplete);

    }
    void OnAnimationComplete()
    {
        // textMesh.rectTransform.DOFade(0, 1f).SetDelay(1f).OnComplete(OnHideComplete);
        
    }

    void OnHideComplete()
    {
        this.gameObject.SetActive(false);
    }
}
