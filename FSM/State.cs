namespace Util.FSM
{
    public abstract class State
    {
        // Name ?
        // ID ?

        public virtual void Enter(StateMachine stateMachine) {}
        public virtual void Update(StateMachine stateMachine) {}
        public virtual void FixedUpdate(StateMachine stateMachine) {}
        public virtual void Exit(StateMachine stateMachine) {}
    }
}
