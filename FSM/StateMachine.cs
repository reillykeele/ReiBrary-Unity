using UnityEngine;

namespace Util.FSM
{
    public class StateMachine : MonoBehaviour
    {
        [SerializeField] private bool _useUpdate = true;
        [SerializeField] private bool _useFixedUpdate = false;

        private State _currentState;

        public State GetCurrentState => _currentState;

        public void SetCurrentState(State nextState)
        {
            if (_currentState == nextState) return;

            _currentState?.Exit(this);

            _currentState = nextState;

            _currentState?.Enter(this);
        }

        public void Update()
        {
            if (_useUpdate)
                _currentState?.Update(this);
        }

        public void FixedUpdate()
        {
            if (_useFixedUpdate)
                _currentState?.FixedUpdate(this);
        }

    }
}
