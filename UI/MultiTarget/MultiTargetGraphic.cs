using UnityEngine;
using UnityEngine.UI;

namespace ReiBrary.UI.MultiTarget
{
    public class MultiTargetGraphic : MonoBehaviour
    {
        [SerializeField] 
        private Graphic[] targetGraphics;

        public Graphic[] GetTargetGraphics => targetGraphics;
    }
}
