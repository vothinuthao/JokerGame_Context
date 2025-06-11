using System;
using System.Threading.Tasks;
using InGame;
using Manager;
using Runtime.Manager;

namespace Runtime.JokerCard.JokerType
{
	public class JokerChangeHandSize : JokerLogic
	{
		private JokerContextEnum _effectEnum = JokerContextEnum.ChangeHandSize;
		private JokerCardSO.JokerCardSo _data;

		public override JokerContextEnum GetProductType()
		{
			return JokerContextEnum.ChangeHandSize;
		}
		public override Task AsyncEffectCard(JokerCardVO data, PokerCardVo pokerCardVo = null, Action<bool> actionActivated = null)
		{
			_data = data.JokerData;
			if (_data.EffectName == DefineNameJokerCard.j_turtle_bean)
			{
				foreach (var type in _data.typeEffect)
				{
					if (PlayerDataManager.Instance.Property.ExtraHandSlots <= 0)
					{
						PlayerDataManager.Instance.Property.AddExtraHand(type.Value);
					}
				}
			}
			if (_data.EffectName == DefineNameJokerCard.j_add_hand)
			{
				foreach (var type in _data.typeEffect)
				{
					PlayerDataManager.Instance.Property.AddExtraHand(type.Value);
				}
			}

			return Task.CompletedTask;
		}

		public override void RemovePassiveCard(JokerCardSO.JokerCardSo data)
		{
			if (data.EffectName == DefineNameJokerCard.j_turtle_bean)
			{
				PlayerDataManager.Instance.Property.SubAllExtraHand();
			}
		}
	}
}