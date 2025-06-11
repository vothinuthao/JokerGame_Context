using System;
using Core.Entity;
using UnityEngine.Serialization;

namespace Entity
{
    [Serializable]
    public class PokerCard : BaseCardEntity
    {
        public TypeCard TypeCard = TypeCard.None;
        public RankCard PokerRank;
        public SuitCard PokerSuit;
        public int ChipValue;
        public int MultValue;

    }
}
