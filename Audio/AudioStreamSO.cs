using UnityEngine;
using UnityEngine.Audio;
using ReiBrary.Attributes;

namespace ReiBrary.Audio
{
    [CreateAssetMenu(fileName = "Stream", menuName = "ReiBrary/Audio/Stream")]
    public class AudioStreamSO : ScriptableObject
    {
        [UniqueIdentifier] public string id;
        public AudioMixerGroup MixerGroup;
    }
}
