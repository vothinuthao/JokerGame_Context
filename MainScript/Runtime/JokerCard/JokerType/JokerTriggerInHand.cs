using System;
using System.Threading.Tasks;
using InGame;
using Runtime.JokerCard;

public class JokerTriggerInHand : JokerLogic
{
    private JokerContextEnum _effectEnum = JokerContextEnum.TriggerCardInHand;
    private JokerCardSO.JokerCardSo _data;

    public override JokerContextEnum GetProductType()
    {
        return JokerContextEnum.TriggerCardInHand;
    }


    public override async Task AsyncEffectCard(JokerCardVO data, PokerCardVo pokerCardVo = null, Action<bool> actionActivated = null)
    {
        _data = data.JokerData;
        if (_data.EffectName == DefineNameJokerCard.j_trigger_on_hand)
        {
            foreach (var type in _data.typeEffect)
            {
                if (pokerCardVo != null && pokerCardVo.Suit == type.SuitCard)
                {
                    AddPointByType(type.Type, type.Value, pokerCardVo);
                    data.OnShakeEffect(type.Value, TypeEffectEnum.Chip);
                    await Task.Delay(500);
                }
                if (pokerCardVo != null && pokerCardVo.Rank == type.RankCard )
                {
                    AddPointByType(type.Type, type.Value, pokerCardVo);
                    data.OnShakeEffect(type.Value, TypeEffectEnum.Chip);
                    await Task.Delay(500);
                }
            }
        }
		if (_data.EffectName == DefineNameJokerCard.j_add_mult_by_hand_data)
		{
			foreach (var type in _data.typeEffect)
			{
				if (pokerCardVo != null && pokerCardVo.Rank == type.RankCard)
				{
					AddPointByType(type.Type, type.Value, pokerCardVo);
					pokerCardVo.OnShakeEffect(type.Value, TypeEffectEnum.MultiplyMult);
					await Task.Delay(500);
				}
			}

		}

	}
}