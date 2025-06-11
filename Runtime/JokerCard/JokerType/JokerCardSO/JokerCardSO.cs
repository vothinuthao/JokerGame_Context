using Sirenix.OdinInspector;
using UnityEngine;

namespace JokerCardSO
{
    [CreateAssetMenu(fileName = "Create New Joker Card", menuName = "Card/Joker", order = 100)]
    public class JokerCardSo : ScriptableObject
    {
        public int ID;
        public string JokerName;
        public JokerContextEnum JokerContext;
        public RarityEnum Rarity;
        public int BuyCost;
        public int SellCost;
        public float Weight;
        public string EffectName;
        public JokerActivationEnum activationEnum;
        public TypeEffectModel[] typeEffect; 
        public Sprite ImageCard;
        [MultiLineProperty(5)]
        public string Description;
        public bool IsActive = true;
    }
}