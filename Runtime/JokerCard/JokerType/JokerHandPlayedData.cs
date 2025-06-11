using Core.Entity;
using Runtime.Manager;
using System;
using System.Threading.Tasks;
using InGame;
using Manager;
using Frameworks.Effect.Scripts;
using System.Linq;
using Core.Manager;
using Entity;

namespace Runtime.JokerCard.JokerType
{
	public class JokerHandPlayedData : JokerLogic
	{
		private JokerContextEnum _effectEnum = JokerContextEnum.HandPlayedData;
		private JokerCardSO.JokerCardSo _data;
		private PlayCardView _cardView;

		public override JokerContextEnum GetProductType()
		{
			return JokerContextEnum.HandPlayedData;
		}
		public override async Task AsyncEffectCard(JokerCardVO data, PokerCardVo pokerCardVo = null, Action<bool> actionActivated = null)
		{
			_data = data.JokerData;

			if (_data.EffectName == DefineNameJokerCard.j_add_point)
			{
				foreach (var type in _data.typeEffect)
				{
					if (type.SuitCard != SuitCard.None || type.RankCard != RankCard.None)
					{
						if (pokerCardVo != null)
						{
							if (type.SuitCard == pokerCardVo.Suit || type.RankCard == pokerCardVo.Rank)
							{
								AddPointByType(type.Type, type.Value, pokerCardVo);
								data.OnShakeEffect(type.Value, TypeEffectEnum.Chip);
								actionActivated?.Invoke(true);
								await Task.Delay(500);
							}
						}
					}

				}
			}

			if (_data.EffectName == DefineNameJokerCard.j_add_mult_by_suit)
			{
				foreach (var type in _data.typeEffect)
				{
					if (pokerCardVo != null && pokerCardVo.Suit == type.SuitCard)
					{
						AddPointByType(type.Type, type.Value, pokerCardVo);
						actionActivated?.Invoke(true);
						data.OnShakeEffect(type.Value, TypeEffectEnum.AddMult);
						await Task.Delay(200);
					}

				}
			}

			if (_data.EffectName == DefineNameJokerCard.j_add_mult_by_rank)
			{
				foreach (var type in _data.typeEffect)
				{
					if (pokerCardVo != null && pokerCardVo.Rank == type.RankCard)
					{
						AddPointByType(type.Type, type.Value, pokerCardVo);
						data.OnShakeEffect(type.Value, TypeEffectEnum.AddMult);
						actionActivated?.Invoke(true);
						await Task.Delay(500);
						//pokerCard.HidePoint();
					}

				}
			}
			if (_data.EffectName == DefineNameJokerCard.j_smiley_face)
			{
				foreach (var type in _data.typeEffect)
				{
					if (pokerCardVo != null && pokerCardVo.IsFaceCard)
					{

						AddPointByType(type.Type, type.Value, pokerCardVo);
						data.OnShakeEffect(type.Value, TypeEffectEnum.AddMult);
						actionActivated?.Invoke(true);
						await Task.Delay(500);
					}

				}
			}

			if (_data.EffectName == DefineNameJokerCard.j_add_mult_by_hand)
			{
				foreach (var type in _data.typeEffect)
				{
					if (PlayCardController.Instance.CurrentPokerHandValue == type.PokerHandValue)
					{
						AddMult(type.Value);
						actionActivated?.Invoke(true);
						data.OnShakeEffect(type.Value, TypeEffectEnum.AddMult);
					}

				}
			}

			if (_data.EffectName == DefineNameJokerCard.j_add_chip_by_hand)
			{
				foreach (var type in _data.typeEffect)
				{
					if (PlayCardController.Instance.CurrentPokerHandValue == type.PokerHandValue)
					{
						AddChip(type.Value);
						actionActivated?.Invoke(true);
						data.OnShakeEffect(type.Value, TypeEffectEnum.Chip);
					}
				}
			}

			if (_data.EffectName == DefineNameJokerCard.j_add_chip_by_rank)
			{
				foreach (var type in _data.typeEffect)
				{
					if (pokerCardVo != null && pokerCardVo.Rank == type.RankCard)
					{
						AddPointByType(type.Type, type.Value, pokerCardVo);
						data.OnShakeEffect(type.Value, TypeEffectEnum.Chip);
						actionActivated?.Invoke(true);
						await Task.Delay(500);
						//pokerCard.HidePoint();
					}
				}
			}
			if (_data.EffectName == DefineNameJokerCard.j_add_chip_by_face_card)
			{
				foreach (var type in _data.typeEffect)
				{
					if (pokerCardVo != null && type.IsFaceCard && pokerCardVo.IsFaceCard)
					{
						AddPointByType(type.Type, type.Value, pokerCardVo);
						data.OnShakeEffect(type.Value, TypeEffectEnum.Chip);
						actionActivated?.Invoke(true);
						await Task.Delay(500);
					}
				}
			}

			if (_data.EffectName == DefineNameJokerCard.j_add_dollar_by_rank)
			{
				foreach (var type in _data.typeEffect)
				{
					if (pokerCardVo != null && pokerCardVo.Rank == type.RankCard)
					{
						var chance = UnityEngine.Random.Range(0f, 1f);
						if (chance <= .5f)
						{
							AddDollar(type.Value);
							data.OnShakeEffect(type.Value, TypeEffectEnum.Dollar);
							actionActivated?.Invoke(true);
							await Task.Delay(500);
						}

					}
				}
			}

			if (_data.EffectName == DefineNameJokerCard.j_half_joker)
			{
				foreach (var type in _data.typeEffect)
				{
					if (PlayCardController.Instance.ListCardOnSelect.Count <= 3)
					{
						AddMult(type.Value);
						actionActivated?.Invoke(true);
						data.OnShakeEffect(type.Value, TypeEffectEnum.AddMult);
					}
				}
			}

			if (_data.EffectName == DefineNameJokerCard.j_greedy_joker)
			{
				foreach (var type in _data.typeEffect)
				{
					if (pokerCardVo != null && pokerCardVo.Suit == type.SuitCard)
					{
						AddPointByType(type.Type, type.Value, pokerCardVo);
						data.OnShakeEffect(type.Value, TypeEffectEnum.Chip);
						actionActivated?.Invoke(true);
						await Task.Delay(300);
					}
					if (pokerCardVo != null && pokerCardVo.Rank == type.RankCard)
					{
						AddPointByType(type.Type, type.Value, pokerCardVo);
						data.OnShakeEffect(type.Value, TypeEffectEnum.Chip);
						actionActivated?.Invoke(true);
						await Task.Delay(300);
					}

				}
			}

			if (_data.EffectName == DefineNameJokerCard.j_ride_the_bus)
			{
				var containFaceCard = false;
				foreach (var type in _data.typeEffect)
				{
					for (int i = 0; i < PlayCardController.Instance.ListCardOnSelect.Count; i++)
					{
						var card = PlayCardController.Instance.ListCardOnSelect[i];

						if (card.IsFaceCard)
						{
							containFaceCard = true;
							PlayerDataManager.Instance.Property.CountConsecutiveHand(true);
						}
					}
				}
				if (!containFaceCard)
				{
					PlayerDataManager.Instance.Property.CountConsecutiveHand(false);
					var multAdd = PlayerDataManager.Instance.Property.ConsecutiveFaceCardCount;
					AddPointByType(TypeEffectEnum.AddMult, multAdd, pokerCardVo);
					await Task.Delay(500);
				}

			}

			if (_data.EffectName == DefineNameJokerCard.j_runner)
			{
				foreach (var type in _data.typeEffect)
				{
					if (PlayCardController.Instance.CurrentPokerHandValue == type.PokerHandValue)
					{
						PlayerDataManager.Instance.Property.CountStraightPlayed(1);
					}
					var chip = PlayerDataManager.Instance.Property.StraightPlayed * type.Value;
					AddChip(chip);
					actionActivated?.Invoke(true);
					data.OnShakeEffect(chip, TypeEffectEnum.Chip);
				}
			}

			if (_data.EffectName == DefineNameJokerCard.j_aegis)
			{
				if (PlayCardController.Instance.Hands == 0)
				{
					var totalPoint = PlayCardController.Instance.TotalDmgPerTurn;
					PlayCardController.Instance.AddPlayerShield((int)totalPoint);
					actionActivated?.Invoke(true);
				}
			}
			if (_data.EffectName == DefineNameJokerCard.j_explosive_joker)
			{
				var enemyList = EnemyController.Instance.ListEnemyScript;
				var selectedEnemy = enemyList[EnemyController.Instance.IdEnemySelected];
				var targetEnemy = enemyList.FindIndex(enemy => enemy.Equals(selectedEnemy));
				var explosiveDamage = PlayCardController.Instance.TotalDmgPerTurn / 2;

				int leftIndex = targetEnemy - 1;
				while (leftIndex >= 0)
				{
					if (!enemyList[leftIndex].IsDefeat)
					{
						EnemyController.Instance.ModifyStatsEnemy(leftIndex, -explosiveDamage);
						enemyList[leftIndex].EnableObjectFeel(2);
						break;
					}
					leftIndex--;
				}
				int rightIndex = targetEnemy + 1;
				while (rightIndex < enemyList.Count) 
				{
					if (!enemyList[rightIndex].IsDefeat)
					{
						EnemyController.Instance.ModifyStatsEnemy(rightIndex, -explosiveDamage);
						enemyList[rightIndex].EnableObjectFeel(2);
						break;
					}
					rightIndex++;
				}
			}

			if (_data.EffectName == DefineNameJokerCard.j_overwhelming_joker)
			{
			}

			if (_data.EffectName == DefineNameJokerCard.j_splash)
			{
				foreach (var type in _data.typeEffect)
				{
					if (pokerCardVo != null)
					{
						var config = ConfigManager.configValuePoints.GetValueByID((int)pokerCardVo.Rank);
						AddPointByType(TypeEffectEnum.Chip, config.chip, pokerCardVo);
						data.OnShakeEffect(type.Value, TypeEffectEnum.Chip);
						actionActivated?.Invoke(true);
						await Task.Delay(300);
					}

				}
			}

			if (_data.EffectName == DefineNameJokerCard.j_extra_lives)
			{
				if (pokerCardVo != null && pokerCardVo.Suit == SuitCard.Heart)
				{
					Heal(50);
					data.OnShakeEffect(pokerCardVo.PokerCard.ChipValue, TypeEffectEnum.Heal);
					actionActivated?.Invoke(true);
					await Task.Delay(300);
				}

			}

			if (_data.EffectName == DefineNameJokerCard.j_pairing_socks)
			{
				foreach (var type in _data.typeEffect)
				{
					if (PlayCardController.Instance.CurrentPokerHandValue == type.PokerHandValue)
					{
						PlayerDataManager.Instance.Property.CountTwoPairPlayed();
						var twoPairPlayed = PlayerDataManager.Instance.Property.TwoPairPlayed;
						var mult = twoPairPlayed * type.Value;
						AddMult(mult);
						data.OnShakeEffect(mult, TypeEffectEnum.AddMult);
						actionActivated?.Invoke(true);
					}
				}
			}
		}
	}
}