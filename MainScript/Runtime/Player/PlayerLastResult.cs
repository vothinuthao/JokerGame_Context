using Core.Entity;
using Core.Observer;

namespace Player
{
    public class PlayerLastResult
    {
        public ResultGameEnum ResultLastGame { get; set; }
        public PokerHandValue PokerHandValue { get; set; }
        public int PokerHandValueCount { get; set; }
        public float BestScoreLastGame { get; set; }
        public int  CountDiscardLastGame { get; set; }
        public int  CountHandLastGame { get; set; }
        // public int AnteLastGame { get; set; }
        // public int RoundLastGame { get; set; }
    }
}