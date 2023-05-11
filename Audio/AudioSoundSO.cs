using UnityEngine;

namespace ReiBrary.Audio
{
    [CreateAssetMenu(fileName = "Sound", menuName = "Audio/Sound")]
    public class AudioSoundSO : ScriptableObject
    {
        public AudioStreamSO AudioStream;
        public AudioClip AudioClip;
    }
}
