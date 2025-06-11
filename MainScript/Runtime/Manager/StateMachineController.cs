using System;
using Core.Controller.Observer;
using Core.Observer;
using Core.Utils;

namespace Manager
{    
    public enum GameState
    {
        None = 0,
        PlayerTurn,
        EnemyTurn,
        FreeTurn
    }
    public class StateMachineController : ManualSingletonMono<StateMachineController>
    {
        public GameState CurrentState { get; private set; }
        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState) return;
            CurrentState = newState;
            this.PostEvent(EventID.OnChangeStateGamePlay, new MessageStateGameplay(){GameState = newState});
        }
        
    }
}