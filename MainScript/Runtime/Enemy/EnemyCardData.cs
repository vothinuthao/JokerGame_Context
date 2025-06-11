using System;
using Core.Entity;

namespace Enemy
{
    public abstract class EnemyCardData
    {
        [Serializable]
        public enum TriggerPhaseEnemyEnum
        {
            None = 0,
            WhenStartLevel,
            Always
            
        }
        public enum PassiveEffectEnemy
        {
            None,
            CardSlicer = 1003,
            DiamondBeast = 1004,
            RoyalGuard = 1005,
            HerMajesty = 1006,
            KingOfKings  = 1007,
            RoyalKing = 1008,
            TheBlackKnight = 1009,
            TheCardManiac = 1010,
            TheBlindingBeauty = 1011,
            TheSlimeGirl = 1012,
            TheStrictMaid = 1013,
            TheCopycat = 1014,
            TheWhipCracker = 1015,
            Splasher = 1016,
        }
        
        public enum TypeEnemyEnum
        {
            None,
            Minion,
            Boss,
        }
        [Serializable]
        public class EnemyLevelHandModify
        {
            public PokerHandValue pokerHandValue;
            public int handValueLevel;
            public int scaleHandValue;
        }

        public abstract class RandomBossPool
        {
            public static readonly int PoolRandomBossAll = 8888;
        }
    }
}