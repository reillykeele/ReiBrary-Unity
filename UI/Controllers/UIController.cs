using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Util.Attributes;
using Util.Helpers;
using Util.UI.Controllers.Selectables;
using Util.UI.Tween;

namespace Util.UI.Controllers
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIController : MonoBehaviour
    {
        [Header("UI Controller")]
        public UIPage Page;
        public UIPage ReturnPage;

        [SerializeField] public Selectable _initialSelected = null;
        [SerializeField, ReadOnly] protected Selectable _lastSelected = null;
        
        // Parent
        protected CanvasController _canvasController;
        protected CanvasAudioController _canvasAudioController;

        // Components
        protected CanvasGroup _canvasGroup;
        protected List<ASelectableController> _selectableControllers;
        protected IEnumerable<BaseTween> _tweens;
        protected IEnumerable<Animator> _animators;

        protected virtual void Awake()
        {
            _canvasController = GetComponentInParent<CanvasController>();
            _canvasAudioController = GetComponentInParent<CanvasAudioController>();

            _canvasGroup = GetComponent<CanvasGroup>();
            _selectableControllers = GetComponentsInChildren<ASelectableController>().ToList();
            _tweens = GetComponentsInChildren<BaseTween>();
            _animators = GetComponentsInChildren<Animator>();
        }

        public virtual void Reset()
        {
            _lastSelected = null;
        }

        public virtual void Enable(bool resetOnSwitch = false)
        {
            if (resetOnSwitch)
                Reset();

            if (Gamepad.current != null)
            {
                if (_lastSelected != null)
                    _lastSelected.Select();
                else if (_initialSelected != null)
                    _initialSelected.Select();
                else
                    _selectableControllers?.FirstOrDefault()?.Select();
            }

            gameObject.Enable();
        }

        public virtual IEnumerator EnableCoroutine(bool resetOnSwitch = false, bool transition = true)
        {
            if (resetOnSwitch)
                Reset();

            if (Gamepad.current != null)
            {
                if (_lastSelected != null)
                    _lastSelected.Select();
                else if (_initialSelected != null)
                    _initialSelected.Select();
                else
                    _selectableControllers?.FirstOrDefault()?.Select();
            }

            if (transition)
                foreach (var tween in _tweens)
                {
                    if (tween.ShouldTweenInOnEnable() == false && tween.ShouldTweenIn())
                        tween.TweenIn();
                    else
                        tween.Reset();
                }

            gameObject.Enable();

            yield break;
        }

        public virtual void Disable()
        {
            _lastSelected = EventSystem.current?.currentSelectedGameObject?.GetComponent<Selectable>();
            gameObject.Disable();
        }

        public virtual IEnumerator DisableCoroutine()
        {
            _lastSelected = EventSystem.current?.currentSelectedGameObject?.GetComponent<Selectable>();

            var transitionDuration = 0f;
            foreach (var tween in _tweens.Where(x => x.gameObject.activeInHierarchy && x.ShouldTweenOut()))
            {
                transitionDuration = Mathf.Max(transitionDuration, tween.GetDurationOut());
                tween.TweenOut();
            }

            yield return new WaitForSecondsRealtime(transitionDuration);

            gameObject.Disable();
        }

        public virtual void ReturnToUI()
        {
            if (ReturnPage != null)
            {
                // _canvasAudioController.Play(CanvasAudioController.CanvasAudioSoundType.Back);
                _canvasController.SwitchUI(ReturnPage, resetTargetOnSwitch: false, transition: true);
            }
        }
    }
}
