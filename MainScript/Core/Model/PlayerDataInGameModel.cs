using System.Collections.Generic;
using Core.Entity;
using Entity;

namespace Model
{
    public class HandTypePoker
    {
        public int LevelHighCard { get; set; }
        public int LevelPair { get; set; }
        public int LevelTwoPair { get; set; }
        public int LevelThreeOfKind { get; set; }
        public int LevelStraight { get; set; }
        public int LevelFlush { get; set; }
        public int LevelFullHouse { get; set; }
        public int LevelFourOfKind { get; set; }
        public int LevelStraightFlush { get; set; }
        public int LevelRoyalFlush { get; set; }
        public int LevelFiveOfAKind { get; set; }
        public int LevelFlushHouse { get; set; }
        public int LevelFlushFive { get; set; }
        public HandTypePoker()
        {
            
        }
    }
    public class CountHandPlaying
    {
        public int CountHigh { get; set; }
        public int CountPair { get; set; }
        public int CountTwoPair { get; set; }
        public int CountThreeOfKind { get; set; }
        public int CountStraight { get; set; }
        public int CountFlush { get; set; }
        public int CountFullHouse { get; set; }
        public int CountFourOfKind { get; set; }
        public int CountStraightFlush { get; set; }
        public int CountRoyalFlush { get; set; }
        public int CountFiveOfAKind { get; set; }
        public int CountFlushHouse { get; set; }
        public int CountFlushFive { get; set; }
        public CountHandPlaying()
        {
            
        }
    }
    public class PlayerDataInGameModel
    {
        public string Uid{ get; set; }
        public float Chip { get; set; }
        public float Mult { get; set; }
        public float TotalDamage { get; set; }
        public int DollarInGame { get; set; }
        public int Discards { get; set; }
        public int Hands { get; set; }
        public int MaxHealth { get; set; }
        public int Health { get; set; }
        public int MaxShield { get; set; }
        public int Shield { get; set; }
        public bool IsBossRound { get; set; }
        public int IDBossInRound { get; set; }
        public List<int> ListPassiveActivated { get; set; }
        public HandTypePoker HandTypePokerClass { get; set; }
        public CountHandPlaying CountHandPlaying { get; set; }
        public List<PokerCard> PokerCardInventory { get; set; }
        public PlayerDataInGameModel()
        {
            
        }
    }
}