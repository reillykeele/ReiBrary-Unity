using UnityEngine;

namespace ReiBrary.Logging
{
    public static class LogHelper
    {
        public static void LogMissingComponent(MonoBehaviour caller, string component)
        {
            Debug.LogError($"{caller.gameObject.name}'s {caller.GetType().Name}: Missing component \"{component}\"");
        }
    }
}
