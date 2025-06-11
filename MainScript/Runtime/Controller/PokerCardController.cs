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
using Runtime.Manager;
using UI.Popups.ClassParameter;
using UnityEngine;
using Random = UnityEngine.Random;


public class PokerCardController : Singleton<PokerCardController>
{
    private List<PokerCard> _listDefaultPoker = new List<PokerCard>();
    
    
    public List<PokerCard> RandomListPokerCard(ConfigBoosterPackRecord data)
    {
        CreateListDefault();
        var randomElements = _listDefaultPoker.OrderBy(x => Random.value).Take(data.sizePack).ToList();
        return randomElements;
    }
    
    public void ConvertPokerCardsToOtherSuit(List<PokerCardVo>  listPoker, SuitCard suit)
    {
        
        foreach (var poker in listPoker)
        {
            ConvertChangeSuit(poker.PokerCard, suit);
        }
    }
    public void UpRankPokerRank(List<PokerCardVo>  listPoker,int countRank)
    {
        foreach (var poker in listPoker)
        {
            UpRankForPokerRank(poker.PokerCard, countRank);
        }
    }
    public void DestroyPokerCard(List<PokerCardVo>  listPoker)
    {
        foreach (var poker in listPoker)
        {
            PlayCardController.Instance.RemovePokerCardToInventory(poker.PokerCard);
            poker.DiscardEffect();
        }
    }
    public void ConvertTwoCard(List<PokerCardVo> listPoker)
    {
        var listSelectCard = ShopController.Instance.ListSelectPokerCardVoTemp;
        var getFirstIndex = listSelectCard.IndexOf(listPoker[0]);
        var getSecondIndex = listSelectCard.IndexOf(listPoker[0]);
        if(getFirstIndex > getSecondIndex)
            ConvertTwoCard(listPoker[0].PokerCard, listPoker[1].PokerCard);
        else
        {
            ConvertTwoCard(listPoker[1].PokerCard, listPoker[0].PokerCard);
        }
    }
    public void AddCopiesPokerCard(List<PokerCardVo> listPoker, int numberCopies)
    {
        var getBaseCard = listPoker[0];
        PokerCard newCard = new PokerCard
        {
            pokerEnhancement = getBaseCard.PokerCard.pokerEnhancement,
            pokerEdition = getBaseCard.PokerCard.pokerEdition,
            pokerSeal = getBaseCard.PokerCard.pokerSeal,
            TypeCard = getBaseCard.PokerCard.TypeCard,
            isDisableCard = getBaseCard.PokerCard.isDisableCard,
            PokerRank = getBaseCard.PokerCard.PokerRank,
            PokerSuit = getBaseCard.PokerCard.PokerSuit,
            ChipValue = getBaseCard.PokerCard.ChipValue,
            MultValue = getBaseCard.PokerCard.MultValue
        };
        for (int i = 0; i < numberCopies; i++)
        {
            PlayCardController.Instance.AddMorePokerCardToInventory(newCard);
        }
        PlayCardController.Instance.SavePlayerInGameData();
    }
    public void ConvertPokerCardToPlayStats(List<PokerCardVo> listPoker)
    {
        var mult = listPoker.Sum(x => x.PokerCard.MultValue);
        var chip = listPoker.Sum(x => x.PokerCard.ChipValue);
        var total = chip * (mult == 0 ? 1 : mult) ;
        if (total > 0)
        {
            PlayCardController.Instance.AddPlayerHealth(total);
            UIManager.Instance.PopupManager.ShowPopup(UIPopupName.PopupNotificationBox, new NotificationParameter()
            {
                TextNotification = $"Your HP: +{total}",
                IconNotificationEnum = IconNotificationEnum.Warning,
            });
        }
           
        PlayCardController.Instance.SavePlayerInGameData();
    }
    private void ConvertChangeSuit(PokerCard oldCard,SuitCard targetSuit)
    {
        var listOwnerPokerCard = PlayCardController.Instance.Property.PokerCardInventory;
        var getCard = listOwnerPokerCard.FirstOrDefault(x => x == oldCard);
        if (getCard != null) getCard.PokerSuit = targetSuit;
        else return;
        PlayCardController.Instance.SavePlayerInGameData();
    }

    private void ConvertTwoCard(PokerCard rightCard, PokerCard leftCard)
    {
        var listOwnerPokerCard = PlayCardController.Instance.Property.PokerCardInventory;
        var getCard = listOwnerPokerCard.FirstOrDefault(x => x == rightCard);
        if (getCard != null)
        {
            getCard.pokerEnhancement = leftCard.pokerEnhancement;
            getCard.pokerEdition = leftCard.pokerEdition;
            getCard.pokerSeal = leftCard.pokerSeal;
            getCard.isDisableCard = leftCard.isDisableCard;
            getCard.TypeCard = leftCard.TypeCard;
            getCard.PokerRank = leftCard.PokerRank;
            getCard.PokerSuit = leftCard.PokerSuit;
            getCard.ChipValue = leftCard.ChipValue;
            getCard.MultValue = leftCard.MultValue;
        }
        else return;
        PlayCardController.Instance.SavePlayerInGameData();
    }

    private void UpRankForPokerRank(PokerCard oldCard,int countRank)
    {
        var listOwnerPokerCard = PlayCardController.Instance.Property.PokerCardInventory;
        var getCard = listOwnerPokerCard.FirstOrDefault(x => x == oldCard);
        if (getCard != null)
        {
            if (getCard.PokerRank != RankCard.None)
            {
                var target = getCard.PokerRank + countRank;
                if (target > RankCard.Ace)
                {
                    var subTarget = target - (int)RankCard.Ace;
                    getCard.PokerRank = (int)RankCard.Two + subTarget;
                }
                else
                {
                    getCard.PokerRank = target;
                }
            }
               
        }
        else return;
        PlayCardController.Instance.SavePlayerInGameData();
    }
    private void CreateListDefault()
    {
        if (_listDefaultPoker.Count != 0) return;
        foreach (SuitCard s in Enum.GetValues(typeof(SuitCard)))
        {
            if (s != SuitCard.None)
            {
                foreach (RankCard r in Enum.GetValues(typeof(RankCard)))
                {
                    if (r != RankCard.None)
                    {
                        PokerCard card = new PokerCard();
                        var data = ConfigManager.configValuePoints.GetValueByID((int)r);
                        card.PokerRank = r;
                        card.PokerSuit = s;
                        card.ChipValue = data.chip;
                        card.MultValue = data.mult;
                        _listDefaultPoker.Add(card);
                    }
                        
                }
            }
        }
    }
}