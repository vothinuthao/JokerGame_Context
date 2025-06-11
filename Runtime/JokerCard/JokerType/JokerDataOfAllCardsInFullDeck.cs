using System;
using System.Threading.Tasks;
using InGame;
using Runtime.JokerCard;
using Runtime.Manager;

public class JokerDataOfAllCardsInFullDeck : JokerLogic
{
    private JokerContextEnum _effectEnum = JokerContextEnum.AllCardInFullDeck;
    private JokerCardSO.JokerCardSo _data;
        
    public override JokerContextEnum GetProductType()
    {
        return JokerContextEnum.AllCardInFullDeck;
    }
    public override Task AsyncEffectCard(JokerCardVO data, PokerCardVo pokerCardVo = null, Action<bool> actionActivated = null)
    {
        _data = data.JokerData;
        if (_data.EffectName == DefineNameJokerCard.j_cloud9)
        {
           foreach (var type in _data.typeEffect)
           {
               var count = PlayCardController.Instance.GetQuantityValuePokerOnDeck(type.RankCard);
               if (count > 0)
               {
                   AddDollar(count * type.Value);
                   AnalyticsManager.LogEarnCurrency(TypeCurrency.Dollar,count * type.Value, "Earn_From_Joker");
               }
           }
        }

        return Task.CompletedTask;
    }
}