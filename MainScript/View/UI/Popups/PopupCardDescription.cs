using System;
using System.Collections.Generic;
using Core.Controller.Observer;
using Core.Manager.Configs;
using Core.Observer;
using Frameworks.Scripts;
using Frameworks.UIButton;
using Lean.Touch;
using Manager;
using Manager.Configs;
using TMPro;
using UI.BaseColor;
using UnityEngine;

namespace UI.Popups
{
    [Serializable]
    public class GameObjectByRarity
    {
        public RarityEnum enumGameObject;
        public GameObject objectByRarity;
            
    }
    public class PopupCardDescription : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransPopup;
        [SerializeField] private TextMeshProUGUI txtName;
        [SerializeField] private TextMeshProUGUI txtDes;
        [SerializeField] private TextMeshProUGUI txtPayJoker;
        [SerializeField] private UIButton btnSellJoker;
        [SerializeField] private List<GameObjectByRarity> listObjectByRarity;
        [SerializeField] private GameObject objIAP;
        [SerializeField] private bool isClickToHide;
        
        private RectTransform _buttonBuySize;
        private Action<bool> _isSellCallback = default;
        private Action _isHidePopup = default;
        private JokerCardSO.JokerCardSo _data;
        private ConfigBoosterPackRecord _dataBoosterPack;
        private ConfigIAPPackRecord _dataIAPPack;
        private Camera _camera;
        public RectTransform RectTransPopup => rectTransPopup;

        private void Start()
        {
            _camera = Camera.main;
        }
        private void OnEnable()
        {
            LeanTouch.OnFingerDown += HandleFingerDown;
            EventDispatcher.Instance?.RegisterListener(EventID.OnChangeStateGamePlay, OnChangeStateGamePlay);
        }

        private void OnDisable()
        {
            LeanTouch.OnFingerDown -= HandleFingerDown;
            EventDispatcher.Instance?.RemoveListener(EventID.OnChangeStateGamePlay, OnChangeStateGamePlay);
        }

        public void OnClickHide()
        {
            rectTransPopup.gameObject.SetActive(false);
            _isHidePopup?.Invoke();
            // AudioManager.Instance.PlaySFX(AudioName.BackOrCancel);
        }
        public void OnSetData(JokerCardSO.JokerCardSo data, Action<bool> isSellCallback = default, Action isHidePopup = default)
        {
            _data = data;
            txtName.text = data.JokerName;
            txtDes.text = JokerCardController.Instance.ShowVariableJoker(data);
            if(txtPayJoker != null) txtPayJoker.gameObject.SetActive(false);
            if (listObjectByRarity != null) 
            {
                foreach (var obj in listObjectByRarity)
                {
                    obj.objectByRarity.SetActive(obj.enumGameObject == data.Rarity);
                }
            }
            _isSellCallback = isSellCallback;
            _isHidePopup = isHidePopup;
            AudioManager.Instance.PlaySFX(AudioName.SoundClick);
            if(objIAP != null) objIAP.SetActive(false);
        }
        public void OnSetDataBoosterPack(ConfigBoosterPackRecord data)
        {
            _dataBoosterPack = data;
            txtName.text = data.name;
            txtDes.text = ColorSchemeManager.Instance.ConvertColorTextFromSymbol(data.description);
            if(objIAP != null) objIAP.SetActive(false);
            if (listObjectByRarity != null)
            {
                foreach (var obj in listObjectByRarity)
                {
                    obj.objectByRarity.SetActive(false);
                }
            }
        }
        public void OnSetDataIAPPack(ConfigIAPPackRecord data)
        {
            _dataIAPPack = data;
            txtName.text = data.name;
            txtDes.text = ColorSchemeManager.Instance.ConvertColorTextFromSymbol(data.description);
            listObjectByRarity.ForEach(x =>x.objectByRarity.SetActive(false));
            if(objIAP != null) objIAP.SetActive(true);
        }
        public void OnSetDataBoosterPackVo(string nameVo, string desVo, RarityEnum rarityEnum = RarityEnum.None)
        {
            txtName.text = nameVo;
            txtDes.text = ColorSchemeManager.Instance.ConvertColorTextFromSymbol(desVo);
            if(objIAP != null) objIAP.SetActive(false);
            if (rarityEnum != RarityEnum.None && listObjectByRarity != null)
            {
                foreach (var obj in listObjectByRarity)    obj.objectByRarity.SetActive(obj.enumGameObject == rarityEnum);
            }
        }
        public void OnSetDataPurchase()
        {
            txtPayJoker.gameObject.SetActive(true);
            txtPayJoker.text = $"Sell {_data.SellCost}$";
        }
        public void SetDataDefault(string nameCard, string description)
        {
            txtName.text = nameCard;
            txtDes.text = ColorSchemeManager.Instance.ConvertColorTextFromSymbol(description);
        }
        public void OnClickSell()
        {
            PlayerDataManager.Instance.Property.AddDollar(_data.SellCost);
            OnClickHide();
            AnalyticsManager.LogEarnCurrency(TypeCurrency.Dollar,_data.SellCost, "Earn_From_Sell_Joker");
            _isSellCallback?.Invoke(true);
            AudioManager.Instance.PlaySFX(AudioName.Coin2);
        }
        private void OnChangeStateGamePlay(object obj)
        {
            MessageStateGameplay msg = (MessageStateGameplay)obj;
            GameState state = msg.GameState;
            btnSellJoker.Interactable = state == GameState.PlayerTurn;
        }
        
        private void HandleFingerDown(LeanFinger finger)
        {
            if(!isClickToHide) return;
            if (this.gameObject.activeSelf)
            {
                OnClickHide();
            }

        }
    }
}
