﻿using UnityEngine;
using UnityEngine.Events;

namespace ReiBrary.GameEvents
{
    [CreateAssetMenu(fileName = "StringGameEvent", menuName = "ReiBrary/Game Event/String Game Event")]
    public class StringGameEventSO : ScriptableObject
    {
        public UnityAction<string> OnEventRaised;

        public void RaiseEvent(string value) => OnEventRaised?.Invoke(value);
    }
}