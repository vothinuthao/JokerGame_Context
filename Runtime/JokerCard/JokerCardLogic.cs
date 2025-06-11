using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Pattern;
using Frameworks.Scripts;
using InGame;
using Manager;
using Runtime.Manager;
using UnityEngine;

namespace Runtime.JokerCard
{
    public class JokerLogic : ITypeOnFactory<JokerContextEnum>
    {
        //protected List<int> possessionJokerCard = PlayCardController.Instance.ListPossessionJokerCard;

		public virtual JokerContextEnum GetProductType() => JokerContextEnum.None;

        public virtual Task AsyncEffectCard(JokerCardVO data, PokerCardVo pokerCardVo = null, Action<bool> actionActivated = null)
        {
            return Task.CompletedTask;
        }
        public  virtual void EffectCard(JokerCardVO data, PokerCardVo pokerCardVo = null, Action<bool> actionActivated = null)
        {
            
        }
        public virtual void RemovePassiveCard(JokerCardSO.JokerCardSo data)
        {
            
        }
        
        public void AddChip(int amount)
        {
            PlayCardController.Instance.AddChip(amount);
            Debug.Log("<color=yellow>Joker</color> Add <color=blue>Chip</color>: " + amount);
        }
        public void AddMult(int amount)
        {
            PlayCardController.Instance.AddMult(amount);
            AudioManager.Instance.PlaySFXWithRandomPitch(AudioName.Chip2);
            Debug.Log("<color=yellow>Joker</color> Add <color=red>Mult</color>: " + amount);
        }

        public void MultiplyMult(float amount)
        {
			PlayCardController.Instance.MultiplyMult(amount);
			Debug.Log("<color=yellow>Joker</color> Multiply <color=red>Mult</color>: " + amount);
		}
        public void AddPointByType(TypeEffectEnum type, int amount,PokerCardVo pokerCard)
        {
            switch (type)
            {
                case TypeEffectEnum.None:
                    break;
                case TypeEffectEnum.Chip:
                    AddChip(amount);
                    // pokerCard.AddChipToPool(amount);
                    if(pokerCard)
                        pokerCard.OnShakeEffect(amount, type);
                    break;
                case TypeEffectEnum.AddMult:
                    AddMult(amount);
                    if(pokerCard)
                        pokerCard.OnShakeEffect(amount, type);
                    // pokerCard.AddMultiToPool(amount);
                    // AudioManager.Instance.PlaySFX(AudioName.Chip2);
                    break; 
                case TypeEffectEnum.MultiplyMult:
					MultiplyMult(amount);
                    // pokerCard.AddMultiToPool(amount, true);
                    if(pokerCard)
					    pokerCard.OnShakeEffect(amount, type);
					break;
            }
        }

        public void AddDollar(int amount)
        {
            PlayerDataManager.Instance.Property.AddDollar(amount);
			Debug.Log("<color=yellow>Joker</color> Add <color=yellow>Dollar</color>: " + amount);
		}

        public void Heal (int amount)
        {
            PlayCardController.Instance.AddPlayerHealth(amount);
			Debug.Log("<color=yellow>Joker</color> <color=red>Healed</color>: " + amount);
		}
            
    }
    public class JokerFactory : AFactory<JokerContextEnum, JokerLogic>
    {
        
    }
}