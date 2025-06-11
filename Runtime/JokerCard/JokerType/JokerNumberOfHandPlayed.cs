using Core.Entity;
using Runtime.Manager;
using System;
using System.Threading.Tasks;
using InGame;
using Manager;
using UnityEngine;
using System.Linq;
using Frameworks.Utils;

namespace Runtime.JokerCard.JokerType
{
	public class JokerNumberOfHandPlayed : JokerLogic
	{
		private JokerContextEnum _effectEnum = JokerContextEnum.NumberOfHandPlayed;
		private JokerCardSO.JokerCardSo _data;

		public override JokerContextEnum GetProductType()
		{
			return JokerContextEnum.NumberOfHandPlayed;
		}
		public override async Task AsyncEffectCard(JokerCardVO data, PokerCardVo pokerCardVo = null, Action<bool> actionActivated = null)
		{
				_data = data.JokerData;

			if (_data.EffectName == DefineNameJokerCard.j_loyalty_card)
			{
				PlayerDataManager.Instance.Property.CountHandPlayed();
				int handsPlayed = PlayerDataManager.Instance.Property.HandSinceLoyaltyJoker;
				int handCounted = handsPlayed % 6;
				foreach (var type in _data.typeEffect)
				{
					if (handCounted == 0)
					{
						AddMult((int)type.Value);
						data.OnShakeEffect((int)type.Value, TypeEffectEnum.Dollar);
						await Task.Delay(500);
					}
				}
			}
			if (_data.EffectName == DefineNameJokerCard.j_supernova)
			{
				var currentHand = PlayCardController.Instance.CurrentPokerHandValue;
				int handsPlayed = PlayCardController.Instance.GetCountPlaying(currentHand);
				AddPointByType(TypeEffectEnum.AddMult, handsPlayed, pokerCardVo);
				data.OnShakeEffect(handsPlayed,TypeEffectEnum.AddMult);
				await Task.Delay(500);
			}

			if (_data.EffectName == DefineNameJokerCard.j_ice_cream)
			{
				int handsPlayed = PlayerDataManager.Instance.Property.TotalHandsPlayed;
				int chipAdd = Math.Max(0,(int)_data.typeEffect[0].Value + (handsPlayed * (int)_data.typeEffect[1].Value));
				AddPointByType(TypeEffectEnum.Chip, chipAdd, pokerCardVo);
				data.OnShakeEffect(chipAdd,TypeEffectEnum.Chip);
				await Task.Delay(500);
				PlayerDataManager.Instance.Property.AddTotalHandsPlayed(1);

			}
		}
		public override void RemovePassiveCard(JokerCardSO.JokerCardSo data)
		{
			if (data.EffectName == DefineNameJokerCard.j_ice_cream)
			{
				PlayCardController.Instance.RemovePassiveActivated(data.ID);
				PlayerDataManager.Instance.Property.ClearTotalHandsPlayed();
			}
			base.RemovePassiveCard(data);
		}
	}
}