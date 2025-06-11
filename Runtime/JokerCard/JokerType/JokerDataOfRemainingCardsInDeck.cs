using System;
using System.Threading.Tasks;
using InGame;
using Runtime.Manager;

namespace Runtime.JokerCard.JokerType
{
    public class JokerDataOfRemainingCardsInDeck : JokerLogic
    {
        private JokerContextEnum _effectEnum = JokerContextEnum.RemainingCardInDeck;
        private JokerCardSO.JokerCardSo _data;
        
        public override JokerContextEnum GetProductType()
        {
            return JokerContextEnum.RemainingCardInDeck;
        }
        public override async Task AsyncEffectCard(JokerCardVO data, PokerCardVo pokerCardVo = null, Action<bool> actionActivated = null)
        {
            _data = data.JokerData;
            if (_data.EffectName == DefineNameJokerCard.J_blue_joker)
            {
                foreach (var type in _data.typeEffect)
                {
                    var dropCardCount = PlayCardController.Instance.ListCardInDeck.Count;
                    var total = dropCardCount * type.Value;
                    AddChip(total);
                    data.OnShakeEffect(total, TypeEffectEnum.Chip);
                    await Task.Delay(300);
                }
            }
        }
    }
}