using System;
using DG.Tweening;
using Frameworks.Base;
using Frameworks.Scripts;
using Frameworks.UIAlert;
using Frameworks.UIPopup;
using Frameworks.Utils;
using MoreMountains.Feedbacks;
using TMPro;
using UI.BaseColor;
using UI.Messages;
using UI.Popups;
using UI.Popups.ClassParameter;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class JokerCardVO : MonoBehaviour
{
   [SerializeField] private RectTransform _rectTransform;
   [SerializeField]
   private GameObject _objCardParent;
   [SerializeField]
   private GameObject objButtonBuy;
   [SerializeField]
   private Image _imgJokerCard;
   [SerializeField]
   private TextMeshProUGUI _txtPoint;
   [SerializeField]
   private TextMeshProUGUI txtPointChip;
   [SerializeField]
   private TextMeshProUGUI txtPointMult;
   [SerializeField]
   private TextMeshProUGUI txtPointGold;
   [SerializeField]
   private GameObject objPointChip;
   [SerializeField]
   private GameObject objPointMult;
   [SerializeField]
   private GameObject objPointGold; 
   [SerializeField]
   private PopupCardDescription objDescription;
   [SerializeField] private MMF_Player mmEffectSelect;
   [SerializeField] private MMF_Player mmfDissolve;
   private JokerCardSO.JokerCardSo _data;
   private Action<int> _callback;
   private Action<JokerCardVO> _callbackForShop;
   private Action _isDone;
   public bool CanActive { get; set; }

   public JokerCardSO.JokerCardSo JokerData => _data;
   public void OnInit(Action<int> callback)
   {
      _callback = callback;
      _txtPoint.gameObject.SetActive(false);
      objDescription.SetActive(false);
   }
   public void SetData(JokerCardSO.JokerCardSo data)
   {
      _data = data;
      if (_data != null)
      {
         var model = _data;
         _imgJokerCard.sprite = model.ImageCard;
         CanActive = true;
      }
     
      
   }
   public void OnShakeEffect(int addPoint, TypeEffectEnum typePoint)
   {
      _txtPoint.gameObject.SetActive(true);
      objPointChip.SetActive(false);
      objPointMult.SetActive(false);
      objPointGold.SetActive(false);
      switch (typePoint)
      {
         case TypeEffectEnum.None:
            objPointChip.SetActive(false);
            objPointMult.SetActive(false);
            objPointGold.SetActive(false);
            break;
         case TypeEffectEnum.Chip:
            txtPointChip.text = "+" + addPoint;
            objPointChip.SetActive(true);
            AudioManager.Instance.PlaySFXWithRandomPitch(AudioName.Chip1);
            break;
         case TypeEffectEnum.AddMult:
            txtPointMult.text = "+" + addPoint;
            objPointMult.SetActive(true);
            AudioManager.Instance.PlaySFXWithRandomPitch(AudioName.Chip2);
            break;
         case TypeEffectEnum.MultiplyMult:
            txtPointMult.text = "x" + addPoint;
            objPointMult.SetActive(true);
            AudioManager.Instance.PlaySFXWithRandomPitch(AudioName.Chip2);
            break;
         case TypeEffectEnum.Dollar:
            txtPointGold.text = "+" + addPoint;
            objPointGold.SetActive(true);
            break;
      }
    
      _txtPoint.text = addPoint.ToString();
      mmEffectSelect.PlayFeedbacks();
      _txtPoint.gameObject.SetActive(false);
      DOVirtual.DelayedCall(0.5f, () =>
      {
         _txtPoint.DOFade(0f, 0.2f).OnComplete(() =>
         {
            _txtPoint.gameObject.SetActive(false);
         });
      });
   }
   public void OnClickShowDescriptionOwner()
   {
      mmEffectSelect.PlayFeedbacks();
      objDescription.gameObject.SetActive(true);
      AudioManager.Instance.PlaySFX(AudioName.Card3);
      objDescription.OnSetData(JokerData, isSellCallback: (b) =>
      {
         if (b)
         {
            mmfDissolve.PlayFeedbacks();
            AudioManager.Instance.PlaySFX(AudioName.Chip1);
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupNotificationBox, new NotificationParameter()
            {
               TextNotification = $"Your Cost: +{_data.SellCost}",
               IconNotificationEnum = IconNotificationEnum.Warning,
            });
            JokerCardController.Instance.RemoveJokerCardFromInventory(JokerData);
            
         }
         
      } );
      objDescription.OnSetDataPurchase();
   }
   public void OnDesTroyCard()
   {
      JokerCardController.Instance.RemoveJokerCardFromInventory(_data);
   }
   
   public void DiscardEffect()
   {
      _rectTransform.DOAnchorPosY(_rectTransform.anchoredPosition.y + 100f, 1f)
         .SetEase(Ease.OutQuad)
         .OnComplete(()=>
         {
            mmfDissolve.PlayFeedbacks();
         });
   }

   public void HideCardJoker()
   {
      gameObject.SetActive(false);
      CanActive = false;
   }
}
