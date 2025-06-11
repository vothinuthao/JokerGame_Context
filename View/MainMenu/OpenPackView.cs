using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Controller.Observer;
using Core.Manager;
using Core.Manager.Configs;
using Core.Utils;
using DG.Tweening;
using Entity;
using Frameworks.Scripts;
using Frameworks.Utils;
using InGame;
using Manager.Configs;
using Sirenix.OdinInspector;
using TMPro;
using UI.Popups;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace MainMenu
{
    public class OpenPackView : MonoBehaviour
    {
        
        [SerializeField] private Transform allPrefabVo;
        [SerializeField] private Transform content;
        [SerializeField] private GridLayoutGroup gridContentLayoutGroup;
        [SerializeField] private PopupCardDescription scriptDescription;
        
        [FoldoutGroup("Joker Card")]
        [SerializeField] private GameObject prefabJokerCard;
        [FoldoutGroup("Joker Card")]
        [SerializeField] private Transform addCardJokerPlace;
        
        [FoldoutGroup("Spell Pack")]
        [SerializeField] private GameObject prefabSpellCard;
        [FoldoutGroup("Spell Pack")]
        [SerializeField] private Transform contentSpawn;
        [FoldoutGroup("Spell Pack")]
        [SerializeField] private Transform contentSpawnCardTemp;
        [FoldoutGroup("Spell Pack")]
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
         [FoldoutGroup("Spell Pack")]
        [SerializeField] private GameObject cardPrefab;
        [FoldoutGroup("Spell Pack")]
        [SerializeField] private PokerHandView pokerHandView;
        
        [FoldoutGroup("Planet Poker Card")]
        [SerializeField] private GameObject prefabBoosterPack;
        [FoldoutGroup("Planet Poker Card")]
        [SerializeField] private GameObject objLevelPlace;
        
        [FoldoutGroup("Standard Poker Card")]
        [SerializeField] private GameObject prefabPokerCard;
        [FoldoutGroup("Standard Poker Card")]
        [SerializeField] private Transform addCardPokerPlace;
        [FoldoutGroup("Standard Poker Card")]
        [SerializeField] private TextMeshProUGUI txtAmountIndeck;
        
        [FoldoutGroup("Information Card")]
        [SerializeField] private GameObject prefabInformation;
        [FoldoutGroup("Information Card")]
        [SerializeField] private GameObject objCountPack;
        [FoldoutGroup("Information Card")]
        [SerializeField] private TextMeshProUGUI txtNamePack;
        [FoldoutGroup("Information Card")]
        [SerializeField] private TextMeshProUGUI txtCountPack;

        private List<PokerCardVo> _listPokerPack = new List<PokerCardVo>();
        private List<ShopJokerCardVo> _listJokerCard = new List<ShopJokerCardVo>();
        private ConfigBoosterPackRecord _data;
        private ShopController _shopController;
        private int _countOpenPack = 0;
        private ShopJokerCardVo _jokerCardSelecting;
        private PokerCardVo _cardSelecting;
        private SpellCardVo _spellCardSelecting;
        private PlanetPackVo _planetCardSelecting;
        private DefineShopData.TypeBoosterPack _typeBoosterPack = DefineShopData.TypeBoosterPack.None;
        private void Start()
        {
            _shopController = ShopController.Instance;
            allPrefabVo.SetActive(false);
            _countOpenPack = 0;
            cardPrefab.SetActive(false);
            prefabInformation.SetActive(false);
        }
        public void OnInitPack(ConfigBoosterPackRecord data)
        {
            prefabInformation.SetActive(false);
            var lisDestroy = ShopController.Instance.ListSpawnPokerCardVoTemp;
            pokerHandView.SpawnBasePokerHand();
            foreach (var card in lisDestroy)
            {
                if(card != null)
                    Destroy(card.gameObject);
            }
            _data = data;
            DefineShopData.TypeBoosterPack typeBoosterPack = (DefineShopData.TypeBoosterPack)_data.typePack;
            _typeBoosterPack = typeBoosterPack;
            GameObjectUtils.Instance.ClearAllChild(content.gameObject);
            ShopController.Instance.ClearAllData();
            switch (_typeBoosterPack)
            {
                case DefineShopData.TypeBoosterPack.None:
                    break;
                case DefineShopData.TypeBoosterPack.Spell:
                    ShopController.Instance.RandomPokerCardTemp();
                    var randomSpellCardPack = ShopController.Instance.GenerateSpellPackFromBoosterPack(data);
                    GenerateSpellPack(randomSpellCardPack);
                    _ = SpawnPokerCard();
                    break;
                case DefineShopData.TypeBoosterPack.Planet:
                    var randomPlanetPack = ShopController.Instance.GeneratePlanetPackFromBoosterPack(data);
                    GeneratePlanetPack(randomPlanetPack);
                    break;
                case DefineShopData.TypeBoosterPack.Standard:
                    var randomStandardCard = PokerCardController.Instance.RandomListPokerCard(data);
                    GenerateStandardPack(randomStandardCard);
                    int maxDeck = PlayCardController.Instance.PokerCardInventory.Count;
                    txtAmountIndeck.text = maxDeck.ToString();
                    break;
                case DefineShopData.TypeBoosterPack.Joker:
                    var randomJokerCard = ShopController.Instance.RandomJokerCard(data);
                    GenerateJokerPack(randomJokerCard);
                    break;
                
            }
            _countOpenPack = 0;
            var spacing = data.sizePack > 3 ? -50 : 20f;
            gridContentLayoutGroup.spacing = new Vector2(spacing, 20);
            UpdateUI();
        }
        private void GenerateJokerPack(List<JokerCardSO.JokerCardSo>  listJokerCard)
        {
            foreach (var pack in listJokerCard)
            {
                GameObject obj = GameObjectUtils.Instance.SpawnGameObject(content, prefabJokerCard);
                ShopJokerCardVo script = obj.GetComponent<ShopJokerCardVo>();
                script.SetData(pack);
                script.OnInit(OnUpdateJokerCard);
                script.CallBackDoneSelectCard(OnCallbackFinishJokerCard);
                obj.SetActive(true);
                _listJokerCard.Add(script);
            }
        }
        private void GenerateStandardPack(List<PokerCard> listPokerCard)
        {
            foreach (var pack in listPokerCard)
            {
                GameObject obj = GameObjectUtils.Instance.SpawnGameObject(content, prefabPokerCard);
                PokerCardVo script = obj.GetComponent<PokerCardVo>();
                script.SetData(pack, pack.PokerRank, pack.PokerSuit);
                script.OnInit(OnUpdatePokerCard);
                script.CallBackDoneSelectCard(OnCallbackFinishPokerCard);
                obj.SetActive(true);
                _listPokerPack.Add(script);
            }
        }
        private void GeneratePlanetPack(List<ConfigPlanetCardRecord> listPlanet)
        {
            objLevelPlace.SetActive(false);
            ShopController.Instance.ClearPlanetPackVo();
            foreach (var pack in listPlanet)
            {
                GameObject obj = GameObjectUtils.Instance.SpawnGameObject(content, prefabBoosterPack);
                PlanetPackVo script = obj.GetComponent<PlanetPackVo>();
                script.SetData(pack);
                script.OnInit(OnUpdatePlanetCard);
                script.OnCallBackWhenDoneEffect(OnCallbackFinish);
                obj.SetActive(true);
                ShopController.Instance.AddPlanetPackVo(script);
            }
        }
        private void GenerateSpellPack(List<ConfigSpellCardRecord> listSpellCard)
        {
            ShopController.Instance.ClearSpellCardVo();
            foreach (var card in listSpellCard)
            {
                GameObject obj = GameObjectUtils.Instance.SpawnGameObject(content, prefabSpellCard);
                SpellCardVo script = obj.GetComponent<SpellCardVo>();
                script.SetData(card);
                script.OnInit(OnUpdateSpellCardCallBack);
                script.OnCallBackWhenDoneEffect(OnCallbackFinishSpellCard);
                obj.SetActive(true);
                ShopController.Instance.AddSpellCardVo(script);
            }
        }
        private async Task SpawnPokerCard()
        {
            ShopController.Instance.ClearPokerCardVoTemp();
            var tempList = ShopController.Instance.ListSpawnPokerCardTemp;
            for (int i = 0; i < tempList.Count; i++)
            {
                var card = tempList[i];
                GameObject newCard = GameObjectUtils.Instance.SpawnGameObject(contentSpawn, cardPrefab);
                PokerCardVo cardScript = newCard.GetComponent<PokerCardVo>();
                newCard.name = $"PokerCard {i}";
                cardScript.SetData(card, card.PokerRank, card.PokerSuit, index: i);
                ShopController.Instance.AddPokerCardVo(cardScript);
                cardScript.OnInit(OperationListSelect);
                cardScript.gameObject.SetActive(true);
            }
            var getAllChild = contentSpawn.transform.GetAllChilds();
            foreach (var t in getAllChild)
            {
                var child = t.GetComponent<PokerCardVo>();
                var checkIndex = ShopController.Instance.ListSpawnPokerCardVoTemp.IndexOf(child);
                pokerHandView.AttachChildToParentByIndex(checkIndex, child.gameObject);
                AudioManager.Instance.PlaySFXWithPitchChanger(AudioName.CardSlide2, checkIndex);
                await Task.Delay(100);
            }
            ReSpawnListCardVisual();
        }
        private void ReSpawnListCardVisual()
        {
            pokerHandView.ClearListCardVisual();
            var getAllChild = contentSpawnCardTemp.transform.GetAllChilds();
            for (int i = 0; i < getAllChild.Count; i++)
            {
                var obj = getAllChild[i];
                pokerHandView.SpawnCardVisualDrag(obj,i);
            }
			
        }
        private void OnUpdateSpellCardCallBack(SpellCardVo data)
        {
            if (_spellCardSelecting != null && _spellCardSelecting == data)
                return;
            if(_spellCardSelecting!=null)
                _spellCardSelecting.OnPlayEffectUnSelectCard();
            _spellCardSelecting = data;
            _spellCardSelecting.OnPlayEffectSelectCard();
            if(data != null)
                SpawnInformationCard();
        }
        private void OperationListSelect(PokerCardVo card)
        {
            var listCardSelect = ShopController.Instance.ListSelectPokerCardVoTemp;
            if (!listCardSelect.Contains(card))
            {
                if(ShopController.Instance.OnAddToSelectCard(card))
                    card.OnPlayEffectSelectCard();
            }
            else
            {
                if(ShopController.Instance.OnRemoveToSelectCard(card))
                    card.OnPlayEffectUnSelectCard();
            }
        }
        private void OnUpdatePokerCard(PokerCardVo data)
        {
            if (_cardSelecting != null && _cardSelecting == data)
                return;
            if(_cardSelecting!=null)
                _cardSelecting.OnPlayEffectUnSelectCard();
            _cardSelecting = data;
            data.OnPlayEffectSelectCard();
        }

        private void OnUpdateJokerCard(ShopJokerCardVo data)
        {
            if (_jokerCardSelecting != null && _jokerCardSelecting == data)
                return;
            if(_jokerCardSelecting != null)
                _jokerCardSelecting.OnPlayEffectUnSelectCard();
            _jokerCardSelecting = data;
            data.OnPlayEffectSelectCard();
            SpawnInformationJokerCard();
                
        }
        private void OnUpdatePlanetCard(PlanetPackVo data)
        {
            if (_planetCardSelecting != null && _planetCardSelecting == data)
                return;
            if(_planetCardSelecting !=null)
                _planetCardSelecting.OnPlayEffectUnSelectCard();
            _planetCardSelecting = data;
            data.OnPlayEffectSelectCard();
            SpawnInformationPlanetCard();
        }
        public void LoopAudio()
        {
            var randomP = Random.Range(1, 10);
            AudioManager.Instance.PlaySFXWithPitchChanger(AudioName.tarot1, randomP);
        }

        public void OnClickSkip()
        {
            this.PostEvent(EventID.OnShowShopView, new MessageActiveUI(){ IsShow = true, IsRefesh = false});
        }
        private void OnCallbackFinish()
        {
            _countOpenPack += 1;
            if(_data.usabilityPack == _countOpenPack)
                this.PostEvent(EventID.OnShowShopView, new MessageActiveUI(){ IsShow = true, IsRefesh = false});
            else
            {
                var listPlanetPack = ShopController.Instance.ListPlanetPackVo;
                foreach (var plant in listPlanetPack)
                {
                    if(_planetCardSelecting != null)
                        plant.SetActive(true);
                }
            }
        }
        private void OnCallbackFinishJokerCard()
        {
            _countOpenPack += 1;
            _jokerCardSelecting.transform.DOMove(addCardJokerPlace.position, 0.2f) 
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => 
                {
                    _jokerCardSelecting.transform.localPosition = Vector3.zero;
                    _jokerCardSelecting.transform.localRotation = Quaternion.identity;
                    _jokerCardSelecting.transform.localScale = Vector3.one;
                    _jokerCardSelecting.SetActive(false);
                    if(_data.usabilityPack == _countOpenPack)
                        this.PostEvent(EventID.OnShowShopView, new MessageActiveUI(){ IsShow = true, IsRefesh = false});
                    else
                    {
                        foreach (var joker in _listJokerCard)
                        {
                            if(_planetCardSelecting != null)
                                joker.SetActive(true);
                        }
                    }
                });
           
        }
        private void OnCallbackFinishPokerCard()
        {
            _countOpenPack += 1;
           
            _cardSelecting.transform.DOMove(addCardPokerPlace.position, 0.2f) 
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => 
                {
                    _cardSelecting.transform.localPosition = Vector3.zero;
                    _cardSelecting.transform.localRotation = Quaternion.identity;
                    _cardSelecting.transform.localScale = Vector3.one;
                    _cardSelecting.SetActive(false);
                    if(_data.usabilityPack == _countOpenPack)
                        this.PostEvent(EventID.OnShowShopView, new MessageActiveUI(){ IsShow = true, IsRefesh = false});
                    else
                    {
                        foreach (var poker in _listPokerPack)
                        {
                            if(_planetCardSelecting != null)
                                poker.SetActive(true);
                        }
                    }
                    int maxDeck = PlayCardController.Instance.PokerCardInventory.Count;
                    txtAmountIndeck.text = maxDeck.ToString();
                });
        }
        private void OnCallbackFinishSpellCard()
        {
            _countOpenPack += 1;
            _ = ChangeViewDelay();
        }
        private async Task ChangeViewDelay()
        {
            var listTemp = ShopController.Instance.ListSelectPokerCardVoTemp;
            var id = _spellCardSelecting.Data.id;
            var config = ConfigManager.configSpell.GetPackByID(id);
            if (config.nameCode.Equals(DefineShopData.SpellCardName.s_card_multiplication))
            {
                var poker = listTemp[0];
                gridLayoutGroup.enabled = false;
                for (int i = 0; i < config.countAttack; i++)
                {
                    GameObject obj = GameObjectUtils.Instance.SpawnGameObject(contentSpawn,poker.transform.parent.gameObject);
                    GameObjectUtils.Instance.MoveToNewLocation(contentSpawnCardTemp,obj.transform as RectTransform);
                    var child = obj.transform.GetChild(0);
                    child.GetComponent<PokerCardVo>().OnPlayEffectUnSelectCard();
                    await Task.Delay(100);
                }
                PerfectSize();
                gridLayoutGroup.enabled = true;
            }
            foreach (var card in listTemp)
            {
                if(card != null)
                    card.UpdateData();
            }
            await Task.Delay(800);
            foreach (var card in listTemp)
            {
                if(card != null)
                    card.OnPlayEffectUnSelectCard();
            }
            ShopController.Instance.ClearSelectCardTemp();
            await Task.Delay(1000);
            if(_data.usabilityPack == _countOpenPack)
                this.PostEvent(EventID.OnShowShopView, new MessageActiveUI(){ IsShow = true, IsRefesh = false});
        }
        private void UpdateUI()
        {
            addCardPokerPlace.gameObject.SetActive(_typeBoosterPack == DefineShopData.TypeBoosterPack.Standard);
            objLevelPlace.SetActive(_typeBoosterPack == DefineShopData.TypeBoosterPack.Planet);

            txtNamePack.text = $"{_data.name}";
            txtCountPack.text = $"Choose {(_data.usabilityPack - _countOpenPack)}";
        }
        private void SpawnInformationCard()
        {
            scriptDescription.OnSetDataBoosterPackVo(_spellCardSelecting.Data.name , _spellCardSelecting.Data.description);
            scriptDescription.SetActive(true);
        }
        private void SpawnInformationJokerCard()
        {
            scriptDescription.OnSetDataBoosterPackVo(_jokerCardSelecting.JokerCardData.JokerName , _jokerCardSelecting.JokerCardData.Description, _jokerCardSelecting.JokerCardData.Rarity);
            scriptDescription.SetActive(true);
        }
        private void SpawnInformationPlanetCard()
        {
            scriptDescription.OnSetDataBoosterPackVo(_planetCardSelecting.Data.name , _planetCardSelecting.Data.description);
            scriptDescription.SetActive(true);
        }
        private void PerfectSize()
        {
            int numberCards = 11;
            float totalCardWidth = numberCards * 360;
            float maxSpacing = 1820 - totalCardWidth;
            float spacing = maxSpacing / (numberCards - 1);
            gridLayoutGroup.spacing = new Vector2(spacing, 0);
        }
        
    }
}