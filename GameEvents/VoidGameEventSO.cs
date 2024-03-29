﻿using UnityEngine;
using UnityEngine.Events;

namespace ReiBrary.GameEvents
{
    [CreateAssetMenu(fileName = "VoidGameEvent", menuName = "ReiBrary/Game Event/Void Game Event")]
    public class VoidGameEventSO : ScriptableObject
    {
        public UnityAction OnEventRaised;

        public void RaiseEvent() => OnEventRaised?.Invoke();
    }
}
