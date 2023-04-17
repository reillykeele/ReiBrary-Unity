using UnityEngine;
using UnityEngine.EventSystems;

namespace Util.UI.Controllers.Selectables
{
    [RequireComponent(typeof(UnityEngine.UI.Selectable))]
    public abstract class ASelectableController : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IPointerExitHandler
    {
        protected CanvasController _canvasController;
        protected CanvasAudioController _canvasAudioController;
        protected UnityEngine.UI.Selectable _selectable;

        protected virtual void Awake()
        {
            _canvasController = GetComponentInParent<CanvasController>();
            _canvasAudioController = GetComponentInParent<CanvasAudioController>();
            _selectable = GetComponent<UnityEngine.UI.Selectable>();
        }

        public virtual void Select() => _selectable.Select();

        public virtual void OnSelect(BaseEventData eventData) { }
        public virtual void OnPointerEnter(PointerEventData eventData) => Select();
        public virtual void OnPointerExit(PointerEventData eventData) => EventSystem.current.SetSelectedGameObject(null);
    }
}