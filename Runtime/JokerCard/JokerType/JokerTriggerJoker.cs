using InGame;
using Runtime.Manager;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Runtime.JokerCard.JokerType
{
	public class JokerTriggerJoker : JokerLogic
	{
		private JokerContextEnum _effectEnum = JokerContextEnum.TriggerJoker;
		private JokerCardSO.JokerCardSo _data;

		public override JokerContextEnum GetProductType()
		{
			return JokerContextEnum.TriggerJoker;
		}


		public override async Task AsyncEffectCard(JokerCardVO data, PokerCardVo pokerCardVo = null, Action<bool> actionActivated = null)
		{
			_data = data.JokerData;
			if (_data.EffectName == DefineNameJokerCard.j_joker)
			{
				foreach (var type in _data.typeEffect)
				{
					AddMult(type.Value);
					data.OnShakeEffect(type.Value, TypeEffectEnum.AddMult);
				}
			}

		}

	}
}