using System;
using System.Collections;
using Coffee.UIExtensions;
using DG.Tweening;
using Frameworks.Base;
using Frameworks.Scripts;
using Frameworks.UIPopup;
using Frameworks.Utils;
using Manager;
using MoreMountains.Feedbacks;
using Runtime.Manager;
using TMPro;
using UI.Messages;
using UI.Popups.ClassParameter;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace InGame
{
    public class ShopJokerCardVo : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransCard;
        [SerializeField] private Image imgJoker;
        [SerializeField] private GameObject objCost;
        [SerializeField] private TextMeshProUGUI txtCost;
        [SerializeField] private GameObject objAllButton;
        [SerializeField] private Button btnButtonBuy;
        [SerializeField] private Button btnButtonAds;
        [SerializeField] private GameObject objShadow;
        [SerializeField] private MMF_Player mmPlayerEffectSelect;
        [SerializeField] private MMF_Player mmPlayerEffectUnSelect;
        [SerializeField] private MMF_Player mmPlayerEffectShake;
        [SerializeField] private MMF_Player mmPlayerEffectAddNewCard;
        private JokerCardSO.JokerCardSo _data;
        private Action<ShopJokerCardVo> _callback;
        private Action _callbackSold;
        private Action _isDoneCallback;
        public JokerCardSO.JokerCardSo JokerCardData => _data;
        // public RectTransform RectButtonBuy=> objAllButton.transform as RectTransform;
        private int _indexJokerInShop;
        public int IndexJokerCardInShop => _indexJokerInShop;
        public RectTransform RectTransformCard => rectTransCard;
        private void Start()
        {
            objCost.SetActive(false);
            objShadow.SetActive(false);
        }
        public void OnInit(Action<ShopJokerCardVo> callback, Action callbackSold = default)
        {
            _callback = callback;
            _callbackSold = callbackSold;
        }
        public void CallBackDoneSelectCard(Action isDoneCallback)
        {
            _isDoneCallback = isDoneCallback;
        }
        public void SetData(JokerCardSO.JokerCardSo data, int index = 0)
        {
            if (data == null) return;
            _data = data;
            imgJoker.sprite = _data.ImageCard;
            _indexJokerInShop = index;
            txtCost.text = $"${_data.BuyCost}";
            if(btnButtonAds != null)
                btnButtonAds.gameObject.SetActive((_indexJokerInShop == PlayerDataManager.Instance.ShopProperty.JokerOnShop.Count - 1)  && PlayerDataManager.Instance.Property.RoundWatchAds == 5);
            btnButtonBuy.gameObject.SetActive(_indexJokerInShop != PlayerDataManager.Instance.ShopProperty.JokerOnShop.Count - 1);
            var checkCost = ShopController.Instance.ValidateCostToBuy(_data.ID);
            btnButtonBuy.interactable = checkCost;
        }
        public void OnClick()
        {
            // OnPlayEffectSelectCard();
            _callback?.Invoke(this);
        }
        public void OnclickBuy()
        {
            var checkCost = ShopController.Instance.ValidateCostToBuy(_data.ID);
            if (checkCost)
            {
                var checkSlot = PlayerDataManager.Instance.Property.CheckSlot();
                if (!checkSlot)
                {
                    UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupNotificationBox, new NotificationParameter()
                    {
                        TextNotification = "Full Joker slot",
                        IconNotificationEnum = IconNotificationEnum.Warning,
                    });
                    return;
                }
                ShopController.Instance.BuyAndResetListJoker(_data);
                if (PlayerDataManager.Instance.Property.SubDolla(_data.BuyCost))
                {
                    StartCoroutine(HideText());
                    gameObject.SetActive(false);
                    ShopController.Instance.RemoveJokerCardInShop(_data);
                    PlayerDataManager.Instance.ShopProperty.RemoveIdJokerShop(_data.ID);
                    // _callback?.Invoke(this);
                    AnalyticsManager.LogSpendCurrency(TypeCurrency.Dollar,_data.BuyCost, "Spend_From_Buy_Joker");
                    AudioManager.Instance.PlaySFX(AudioName.Gold_Add);
                    _callbackSold?.Invoke();
                }
            }
            else
            {
                UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupNotificationBox, new NotificationParameter()
                {
                    TextNotification = "Not enough $!",
                    IconNotificationEnum = IconNotificationEnum.Warning,
                });
            }
        }
        public void FinishEffect()
        {
            _callback?.Invoke(this);
            this.gameObject.SetActive(false);
        }
        public void OnClickUse()
        {
           JokerCardController.Instance.AddJokerCardToInventory(_data);
           mmPlayerEffectAddNewCard.PlayFeedbacks();
        }

        public void OnClickAds()
        {
            if (PlayerDataManager.Instance.Property.RemoveAds)
            {
                JokerFreeByAds();
                return;
            }
            if (IronSourceManager.Instance.IsRewardedAdsAvailable(RewardedAdsPlacement.Joker_Free))
            {
                IronSourceManager.Instance.ShowRewardedVideo(RewardedAdsPlacement.Joker_Free, (success) =>
                {
                    if (!success) return;
                    JokerFreeByAds();
                });
            }
            else
            {
                Debug.LogError("Ads not available, please try later!");
            }
        }

        private void JokerFreeByAds()
        {
            ShopController.Instance.BuyAndResetListJoker(_data);
            StartCoroutine(HideText());
            gameObject.SetActive(false);
            ShopController.Instance.RemoveJokerCardInShop(_data);
            PlayerDataManager.Instance.ShopProperty.RemoveIdJokerShop(_data.ID);
            // _callback?.Invoke(this);
            AnalyticsManager.LogSpendCurrency(TypeCurrency.Dollar,_data.BuyCost, "Spend_From_Buy_Joker");
            AudioManager.Instance.PlaySFX(AudioName.Gold_Add);
            _callbackSold?.Invoke();
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupNotificationBox, new NotificationParameter()
            {
                TextNotification = "Congratulations! your Shop has been joker",
                IconNotificationEnum = IconNotificationEnum.Warning,
            });
            PlayerDataManager.Instance.Property.ResetWatchAds();
        }
        public void OnCallBackChooseJokerCard()
        {
            _isDoneCallback?.Invoke();
        }
        public void OnPlayEffectSelectCard()
        {
            // if( ShopController.Instance.JokerSelected == this)
            //     return;
            mmPlayerEffectSelect.PlayFeedbacks();
            objCost.SetActive(true);
            btnButtonBuy.interactable = true;
            // canvasControl.sortingOrder = 200;
        }
        // ReSharper disable Unity.PerformanceAnalysis
        public void OnPlayEffectUnSelectCard()
        {
            mmPlayerEffectUnSelect.PlayFeedbacks();
            objCost.SetActive(false);
            btnButtonBuy.interactable = false;
            // canvasControl.sortingOrder = 10;
        }
        IEnumerator HideText()
        {
            yield return new WaitForSeconds(1f);
        }
    }
}