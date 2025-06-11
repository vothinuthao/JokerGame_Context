using System;
using System.Threading.Tasks;
using InGame;
using Runtime.Manager;

namespace Runtime.JokerCard.JokerType
{
    public class JokerChangeDiscardAmount : JokerLogic
    {
        private JokerContextEnum _effectEnum = JokerContextEnum.ChangeDiscardAmount;
        private JokerCardSO.JokerCardSo _data;
        
        public override JokerContextEnum GetProductType()
        {
            return JokerContextEnum.ChangeDiscardAmount;
        }
        public override Task AsyncEffectCard(JokerCardVO data, PokerCardVo pokerCardVo = null, Action<bool> actionActivated = null)
        {
            _data = data.JokerData;
            if (_data.EffectName == DefineNameJokerCard.j_burglar)
            {
                foreach (var type in _data.typeEffect)
                {
                    PlayCardController.Instance.AddHands(type.Value);
                    PlayCardController.Instance.SubAllDiscard();
                }
            }

            return Task.CompletedTask;
        }
    }
}