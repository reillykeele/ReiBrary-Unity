using UnityEngine;

namespace ReiBrary.Helpers
{
    public class DontDestroyOnLoadBehaviour : MonoBehaviour
    {
        void Awake() => DontDestroyOnLoad(this);
    }
}