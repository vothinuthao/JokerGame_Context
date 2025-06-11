using System;
using Coffee.UIExtensions;
using Core.Controller.Observer;
using Core.Entity;
using Core.Manager;
using Core.Manager.Configs;
using Core.Observer;
using DG.Tweening;
using Entity;
using Frameworks.Base;
using Frameworks.Scripts;
using Frameworks.UIAlert;
using Frameworks.UIPopup;
using Frameworks.Utils;
using Manager;
using Runtime.Manager;
using TMPro;
using UI.Popups.ClassParameter;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace InGame
{
    public class BoosterPackVo : MonoBehaviour
    {
         public RectTransform rectTransformCard;
        [SerializeField] private GameObject objPack;
        [SerializeField] private GameObject objShadow;
        [SerializeField] private GameObject objCost;
        [SerializeField] private CanvasGroup objBuy;
        [SerializeField] private Image imgBoosterPack; 
        [SerializeField] private TextMeshProUGUI textCost;
        [SerializeField]
        private float moveUpDistance = 100f;
        [SerializeField]
        private float animationDuration = 0.1f;
        [SerializeField]
        private float fadeDuration = 0.2f;
        [SerializeField]
        private UnityEvent onShakeBoosterPack;
        [SerializeField]
        private UnityEvent onSelectBoosterPack;
        [SerializeField]
        private UnityEvent onUnSelectBoosterPack;

        
        private Action<BoosterPackVo> _callback;
        private ConfigBoosterPackRecord _data;
        private Vector3 _originalPosition;
        private bool _isSelect;
        private ShopController _shopController;
        public ConfigBoosterPackRecord Data => _data;
        private void Start()
        {
            _originalPosition = objPack.transform.localPosition; 
            objBuy.alpha = 0;
            objShadow.SetActive(true);
            _shopController = ShopController.Instance;
        }
        public void OnInit(Action<BoosterPackVo> callback)
        {
            _callback = callback;
        }
        public void SetData(ConfigBoosterPackRecord data)
        {
            _data = data;
            var randomImg = ConfigManager.configBoosterPack.RandomListImage(_data.id);
            Sprite sptPack = SpritesManager.Instance.GetSpriteBoosterPackById(randomImg);
            imgBoosterPack.sprite = sptPack;
            textCost.text = $"${_data.cost}";
        }
        public void OnClick()
        {
            _callback?.Invoke(this);
            _isSelect = !_isSelect;
            if (_isSelect)
            {
                OnSelect();
            }
            else
            {
                OnNotSelect();
            }
        }

        private void BoosterDestroys()
        {
            Destroy(this.gameObject);
            ShopController.Instance.RemoveBoosterPackVo(this);
        }

        public void OnClickBuy()
        {
            if (_shopController.BuyBoosterHandle(_data))
            {
                objCost.SetActive(false);
                objBuy.gameObject.SetActive(false);
                objShadow.SetActive(false);
                onShakeBoosterPack?.Invoke();
                AnalyticsManager.LogSpendCurrency(TypeCurrency.Dollar,_data.cost, "Spend_From_Buy_Consumable");
                PlayerDataManager.Instance.ShopProperty.RemoveIdBoosterPackShop(_data.id);
            }
            else
            {
                Debug.Log("You not enough Cost");
                UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupNotificationBox, new NotificationParameter()
                {
                    TextNotification = "You not enough Cost",
                    IconNotificationEnum = IconNotificationEnum.Warning,
                });
            }
        }
        
        public void OnSelect()
        {
            onSelectBoosterPack?.Invoke();
            objPack.transform.DOLocalMoveY(_originalPosition.y + moveUpDistance, animationDuration).OnComplete(() =>
            {
                objBuy.DOFade(1, fadeDuration);
            });
        }
        public void OnNotSelect()
        {
            objPack.transform.DOLocalMove(_originalPosition, animationDuration);
            onUnSelectBoosterPack?.Invoke();
            objBuy.DOFade(0, fadeDuration).OnComplete(() => 
            {
                // objCost.gameObject.SetActive(false);
            });
        }

        public void LoopAudio()
        {
            var randomP = Random.Range(1, 10);
            AudioManager.Instance.PlaySFXWithPitchChanger(AudioName.tarot1, randomP);
        }
        public void OnChangeSceneOpenPack()
        {
            this.PostEvent(EventID.OnShowOpenPackView, new MessagePack(){IsShow = true, DataPack = new ConfigBoosterPackRecord()
            {
                id = _data.id,
                name = _data.name,
                sizePackName = _data.sizePackName,
                sizePack = _data.sizePack,
                usabilityPack = _data.usabilityPack,
                imageList = _data.imageList,
                cost =_data.cost,
                typePack = _data.typePack,
                ratioPack = _data.ratioPack,
                description = _data.description
            }});
            BoosterDestroys();
        }
    }
}