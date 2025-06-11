using System;
using System.Linq;
using System.Threading.Tasks;
using InGame;
using Runtime.JokerCard;
using Runtime.Manager;

public class JokerPokerHandRequirement : JokerLogic
{
    private JokerContextEnum _effectEnum = JokerContextEnum.PokerHandRequirement;
    private JokerCardSO.JokerCardSo _data;
    public override JokerContextEnum GetProductType()
    {
        return JokerContextEnum.PokerHandRequirement;
    }
    public override Task AsyncEffectCard(JokerCardVO data, PokerCardVo pokerCardVo = null, Action<bool> actionActivated = null)
    {
        _data = data.JokerData;
        if (_data.EffectName == DefineNameJokerCard.j_four_fingers)
        {
            PlayCardController.Instance.AddPassiveActivated(data.JokerData.ID);
        }
        return Task.CompletedTask;
    }

    public override void RemovePassiveCard(JokerCardSO.JokerCardSo data)
    {
        if (_data.EffectName == DefineNameJokerCard.j_four_fingers)
        {
            PlayCardController.Instance.RemovePassiveActivated(data.ID);
        }
        base.RemovePassiveCard(data);
    }
}