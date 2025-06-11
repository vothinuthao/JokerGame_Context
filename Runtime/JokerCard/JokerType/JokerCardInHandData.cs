using Core.Entity;
using Runtime.Manager;
using System;
using System.Threading.Tasks;
using InGame;
using UnityEngine;
using UnityEngine.Diagnostics;
using System.Linq;
using System.Drawing.Printing;

namespace Runtime.JokerCard.JokerType
{
	public class JokerCardInHandData : JokerLogic
	{
		private JokerContextEnum _effectEnum = JokerContextEnum.CardInHandData;
		private JokerCardSO.JokerCardSo _data;

		public override JokerContextEnum GetProductType()
		{
			return JokerContextEnum.CardInHandData;
		}
		public override async Task AsyncEffectCard(JokerCardVO data, PokerCardVo pokerCardVo = null, Action<bool> actionActivated = null)
		{
			_data = data.JokerData;

			if (_data.EffectName == DefineNameJokerCard.j_blackboard)
			{
				var suitedCardCount = 0;
				var hand = PlayCardController.Instance.ListCardOnHandScripts.ToList();
				var listCardOnSelect = PlayCardController.Instance.ListCardOnSelect;
				for (int i = 0; i< listCardOnSelect.Count; i++) 
				{
					hand.Remove(listCardOnSelect[i]);
				}
				foreach (var type in _data.typeEffect)
				{
					for (int i = 0; i < hand.Count; i++)
					{
						if (hand[i].Suit == SuitCard.Club || hand[i].Suit == SuitCard.Spade) //check if there is any card in hand that the same suit as specified in the joker
						{
							suitedCardCount++; //count every card met the requirement
						}
					}
				}
				if (suitedCardCount == hand.Count)
				{
					var multMultiplier = _data.typeEffect[0].Value; //get the FIRST value of the typeEffect
					MultiplyMult(multMultiplier); //multiply the mult by the value
					data.OnShakeEffect(multMultiplier, TypeEffectEnum.MultiplyMult);
					actionActivated?.Invoke(true);
					await Task.Delay(500);
				}


			}
		}
	}
}