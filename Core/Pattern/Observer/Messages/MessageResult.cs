namespace Core.Observer
{
    public class MessageResult
    {
        public ResultGameEnum ResultGame;
        public bool IsShow;
    }

    public enum ResultGameEnum
    {
        None = 0,
        WinPerRound,
        WinGame,
        Lose,
       
    }
}