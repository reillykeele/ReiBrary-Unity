using System;
using System.Collections;
using ReiBrary.Extensions;
using UnityEngine;
using ReiBrary.UI.Controllers;

namespace ReiBrary.Helpers
{
    public static class UIHelper
    {
        // TODO: can probably remove this 
        public static IEnumerator FadeIn(CanvasGroup canvasGroup, Action after = null)
        {
            canvasGroup.alpha = 0f;

            while (canvasGroup.alpha < 1f)
            {
                canvasGroup.alpha += 0.05f;
                yield return null;
            }

            canvasGroup.alpha = 1f;
            if (after != null) after();
        }

        public static IEnumerator FadeOut(CanvasGroup canvasGroup, Action after = null)
        {
            canvasGroup.alpha = 1f;

            while (canvasGroup.alpha > 0f)
            {
                canvasGroup.alpha -= 0.05f;
                yield return null;
            }

            canvasGroup.alpha = 0f;
            if (after != null) after();
        }

        public static IEnumerator FadeInAndEnable(UIController uiController, CanvasGroup canvasGroup, float duration = 1f, Action after = null)
        {
            canvasGroup.alpha = 0f;
            uiController.gameObject.Enable();

            LeanTween.value(canvasGroup.gameObject, 0f, 1f, duration)
                .setOnUpdate((a) => canvasGroup.alpha = a)
                .setIgnoreTimeScale(true);

            yield return new WaitForSecondsRealtime(duration);

            after?.Invoke();
        }

        public static IEnumerator FadeOutAndDisable(UIController uiController, CanvasGroup canvasGroup, float duration = 1f, Action after = null)
        {
            canvasGroup.alpha = 1f;

            LeanTween.value(canvasGroup.gameObject, 1f, 0f, duration)
                .setOnUpdate((a) => canvasGroup.alpha = a)
                .setIgnoreTimeScale(true);

            yield return new WaitForSecondsRealtime(duration);

            uiController.gameObject.Disable();
            after?.Invoke();
        }

        /// <summary>
        /// ???
        /// </summary>
        /// <param name="worldPos"></param>
        /// <param name="toTransform"></param>
        /// <returns></returns>
        public static Vector2 WorldToRectTransform(Vector3 worldPos, RectTransform toTransform)
        {
            var viewportPos = Camera.main.WorldToViewportPoint(worldPos);
            var v = new Vector2(viewportPos.x, viewportPos.y) * Mathf.Sign(viewportPos.z);

            var rectTransform = toTransform.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            var v3 = Vector2.Scale(rectTransform.sizeDelta, v - Vector2.one * 0.5f);

            return rectTransform.TransformPointTo(v3, toTransform);
        }
    }
}