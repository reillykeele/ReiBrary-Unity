using UnityEngine;
using UnityEngine.Events;

namespace ReiBrary.GameEvents
{
    [CreateAssetMenu(fileName = "IntGameEvent", menuName = "Game Event/Int Game Event")]
    public class IntGameEventSO : ScriptableObject
    {
        public UnityAction<int> OnEventRaised;

        public void RaiseEvent(int value) => OnEventRaised?.Invoke(value);
    }
}
