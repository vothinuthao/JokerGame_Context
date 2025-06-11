using System;
using System.Linq;
using System.Threading.Tasks;
using InGame;
using Manager;

namespace Runtime.JokerCard.JokerType
{
	public class JokerNumberOfJokerOwned : JokerLogic
	{
		private JokerContextEnum _effectEnum = JokerContextEnum.NumberOfJokerOwned;
		private JokerCardSO.JokerCardSo _data;
		public override JokerContextEnum GetProductType()
		{
			return JokerContextEnum.NumberOfJokerOwned;
		}
		public override async Task AsyncEffectCard(JokerCardVO data, PokerCardVo pokerCardVo = null, Action<bool> actionActivated = null)
		{
			_data = data.JokerData;
			if (_data.EffectName == DefineNameJokerCard.j_swashbuckler)
			{
				var listJokerOwned = JokerCardController.Instance.ListJokerCardOwner;
				var getSumSellValue =
					listJokerOwned.Where(x => x.EffectName != DefineNameJokerCard.j_swashbuckler)
						.Sum(x => x.SellCost);
				AddMult(getSumSellValue);
				data.OnShakeEffect(getSumSellValue, TypeEffectEnum.AddMult);
				await Task.Delay(300);
			}

			if (_data.EffectName == DefineNameJokerCard.j_joker_stencil)
			{
				var listJokerOwned = JokerCardController.Instance.ListJokerCardOwner;
				var emptyJokerSlots = PlayerDataManager.Instance.Property.MaxJokerSlots - listJokerOwned.Count;
				MultiplyMult(emptyJokerSlots);
				data.OnShakeEffect(emptyJokerSlots, TypeEffectEnum.MultiplyMult);
				await Task.Delay(300);

			}

			if (_data.EffectName == DefineNameJokerCard.j_abstract_joker)
			{
				foreach (var type in _data.typeEffect)
				{
					var listJokerOwned = JokerCardController.Instance.ListJokerCardOwner;
					var multAdd = type.Value * listJokerOwned.Count;
					AddMult(multAdd);
					data.OnShakeEffect(multAdd, TypeEffectEnum.AddMult);
					await Task.Delay(300);
				}
			}

			if (_data.EffectName == DefineNameJokerCard.j_baseball_card)
			{
				foreach (var type in _data.typeEffect)
				{
					var listJokerOwned = JokerCardController.Instance.ListJokerCardOwner;
					float uncommonJokerCount = 0;
					for (var i = 0; i < listJokerOwned.Count; i++)
					{
						if (listJokerOwned[i].Rarity == RarityEnum.Uncommon)
						{
							uncommonJokerCount++;
						}
					}
					MultiplyMult(uncommonJokerCount * type.xMultValue);
					data.OnShakeEffect((int)uncommonJokerCount, TypeEffectEnum.MultiplyMult);
					await Task.Delay(300);
				}
			}
		}
	}
}