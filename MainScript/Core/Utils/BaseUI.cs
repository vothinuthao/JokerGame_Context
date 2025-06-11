using System.Collections;
using UnityEngine;
using DG.Tweening;
using Frameworks.Scripts;
using Manager;
using UnityEngine.Events;

public class BaseUI : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup;
    [Tooltip("type popup")]
    public bool isPopup;
    [Tooltip("On off control with on off popup")]
    [SerializeField] bool isChangeControl;
    [Tooltip("show popup while begin")]
    [SerializeField] bool isShow;
    [Tooltip("auto hide popup")]
    [SerializeField] bool isAutoHide;
    [Tooltip("sound while open popup")]
    [SerializeField] bool soundOpen;
    [Tooltip("sound while hide popup")]
    [SerializeField] bool soundClose;
    [Tooltip("assign object if want anim scale")]
    [SerializeField] GameObject PopupShow;
    [SerializeField] UnityEvent eventShow;
    [SerializeField] UnityEvent eventHide;
    Coroutine coroutine_autoHide;
    Vector3 originScalePopup;
    [Header("anim popup slide")]
    [SerializeField] GameObject popupSlide;
    [SerializeField] float fromY, toY;
    void Awake()
    {
        if (PopupShow != null)
        {
            originScalePopup = PopupShow.transform.localScale;
        }

    }
    private void Start()
    {
        if (!isShow)
            HideView();
        else
            ShowView(false);
    }
    public void ShowView(bool smooth = true)
    {
        // if (GameManager.instance != null && isChangeControl)
        //     GameManager.instance.pauseControl = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        if (smooth)
            canvasGroup.DOFade(1, .5f);
        else
            canvasGroup.alpha = 1;
        eventShow?.Invoke();

        if (isAutoHide)
        {
            if (coroutine_autoHide != null)
                StopCoroutine(coroutine_autoHide);
            coroutine_autoHide = StartCoroutine(AutoHide());
        }
        if (PopupShow != null)
        {
            PopupShow.transform.localScale = originScalePopup;
            // PopupShow.transform.DOScale(originScalePopup, .5f).SetEase(Ease.OutElastic);
        }
        if (popupSlide != null)
        {
            popupSlide.GetComponent<RectTransform>().DOAnchorPosY(fromY, 0);
            popupSlide.GetComponent<RectTransform>().DOAnchorPosY(toY, .5f);
        }
       
    }
    // ReSharper disable Unity.PerformanceAnalysis
    public void HideView(bool smooth = true)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        if (smooth)
            canvasGroup.DOFade(0, .5f);
        else
            canvasGroup.alpha = 0;
        eventShow?.Invoke();
    }
    IEnumerator AutoHide()
    {
        yield return new WaitForSeconds(2f);
        HideView(true);
    }
}
