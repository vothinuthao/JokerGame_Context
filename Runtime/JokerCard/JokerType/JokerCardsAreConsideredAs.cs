using System;
using System.Threading.Tasks;
using InGame;
using Runtime.JokerCard;
using Runtime.Manager;

public class JokerCardsAreConsideredAs : JokerLogic
{
    private JokerContextEnum _effectEnum = JokerContextEnum.CardsAreConsideredAs;
    private JokerCardSO.JokerCardSo _data;
    public override JokerContextEnum GetProductType()
    {
        return JokerContextEnum.CardsAreConsideredAs;
    }
    public override Task AsyncEffectCard(JokerCardVO data, PokerCardVo pokerCardVo = null, Action<bool> actionActivated = null)
    {
        _data = data.JokerData;
        if (_data.EffectName == DefineNameJokerCard.j_pareidolia)
        {
            PlayCardController.Instance.AddPassiveActivated(data.JokerData.ID);
        }
        return Task.CompletedTask;
    }

    public override void RemovePassiveCard(JokerCardSO.JokerCardSo data)
    {
        if (_data.EffectName == DefineNameJokerCard.j_pareidolia)
        {
            PlayCardController.Instance.RemovePassiveActivated(data.ID);
        }
        base.RemovePassiveCard(data);
    }
}