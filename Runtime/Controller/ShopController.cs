using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entity;
using Core.Manager;
using Core.Manager.Configs;
using Core.Utils;
using Entity;
using Frameworks.Base;
using Frameworks.UIPopup;
using InGame;
using Manager;
using Manager.Configs;
using Runtime.JokerCard;
using UI.Popups.ClassParameter;
using Random = UnityEngine.Random;

public class ShopController : Singleton<ShopController>
{
    private List<JokerCardSO.JokerCardSo> _listAllCardJoker = new List<JokerCardSO.JokerCardSo>();
    private List<JokerCardSO.JokerCardSo> _listOwnerCardJoker = new List<JokerCardSO.JokerCardSo>();
    private List<JokerCardSO.JokerCardSo> _listNewJokerCard = new List<JokerCardSO.JokerCardSo>();
    private List<JokerCardSO.JokerCardSo> _listJokerCardInShop = new List<JokerCardSO.JokerCardSo>();
        
    private List<SpellCardVo> _listSpellCardVo = new List<SpellCardVo>();
    private List<PokerCard> _listSpawnPokerCardTemp = new List<PokerCard>();
    private List<PokerCardVo> _listSpawnPokerCardVoTemp = new List<PokerCardVo>();
    private List<PokerCardVo> _listSelectPokerCardVoTemp = new List<PokerCardVo>();
        
    private List<BoosterPackVo> _listBoosterPackVo = new List<BoosterPackVo>();
    private List<PlanetPackVo> _listPlanetPackVo = new List<PlanetPackVo>();
    private List<ShopJokerCardVo> _listJokerCardVoInShop = new List<ShopJokerCardVo>();

    public int watchAdsTime { get; set; }
    public ShopJokerCardVo JokerSelected { get; set; }

    // Spell Card
    public List<SpellCardVo> ListSpellCardVo => _listSpellCardVo;
    public List<PlanetPackVo> ListPlanetPackVo => _listPlanetPackVo;
    public List<PokerCard> ListSpawnPokerCardTemp => _listSpawnPokerCardTemp;
    public List<PokerCardVo> ListSpawnPokerCardVoTemp => _listSpawnPokerCardVoTemp;
    public List<PokerCardVo> ListSelectPokerCardVoTemp => _listSelectPokerCardVoTemp;

    //for booster pack 
    private Dictionary<int, List<ConfigBoosterPackRecord>> _dicGroupedBoosterPacks = new Dictionary<int, List<ConfigBoosterPackRecord>>();
    public int AmountFreeReRoll { get; set; }
    public List<ShopJokerCardVo> ListJokerCardVoInShop => _listJokerCardVoInShop;
    public List<JokerCardSO.JokerCardSo> ListJokerCardInShop => _listJokerCardInShop;
    private int _selectID = -1;
    public void InitData()
    {
        if (PlayCardController.IsInstanceValid())
        {
            _listAllCardJoker = JokerCardController.Instance.ListAllCardJoker;
            _listOwnerCardJoker = JokerCardController.Instance.ListJokerCardOwner;
        }
        ClassifyBoosterPack();
                    
    }

    public List<JokerCardSO.JokerCardSo> RandomJokerCard(int amount)
    {
        AbsoluteNotNull();
        var jokerOwner = JokerCardController.Instance.ListJokerCardOwner;
        if (_listNewJokerCard.Count == 0)
            foreach (var item in _listAllCardJoker)
            {
                if (_listOwnerCardJoker.All(x => x.ID != item.ID) && !jokerOwner.Contains(item))
                    _listNewJokerCard.Add(item);
            }

        List<JokerCardSO.JokerCardSo> finalList = new List<JokerCardSO.JokerCardSo>();
        int i = 0;
        while (i < amount && _listNewJokerCard.Count > 0)
        {
            var newJoker = GetRandomCard(_listNewJokerCard);
            _listJokerCardInShop.Add(newJoker);
            if (!finalList.Contains(newJoker))
            {
                finalList.Add(newJoker);
                i++;
            }
        }
        return finalList;
    }
    public List<JokerCardSO.JokerCardSo> RandomJokerCard(ConfigBoosterPackRecord data)
    {
        AbsoluteNotNull();
        if(_listNewJokerCard.Count == 0)
            foreach (var item in _listAllCardJoker)
            {
                if(_listOwnerCardJoker.All(x => x.ID != item.ID))
                    _listNewJokerCard.Add(item);
            } 
        List<JokerCardSO.JokerCardSo> finalList = new List<JokerCardSO.JokerCardSo>();
        int i = 0;
        while (i < data.sizePack && _listNewJokerCard.Count > 0)
        {
            var newJoker = GetRandomCard(_listNewJokerCard);
            _listJokerCardInShop.Add(newJoker);
            if (!finalList.Contains(newJoker))
            {
                finalList.Add(newJoker);
                i++;
            }
        }
        return finalList;
    }
    public void RemoveJokerCardInShop(JokerCardSO.JokerCardSo data)
    {
        if(_listJokerCardInShop.Contains(data))
            _listJokerCardInShop.Remove(data);
    }
    public void ClearJokerCardInShop()
    {
        _listJokerCardInShop.Clear();
    }
    public void ResetPoolRandom()
    {
        _listNewJokerCard.Clear();
    }
    public bool ValidateCostToBuy(int id)
    {
        var currency = PlayerDataManager.Instance.Property.Dollar;
        var joker = _listAllCardJoker.FirstOrDefault(x => x.ID == id);
        return joker != null && joker.BuyCost <= currency;
    }
    public void AddJokerVo(ShopJokerCardVo joker)
    {
        if(!_listJokerCardVoInShop.Contains(joker))
            _listJokerCardVoInShop.Add(joker);
    }
    public void AddBoosterPackVo(BoosterPackVo pack)
    {
        if(!_listBoosterPackVo.Contains(pack))
            _listBoosterPackVo.Add(pack);
    }
    public void RemoveBoosterPackVo(BoosterPackVo pack)
    {
        if(_listBoosterPackVo.Contains(pack))
            _listBoosterPackVo.Remove(pack);
    }
    public void ClearBoosterPackVo()
    {
        _listBoosterPackVo.Clear();
    }
    public void AddPlanetPackVo(PlanetPackVo pack)
    {
        if(!_listPlanetPackVo.Contains(pack))
            _listPlanetPackVo.Add(pack);
    }
    public void ClearPlanetPackVo()
    {
        _listPlanetPackVo.Clear();
    }
    public void RandomPokerCardTemp()
    {
        var listCurrentDeck = PlayCardController.Instance.Property.PokerCardInventory;
        var maxCard = PlayerDataManager.Instance.Property.MaxDrawCardInHand;
        List<PokerCard> shuffledDeck = listCurrentDeck.OrderBy(a => Guid.NewGuid()).ToList();
        List<PokerCard> randomCards = shuffledDeck.Take(maxCard).ToList();
        _listSpawnPokerCardTemp = randomCards;
    }
    public void AddPokerCardVo(PokerCardVo card)
    {
        _listSpawnPokerCardVoTemp.Add(card);
    }
    public void ClearPokerCardVoTemp()
    {
        _listSpawnPokerCardVoTemp.Clear();
    }
    public bool OnAddToSelectCard(PokerCardVo card)
    {
        if (!_listSelectPokerCardVoTemp.Contains(card))
        {
            _listSelectPokerCardVoTemp.Add(card);
            return true;
        }
        return false;
    }
    public bool OnRemoveToSelectCard(PokerCardVo card)
    {
        if (_listSelectPokerCardVoTemp.Contains(card))
        {
            _listSelectPokerCardVoTemp.Remove(card);
            return true;
        }
        return false;
    }
    public void ClearSelectCardTemp()
    {
        _listSelectPokerCardVoTemp.Clear();
    }
    public bool OnApplySpellCard(int id)
    {
        var config = ConfigManager.configSpell.GetPackByID(id);
        if (config.nameCode.Equals(DefineShopData.SpellCardName.s_heart_morph))
        {
            if (_listSelectPokerCardVoTemp.Count <= config.countAttack &&
                _listSelectPokerCardVoTemp.Count != 0)
            {
                PokerCardController.Instance.ConvertPokerCardsToOtherSuit(_listSelectPokerCardVoTemp, SuitCard.Heart);
                return true;
            }
            else
            {
                UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupNotificationBox, new NotificationParameter()
                {
                    TextNotification = "Choose 3 Card",
                    IconNotificationEnum = IconNotificationEnum.Warning,
                });
            }
        }
        if (config.nameCode.Equals(DefineShopData.SpellCardName.s_club_morph))
        {
            if (_listSelectPokerCardVoTemp.Count <= config.countAttack &&
                _listSelectPokerCardVoTemp.Count != 0)
            {
                PokerCardController.Instance.ConvertPokerCardsToOtherSuit(_listSelectPokerCardVoTemp, SuitCard.Club);
                return true;
            }
            else
            {
                UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupNotificationBox, new NotificationParameter()
                {
                    TextNotification = "Choose 3 Card",
                    IconNotificationEnum = IconNotificationEnum.Warning,
                });
            }
        }
        if (config.nameCode.Equals(DefineShopData.SpellCardName.s_diamond_morph))
        {
            if (_listSelectPokerCardVoTemp.Count <= config.countAttack &&
                _listSelectPokerCardVoTemp.Count != 0)
            {
                PokerCardController.Instance.ConvertPokerCardsToOtherSuit(_listSelectPokerCardVoTemp, SuitCard.Diamond);
                return true;
            }
            else
            {
                UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupNotificationBox, new NotificationParameter()
                {
                    TextNotification = "Choose 3 Card",
                    IconNotificationEnum = IconNotificationEnum.Warning,
                });
            }
        }
        if (config.nameCode.Equals(DefineShopData.SpellCardName.s_spade_morph))
        {
            if (_listSelectPokerCardVoTemp.Count <= config.countAttack &&
                _listSelectPokerCardVoTemp.Count != 0)
            {
                PokerCardController.Instance.ConvertPokerCardsToOtherSuit(_listSelectPokerCardVoTemp, SuitCard.Spade);
                return true;
            }
            else
            {
                UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupNotificationBox, new NotificationParameter()
                {
                    TextNotification = "Choose 3 Card",
                    IconNotificationEnum = IconNotificationEnum.Warning,
                });
            }
        }
        if (config.nameCode.Equals(DefineShopData.SpellCardName.s_spreading_growth))
        {
            if (_listSelectPokerCardVoTemp.Count <= config.countAttack &&
                _listSelectPokerCardVoTemp.Count != 0)
            {
                PokerCardController.Instance.UpRankPokerRank(_listSelectPokerCardVoTemp, 1 );
                return true;
            }
        }
        if (config.nameCode.Equals(DefineShopData.SpellCardName.s_spreading_growth))
        {
            if (_listSelectPokerCardVoTemp.Count <= config.countAttack &&
                _listSelectPokerCardVoTemp.Count != 0)
            {
                PokerCardController.Instance.UpRankPokerRank(_listSelectPokerCardVoTemp, 1 );
                return true;
            }
        }
        if (config.nameCode.Equals(DefineShopData.SpellCardName.s_decimation))
        {
            if (_listSelectPokerCardVoTemp.Count <= config.countAttack &&
                _listSelectPokerCardVoTemp.Count != 0)
            {
                PokerCardController.Instance.DestroyPokerCard(_listSelectPokerCardVoTemp);
                return true;
            }
        }
        if (config.nameCode.Equals(DefineShopData.SpellCardName.s_money_multiplication))
        {
            var currentDollar = PlayerDataManager.Instance.Property.Dollar;
            PlayerDataManager.Instance.Property.AddDollar(currentDollar <= config.countAttack
                ? currentDollar
                : config.countAttack);
            return true;
        }
        if (config.nameCode.Equals(DefineShopData.SpellCardName.s_wild_growth))
        {
            if (_listSelectPokerCardVoTemp.Count <= config.countAttack &&
                _listSelectPokerCardVoTemp.Count != 0)
            {
                PokerCardController.Instance.UpRankPokerRank(_listSelectPokerCardVoTemp, 5 );
                return true;
            }
        }
        if (config.nameCode.Equals(DefineShopData.SpellCardName.s_interchange))
        {
            if (_listSelectPokerCardVoTemp.Count == config.countAttack &&
                _listSelectPokerCardVoTemp.Count != 0)
            {
                PokerCardController.Instance.ConvertTwoCard(_listSelectPokerCardVoTemp);
                return true;
            }
            else
            {
                UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupNotificationBox, new NotificationParameter()
                {
                    TextNotification = "Choose 3 Card",
                    IconNotificationEnum = IconNotificationEnum.Warning,
                });
            }
               
        }
        if (config.nameCode.Equals(DefineShopData.SpellCardName.s_card_multiplication))
        {
            if (_listSelectPokerCardVoTemp.Count == 1 &&
                _listSelectPokerCardVoTemp.Count != 0)
            {
                PokerCardController.Instance.AddCopiesPokerCard(_listSelectPokerCardVoTemp, config.countAttack);
                return true;
            }
        }
        if (config.nameCode.Equals(DefineShopData.SpellCardName.s_card_transfusion))
        {
            if (_listSelectPokerCardVoTemp.Count <= config.countAttack &&
                _listSelectPokerCardVoTemp.Count != 0)
            {
                PokerCardController.Instance.ConvertPokerCardToPlayStats(_listSelectPokerCardVoTemp);
                return true;
            }
        }
        return false;
    }
    public void AddSpellCardVo(SpellCardVo card)
    {
        if(!_listSpellCardVo.Contains(card))
            _listSpellCardVo.Add(card);
    }
    public void ClearSpellCardVo()
    {
        _listSpellCardVo.Clear();
    }
    public void ClearAllData()
    {
        _listSpellCardVo = new List<SpellCardVo>();
        _listSpawnPokerCardTemp = new List<PokerCard>();
        // _listSpawnPokerCardVoTemp = new List<PokerCardVo>();
        _listSelectPokerCardVoTemp = new List<PokerCardVo>();
    }
    public void SelectBoosterPack(int id)
    {
         
        foreach (var sc in _listBoosterPackVo)
        {
            if (sc.Data.id != id)
                sc.OnNotSelect();
        }
            
    }
    public void ClearListScriptJokerCardInShop()
    {
        _listJokerCardVoInShop.Clear();
    }
    public void BuyAndResetListJoker(JokerCardSO.JokerCardSo jokerCardSo)
    {
        JokerCardController.Instance.AddJokerCardToInventory(jokerCardSo);
        InitData();
    }
    public void SellAndRemovePassive(JokerCardSO.JokerCardSo jokerCardSo)
    {
        JokerCardController.Instance.RemoveJokerCardFromInventory(jokerCardSo);
        if (jokerCardSo.activationEnum == JokerActivationEnum.Passive)
        {
            var joker = JokerFactory.GetProduct(jokerCardSo.JokerContext);
            joker.RemovePassiveCard(jokerCardSo);
        }
    }
    // for booster pack 
    public List<ConfigBoosterPackRecord> RandomBoosterPackHaveAmount(int amount)
    {
        List<ConfigBoosterPackRecord> listConfig = new List<ConfigBoosterPackRecord>();
        for (int i = 0; i < amount; i++)
        {
            listConfig.Add(GetRandomPack());
        }
        return listConfig;
    }
    public ConfigBoosterPackRecord GetRandomPack()
    {
        var listBoostPack = ConfigManager.configBoosterPack.Records;
        var canUsePack = listBoostPack.Where(x => x.ratioPack > 0).ToList();

        if (canUsePack.Count == 0)
        {
            return null;
        }
        float totalRatio = canUsePack.Sum(x => x.ratioPack);
        float randomValue = Random.Range(0, totalRatio);
        float cumulativeSum = 0;
        foreach (var pack in canUsePack)
        {
            cumulativeSum += pack.ratioPack;
            if (randomValue <= cumulativeSum)
            {
                return pack;
            }
        }
        return null;
    }
    // view boosterPack
    public List<ConfigPlanetCardRecord> GeneratePlanetPackFromBoosterPack(ConfigBoosterPackRecord data)
    {
        var configPlanet = ConfigManager.configPlanetCard.Records;
        List<ConfigPlanetCardRecord> randomElements = configPlanet.OrderBy(x => Random.value).Take(data.sizePack).ToList();
        return randomElements;
    }
    public List<ConfigSpellCardRecord> GenerateSpellPackFromBoosterPack(ConfigBoosterPackRecord data)
    {
        var configSpellCard = ConfigManager.configSpell.Records;
        var configSpellCardRecords = configSpellCard.Where(x => x.isActive).ToList();
        List<ConfigSpellCardRecord> randomElements = configSpellCardRecords.OrderBy(x => Random.value).Take(data.sizePack).ToList();
        return randomElements;
    }
    public bool BuyBoosterHandle(ConfigBoosterPackRecord pack)
    {
        var checkCostPlayer = PlayerDataManager.Instance.Property.Dollar;
        if (pack.cost <= checkCostPlayer)
        {
            // handle sub Dolla 
            PlayerDataManager.Instance.Property.SubDolla(pack.cost);
                
            return true;
        }
        else
        {
            // here will make popup notification tell not enough dolla
            return false;
        }
            
        return false;
    }

    public void InitAmountFreeReroll()
    {
        var checkPassive = PlayCardController.Instance.CheckPassiveSpecial(DefineNameJokerCard.j_chaos_the_clown);
        var getJoker = JokerCardController.Instance.GetJokerFromPool(DefineNameJokerCard.j_chaos_the_clown);
        if(checkPassive)
            AmountFreeReRoll = getJoker.typeEffect.Sum(x=>x.Value);
            
    }

    public void SubReRoll()
    {
        if(AmountFreeReRoll != 0)
            AmountFreeReRoll -= 1;
    }
        
    private JokerCardSO.JokerCardSo GetRandomCard(List<JokerCardSO.JokerCardSo> listCard)
    {
        float totalWeight = 0;
        foreach (JokerCardSO.JokerCardSo card in listCard)
            totalWeight += card.Weight;
        float randomWeight = Random.Range(0, totalWeight);
        float currentWeight = 0;
        foreach (JokerCardSO.JokerCardSo card in listCard)
        {
            currentWeight += card.Weight;
            if (randomWeight < currentWeight)
                return card;
        }
        return null; 
    }
    private void AbsoluteNotNull()
    {
        if (_listAllCardJoker.Count == 0)
            _listAllCardJoker = JokerCardController.Instance.ListAllCardJoker;
        if(_listOwnerCardJoker.Count == 0)
            _listOwnerCardJoker = JokerCardController.Instance.ListJokerCardOwner;
    }
    private void ClassifyBoosterPack()
    {
        var listBoostPack = ConfigManager.configBoosterPack.Records;
        if (listBoostPack != null)
        {
            _dicGroupedBoosterPacks = listBoostPack
                .GroupBy(pack => pack.typePack)
                .ToDictionary(group => group.Key, group => group.ToList());
                
        }
    }
        
        
}