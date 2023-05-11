using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ReiBrary.AddressableUtil
{
    public class AssetReferenceAudioClip : AssetReferenceT<AudioClip>
    {
        public AssetReferenceAudioClip(string guid) : base(guid) { }
    }
}