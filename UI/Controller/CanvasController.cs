using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Util.Coroutine;

namespace Util.UI.Controller
{
    public class CanvasController : MonoBehaviour
    {
        public UIPage DefaultUiPage;

        public float MinLoadingScreenTime = 0f;

        private List<UIController> uiControllers;
        private Hashtable uiHashtable;

        private UIPage _lastActiveUiPage;

        void Awake()
        {
            uiControllers = GetComponentsInChildren<UIController>().ToList();
            uiHashtable = new Hashtable();

            RegisterUIControllers(uiControllers);
        }

        void Update()
        {
            if (Keyboard.current?.escapeKey.wasPressedThisFrame == true ||
                Gamepad.current?.buttonEast.wasPressedThisFrame == true)
                ReturnToPrevious();
        }

        void Start()
        {
            foreach (var controller in uiControllers)
                controller.Disable();

            EnableUI(DefaultUiPage);
        }

        public void ReturnToPrevious() => GetUI(_lastActiveUiPage)?.ReturnToUI();

        public void EnableUI(UIPage target, bool resetOnSwitch = false)
        {
            if (target == null) return;

            GetUI(target)?.Enable(resetOnSwitch);
            _lastActiveUiPage = target;
        }

        public IEnumerator EnableUICoroutine(UIPage target, bool resetOnSwitch = false, bool transition = true)
        {
            if (target == null) yield break;

            _lastActiveUiPage = target;
            yield return GetUI(target)?.EnableCoroutine(resetOnSwitch, transition);
        }

        public void DisableUI(UIPage target)
        {
            if (target == null) return;

            GetUI(target)?.Disable();
        }

        public IEnumerator DisableUICoroutine(UIPage target, bool resetOnSwitch = false)
        {
            if (target == null) yield break;

            yield return GetUI(target)?.DisableCoroutine();
        }

        public void DisplayUI(UIPage target, bool fadeIn = false) => EnableUI(target);
        public void HideUI(UIPage target) => DisableUI(target);

        public void SwitchUI(UIPage target, bool resetCurrentOnSwitch = false, bool resetTargetOnSwitch = true, bool transition = true)
        {
            if (_lastActiveUiPage == target) return;

            StartCoroutine(CoroutineUtil.Sequence(
                DisableUICoroutine(_lastActiveUiPage, resetCurrentOnSwitch),
                EnableUICoroutine(target, resetTargetOnSwitch, transition)
                ));
        }

        private UIController GetUI(UIPage page) => (UIController) uiHashtable[page];

        private void RegisterUIControllers(IEnumerable<UIController> controllers)
        {
            foreach (var controller in controllers)
            {
                if (DoesPageExist(controller.page) == false)
                    uiHashtable.Add(controller.page, controller);
            }
        }

        private bool DoesPageExist(UIPage page) => uiHashtable.ContainsKey(page);
    }
}
