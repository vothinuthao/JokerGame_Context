using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Entity
{
    public class FileDataName
    {
        public const string PlayerData = "PlayerData.bin";
        public const string ShopData = "ShopData.bin";
        public const string PlayerInGameData = "PlayerInGameData.bin";
    }
    public enum RankCard
    {
        None = 0, Two = 1, Three = 2 , Four = 3 , Five = 4, Six = 5, Seven = 6, Eight = 7, Nine = 8, Ten = 9, Jack = 10, Queen = 11, King = 12, Ace = 13
    }

    public enum SuitCard
    {
        None = 0, Heart = 1, Diamond = 2, Club = 3, Spade = 4
    }
    public enum PokerHandValue
    {
        None = 0,
        High = 1,
        Pair = 2,
        TwoPair = 3 ,
        ThreeOfAKind = 4,
        Straight = 5,
        Flush = 6,
        FullHouse = 7,
        FourOfAKind = 8,
        StraightFlush = 9,
        RoyalFlush = 10,
        FiveOfAKind = 11,
        FlushHouse = 12,
        FlushFive = 13
    }

    public enum TypeCard
    {
        None,
        ModifyCard,
        
    }

    public enum ModifyCardEnum
    {
        None,
        Enhancements,
        Editions,
        Seals
    }
    public enum EnhancementsModifyCardEnum
    {
        None,
        BonusCard,
        MultCard,
        WildCard,
        GlassCard,
        SteelCard,
        StoneCard,
        GoldCard,
        LuckyCard,
    }
    public enum EditionsModifyCardEnum
    {
        None,
        Base,
        Foil,
        Holographic,
        Polychrome,
        Negative,
    }
    public enum SealsModifyCardEnum
    {
        None,
        GoldSeal,
        RedSeal,
        BlueSeal,
        PurpleSea,
    }

    [Serializable]
    public class PointChipAndMult
    {
        public int pokerChip;
        public int pokerMult;
    }
    
    // enemy here
    public enum EnemyDifficultyEnum
    {
        Easy, Medium, Hard
    }

    public enum EnemyTagsEnum
    {
        None,
        Standard,
        Debuff,
        Modified
    }

    public enum StateTurnPlay
    {
        None = 0 ,
        PlayTurn,
        EnemyTurn,
    }

    public enum StateScene
    {
        None = 0,
        GameView,
        ResultView,
        ShopView,
        // TutorialView
    }
    
    public enum ColorScoreEnum
    {
        None,
        HandColor,
        DiscardColor,
        MultColor,
        ChipColor,
        DollarColor,
        RankColor,
        SuitColor
    }

    public enum BackgroundEnum
    {
        None = 0,
        BackgroundRed = 1,
        BackgroundGreen = 2,
        BackgroundBlue = 3,
        BackgroundMain = 4,
    }
    
    public enum LocalizationCategory
    {
        NONE,
        LOGIN,
        MAIN,
        SETTING,
    }
}