using System;
using System.Collections;
using Coffee.UIExtensions;
using Core.Entity;
using Core.Manager.Configs;
using DG.Tweening;
using Entity;
using Frameworks.Base;
using Frameworks.UIPopup;
using Manager;
using Manager.Configs;
using MoreMountains.Feedbacks;
using Runtime.Manager;
using TMPro;
using UI.Popups.ClassParameter;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace InGame
{
    public class SpellCardVo : MonoBehaviour
    {
        [SerializeField] private GameObject objPack;
        [SerializeField] private GameObject objShadow;
        [SerializeField] private GameObject objOpenZone;
        [SerializeField] private GameObject objBuy;
        [SerializeField] private Button btnBuy;
        [SerializeField] private Image imgPlanetPack;
        [SerializeField] public RectTransform rectTransInfoPosition;
        [SerializeField] private MMF_Player mmPlayerEffectSelect;
        [SerializeField] private MMF_Player mmPlayerEffectUnSelect;
        [SerializeField] private MMF_Player mmPlayerEffectShake;
        private ConfigSpellCardRecord _data;
        private Vector3 _originalPosition;
        private bool _isSelect;
        private ShopController _shopController;
        private Action<SpellCardVo> _callback;
        private Action _isDoneCallback;
        public ConfigSpellCardRecord Data => _data;
        public void OnInit(Action<SpellCardVo> callback)
        {
            _callback = callback;
        }
        public void SetData(ConfigSpellCardRecord data)
        {
            _data = data;
            var image = SpritesManager.Instance.GetSpriteSpellCardById(data.indexSprite);
            imgPlanetPack.sprite = image;
            objBuy.SetActive(false);
            btnBuy.interactable = false;
        }
        public void OnClick()
        {
            _callback?.Invoke(this);
        }
        public void OnCallBackWhenDoneEffect(Action isDone)
        {
            _isDoneCallback = isDone;
        }
        public void OnClickUse()
        {
            if (ShopController.Instance.OnApplySpellCard(_data.id))
            {
                mmPlayerEffectShake.PlayFeedbacks();
                objBuy.SetActive(false);
                btnBuy.interactable = false;
            }
            else
            {
                UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupNotificationBox, new NotificationParameter()
                {
                    TextNotification = "Can not convert",
                    IconNotificationEnum = IconNotificationEnum.Warning,
                });
            }
        }
        public void IsDoneEffect()
        {
            _isDoneCallback?.Invoke();
            this.gameObject.SetActive(false);
        }
        public void OnPlayEffectSelectCard()
        {
            mmPlayerEffectSelect.PlayFeedbacks();
            objBuy.SetActive(true);
            btnBuy.interactable = true;
        }
        public void OnPlayEffectUnSelectCard()
        {
            mmPlayerEffectUnSelect.PlayFeedbacks();
            objBuy.SetActive(false);
            btnBuy.interactable = false;
        }
        
    }
}