using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entity;
using Core.Utils;
using Enemy;
using Frameworks.Base;
using Frameworks.UIPopup;
using Manager;
using Runtime.JokerCard;
using Runtime.Manager;
using UI.BaseColor;
using UI.Popups.ClassParameter;
using UnityEngine;

public class JokerCardController : Singleton<JokerCardController>
{
    public event Action OnChangeJokerCard;
    
    private List<JokerCardSO.JokerCardSo> _listAllCardJoker = new List<JokerCardSO.JokerCardSo>();
    private List<JokerCardSO.JokerCardSo> _listJokerCardOwner = new List<JokerCardSO.JokerCardSo>();
    private List<JokerCardVO> _listScriptCardVO = new List<JokerCardVO>();
    public List<JokerCardVO> ListJokerCardVO => _listScriptCardVO;
    public List<JokerCardSO.JokerCardSo> ListJokerCardOwner => _listJokerCardOwner;
    public List<JokerCardSO.JokerCardSo> ListAllCardJoker => _listAllCardJoker;
    public void OnInitJokerCard()
    {
        JokerCardSO.JokerCardSo[] arrJokerCardSOs  = Resources.LoadAll<JokerCardSO.JokerCardSo>("SO");
        _listAllCardJoker = arrJokerCardSOs.Where(x=>x.IsActive).ToList();
    }

    public void UpdateListJoker() 
    {
        UpdateListJokerCard();
    }
    public void AddScript(JokerCardVO script)
    {
        if(!_listScriptCardVO.Contains(script))
            _listScriptCardVO.Add(script);
    }
    public void ReNewScript()
    {
        _listScriptCardVO = new List<JokerCardVO>();
    }
    public void ClearAllScript()
    {
        _listScriptCardVO.Clear();
    }
    public void AddJokerCardToInventory(JokerCardSO.JokerCardSo jokerCardSo)
    {
        var listIDOwner = PlayerDataManager.Instance.Property.TotalOwnerJoker;
        if (!listIDOwner.Contains(jokerCardSo.ID))
        {
            var checkSlot = PlayerDataManager.Instance.Property.CheckSlotJokerInventory();
            if (checkSlot)
            {
                PlayerDataManager.Instance.Property.AddJokerCardToInventory(jokerCardSo.ID);
                UpdateListJokerCard();
                OnChangeJokerCard?.Invoke();
                AnalyticsManager.LogBuyJokerPerRound(jokerCardSo.ID);
            }
            else
            {
                UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupNotificationBox, new NotificationParameter()
                {
                    TextNotification = "You have enough slots and can't add more !!!",
                    IconNotificationEnum = IconNotificationEnum.Warning,
                });
            }
        }
        else
        {
            Debug.Log("Joker Card have available !!!");
        }
       
    }
    public void RemoveJokerCardFromInventory(JokerCardSO.JokerCardSo jokerCardSo)
    {
        var listIDOwner = PlayerDataManager.Instance.Property.TotalOwnerJoker;
        if (listIDOwner.Contains(jokerCardSo.ID))
        {
            PlayerDataManager.Instance.Property.SubJokerCardToInventory(jokerCardSo.ID);
            UpdateListJokerCard();
            var joker = JokerFactory.GetProduct(jokerCardSo.JokerContext);
            joker.RemovePassiveCard(jokerCardSo);
            OnChangeJokerCard?.Invoke();
        }
           
        else
        {
            Debug.Log("Your Card Unvaliable !!!");
        }
    }
    public bool CheckPassiveAndConsiderToFaceCard(RankCard r, SuitCard s)
    {
        var listRankFaceCard = new List<RankCard>()
        {
            RankCard.Jack, RankCard.Queen, RankCard.King
        };
        var checkPassive = PlayCardController.Instance.CheckPassiveSpecial(DefineNameJokerCard.j_pareidolia);
        return listRankFaceCard.Contains(r) || checkPassive;
    }
    public JokerCardSO.JokerCardSo GetJokerFromPool(string name)
    {
        if (_listAllCardJoker.Any(x => x.EffectName == name));
        {
            return _listAllCardJoker.FirstOrDefault(x => x.EffectName == name);
        }
    }
    public JokerCardSO.JokerCardSo GetJokerFromPool(int id)
    {
        if (_listAllCardJoker.Any(x => x.ID == id));
        {
            return _listAllCardJoker.FirstOrDefault(x => x.ID == id);
        }
    }
    public JokerCardSO.JokerCardSo GetJokerFromInventory(string name)
    {
        if (_listJokerCardOwner.Any(x => x.EffectName == name));
        {
            return _listJokerCardOwner.FirstOrDefault(x => x.EffectName == name);
        }
    }
    public void SubRandomJokerCardToInventoryButNotSave()
    {
        if (_listScriptCardVO.Count == 0) return;
        var random = _listScriptCardVO.OrderBy(x => Guid.NewGuid()).Take(1).FirstOrDefault();
        if (_listScriptCardVO.Contains(random))
        {
            _listScriptCardVO.Remove(random);
            if (random != null) random.DiscardEffect();
        }
        
        
    }
    public string ShowVariableJoker(JokerCardSO.JokerCardSo data)
    {
        if (data == null) return "";
        var textStr = data.Description;
        if (data.EffectName == DefineNameJokerCard.j_joker_stencil)
        {
            var listJokerOwned = ListJokerCardOwner;
            var emptyJokerSlots = PlayerDataManager.Instance.Property.MaxJokerSlots - listJokerOwned.Count;
            var currentEmpty = emptyJokerSlots != 0 ? emptyJokerSlots : 1;
            textStr = textStr.Replace("%s2var", $"x{currentEmpty}");
        }
        else if (data.EffectName == DefineNameJokerCard.j_loyalty_card)
        {
            int handsPlayed = PlayerDataManager.Instance.Property.HandSinceLoyaltyJoker;
            int handCounted = (6 - (handsPlayed % 6)) + 1;
            textStr = textStr.Replace("%s2var", $"{handCounted}");
        }
        else if (data.EffectName == DefineNameJokerCard.j_abstract_joker)
        {
            int handsPlayed = _listScriptCardVO.Count();
            int calMult = handsPlayed * 3;
            textStr = textStr.Replace("%s2var", $"+{calMult}");
        }
        else if (data.EffectName == DefineNameJokerCard.j_ride_the_bus)
        {
            int handsPlayed = PlayerDataManager.Instance.Property.ConsecutiveFaceCardCount;
            textStr = textStr.Replace("%s2var", $"+{handsPlayed}");
        }
        else if (data.EffectName == DefineNameJokerCard.j_runner)
        {
            int handsPlayed = PlayerDataManager.Instance.Property.StraightPlayed;
            int handCount = handsPlayed * 15;
            textStr = textStr.Replace("%s2var", $"+{handCount}");
        }
        else if (data.EffectName == DefineNameJokerCard.j_ice_cream)
        {
            int handsPlayed = PlayerDataManager.Instance.Property.TotalHandsPlayed;
            int chipAdd = data.typeEffect[0].Value + (handsPlayed * data.typeEffect[1].Value);
            textStr = textStr.Replace("%s2var", $"+{chipAdd}");
        }
        else if (data.EffectName == DefineNameJokerCard.J_blue_joker)
        {
            var dropCardCount = PlayCardController.Instance.ListCardInDeck.Count;
            var total = dropCardCount * data.typeEffect[0].Value;
            textStr = textStr.Replace("%s2var", $"+{total}");
        }
        else if (data.EffectName == DefineNameJokerCard.j_cloud9)
        {
            var count = PlayCardController.Instance.GetQuantityValuePokerOnDeck(data.typeEffect[0].RankCard);
            var dollar = count > 0 ? count : 0;
            textStr = textStr.Replace("%s2var", $"${dollar}");
        }
        else if (data.EffectName == DefineNameJokerCard.j_swashbuckler)
        {
            var getSumSellValue =
                _listJokerCardOwner.Where(x => x.EffectName != DefineNameJokerCard.j_swashbuckler)
                    .Sum(x => x.SellCost);
            textStr = textStr.Replace("%s2var", $"+{getSumSellValue}");
        }
        else if (data.EffectName == DefineNameJokerCard.j_pairing_socks)
        {
            var count = PlayerDataManager.Instance.Property.TwoPairPlayed;
            var value= data.typeEffect[0].Value;
            textStr = textStr.Replace("%s2var", $"+{count * value}");
        }
        var formatText = ColorSchemeManager.Instance.ConvertColorTextFromSymbol(textStr);
        return formatText;
    }
    public void OnUpdateUI()
    {
        UpdateListJokerCard();
        OnChangeJokerCard?.Invoke();
    }
    private void UpdateListJokerCard()
    {
        var listID = PlayerDataManager.Instance.Property.TotalOwnerJoker;
        _listJokerCardOwner.Clear();
        foreach (var id in listID)
        {
            var item = _listAllCardJoker.Find(x => x.ID == id);
            if (!_listJokerCardOwner.Contains(item))
            {
                _listJokerCardOwner.Add(item);
            }
        }
    }
}