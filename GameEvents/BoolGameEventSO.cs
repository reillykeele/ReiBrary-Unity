using UnityEngine;
using UnityEngine.Events;

namespace ReiBrary.GameEvents
{
    [CreateAssetMenu(fileName = "BoolGameEvent", menuName = "ReiBrary/Game Event/Bool Game Event")]
    public class BoolGameEventSO : ScriptableObject
    {
        public UnityAction<bool> OnEventRaised;

        public void RaiseEvent(bool value) => OnEventRaised?.Invoke(value);
    }
}