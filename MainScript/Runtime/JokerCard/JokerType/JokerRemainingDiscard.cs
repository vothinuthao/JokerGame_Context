using System;
using System.Threading.Tasks;
using Core.Model;
using InGame;
using Runtime.Manager;

namespace Runtime.JokerCard.JokerType
{
    public class JokerRemainingDiscard : JokerLogic
    {
        private JokerContextEnum _effectEnum = JokerContextEnum.RemainingDiscard;
        private JokerCardSO.JokerCardSo _data;
        
        public override JokerContextEnum GetProductType()
        {
            return JokerContextEnum.RemainingDiscard;
        }
        public override async Task AsyncEffectCard(JokerCardVO data, PokerCardVo pokerCardVo = null, Action<bool> actionActivated = null)
        {
            _data = data.JokerData;
            if (_data.EffectName == DefineNameJokerCard.j_banner)
            {
                foreach (var type in _data.typeEffect)
                {
                    var discardCount = PlayCardController.Instance.Property.Discards;
                    var totalChip = discardCount * type.Value;
                    AddChip(discardCount * type.Value);
                    data.OnShakeEffect(totalChip, TypeEffectEnum.Chip);
                    await Task.Delay(300);
                }
            }
			if (_data.EffectName == DefineNameJokerCard.j_mystic_summit)
			{
                if (PlayCardController.Instance.Property.Discards <= 0)
                {
                    foreach (var type in _data.typeEffect)
                    {
                        AddPointByType(type.Type, type.Value, pokerCardVo);
                    }
                }
			}
		}
    }
}