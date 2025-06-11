using System.Collections.Generic;
using Core.Entity;
using Core.Observer;

namespace Player
{
    public class PlayerDataModel
    {
        public string Uid { get; set; }
        public string NickName { get; set; }
        public int Level { get; set; }
        public int Dollar { get; set; }
        public int Gem { get; set; }
        public int Exp { get; set; }
        public int Ante { get; set; }
        public int Round { get; set; }
        public List<int> TotalOwnerJoker { get; set; }
		public int TotalHandsPlayed { get; set; }
		// public List<PokerCard> ExtraPokerCards  { get; set; }
        public int MaxHandSlots { get; set; }
        public int ExtraHandSlots { get; set; }
        public int MaxJokerSlots { get; set; }
        public bool JokerSlotAds { get; set; }
        public bool JokerSlotIAP { get; set; }
        public bool BeginnerPack { get; set; }
        public int MaxConsumableOwner { get; set; }
        public int MaxDrawCardInHand { get; set; }
        public bool IsCompleteTutorial { get; set; }
        public int HandSinceLoyaltyJoker { get; set; }
        public int ConsecutiveFaceCardCount { get; set; }
        public int TwoPairPlayed { get;set; }
        public int StraightPlayed { get; set; }
        public bool HasInterstitialAds { get; set; }
        public bool IsCompletedTutorial { get; set; }
        public bool IsCompletedFirstPlay { get; set; }
        public int CurrentIdTutorial { get; set; }
        public StateScene CurrentPlayerStay { get; set; }
        public PlayerLastResult PlayerLastResult { get; set; }
        public int RoundWatchAds { get; set; }
        public bool RemoveAds { get; set; }
        public PlayerDataModel()
        {
        
        }
    }
    
    
}