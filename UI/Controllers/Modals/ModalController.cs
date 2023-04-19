using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util.Coroutine;
using Util.Helpers;
using Util.UI.Tween;

namespace Util.UI.Modals
{
    public class ModalController : MonoBehaviour
    {
        [Header("Modal Components")]
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private Button _yesButton;
        [SerializeField] private TextMeshProUGUI _yesButtonText;
        [SerializeField] private Button _noButton;
        [SerializeField] private TextMeshProUGUI _noButtonText;

        private Action _yesAction;
        private Action _noAction;

        private List<BaseTween> _tweens;

        void Awake()
        {
            _tweens = GetComponents<BaseTween>().ToList();
        }

        void OnEnable()
        {
            _yesButton.onClick.AddListener(OnYes);
            _noButton.onClick.AddListener(OnNo);
        }

        void OnDisable()
        {
            _yesButton.onClick.RemoveListener(OnYes);
            _noButton.onClick.RemoveListener(OnNo);
        }

        public void DisplayModal(string title, string description, Action yesAction = null, Action noAction = null

            /*, string yesButtonText = "Yes", string noButtonText = "No"*/)
        {
            if (title.IsNullOrEmpty() == false)
                _title.text = title;
            else
                _title.gameObject.Disable();

            if (description.IsNullOrEmpty() == false) 
                _description.text = description;
            else 
                _description.gameObject.Disable();
            
            // if (yesButtonText.IsNullOrEmpty() == false)
            //     _yesButtonText.text = yesButtonText;
            // else
            //     _yesButton.gameObject.Disable();
            //
            // if (noButtonText.IsNullOrEmpty() == false)
            //     _noButtonText.text = noButtonText;
            // else 
            //     _noButton.gameObject.Disable();

            _yesAction = yesAction;
            _noAction = noAction;

            gameObject.Enable();
        }

        public void CloseModal()
        {
            StartCoroutine(TweenOut());
        }

        private void OnYes()
        {
            _yesAction?.Invoke();
            CloseModal();
        }

        private void OnNo()
        {
            _noAction?.Invoke();
            CloseModal();
        }

        private IEnumerator TweenIn()
        {
            foreach (var tween in _tweens)
            {
                if (tween.ShouldTweenInOnEnable() == false && tween.ShouldTweenIn())
                    tween.TweenIn();
                else
                    tween.Reset();
            }

            yield break;
        }

        private IEnumerator TweenOut()
        {
            var transitionDuration = 0f;
            foreach (var tween in _tweens.Where(x => x.gameObject.activeInHierarchy && x.ShouldTweenOut()))
            {
                transitionDuration = Mathf.Max(transitionDuration, tween.GetDurationOut());
                tween.TweenOut();
            }

            yield return new WaitForSecondsRealtime(transitionDuration);

            gameObject.Disable();
        }
    }
}
