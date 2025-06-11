using System.Collections.Generic;
using System.Linq;
using Core.Controller.Observer;
using Core.Manager;
using Core.Manager.Configs;
using Core.Utils;
using Frameworks.Base;
using Frameworks.Scripts;
using Frameworks.UIButton;
using Frameworks.UIPopup;
using Frameworks.Utils;
using IAP;
using InGame;
using JokerCardSO;
using Manager;
using Manager.Configs;
using Sirenix.OdinInspector;
using TMPro;
using UI.Popups;
using UI.Popups.ClassParameter;
using UnityEngine;
using UnityEngine.Serialization;

namespace MainMenu
{
    public class ShopView : MonoBehaviour
    {
        [SerializeField] private NoAds noAds;
        [SerializeField] private GameObject panelWait;
        [FoldoutGroup("Player")]
        [SerializeField]
        private TextMeshProUGUI textCostOfPlayer;
        [FoldoutGroup("Joker Shop")]
        [SerializeField]
        private GameObject prefabJokerCard;
        [FoldoutGroup("Joker Shop")]
        [SerializeField]
        private Transform content;
        [FoldoutGroup("Joker Shop")]
        [SerializeField]
        private int baseCostToReRoll = 5;
        [FoldoutGroup("Joker Shop")]
        [SerializeField]
        private TextMeshProUGUI textCost;
        [FoldoutGroup("Joker Shop")]
        [SerializeField]
        private GameObject objHideTable;
        [FoldoutGroup("Popup Shop")]
        [SerializeField]
        private GameObject objInformationVocher;
        [FoldoutGroup("Popup Shop")]
        [SerializeField]
        private GameObject objInformationBooster1;
        [FoldoutGroup("Popup Shop")]
        [SerializeField]
        private GameObject objInformationBooster2;
        [FormerlySerializedAs("scriptPopupLeftRight")]
        [FoldoutGroup("Popup Shop")]
        [SerializeField]
        private PopupCardDescription PopupDescription;
        [FoldoutGroup("Popup Shop")]
        [SerializeField]
        private PopupCardDescription scriptPopupBoosterPack;
        [FoldoutGroup("Button Shop")]
        [SerializeField]
        private GameObject objButtonReroll;
        [FoldoutGroup("Button Shop")]
        [SerializeField]
        private UIButton uiBtnButtonReroll;
        [FoldoutGroup("IAPPack Shop")]
        [SerializeField] private Transform contentPack;
        [FoldoutGroup("IAPPack Shop")]
        [SerializeField] private GameObject prefabPack;
        
        [SerializeField] private BoosterPackView boosterPackView;
        private JokerCardSO.JokerCardSo _jokerOwnerSelected;
        private BoosterPackVo _boosterPackSelected;
        private ConfigIAPPackRecord _iapPackSelected;
        private int _timeToReRoll;
        private int _costToReRoll;
        private bool _showPopupFreeDollar = true;
        private void Start()
        {
            _showPopupFreeDollar = true;
            JokerCardController.Instance.OnInitJokerCard();
            ShopController.Instance.InitData();
            prefabJokerCard.SetActive(false);
            objHideTable.SetActive(false);
            panelWait.SetActive(false);
            prefabPack.SetActive(false);
            PopupDescription.gameObject.SetActive(false);
        }
        public void InitDataShop()
        {
            noAds.OnSetData();
            if (PlayerDataManager.Instance.Property.Round % 5 == 0 && _showPopupFreeDollar)
            {
                UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupBonusDollar, new MessageFreeDollar
                {
                    Callback = (success) =>
                    {
                        _showPopupFreeDollar = false;
                    }
                });
            }
            PopupDescription.gameObject.SetActive(false);
            OnRollNewJoker();
            boosterPackView.OnInitPack(OnUpdateBoosterPack);
            _costToReRoll = baseCostToReRoll;
            ShopController.Instance.InitAmountFreeReroll();
            OnShowIAPPack();
            OnUpdateUI();
        }
        private void OnUpdateBoosterPack(BoosterPackVo data)
        {
            OnShowPopupBoosterPackInformation(data);
        }
        private void OnShowIAPPack()
        {
            if (!PlayerDataManager.Instance.Property.BeginnerPack)
            {
                var getConfig = ConfigManager.configIAPPack.GetValueByID(2);
                GameObjectUtils.Instance.ClearAllChild(contentPack.gameObject);
                GameObject obj = GameObjectUtils.Instance.SpawnGameObject(contentPack, prefabPack);
                var script = obj.GetComponent<IAPPackVO>();
                script.OnInit(OnClickPack,PackBuy);
                script.SetData(getConfig);
                obj.SetActive(true);
            }
        }
        private void OnClickPack(ConfigIAPPackRecord data)
        {
            if (data != null)
            {
                PopupDescription.OnSetDataIAPPack(data);
                PopupDescription.RectTransPopup.gameObject.SetActive(true);
            }
            OnShowIAPPack();
        }
        private void PackBuy(ConfigIAPPackRecord data)
        {
            _iapPackSelected = data;
            panelWait.SetActive(true);
            if (data.id == 2)
            {
                if(PlayerDataManager.Instance.Property.BeginnerPack) return;
                IAPManager.instance.BuyProduct(IAPManager.ProductBegin, delegate
                {
                    var config = ConfigManager.configBoosterPack.GetPackByID(data.idPack);
                    PlayerDataManager.Instance.Property.CheckBeginnerPack();
                    OpenBoosterPack(config);
                    OnShowIAPPack();
                    AudioManager.Instance?.PlaySFX(AudioName.tarot1);
                    panelWait.SetActive(false);
                    GameObjectUtils.Instance.ClearAllChild(contentPack.gameObject);
                }, delegate
                {
                    UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupNotificationBox, new NotificationParameter()
                    {
                        TextNotification = "Purchase Fail!",
                        IconNotificationEnum = IconNotificationEnum.Warning,
                    });
                    panelWait.SetActive(false);
                });
                
            }
        }
        public void OnClickChangeView()
        {
            _showPopupFreeDollar = true;
            objHideTable.SetActive(true);
            ShopController.Instance.ClearBoosterPackVo();
            PlayerDataManager.Instance.ShopProperty.ClearDataBoosterPackOnShop();
            PlayerDataManager.Instance.ShopProperty.ClearDataJokerOnShop();
            ShopController.Instance.watchAdsTime = 0;
            if(  PlayerDataManager.Instance.Property.RoundWatchAds != 5)
                PlayerDataManager.Instance.Property.AddRoundWatchAds();
        }
        public void HideComplete()
        {
            objHideTable.SetActive(false);
            this.PostEvent(EventID.OnShowGamePlayView, new MessageActiveUI{IsShow = true});
        }
        private void OnEnable()
        {
            PlayerDataManager.Instance.Property.OnDollaChanged += OnDollarChanged;
        }
        private void OnDisable()
        {
            PlayerDataManager.Instance.Property.OnDollaChanged -= OnDollarChanged;
        }
        public void OnClickReroll(bool useCost)
        {
            if (useCost)
            {
                var checkCostPlayer = PlayerDataManager.Instance.Property.Dollar;
                if ( ShopController.Instance.AmountFreeReRoll == 0)
                {
                    if (_costToReRoll<= checkCostPlayer)
                    {
                        PlayerDataManager.Instance.Property.SubDolla(_costToReRoll);
                        _costToReRoll += 1;
                        PlayerDataManager.Instance.ShopProperty.ClearDataJokerOnShop();
                    }
                    else
                    {
                        Debug.Log("Not enough $!");
                        UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupNotificationBox, new NotificationParameter()
                        {
                            TextNotification = "Not enough $!",
                            IconNotificationEnum = IconNotificationEnum.Warning,
                        });
                        return;
                    }
                }
                else
                {
                    PlayerDataManager.Instance.ShopProperty.ClearDataJokerOnShop();
                    ShopController.Instance.SubReRoll();
                }
            }
            OnRollNewJoker();
        }
        public void OnWatchAds()
        {
            
            if (IronSourceManager.Instance.IsRewardedAdsAvailable(RewardedAdsPlacement.FreeReroll))
            {
                IronSourceManager.Instance.ShowRewardedVideo(RewardedAdsPlacement.FreeReroll, (success) =>
                {
                    if (!success) return;
                    PlayerDataManager.Instance.ShopProperty.ClearDataJokerOnShop();
                    ShopController.Instance.watchAdsTime += 1;
                    _costToReRoll += 1;
                    UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupNotificationBox, new NotificationParameter()
                    {
                        TextNotification = "Congratulations! your Shop has been rerolled",
                        IconNotificationEnum = IconNotificationEnum.Warning,
                    });
                    OnRollNewJoker();
                });
            }
            else
            {
                Debug.LogError("Ads not available, please try later!");
            }
            
        }
        private void OnRollNewJoker()
        {
            ShopController.Instance.ClearJokerCardInShop();
            GameObjectUtils.Instance.ClearAllChild(content.gameObject);
            List<JokerCardSo> listJokerCard = new List<JokerCardSo>();
            if (PlayerDataManager.Instance.ShopProperty.JokerOnShop.Count() != 0)
            {
                // listJokerCard = JokerCardController.Instance.Get
                foreach (var id in PlayerDataManager.Instance.ShopProperty.JokerOnShop)
                {
                    var joker = JokerCardController.Instance.GetJokerFromPool(id);
                    listJokerCard.Add(joker);
                }
            }
            else
            {
                listJokerCard = ShopController.Instance.RandomJokerCard(PlayerDataManager.Instance.Property.RoundWatchAds == 5 ? 3 : 2);
                listJokerCard.ForEach(x=>  PlayerDataManager.Instance.ShopProperty.SaveIdJokerShop(x.ID));
            }
            ShopController.Instance.ClearListScriptJokerCardInShop();
            for (var i = 0; i < listJokerCard.Count; i++)
            {
                var joker = listJokerCard[i];
                GameObject obj = GameObjectUtils.Instance.SpawnGameObject(content, prefabJokerCard);
                ShopJokerCardVo script = obj.GetComponent<ShopJokerCardVo>();
                script.OnInit(OnShowPopupInformation, () =>
                {
                    PopupDescription.SetActive(false);
                });
                script.SetData(joker, index:i);
                script.gameObject.SetActive(true);
                PlayerDataManager.Instance.ShopProperty.SaveIdJokerShop(joker.ID);
                ShopController.Instance.AddJokerVo(script);
            }

            OnUpdateUI();
        }
        private void OnUpdateUI()
        {
            var checkFree = ShopController.Instance.AmountFreeReRoll > 0 ? "Free" : _costToReRoll.ToString() + "$";
            textCost.text = $"{checkFree}";
            textCostOfPlayer.text = PlayerDataManager.Instance.Property.Dollar.ToString();
            
            var checkCostPlayer = PlayerDataManager.Instance.Property.Dollar;
            uiBtnButtonReroll.Interactable = _costToReRoll < checkCostPlayer; 
            objButtonReroll.SetActive(true);
        }
        private void OnDollarChanged(int dollar)
        {
            if (!gameObject.activeInHierarchy) return;
            OnUpdateUI();
        }
        private void OnShowPopupInformation(ShopJokerCardVo data)
        {
            var jokerSelected = ShopController.Instance.JokerSelected;
            if(jokerSelected!=null)
                jokerSelected.OnPlayEffectUnSelectCard();
            if (data != null)
            {
                ShopController.Instance.JokerSelected = data;
                PopupDescription.OnSetData(data.JokerCardData);// isHidePopup: data.OnPlayEffectUnSelectCard);
                PopupDescription.RectTransPopup.gameObject.SetActive(true);
                data.OnPlayEffectSelectCard();
            }
            else
            {
                jokerSelected.OnPlayEffectUnSelectCard();
            }
        }
        private void OnShowPopupBoosterPackInformation(BoosterPackVo data)
        {
            if(_boosterPackSelected!=null)
                _boosterPackSelected.OnNotSelect();
            if (data != null)
            {
                _boosterPackSelected = data;
                PopupDescription.OnSetDataBoosterPack(data.Data);
                PopupDescription.RectTransPopup.gameObject.SetActive(true);
            }
            else
            {
                PopupDescription.RectTransPopup.gameObject.SetActive(false);
            }
            
        }
        private void OpenBoosterPack(ConfigBoosterPackRecord data)
        {
            this.PostEvent(EventID.OnShowOpenPackView, new MessagePack(){IsShow = true, DataPack = new ConfigBoosterPackRecord()
            {
                id = data.id,
                name = data.name,
                sizePackName = data.sizePackName,
                sizePack = data.sizePack,
                usabilityPack = data.usabilityPack,
                imageList = data.imageList,
                cost =data.cost,
                typePack = data.typePack,
                ratioPack = data.ratioPack,
                description = data.description
            }});
        }
    }
}
