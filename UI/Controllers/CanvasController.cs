using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Util.Coroutine;

namespace Util.UI.Controllers
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
            // TODO: Move this to the input reader event callback!
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

        /// <summary>
        /// Returns to the previously active UI page.
        /// </summary>
        public void ReturnToPrevious() => GetUI(_lastActiveUiPage)?.ReturnToUI();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target">The UI page to be enabled.</param>
        /// <param name="resetOnSwitch"></param>
        public void EnableUI(UIPage target, bool resetOnSwitch = false)
        {
            if (target == null) return;

            GetUI(target)?.Enable(resetOnSwitch);
            _lastActiveUiPage = target;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="resetOnSwitch"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public IEnumerator EnableUICoroutine(UIPage target, bool resetOnSwitch = false, bool transition = true)
        {
            if (target == null) yield break;

            _lastActiveUiPage = target;
            yield return GetUI(target)?.EnableCoroutine(resetOnSwitch, transition);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target">The UI page to be disabled.</param>
        public void DisableUI(UIPage target)
        {
            if (target == null) return;

            GetUI(target)?.Disable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="resetOnSwitch"></param>
        /// <returns></returns>
        public IEnumerator DisableUICoroutine(UIPage target, bool resetOnSwitch = false)
        {
            if (target == null) yield break;

            yield return GetUI(target)?.DisableCoroutine();
        }

        /// <summary>
        /// Disables the currently active UI and enables the <c>target</c> UI page. 
        /// </summary>
        /// <param name="target">The desired UI page to display.</param>
        /// <param name="resetCurrentOnSwitch">Whether the current UI page should be reset.</param>
        /// <param name="resetTargetOnSwitch">Whether the target UI page should be reset.</param>
        /// <param name="transition">Whether tweens should be animated.</param>
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
