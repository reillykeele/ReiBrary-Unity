using UnityEngine;
using ReiBrary.Attributes;

namespace ReiBrary.UI
{
    [CreateAssetMenu(fileName = "Page", menuName = "ReiBrary/UI/UI Page")]
    public class UIPage : ScriptableObject
    {
        [UniqueIdentifier] public string id;
    }
}
