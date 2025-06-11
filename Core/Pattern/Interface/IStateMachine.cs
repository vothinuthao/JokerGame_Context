using Manager;
using Runtime.Manager;

namespace Interface
{
    public interface IStateMachine
    {
        void EnterState(StateMachineController stateMachine);
        void UpdateState(StateMachineController stateMachine);
        void ExitState(StateMachineController stateMachine);
    }
}