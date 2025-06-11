using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Model;
using InGame;
using Runtime.Manager;
using UnityEngine;

namespace Runtime.JokerCard.JokerType
{
	public class JokerTriggerCardPlayed : JokerLogic
	{
		private JokerContextEnum _effectEnum = JokerContextEnum.TriggerCardPlayed;
		private JokerCardSO.JokerCardSo _data;

		public override JokerContextEnum GetProductType()
		{
			return JokerContextEnum.TriggerCardPlayed;
		}
		public override async Task AsyncEffectCard(JokerCardVO data, PokerCardVo pokerCardVo = null, Action<bool> actionActivated = null)
		{
			_data = data.JokerData;
			if (_data.EffectName == DefineNameJokerCard.j_dusk)
			{
				if (PlayCardController.Instance.Property.Health <= 300)
				{
					if (pokerCardVo != null)
					{
						var listJoker = pokerCardVo.ListJokerCardVo;
						foreach (var j in listJoker)
						{
							var joker = JokerFactory.GetProduct(j.JokerData.JokerContext);
							await joker.AsyncEffectCard(j, pokerCardVo: pokerCardVo);
							data.OnShakeEffect(0, TypeEffectEnum.AddMult);
							pokerCardVo.OnShakeEffect(0, TypeEffectEnum.AddMult);
							await Task.Delay(300);
						}
						pokerCardVo.RemoveJokerInPool();
					}
				}
			}
			if (_data.EffectName == DefineNameJokerCard.j_double_counting)
			{
				if (pokerCardVo != null)
				{
					if (PlayCardController.Instance.ListCardOnSelect[1] == pokerCardVo)
					{
						var listJoker = pokerCardVo.ListJokerCardVo;
						foreach (var j in listJoker)
						{
							var joker = JokerFactory.GetProduct(j.JokerData.JokerContext);
							await joker.AsyncEffectCard(j, pokerCardVo: pokerCardVo);
							data.OnShakeEffect(0, TypeEffectEnum.AddMult);
							pokerCardVo.OnShakeEffect(0, TypeEffectEnum.AddMult);
							await Task.Delay(300);
						}
						pokerCardVo.RemoveJokerInPool();
					}
				}
			}
		}
	}
}